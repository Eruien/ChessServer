
public class Managers
{
    private static Managers _instance = new Managers();
    public static Managers Instance { get { return _instance; } }

    private ObjectManager _object = new ObjectManager();
    private LabManager _lab = new LabManager();
    private DataManager _data = new DataManager();

    public static ObjectManager Object { get { return Instance._object; } }
    public static LabManager Lab { get { return Instance._lab; } }
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
