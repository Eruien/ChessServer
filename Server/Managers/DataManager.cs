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
    public Dictionary<string, MonsterStat> monsterDict { get; private set; } = new Dictionary<string, MonsterStat>();
    
    public void Init()
    {
        monsterDict = LoadJson<MonsterData, string, MonsterStat>("MonsterData.json")!.MakeDict();
    }

    private Loader? LoadJson<Loader, Key, Value>(string path) where Loader : IDict<Key, Value>
    {
        string ResourcesPath = @"../../../../Resources/";
        string fullPath = Path.Combine(ResourcesPath, path);

        if (File.Exists(fullPath))
        {
            string jsonText = File.ReadAllText(fullPath);
            return JsonConvert.DeserializeObject<Loader>(jsonText);
        }
        return default;
    }
}
