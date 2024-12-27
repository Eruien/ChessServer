using ServerContent;

public class MonsterManager
{
    private Dictionary<int, BaseMonster> MonsterList = new Dictionary<int, BaseMonster>(); 

    public void Register(int id, BaseMonster obj)
    {
        MonsterList.Add(id, obj);
    }

    public void UnRegister(BaseMonster obj)
    {
       
    }

    public void Frame()
    {
        foreach (var monster in MonsterList)
        {
            monster.Value.Frame();
        }
    }
}
