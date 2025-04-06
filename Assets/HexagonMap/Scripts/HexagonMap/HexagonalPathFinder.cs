using System;
using UnityEngine;

public class HexagonalPathFinder
{
    public HexagonalMapCellRoot hexagonalMapCellRoot;
    public Action<HexagonalPath> _PathFindingComplete;
    private HexagonalPath hexagonalPath = new HexagonalPath();
    public HexagonalPathFinder(HexagonalMapCellRoot hexagonalMapCellRoot)
    {
        this.hexagonalMapCellRoot = hexagonalMapCellRoot;
    }
    public void UpdatePath(Vector2Int start,Vector2Int target)
    {
        if (hexagonalPath == null)
        {
            hexagonalPath = new HexagonalPath();
        }
        else
        {
            hexagonalPath.Clear();
        }
        HexagonalMapCell start_cell = hexagonalMapCellRoot.GetHexagonalMapCell(start.x, start.y, -start.x - start.y);
        HexagonalMapCell target_cell = hexagonalMapCellRoot.GetHexagonalMapCell(target.x, target.y, -target.x - target.y);
        if (start_cell == null || target_cell == null)
        {
            Debug.LogError("有不存在格子点的数据");
            return;
        }

        HexagonalMapCell pointer = start_cell;
        while (pointer != null && pointer.NearCellIndex != -1)
        {
            hexagonalPath.AddPathPoint(pointer.arrayIndex);

            #region Gizemos Test
            hexagonalMapCellRoot.hexagonalMapMgr.PathIndex.TryAdd(pointer.NearCellIndex, pointer.NearCellIndex);
            #endregion

            pointer = hexagonalMapCellRoot.GetHexagonalMapCell(pointer.NearCellIndex);
        }
        if (pointer == null)
        {
            hexagonalPath.reachable = false;
        }
        else
        {
            hexagonalPath.AddPathPoint(pointer.arrayIndex);
            hexagonalPath.reachable = pointer.arrayIndex == target_cell.arrayIndex;
        }
        _PathFindingComplete?.Invoke(hexagonalPath);
    }
    public bool GetPointInPath(int arrayIndex)
    {
        return hexagonalPath.GetPointInPath(arrayIndex);
    }
}
