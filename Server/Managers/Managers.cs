
public class Managers
{
    private static Managers _instance = new Managers();
    public static Managers Instance { get { return _instance; } }

    private MonsterManager _monster = new MonsterManager();
    private DataManager _data = new DataManager();

    public static MonsterManager Monster { get { return Instance._monster; } }
    public static DataManager Data { get { return Instance._data; } }

    public static void Init()
    {
        // �Ŵ����� �ʱ�ȭ
        _instance._data.Init();
    }

    public static void Clear()
    {
        // �Ŵ����� Clear �۾�
    }
}
