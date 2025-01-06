
public class Managers
{
    private static Managers _instance = new Managers();
    public static Managers Instance { get { return _instance; } }

    private ObjectManager _object = new ObjectManager();
    private LaboManager _labo = new LaboManager();
    private DataManager _data = new DataManager();

    public static ObjectManager Object { get { return Instance._object; } }
    public static LaboManager Labo { get { return Instance._labo; } }
    public static DataManager Data { get { return Instance._data; } }

    public static void Init()
    {
        // 매니저들 초기화
        _instance._data.Init();
    }

    public static void Clear()
    {
        // 매니저들 Clear 작업
    }
}
