using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
public class ShowHexagonalMap : MonoBehaviour
{
    public HexagonalMapCellRoot hexagonalMapCellRoot;
    public HexagonalMapMgr hexagonalMapMgr;
    public Color DefaltColor;
    private HexagonalMapCell[] drawCompletehexagonalMapCells;
    public int GreenIndex;
    public bool Drawing;
    public bool DrawOriginOffset;
    public bool DrawDirection;
    public bool DrawRoad;
    public bool DrawWall;
    public bool DrawPath;
    private void LateUpdate()
    {
        if (!hexagonalMapMgr.InitializationComplete)
        {
            Debug.Log("还没初始化完成");
            return;
        }
        #region 设置当前鼠标的位置点
        //Debug.Log("计算当前索引");
        GreenIndex = hexagonalMapCellRoot.GetHexagonArrayIndex(GetScreenToWorldCamerForward());
        HexagonalMapCell mouseCell = hexagonalMapCellRoot.GetHexagonalMapCell(GreenIndex);
        if (mouseCell == null) return;
        hexagonalMapMgr.currentTarget = new Vector2Int(mouseCell.q, mouseCell.r);
        #endregion        
    }
    private void OnDrawGizmos()
    {
        if (!Drawing|| hexagonalMapCellRoot.hexagonalMapCells == null)
        {
            return;
        }
        if (drawCompletehexagonalMapCells == null || drawCompletehexagonalMapCells.Length != hexagonalMapCellRoot.hexagonalMapCells.Length)
        {
            drawCompletehexagonalMapCells = new HexagonalMapCell[hexagonalMapCellRoot.hexagonalMapCells.Length];
        }

        #region 六边形绘制
        #region 绘制出所有的六边形
        foreach (HexagonalMapCell cell in hexagonalMapCellRoot.hexagonalMapCells)
        {
            if (cell != null && DrawHexagonal(cell))
            {
                drawCompletehexagonalMapCells[cell.arrayIndex] = cell;
            }
        }
        #endregion
        #region 制空之前绘制的六边形
        for (int i = 0; i < drawCompletehexagonalMapCells.Length; i++)
        {
            if (drawCompletehexagonalMapCells[i] != null)
            {
                drawCompletehexagonalMapCells[i] = null;
            }
        }
        #endregion
        #endregion

        if (DrawOriginOffset)
        {
            DrawMapOriginOffset();
        }
    }

    #region 绘制六边形
    private bool DrawHexagonal(HexagonalMapCell cell)
    {
        bool result = false;
        HexagonalMapCell Round_1 = cell.RoundIndex_1 == -1 ? null : hexagonalMapCellRoot.hexagonalMapCells[cell.RoundIndex_1];
        HexagonalMapCell Round_2 = cell.RoundIndex_2 == -1 ? null : hexagonalMapCellRoot.hexagonalMapCells[cell.RoundIndex_2];
        HexagonalMapCell Round_3 = cell.RoundIndex_3 == -1 ? null : hexagonalMapCellRoot.hexagonalMapCells[cell.RoundIndex_3];
        HexagonalMapCell Round_4 = cell.RoundIndex_4 == -1 ? null : hexagonalMapCellRoot.hexagonalMapCells[cell.RoundIndex_4];
        HexagonalMapCell Round_5 = cell.RoundIndex_5 == -1 ? null : hexagonalMapCellRoot.hexagonalMapCells[cell.RoundIndex_5];
        HexagonalMapCell Round_6 = cell.RoundIndex_6 == -1 ? null : hexagonalMapCellRoot.hexagonalMapCells[cell.RoundIndex_6];

        if (hexagonalMapMgr.GetPointInPath(cell.arrayIndex) && DrawPath)
        {
            DrawLine(cell, Round_5, cell.PointIndex_1, cell.PointIndex_2);
            DrawLine(cell, Round_6, cell.PointIndex_2, cell.PointIndex_3);
            DrawLine(cell, Round_4, cell.PointIndex_6, cell.PointIndex_1);
            DrawLine(cell, Round_1, cell.PointIndex_3, cell.PointIndex_4);
            DrawLine(cell, Round_2, cell.PointIndex_4, cell.PointIndex_5);
            DrawLine(cell, Round_3, cell.PointIndex_5, cell.PointIndex_6);
            result = true;
        }

        switch (cell.GetTileType())
        {
            case TileType.Empty:
                break;
            case TileType.Road:
                if (DrawRoad)
                {
                    DrawLine(cell, Round_5, cell.PointIndex_1, cell.PointIndex_2);
                    DrawLine(cell, Round_6, cell.PointIndex_2, cell.PointIndex_3);
                    DrawLine(cell, Round_4, cell.PointIndex_6, cell.PointIndex_1);
                    DrawLine(cell, Round_1, cell.PointIndex_3, cell.PointIndex_4);
                    DrawLine(cell, Round_2, cell.PointIndex_4, cell.PointIndex_5);
                    DrawLine(cell, Round_3, cell.PointIndex_5, cell.PointIndex_6);
                    result = true;
                }
                break;
            case TileType.Wall:
                if (DrawWall)
                {
                    DrawLine(cell, Round_5, cell.PointIndex_1, cell.PointIndex_2);
                    DrawLine(cell, Round_6, cell.PointIndex_2, cell.PointIndex_3);
                    DrawLine(cell, Round_4, cell.PointIndex_6, cell.PointIndex_1);
                    DrawLine(cell, Round_1, cell.PointIndex_3, cell.PointIndex_4);
                    DrawLine(cell, Round_2, cell.PointIndex_4, cell.PointIndex_5);
                    DrawLine(cell, Round_3, cell.PointIndex_5, cell.PointIndex_6);
                    result = true;
                }
                break;
            default:
                break;
        }

        #region 绘制箭头方向
        HexagonalMapCell target = hexagonalMapCellRoot.GetHexagonalMapCell(cell.NearCellIndex);
        if (DrawDirection && target != null)
        {
            DrawDirectionArrow(cell.pos,target.pos);
        }
        #endregion
        return result;
    }
    /// <summary>
    /// 绘制六边形边,根据相邻的两个六边形的状态来决定绘制的颜色
    /// </summary>
    /// <param name="curCell">当前的六边形</param>
    /// <param name="otherCell">共用这条边的六边形</param>
    /// <param name="lineIndex_1">第一个顶点</param>
    /// <param name="lineIndex_2">第二个顶点</param>
    private void DrawLine(HexagonalMapCell curCell, HexagonalMapCell otherCell,int lineIndex_1,int lineIndex_2)
    {
        if (otherCell != null && drawCompletehexagonalMapCells[otherCell.arrayIndex] != null) 
        {
            return;
        }
        Gizmos.color = GetLineColor(curCell,otherCell);
        Gizmos.DrawLine(hexagonalMapCellRoot.vertexs[lineIndex_1], hexagonalMapCellRoot.vertexs[lineIndex_2]);
    }

    /// <summary>
    /// 获取到这条边的颜色 后续如果有具体的需求直接改这里
    /// </summary>
    /// <param name="curCell">当前的六边形</param>
    /// <param name="otherCell">相邻的六边形</param>
    /// <returns></returns>
    private Color GetLineColor(HexagonalMapCell curCell, HexagonalMapCell otherCell)
    {
        Color result = DefaltColor;
        if (curCell.GetTileType() == TileType.Wall ||(otherCell != null && otherCell.GetTileType() == TileType.Wall))
        {
            result = Color.black;
        }
        if (hexagonalMapMgr.GetPointInPath(curCell.arrayIndex) ||
            (otherCell != null && hexagonalMapMgr.GetPointInPath(otherCell.arrayIndex)))
        {
            result = Color.green;
        }
        return result;
    }
    /// <summary>
    /// 绘制一个方向箭头
    /// </summary>
    /// <param name="start">起始点</param>
    /// <param name="end">终结点</param>
    private void DrawDirectionArrow(Vector3 start,Vector3 end)
    {
        Color ori = Gizmos.color;
        Gizmos.color = Color.green;
        Vector3 dir = end - start;
        if (dir == Vector3.zero)
        {
            Gizmos.DrawSphere(start,0.5f);
        }
        else
        {
            Handles.ArrowHandleCap(
                controlID: 0,
                position: start,
                rotation: Quaternion.LookRotation(dir),
                size: 1,
                eventType: EventType.Repaint
            );
        }

        Gizmos.color = ori;
    }
    #endregion

    #region 获取当前鼠标在Scene面板上世界坐标的位置
    private Vector3 GetScreenToWorldCamerForward()
    {
        //Event e = Event.current;
        Vector3 result = Vector3.zero;
        //if (e == null) return result;
        //Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        //result = ray.origin + ray.direction * 10f;
        result = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return result;
    }
    #endregion

    #region Other
    private void DrawMapOriginOffset()
    {
        HexagonalMapCell hexagonalMapCell = hexagonalMapCellRoot.GetOriginOffsetCell();
        if (hexagonalMapCell == null)
        {
            Debug.LogError("原点获取失败");
        }

        #region 计算出六个顶点
        float innerRadius = Mathf.Sqrt(3) / 2;
        Vector3 pos_1 = hexagonalMapCell.pos + new Vector3(-innerRadius, 0, 0.5f) * hexagonalMapMgr.cellSize;
        Vector3 pos_2 = hexagonalMapCell.pos + new Vector3(0, 0, 1) * hexagonalMapMgr.cellSize;
        Vector3 pos_3 = hexagonalMapCell.pos + new Vector3(innerRadius, 0, 0.5f) * hexagonalMapMgr.cellSize;
        Vector3 pos_4 = hexagonalMapCell.pos + new Vector3(innerRadius, 0, -0.5f) * hexagonalMapMgr.cellSize;
        Vector3 pos_5 = hexagonalMapCell.pos + new Vector3(0, 0, -1f) * hexagonalMapMgr.cellSize;
        Vector3 pos_6 = hexagonalMapCell.pos + new Vector3(-innerRadius, 0, -0.5f) * hexagonalMapMgr.cellSize;
        #endregion

        #region 绘制出六个边
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pos_1, pos_2);
        Gizmos.DrawLine(pos_2, pos_3);
        Gizmos.DrawLine(pos_3, pos_4);
        Gizmos.DrawLine(pos_4, pos_5);
        Gizmos.DrawLine(pos_5, pos_6);
        Gizmos.DrawLine(pos_6, pos_1);
        #endregion

    }
    #endregion
}

