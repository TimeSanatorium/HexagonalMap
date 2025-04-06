using System.Collections.Generic;
using UnityEngine;
public class HexagonalMapMgr : SingleMonoBehaviour<HexagonalMapMgr>
{
    public MapType mapType;
    public HexagonTopType hexagonTopType;
    public Vector2Int _MapSize;
    public Vector2Int currentTarget;
    public List<Vector2Int> currentStarts = new List<Vector2Int>();
    public HexagonalMapCellRoot hexagonalMapCellRoot;
    public HexagonalMapGenerator hexagonalMapGenerator;
    public HexagonalMapCtr hexagonalMapCtr;
    public ShowHexagonalMap showHexagonal;
    public List<HexagonalPathFinder> hexagonalPathFinders = new List<HexagonalPathFinder>();
    public bool InitializationComplete;

    #region 单个格子相关信息
    public float cellSize;
    public float InsideRadius, ExternalRadius;
    #endregion
    public GameObject hexPrefab;
    [Button("创建地图")]
    public void CreateMap()
    {
        hexagonalMapCellRoot.InitializeHexagonalMapRoot(this, _MapSize);
        if (hexagonalMapGenerator == null)
        {
            hexagonalMapGenerator = new HexagonalMapGenerator(this);
        }
        hexagonalMapGenerator.CreateMap();
        hexagonalMapCellRoot.InitializeLinkedList();
        InsideRadius = cellSize * Mathf.Sqrt(3) * 0.5f;
        ExternalRadius = cellSize;
        //Debug.Log($"地图创建完成创建的地图数量为{hexagonalMapCellRoot.GetCellCount()}");
    }
    [Button("更新当前地图的热度值")]
    public void SetHeatValue()
    {
        if (hexagonalMapCtr == null)
        {
            hexagonalMapCtr = new HexagonalMapCtr(this);
        }
        hexagonalMapCtr.SetHeatValue(currentTarget);
    }
    [Button("删除当前地图")]
    public void ClearMapData()
    {
        hexagonalMapCellRoot.DestoryMap();
    }
    [Button("预制体创建出对应的显示")]
    public void CreatePrefab()
    {
        CellInfo[] cellInfos = new CellInfo[hexagonalMapCellRoot.hexagonalMapCells.Length];
        foreach (HexagonalMapCell cell in hexagonalMapCellRoot.hexagonalMapCells)
        {
            if (cell == null)
            {
                continue;
            }
            GameObject obj = GameObject.Instantiate(hexPrefab);
            obj.SetActive(true);
            obj.transform.position = cell.pos;
            CellInfo cellInfo = obj.GetComponent<CellInfo>();
            cellInfo.Q = cell.q;
            cellInfo.R = cell.r;
            cellInfo.S = cell.s;
            cellInfo.ArrayPos = hexagonalMapCellRoot.GetHexagonArrayPos(cell.q, cell.r);
            cellInfo.ArrayIndex = hexagonalMapCellRoot.GetHexagonArrayIndex(cellInfo.ArrayPos);
            cellInfos[cellInfo.ArrayIndex] = cellInfo;
        }
        foreach (CellInfo cellinfo in cellInfos)
        {
            if (cellinfo == null)
            {
                continue;
            }
            HexagonalMapCell hexagonalMapCell = hexagonalMapCellRoot.hexagonalMapCells[cellinfo.ArrayIndex];
            cellinfo.Round_1 = hexagonalMapCell.RoundIndex_1 == -1 ? null : cellInfos[hexagonalMapCell.RoundIndex_1];
            cellinfo.Round_2 = hexagonalMapCell.RoundIndex_2 == -1 ? null : cellInfos[hexagonalMapCell.RoundIndex_2];
            cellinfo.Round_3 = hexagonalMapCell.RoundIndex_3 == -1 ? null : cellInfos[hexagonalMapCell.RoundIndex_3];
            cellinfo.Round_4 = hexagonalMapCell.RoundIndex_4 == -1 ? null : cellInfos[hexagonalMapCell.RoundIndex_4];
            cellinfo.Round_5 = hexagonalMapCell.RoundIndex_5 == -1 ? null : cellInfos[hexagonalMapCell.RoundIndex_5];
            cellinfo.Round_6 = hexagonalMapCell.RoundIndex_6 == -1 ? null : cellInfos[hexagonalMapCell.RoundIndex_6];
        }
    }
    [Button("随机生成墙面")]
    public void RandomWall()
    {
        ResetMapData();
        foreach (HexagonalMapCell map in hexagonalMapCellRoot.hexagonalMapCells)
        {
            if (map != null)
            {
                float rat = Random.Range(0,1f);
                if (rat > 0.8f)
                {
                    map.SetTileType(TileType.Wall);
                    map.SetPathfindingState(PathfindingState.UDIS);
                }
                else
                {
                    hexagonalMapCellRoot.Roads.Add(map.arrayIndex);
                }
            }
        }
    }
    [Button("清除当前地图数据")]
    public void ResetMapData()
    {
        hexagonalMapCellRoot.ResetRoot();
    }
    [Button("添加一个开始点")]
    public void AddStart()
    {
        HexagonalMapCell startCell = hexagonalMapCellRoot.GetHexagonalMapCell(hexagonalMapCellRoot.GetRandomRoad());
        currentStarts.Add(new Vector2Int(startCell.q, startCell.r));
    }
    [Button("删除一个起始点")]
    public void RemoveStart()
    {
        if (currentStarts.Count > 1)
        {
            currentStarts.RemoveAt(currentStarts.Count - 1);
        }
    }
    [Button("随机刷新起始点")]
    public void ResetStartPoint()
    {
        int count = hexagonalMapCellRoot.Roads.Count;
        if (count <= currentStarts.Count)
        {
            return;
        }
        int step = Mathf.FloorToInt(count / (currentStarts.Count + 1));
        for (int i = 1; i < currentStarts.Count; i++)
        {
            int index = hexagonalMapCellRoot.Roads[Random.Range((i - 1) * step, i * step)];
            HexagonalMapCell hexagonalMapCell = hexagonalMapCellRoot.GetHexagonalMapCell(index);
            currentStarts[i - 1] = new Vector2Int(hexagonalMapCell.q, hexagonalMapCell.r);
        }
        UpdatePath();
    }

    [Button("初始化当前的路径数据")]
    private void ResetRoad()
    {
        foreach (HexagonalMapCell map in hexagonalMapCellRoot.hexagonalMapCells)
        {
            if (map != null && map.GetTileType() == TileType.Road)
            {
                map.ResetCell();
            }
        }
    }

    #region 测试相关内容
    private Vector2Int target;
    private void Update()
    {
        InitializationComplete = !(hexagonalMapCellRoot.vertexs.Length <= 0);
        if (!InitializationComplete)
        {
            return;
        }
        if (target != currentTarget)
        {
            target = currentTarget;
            ResetRoad();
            SetHeatValue();
            UpdatePath();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RandomWall();
            UpdatePath();
        }
    }
    public void UpdatePath()
    {
        PathIndex.Clear();
        if (hexagonalPathFinders.Count < currentStarts.Count)
        {
            for (int i = hexagonalPathFinders.Count; i < currentStarts.Count; i++)
            {
                hexagonalPathFinders.Add(new HexagonalPathFinder(hexagonalMapCellRoot));
            }
        }
        for (int i = 0; i < currentStarts.Count; i++)
        {
            HexagonalPathFinder finder = hexagonalPathFinders[i];
            finder.UpdatePath(currentStarts[i], currentTarget);
        }
    }
    #region Gizmos Test
    public Dictionary<int, int> PathIndex = new Dictionary<int, int>();
    #endregion
    public bool GetPointInPath(int arrayIndex)
    {
        bool result = PathIndex.ContainsKey(arrayIndex);
        return result;
    }
    #endregion
}
