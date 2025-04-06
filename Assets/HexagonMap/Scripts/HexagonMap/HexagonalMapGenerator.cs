using UnityEngine;
/// <summary>
/// 地图生成器
/// </summary>
public class HexagonalMapGenerator
{
    private HexagonalMapMgr m_hexagonalMapMgr;
    public HexagonalMapGenerator(HexagonalMapMgr hexagonalMapMgr)
    {
        m_hexagonalMapMgr = hexagonalMapMgr;
    }
    public void CreateMap()
    {
        switch (m_hexagonalMapMgr.mapType)
        {
            case MapType.Square:
                CreateMapSquare(m_hexagonalMapMgr._MapSize.x, m_hexagonalMapMgr._MapSize.y);
                break;
            case MapType.Hexagon:
                CreateMapHexagon(Mathf.Max(m_hexagonalMapMgr._MapSize.x, m_hexagonalMapMgr._MapSize.y), m_hexagonalMapMgr.cellSize);
                break;
            default:
                break;
        }
    }
    private void CreateMapSquare(int mapWidth, int mapHeight,float cellsize = 1)
    {
        #region 未适配存档信息 这里内容需要重新写
        for (int y = 0; y < mapHeight; y++)
        {
            int x_Start = Mathf.FloorToInt((y + 1) / 2);
            for (int x = 0; x < mapWidth; x++)
            {
                Vector3 pos = new Vector3(y % 2 == 0 ? x : x + 0.5f, 0, y * Mathf.Sqrt(3) / 2) * cellsize;
                Vector3Int qrs = new Vector3Int(x + x_Start, -y, y - x - x_Start);
                HexagonalMapCell hexagonalMapCell = CreateHexagonalMap(qrs);
                hexagonalMapCell.pos = pos;
                m_hexagonalMapMgr.hexagonalMapCellRoot.AddCell(hexagonalMapCell);
            }
        }
        #endregion
    }


    private void CreateMapHexagon(int radious,float cellsize = 1)
    {
        float Sqrt_3 = Mathf.Sqrt(3);//内直径
        #region 创建出原点
        HexagonalMapCell orihexagonalMapCell = CreateHexagonalMap(Vector3Int.zero);
        orihexagonalMapCell.pos = Vector3.zero * cellsize;
        orihexagonalMapCell.cellsize = cellsize;
        m_hexagonalMapMgr.hexagonalMapCellRoot.AddCell(orihexagonalMapCell);
        #endregion
        for (int i = 0; i < radious; i++)
        {
            Vector3Int qrs_0 = new Vector3Int(i, 0, -i);
            for (int j = 0; j < i; j++)
            {
                Vector3Int curqrs = new Vector3Int(qrs_0.x - j, qrs_0.y + j, qrs_0.z);
                HexagonalMapCell hexagonalMapCell = CreateHexagonalMap(curqrs);
                hexagonalMapCell.pos = m_hexagonalMapMgr.hexagonalMapCellRoot.GetHexagonWorldPos(hexagonalMapCell.q, hexagonalMapCell.r, Sqrt_3, cellsize);
                hexagonalMapCell.cellsize = cellsize;
                m_hexagonalMapMgr.hexagonalMapCellRoot.AddCell(hexagonalMapCell);
            }
            Vector3Int qrs_1 = new Vector3Int(0, i, -i);
            for (int j = 0; j < i; j++)
            {
                Vector3Int curqrs = new Vector3Int(qrs_1.x - j, qrs_1.y, qrs_1.z + j);
                HexagonalMapCell hexagonalMapCell = CreateHexagonalMap(curqrs);
                hexagonalMapCell.pos = m_hexagonalMapMgr.hexagonalMapCellRoot.GetHexagonWorldPos(hexagonalMapCell.q, hexagonalMapCell.r, Sqrt_3, cellsize);
                hexagonalMapCell.cellsize = cellsize;
                m_hexagonalMapMgr.hexagonalMapCellRoot.AddCell(hexagonalMapCell);
            }
            Vector3Int qrs_2 = new Vector3Int(-i, i, 0);
            for (int j = 0; j < i; j++)
            {
                Vector3Int curqrs = new Vector3Int(qrs_2.x, qrs_2.y - j, qrs_2.z + j);
                HexagonalMapCell hexagonalMapCell = CreateHexagonalMap(curqrs);
                hexagonalMapCell.pos = m_hexagonalMapMgr.hexagonalMapCellRoot.GetHexagonWorldPos(hexagonalMapCell.q, hexagonalMapCell.r, Sqrt_3, cellsize);
                hexagonalMapCell.cellsize = cellsize;
                m_hexagonalMapMgr.hexagonalMapCellRoot.AddCell(hexagonalMapCell);
            }
            Vector3Int qrs_3 = new Vector3Int(-i, 0, i);
            for (int j = 0; j < i; j++)
            {
                Vector3Int curqrs = new Vector3Int(qrs_3.x + j, qrs_3.y - j, qrs_3.z);
                HexagonalMapCell hexagonalMapCell = CreateHexagonalMap(curqrs);
                hexagonalMapCell.pos = m_hexagonalMapMgr.hexagonalMapCellRoot.GetHexagonWorldPos(hexagonalMapCell.q, hexagonalMapCell.r, Sqrt_3, cellsize);
                hexagonalMapCell.cellsize = cellsize;
                m_hexagonalMapMgr.hexagonalMapCellRoot.AddCell(hexagonalMapCell);
            }
            Vector3Int qrs_4 = new Vector3Int(0, -i, i);
            for (int j = 0; j < i; j++)
            {
                Vector3Int curqrs = new Vector3Int(qrs_4.x + j, qrs_4.y, qrs_4.z - j);
                HexagonalMapCell hexagonalMapCell = CreateHexagonalMap(curqrs);
                hexagonalMapCell.pos = m_hexagonalMapMgr.hexagonalMapCellRoot.GetHexagonWorldPos(hexagonalMapCell.q, hexagonalMapCell.r, Sqrt_3, cellsize);
                hexagonalMapCell.cellsize = cellsize;
                m_hexagonalMapMgr.hexagonalMapCellRoot.AddCell(hexagonalMapCell);
            }
            Vector3Int qrs_5 = new Vector3Int(i, -i, 0);
            for (int j = 0; j < i; j++)
            {
                Vector3Int curqrs = new Vector3Int(qrs_5.x, qrs_5.y + j, qrs_5.z - j);
                HexagonalMapCell hexagonalMapCell = CreateHexagonalMap(curqrs);
                hexagonalMapCell.pos = m_hexagonalMapMgr.hexagonalMapCellRoot.GetHexagonWorldPos(hexagonalMapCell.q, hexagonalMapCell.r, Sqrt_3, cellsize);
                hexagonalMapCell.cellsize = cellsize;
                m_hexagonalMapMgr.hexagonalMapCellRoot.AddCell(hexagonalMapCell);
            }
        }
    }

    #region Other
    private HexagonalMapCell CreateHexagonalMap(Vector3Int qrs)
    {
        HexagonalMapCell result = new HexagonalMapCell(qrs.x, qrs.y, qrs.z, m_hexagonalMapMgr.hexagonalMapCellRoot);
        return result;
    }
    #endregion
}
