using ServerContent;

public class ObjectManager
{
    public static int m_ObjectCount { get; set; } = 0;
    private Dictionary<int, BaseObject> m_ObjectDict = new Dictionary<int, BaseObject>(); 

    public int Register(BaseObject obj)
    {
        m_ObjectCount++;
        m_ObjectDict.Add(m_ObjectCount, obj);
        return m_ObjectCount;
    }

    public void UnRegister(BaseObject obj)
    {
        m_ObjectDict.Remove(m_ObjectCount);
    }

    public void RemoveAll()
    {
        m_ObjectDict.Clear();
    }

    public BaseObject GetObject(int objectId)
    {
        BaseObject? monster = null;

        if (m_ObjectDict.TryGetValue(objectId, out monster))
        {
            return monster;
        }

        return null;
    }

    public void Frame()
    {
        foreach (var monster in m_ObjectDict)
        {
            monster.Value.Frame();
        }
    }
}
