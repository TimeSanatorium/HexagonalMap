using UnityEngine;
/// <summary>
/// 场景挂载脚本单例对象 默认在加载场景的时候不会销毁这个对象
/// </summary>
public class SingleMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T current;
    public static T Current
    {
        get 
        { 
            if (current == null) 
            {
                current = GameObject.FindObjectOfType<T>();
                DontDestroyOnLoad(current.gameObject);
            }
            if (current == null)
            {
                throw new System.Exception($"场景中不存在挂载{typeof(T)}脚本的对象");
            }
            return current;
        }
        private set {  }
    }

    /// <summary>
    /// 删除当前静态对象
    /// </summary>
    public static void DestoryCurrentObject()
    {
        if (current != null)
        {
            Destroy(current.gameObject);
        }
    }
}
