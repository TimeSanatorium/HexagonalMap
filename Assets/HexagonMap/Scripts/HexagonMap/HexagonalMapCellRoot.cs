using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 存档需要保存请序列化此内容
/// </summary>
public class HexagonalMapCellRoot : MonoBehaviour
{
    public HexagonalMapMgr hexagonalMapMgr;
    public HexagonTopType hexagonTop;
    public HexagonalMapCell[] hexagonalMapCells;
    public HexagonalMapCell[] columnLeftHexagonalMapCell;
    public int m_ContainerLength;
    public Vector3[] vertexs;
    public  float Max_Z, Min_Z;

    private int m_MapCount;
    [SerializeField]
    private Vector2Int originOffset;
    [SerializeField]
    private Vector3 originOffset_pos;

    #region Test
    public List<int> Roads;
    #endregion

    #region 初始化
    public void InitializeHexagonalMapRoot(HexagonalMapMgr hexagonalMapMgr, Vector2Int mapSize)
    {
        this.hexagonalMapMgr = hexagonalMapMgr;
        hexagonTop = hexagonalMapMgr.hexagonTopType;
        switch (hexagonalMapMgr.mapType)
        {
            case MapType.Square:
                hexagonalMapCells = new HexagonalMapCell[mapSize.x * mapSize.y];
                Debug.Log("这里还没弄");
                break;
            case MapType.Hexagon:
                int radious = Mathf.Max(mapSize.x, mapSize.y);
                originOffset = new Vector2Int(-(radious - 1), -(radious - 1));
                originOffset_pos = GetHexagonWorldPos(originOffset.x, originOffset.y,Mathf.Sqrt(3),hexagonalMapMgr.cellSize);
                m_ContainerLength = radious * 2 - 1;
                hexagonalMapCells = new HexagonalMapCell[m_ContainerLength * m_ContainerLength];
                Roads = new List<int>(hexagonalMapCells.Length);
                vertexs = new Vector3[((m_ContainerLength + 1) * 2 ) * (m_ContainerLength + 1)];
                m_MapCount = 0;
                break;
            default:
                break;
        }
    }

    public void InitializeLinkedList()
    {
        for (int i = 0; i < hexagonalMapCells.Length; i++)
        {
            HexagonalMapCell hexagonalMapCell = hexagonalMapCells[i];
            if (hexagonalMapCell != null)
            {
                hexagonalMapCell.InitializationData();
            }
        }
        int Radios = (m_ContainerLength + 1) / 2;
        Max_Z = GetHexagonalMapCell(Radios - 1, 1 - Radios , 0).pos.z;
        Min_Z = GetHexagonalMapCell(1 - Radios, Radios - 1 ,0).pos.z;
        InitializeColumnLeftHexagonalMapCell();
    }

    public void InitializeColumnLeftHexagonalMapCell()
    {
        columnLeftHexagonalMapCell = new HexagonalMapCell[m_ContainerLength];
        for (int i = 0; i < columnLeftHexagonalMapCell.Length; i++)
        {
            HexagonalMapCell columnLeft = new HexagonalMapCell(originOffset.x, i + originOffset.y, -i - originOffset.x - originOffset.y, this);
            columnLeft.pos = GetHexagonWorldPos(columnLeft.q, columnLeft.r, Mathf.Sqrt(3), hexagonalMapMgr.cellSize);
            columnLeftHexagonalMapCell[i] = columnLeft;
        }
    }
    #endregion

    #region Get

    public int GetCellCount()
    {
        return m_MapCount;
    }

    public int GetHexagonArrayIndex(Vector3 pos)
    {
        int result = 0;
        #region 先确认当前点所在行的位置
        if (pos.z > Max_Z || pos.z < Min_Z)
        {
            return result;
        }
        float Step_Z = hexagonalMapMgr.ExternalRadius * 1.5f;
        float offset_z = originOffset_pos.z - pos.z;
        float range_z = offset_z / Step_Z;
        int line_1 = Mathf.CeilToInt(range_z);
        int line_2 = Mathf.FloorToInt(range_z);
        #endregion

        #region 确定当前所在的列位置
        HexagonalMapCell line_LeftCell1 = GetColumnLeftHexagonalMapCell(line_1);
        HexagonalMapCell line_LeftCell2 = GetColumnLeftHexagonalMapCell(line_2);
        float hexagonalwidth = hexagonalMapMgr.InsideRadius * 2;
        int offset_x1 = Mathf.FloorToInt((pos.x - line_LeftCell1.pos.x) / hexagonalwidth + 0.5f);
        int offset_x2 = Mathf.FloorToInt((pos.x - line_LeftCell2.pos.x) / hexagonalwidth + 0.5f);
        #endregion

        int index_1 = line_1 * m_ContainerLength + offset_x1;
        int index_2 = line_2 * m_ContainerLength + offset_x2;

        HexagonalMapCell cell_1 = (index_1 < 0 || index_1 > hexagonalMapCells.Length) ? null : hexagonalMapCells[index_1];
        HexagonalMapCell cell_2 = (index_2 < 0 || index_2 > hexagonalMapCells.Length)? null : hexagonalMapCells[index_2];
        if (cell_1 == null || cell_2 == null)
        {
            return 0;
        }

        if ((Mathf.Pow(cell_1.pos.x - pos.x, 2) + Mathf.Pow(cell_1.pos.z - pos.z, 2)) < (Mathf.Pow(cell_2.pos.x - pos.x, 2) + Mathf.Pow(cell_2.pos.z - pos.z, 2)))
        {
            result = pos.x > line_LeftCell1.pos.x ? index_1 : 0;
        }
        else
        {
            result = pos.x > line_LeftCell2.pos.x ? index_2 : 0;
        }

        return result;
    }

    public int GetHexagonArrayIndex(Vector2Int pos)
    {
        return pos.x + pos.y * m_ContainerLength;
    }

    public int GetRandomRoad()
    {
        int result = 0;
        if (Roads.Count  <= 0)
        {
            return result;
        }
        result = Roads[Random.Range(0, Roads.Count)];
        return result;

    }

    public Vector2Int GetHexagonArrayPos(int q,int r)
    {
        return new Vector2Int(q - originOffset.x, r - originOffset.y);
    }

    public Vector3 GetHexagonWorldPos(int q,int r,float Sqrt_3,float cellsize)
    {
        Vector3 result = new Vector3(q * Sqrt_3 + r * 0.5f * Sqrt_3,0,-r * (1 + 0.5f)) * cellsize;
        return result;
    }

    public HexagonalMapCell GetHexagonalMapCell(int q, int r, int s)
    {
        if (hexagonalMapCells != null)
        {
            Vector2Int newPos = GetHexagonArrayPos(q, r);
            int index = GetHexagonArrayIndex(newPos);
            if (Mathf.Abs(q) >= (m_ContainerLength + 1) / 2)
            {
                return null;
            }
            if (0 <= index && index < hexagonalMapCells.Length)
            {
                return hexagonalMapCells[index];
            }
        }
        return null;
    }

    public HexagonalMapCell GetHexagonalMapCell(int arrayIndex)
    {
        if (arrayIndex<0||arrayIndex> hexagonalMapCells.Length)
        {
            return null;
        }
        return hexagonalMapCells[arrayIndex];
    }

    public HexagonalMapCell GetOriginOffsetCell()
    {
        HexagonalMapCell result = new HexagonalMapCell(originOffset.x, originOffset.y, -originOffset.x - originOffset.y, hexagonalMapMgr.hexagonalMapCellRoot);
        if (result == null)
        {
            return null;
        }
        result.pos = GetHexagonWorldPos(result.q,result.r,Mathf.Sqrt(3),hexagonalMapMgr.cellSize);
        return result;
    }

    public HexagonalMapCell GetColumnLeftHexagonalMapCell(int line)
    {
        if (line < 0 || line >= columnLeftHexagonalMapCell.Length)
        {
            Debug.LogError("超出当前格子索引无法获取");
        }
        return columnLeftHexagonalMapCell[line];
    }
    #endregion

    #region Set
    public void ResetRoot()
    {
        for (int i = 0; i < hexagonalMapCells.Length; i++)
        {
            HexagonalMapCell hexagonalMapCell = hexagonalMapCells[i];
            if (hexagonalMapCell != null)
            {
                hexagonalMapCell.ResetCell();
            }
        }
    }
    public void DestoryMap()
    {
        if (hexagonalMapCells == null)
        {
            return;
        }
        hexagonalMapCells = null;
    }
    #endregion

    #region Cell
    public void AddCell(HexagonalMapCell cell)
    {
        switch (hexagonalMapMgr.mapType)
        {
            case MapType.Square:
                AddSquare(cell);
                break;
            case MapType.Hexagon:
                AddHexagon(cell);
                break;
            default:
                break;
        }
    }
    private void AddSquare(HexagonalMapCell cell)
    {

    }
    private void AddHexagon(HexagonalMapCell cell)
    {
        Vector2Int newPos = GetHexagonArrayPos(cell.q, cell.r);
        int index = GetHexagonArrayIndex(newPos);
        cell.arrayIndex = index;
        if (hexagonalMapCells[index] == null)
        {
            hexagonalMapCells[index] = cell;
            m_MapCount++;
        }
        else
        {
            HexagonalMapCell ori = hexagonalMapCells[index];
            Debug.LogError($"当前的格子已经存在发生碰撞 碰撞的格子为 ori{ori.q} {ori.r} {ori.s}  cur{cell.q} {cell.r} {cell.s}");
        }
    }
    #endregion
}
