using System.Collections.Generic;
public class HexagonalPath
{
    public List<int> pathIndexs = new List<int>();
    public bool reachable;
    #region ≥ı ºªØ
    public HexagonalPath()
    {
        pathIndexs = new List<int>();
    }
    #endregion

    #region Add
    public void AddPathPoint(int pathIndex)
    {
        pathIndexs.Add(pathIndex);
    }
    #endregion

    #region Get
    public int GetPathPointCount()
    {
        return pathIndexs.Count;
    }
    public bool GetPointInPath(int cellIndex)
    {
        return pathIndexs.Contains(cellIndex);
    }
    #endregion

    #region Set
    public void Clear()
    {
        pathIndexs.Clear();
        reachable = false;
    }
    #endregion
}
