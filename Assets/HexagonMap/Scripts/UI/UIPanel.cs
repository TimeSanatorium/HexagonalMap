//宇宙第一帅的男人写的代码
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// 面板基类
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class UIPanel : MonoBehaviour
{
    public Dictionary<string, Transform> allChild;
    
    #region Event
    public virtual void OnEnter()
    {
        GetAllChild(transform);
    }
    public virtual void OnPause()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
    }
    public virtual void OnContinue() 
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.interactable = true;
    }
    public virtual void OnExit() 
    {
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
    public virtual void OnClose()
    {
    }
    #endregion

    private void GetAllChild(Transform root)
    {
        if (allChild==null)
        {
            allChild = new Dictionary<string, Transform>();
        }
        foreach (Transform child in root)
        {
            if (!allChild.ContainsKey(child.name) && !child.name.StartsWith('_'))//同名、特殊标识不处理的东西
            {
                allChild.Add(child.name, child);
                if (child.childCount>0)
                {
                    GetAllChild(child);
                }
            }
        }
    }
    public T GetOrAddComponent<T>(Transform tra)where T : Component
    {
        T comp = tra.GetComponent<T>();
        if (comp == null)
        {
            comp = tra.gameObject.AddComponent<T>();
        }
        return comp;
    }
    public T GetOrAddComponent<T>(string panelName) where T : Component
    {
        Transform panel;
        T result;
        if (allChild.TryGetValue(panelName, out panel))
        {
            result = panel.GetComponent<T>();
            if (result == null)
            {
                result = panel.AddComponent<T>();
            }
        }
        else
        {
            Debug.LogError($"{panelName}不存在");
            return null;
        }
        return result;
    }

}
