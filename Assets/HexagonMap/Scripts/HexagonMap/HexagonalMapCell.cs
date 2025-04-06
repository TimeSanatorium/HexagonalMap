using UnityEngine;
[System.Serializable]
public class HexagonalMapCell
{
    public TileType tileType = TileType.Road;
    public int q, r, s,arrayIndex;
    public float heatValue;
    public PathfindingState pathfindingState = PathfindingState.Updated;
    public int NearCellIndex;
    public int RoundIndex_1, RoundIndex_2, RoundIndex_3, RoundIndex_4, RoundIndex_5, RoundIndex_6;
    public int PointIndex_1, PointIndex_2, PointIndex_3, PointIndex_4, PointIndex_5, PointIndex_6;
    private HexagonalMapCellRoot hexagonalMapCellRoot;
    public Vector3 pos;
    public float cellsize;

    #region 初始化
    public HexagonalMapCell(int q,int r,int s ,HexagonalMapCellRoot hexagonalMapCellRoot)
    {
        this.q = q;
        this.r = r;
        this.s = s;
        this.hexagonalMapCellRoot = hexagonalMapCellRoot;
        NearCellIndex = -1;
    }
    public void InitializationData()
    {
        InitializationRoundCell();
        InitializationPoint();
    }
    private void InitializationRoundCell()
    {
        HexagonalMapCell Round_1 = hexagonalMapCellRoot.GetHexagonalMapCell(q + 1, r, s - 1);
        HexagonalMapCell Round_2 = hexagonalMapCellRoot.GetHexagonalMapCell(q, r + 1, s - 1);
        HexagonalMapCell Round_3 = hexagonalMapCellRoot.GetHexagonalMapCell(q - 1, r + 1, s);
        HexagonalMapCell Round_4 = hexagonalMapCellRoot.GetHexagonalMapCell(q - 1, r, s + 1);
        HexagonalMapCell Round_5 = hexagonalMapCellRoot.GetHexagonalMapCell(q, r - 1, s + 1);
        HexagonalMapCell Round_6 = hexagonalMapCellRoot.GetHexagonalMapCell(q + 1, r - 1, s);
        RoundIndex_1 = Round_1 == null ? -1 : Round_1.arrayIndex;
        RoundIndex_2 = Round_2 == null ? -1 : Round_2.arrayIndex;
        RoundIndex_3 = Round_3 == null ? -1 : Round_3.arrayIndex;
        RoundIndex_4 = Round_4 == null ? -1 : Round_4.arrayIndex;
        RoundIndex_5 = Round_5 == null ? -1 : Round_5.arrayIndex;
        RoundIndex_6 = Round_6 == null ? -1 : Round_6.arrayIndex;
    }
    private void InitializationPoint()
    {
        switch (hexagonalMapCellRoot.hexagonTop)
        {
            case HexagonTopType.FlatTop:
                Debug.LogError("平顶的部分还没弄");
                break;
            case HexagonTopType.PointyTop:
                InitializationPointyTopPoint();
                break;
            default:
                break;
        }
    }
    private void InitializationPointyTopPoint()
    {
        Vector2Int newpos = hexagonalMapCellRoot.GetHexagonArrayPos(q, r);
        int upL, downL;
        if (newpos.y == 0)
        {
            upL = 0;
        }
        else
        {
            upL = newpos.y* 2 * (1 + hexagonalMapCellRoot.m_ContainerLength) + 1;
        }
        downL = (newpos.y + 1) * 2 * (1 + hexagonalMapCellRoot.m_ContainerLength);
        PointIndex_1 = upL + newpos.x * 2;
        PointIndex_2 = PointIndex_1 + 1;
        PointIndex_3 = PointIndex_2 + 1;
        PointIndex_6 = downL + newpos.x * 2;
        PointIndex_5 = PointIndex_6 + 1;
        PointIndex_4 = PointIndex_5 + 1;
        float innerRadius = Mathf.Sqrt(3) / 2;
        if (hexagonalMapCellRoot.vertexs[PointIndex_1] == Vector3.zero)
        {
            hexagonalMapCellRoot.vertexs[PointIndex_1] = pos + new Vector3(-innerRadius, 0, 0.5f) * cellsize;
        }
        if (hexagonalMapCellRoot.vertexs[PointIndex_2] == Vector3.zero)
        {
            hexagonalMapCellRoot.vertexs[PointIndex_2] = pos + new Vector3(0, 0, 1) * cellsize;
        }
        if (hexagonalMapCellRoot.vertexs[PointIndex_3] == Vector3.zero)
        {
            hexagonalMapCellRoot.vertexs[PointIndex_3] = pos + new Vector3(innerRadius, 0, 0.5f) * cellsize;
        }
        if (hexagonalMapCellRoot.vertexs[PointIndex_4] == Vector3.zero)
        {
            hexagonalMapCellRoot.vertexs[PointIndex_4] = pos + new Vector3(innerRadius, 0, -0.5f) * cellsize;
        }
        if (hexagonalMapCellRoot.vertexs[PointIndex_5] == Vector3.zero)
        {
            hexagonalMapCellRoot.vertexs[PointIndex_5] = pos + new Vector3(0, 0, -1f) * cellsize;
        }
        if (hexagonalMapCellRoot.vertexs[PointIndex_6] == Vector3.zero)
        {
            hexagonalMapCellRoot.vertexs[PointIndex_6] = pos + new Vector3(-innerRadius, 0, -0.5f) * cellsize;
        }
    }
    #endregion

    #region Get
    public float GetHeatValue()
    {
        return heatValue;
    }
    public TileType GetTileType()
    {
        return tileType;
    }
    public PathfindingState GetPathfindingState()
    {
        return pathfindingState;
    }
    #endregion

    #region Set
    public void SetHeatValue(float heatValue)
    {
        this.heatValue = heatValue;
    }
    public void SetTileType(TileType tileType)
    {
        this.tileType = tileType;
    }
    public void SetPathfindingState(PathfindingState pathfindingState)
    {
        this.pathfindingState = pathfindingState;
    }
    public void ResetCell()
    {
        heatValue = 0;
        tileType = TileType.Road;
        pathfindingState = PathfindingState.Unupdated;
        NearCellIndex = -1;
    }
    #endregion
}

