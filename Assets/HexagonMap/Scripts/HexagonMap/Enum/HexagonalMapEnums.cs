#region 地图格子相关
/// <summary>
/// 地图格子的类型
/// </summary>
public enum TileType
{
    Empty,
    Road,//道路
    Wall//墙面
}
/// <summary>
/// 地图格子更新的状态
/// </summary>
public enum PathfindingState
{
    Unupdated,//还没有更新
    InBatch,//在更新批中
    Updated,//更新完成
    UDIS//这个格子不可达
}
/// <summary>
/// 地图格子的朝向
/// </summary>
public enum HexagonTopType
{
    FlatTop,    // 平顶
    PointyTop   // 尖顶
}
#endregion

#region 大地图相关
public enum MapType
{
    Square,
    Hexagon
}
#endregion
