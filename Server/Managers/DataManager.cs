using ServerContent;
using Newtonsoft.Json;

public interface IDict<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

[Serializable]
public class MonsterStat
{
    public string name;
    public MonsterType monsterType;
    public float hp;
    public float attackRange;
    public float attackRangeCorrectionValue;
    public float attackDistance;
    public float defaultAttackDamage;
    public float moveSpeed;
    public float projectTileSpeed;
}

[Serializable]
public class MonsterData : IDict<string, MonsterStat>
{
    public List<MonsterStat> monsterStat = new List<MonsterStat>();

    public Dictionary<string, MonsterStat> MakeDict()
    {
        Dictionary<string, MonsterStat> dict = new Dictionary<string, MonsterStat>();
        foreach (MonsterStat stat in monsterStat)
        {
            dict.Add(stat.name, stat);
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
