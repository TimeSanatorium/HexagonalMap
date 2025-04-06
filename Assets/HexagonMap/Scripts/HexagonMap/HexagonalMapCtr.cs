using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 更新地图的热度值
/// </summary>
public class HexagonalMapCtr
{
    private HexagonalMapCellRoot m_hexagonalMapCellRoot;
    private HexagonalMapMgr m_hexagonalMapMgr;
    public HexagonalMapCtr(HexagonalMapMgr hexagonalMapMgr)
    {
        m_hexagonalMapMgr = hexagonalMapMgr;
        m_hexagonalMapCellRoot = hexagonalMapMgr.hexagonalMapCellRoot;
    }
    Queue<(int, HexagonalMapCell)> batch = new Queue<(int, HexagonalMapCell)>();
    public void SetHeatValue(Vector2Int target)
    {
        HexagonalMapCell startCell = m_hexagonalMapCellRoot.GetHexagonalMapCell(target.x, target.y, -target.x - target.y);
        if (startCell == null)
        {
            Debug.LogError("这里是空");
            return;
        }
        int heatValue = 0;
        //batch.Enqueue((heatValue, startCell));
        EntryBatch(heatValue, startCell,ref batch);
        #region 分批次运算整张地图的热度值
        while (batch.Count > 0)
        {
            int batchCount = batch.Count;
            for (int i = 0; i < batchCount; i++)
            {
                #region 更新值
                (int, HexagonalMapCell) hexagonalMapCell = batch.Dequeue();
                hexagonalMapCell.Item2.SetHeatValue(hexagonalMapCell.Item1);
                hexagonalMapCell.Item2.SetPathfindingState(PathfindingState.Updated);
                #endregion

                #region 将当前圈的外圈放到下一批中去
                HexagonalMapCell Round_1 = m_hexagonalMapMgr.hexagonalMapCellRoot.GetHexagonalMapCell(hexagonalMapCell.Item2.RoundIndex_1);
                HexagonalMapCell Round_2 = m_hexagonalMapMgr.hexagonalMapCellRoot.GetHexagonalMapCell(hexagonalMapCell.Item2.RoundIndex_2);
                HexagonalMapCell Round_3 = m_hexagonalMapMgr.hexagonalMapCellRoot.GetHexagonalMapCell(hexagonalMapCell.Item2.RoundIndex_3);
                HexagonalMapCell Round_4 = m_hexagonalMapMgr.hexagonalMapCellRoot.GetHexagonalMapCell(hexagonalMapCell.Item2.RoundIndex_4);
                HexagonalMapCell Round_5 = m_hexagonalMapMgr.hexagonalMapCellRoot.GetHexagonalMapCell(hexagonalMapCell.Item2.RoundIndex_5);
                HexagonalMapCell Round_6 = m_hexagonalMapMgr.hexagonalMapCellRoot.GetHexagonalMapCell(hexagonalMapCell.Item2.RoundIndex_6);
                if (EntryBatch(hexagonalMapCell.Item1 + 1, Round_1, ref batch)) { Round_1.NearCellIndex = hexagonalMapCell.Item2.arrayIndex; }
                if (EntryBatch(hexagonalMapCell.Item1 + 1, Round_2, ref batch)) { Round_2.NearCellIndex = hexagonalMapCell.Item2.arrayIndex; }
                if (EntryBatch(hexagonalMapCell.Item1 + 1, Round_3, ref batch)) { Round_3.NearCellIndex = hexagonalMapCell.Item2.arrayIndex; }
                if (EntryBatch(hexagonalMapCell.Item1 + 1, Round_4, ref batch)) { Round_4.NearCellIndex = hexagonalMapCell.Item2.arrayIndex; }
                if (EntryBatch(hexagonalMapCell.Item1 + 1, Round_5, ref batch)) { Round_5.NearCellIndex = hexagonalMapCell.Item2.arrayIndex; }
                if (EntryBatch(hexagonalMapCell.Item1 + 1, Round_6, ref batch)) { Round_6.NearCellIndex = hexagonalMapCell.Item2.arrayIndex; }
                #endregion
            }
            heatValue++;
        }
        #endregion
        //Debug.Log($"更新的批次为{heatValue}");
    }
    private bool EntryBatch(int batchIndex, HexagonalMapCell hexagonalMapCell, ref Queue<(int, HexagonalMapCell)> batch)
    {
        if (hexagonalMapCell == null)
        {
            return false;
        }
        if (hexagonalMapCell.GetPathfindingState() == PathfindingState.Unupdated)
        {
            hexagonalMapCell.SetPathfindingState(PathfindingState.InBatch);
            batch.Enqueue((batchIndex, hexagonalMapCell));
            return true;
        }
        else
        {
            return false;
        }
    }
    HexagonalMapCell[] batchs;
}
