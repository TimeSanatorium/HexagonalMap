/// <summary>
/// 普通单例对象
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> where T : new()
{
    private static T instance;
    public static T Instance 
    {
        get 
        {
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }
        private set{}
    }
}
