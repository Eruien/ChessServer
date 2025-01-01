using ServerContent;

public class ObjectManager
{
    public static int ObjectCount { get; set; } = 0;
    private Dictionary<int, BaseObject> ObjectList = new Dictionary<int, BaseObject>(); 

    public int Register(BaseObject obj)
    {
        ObjectCount++;
        ObjectList.Add(ObjectCount, obj);
        return ObjectCount;
    }

    public void UnRegister(BaseObject obj)
    {
       
    }

    public BaseObject GetObject(int objectId)
    {
        BaseObject? monster = null;

        if (ObjectList.TryGetValue(objectId, out monster))
        {
            return monster;
        }

        return null;
    }

    public void Frame()
    {
        foreach (var monster in ObjectList)
        {
            monster.Value.Frame();
        }
    }
}
