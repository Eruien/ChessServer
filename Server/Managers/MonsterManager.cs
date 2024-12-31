using ServerContent;

public class MonsterManager
{
    public static int MonsterCount { get; set; } = 0;
    private Dictionary<int, BaseMonster> MonsterList = new Dictionary<int, BaseMonster>(); 

    public int Register(BaseMonster obj)
    {
        MonsterCount++;
        MonsterList.Add(MonsterCount, obj);
        return MonsterCount;
    }

    public void UnRegister(BaseMonster obj)
    {
       
    }

    public BaseMonster GetMonster(int objectId)
    {
        BaseMonster? monster = null;

        if (MonsterList.TryGetValue(objectId, out monster))
        {
            return monster;
        }

        return null;
    }

    public void Frame()
    {
        foreach (var monster in MonsterList)
        {
            monster.Value.Frame();
        }
    }
}
