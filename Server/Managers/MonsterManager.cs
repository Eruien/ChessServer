using ServerContent;

public class MonsterManager
{
    private List<BaseMonster> MonsterList = new List<BaseMonster>(); 

    public void Register(BaseMonster obj)
    {
        MonsterList.Add(obj);
    }

    public void UnRegister(BaseMonster obj)
    {
        MonsterList.Remove(obj);
        obj = null;
    }
}
