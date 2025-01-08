using ServerContent;
using Newtonsoft.Json;

public interface IDict<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

[Serializable]
public class MonsterStat
{
    public string m_Name;
    public MonsterType m_MonsterType;
    public float m_HP;
    public float m_AttackRange;
    public float m_AttackRangeCorrectionValue;
    public float m_AttackDistance;
    public float m_DefaultAttackDamage;
    public float m_MoveSpeed;
    public float m_ProjectTileSpeed;
}

[Serializable]
public class MonsterData : IDict<string, MonsterStat>
{
    public List<MonsterStat> m_MonsterStat = new List<MonsterStat>();

    public Dictionary<string, MonsterStat> MakeDict()
    {
        Dictionary<string, MonsterStat> dict = new Dictionary<string, MonsterStat>();
        foreach (MonsterStat stat in m_MonsterStat)
        {
            dict.Add(stat.m_Name, stat);
        }
        return dict;
    }
}

public class DataManager
{
    public Dictionary<string, MonsterStat> m_MonsterDict { get; private set; } = new Dictionary<string, MonsterStat>();
    
    public void Init()
    {
        m_MonsterDict = LoadJson<MonsterData, string, MonsterStat>("MonsterData.json")!.MakeDict();
    }

    private Loader? LoadJson<Loader, Key, Value>(string path) where Loader : IDict<Key, Value>
    {
        string resourcesPath = @"../../../../Resources/";
        string fullPath = Path.Combine(resourcesPath, path);

        if (File.Exists(fullPath))
        {
            string jsonText = File.ReadAllText(fullPath);
            return JsonConvert.DeserializeObject<Loader>(jsonText);
        }
        return default;
    }
}
