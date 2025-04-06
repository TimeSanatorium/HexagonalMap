using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TESTPANEL : UIPanel
{
    public KeyCode ShowGizmos = KeyCode.F1;
    public KeyCode RandomWall = KeyCode.Q;
    public KeyCode RandomStart = KeyCode.Space;
    public KeyCode AddStart = KeyCode.A;
    public KeyCode RemoveStart = KeyCode.D;

    private GameObject currentShow;
    private void Start()
    {
        OnEnter();
    }
    public override void OnEnter()
    {
        base.OnEnter();
        InitPanel();
        EventAddition();
    }

    #region Init
    private void InitPanel()
    {
        GetOrAddComponent<Toggle>("drawing").isOn = HexagonalMapMgr.Current.showHexagonal.Drawing;
        GetOrAddComponent<Toggle>("drawWall").isOn = HexagonalMapMgr.Current.showHexagonal.DrawWall;
        GetOrAddComponent<Toggle>("drawRoad").isOn = HexagonalMapMgr.Current.showHexagonal.DrawRoad;
        GetOrAddComponent<Toggle>("drawPath").isOn = HexagonalMapMgr.Current.showHexagonal.DrawPath;
        GetOrAddComponent<Toggle>("drawDirection").isOn = HexagonalMapMgr.Current.showHexagonal.DrawDirection;
    }
    #endregion

    #region EventAddition
    private void EventAddition()
    {
        #region InputField
        GetOrAddComponent<TMP_InputField>("mapSize").onValueChanged.AddListener((value) => 
        {
            int size = 0;
            if (int.TryParse(value,out size) && size >= 1)
            {
                HexagonalMapMgr.Current._MapSize.x = size;
            }
        });
        #endregion

        #region Button
        GetOrAddComponent<Button>("createmMap").onClick.AddListener(() => 
        {
            HexagonalMapMgr.Current.CreateMap();
            GetOrAddComponent<TMP_Text>("mapCount").text = $"当前格子数量：{HexagonalMapMgr.Current.hexagonalMapCellRoot.GetCellCount()}";
        });
        GetOrAddComponent<Button>("randomWall").onClick.AddListener(() => { HexagonalMapMgr.Current.RandomWall(); });
        GetOrAddComponent<Button>("addStart").onClick.AddListener(() => { HexagonalMapMgr.Current.AddStart(); });
        GetOrAddComponent<Button>("removeStart").onClick.AddListener(() => { HexagonalMapMgr.Current.RemoveStart(); });
        GetOrAddComponent<Button>("resetStartPoint").onClick.AddListener(() => { HexagonalMapMgr.Current.ResetStartPoint(); });
        GetOrAddComponent<Button>("cameraReduce").onClick.AddListener(() => { Camera.main.orthographicSize -= 10; });  
        GetOrAddComponent<Button>("cameraMagnify").onClick.AddListener(() => { Camera.main.orthographicSize += 10; });
        #endregion

        #region Toggle
        GetOrAddComponent<Toggle>("drawing").onValueChanged.AddListener((isOn) => { HexagonalMapMgr.Current.showHexagonal.Drawing = isOn; });
        GetOrAddComponent<Toggle>("drawWall").onValueChanged.AddListener((isOn) => { HexagonalMapMgr.Current.showHexagonal.DrawWall = isOn; });
        GetOrAddComponent<Toggle>("drawRoad").onValueChanged.AddListener((isOn) => { HexagonalMapMgr.Current.showHexagonal.DrawRoad = isOn; });
        GetOrAddComponent<Toggle>("drawPath").onValueChanged.AddListener((isOn) => { HexagonalMapMgr.Current.showHexagonal.DrawPath = isOn; });
        GetOrAddComponent<Toggle>("drawDirection").onValueChanged.AddListener((isOn) => { HexagonalMapMgr.Current.showHexagonal.DrawDirection = isOn; });
        #endregion
    }
    #endregion
    private void Update()
    {
        if (Input.GetKeyDown(ShowGizmos))
        {
            if (currentShow != null)
            {
                currentShow.SetActive(false);
                currentShow = null;
            }
            currentShow = GetOrAddComponent<Transform>("gizmos").gameObject;
            currentShow.gameObject.SetActive(true);
        }
        if (Input.GetKeyDown(RandomWall))
        {
            HexagonalMapMgr.Current.RandomWall();
        }
        if (Input.GetKeyDown(RandomStart))
        {
            HexagonalMapMgr.Current.ResetStartPoint();
        }
        if (Input.GetKeyDown(AddStart))
        {
            HexagonalMapMgr.Current.AddStart();
        }
        if (Input.GetKeyDown(RemoveStart))
        {
            HexagonalMapMgr.Current.RemoveStart();
        }
        #region 滚轮缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            float size = Mathf.Clamp(Camera.main.orthographicSize - 200 * Time.deltaTime, 1, float.MaxValue);
            Camera.main.orthographicSize = size;
        }
        else if (scroll < 0f)
        {
            Camera.main.orthographicSize += 200 * Time.deltaTime;
        }
        #endregion
    }
}

public enum PaneState
{
    Empty,
    Hint
}
