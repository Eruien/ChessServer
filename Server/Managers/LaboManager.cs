using ServerContent;

public class LaboManager
{
    private Dictionary<string, BaseObject> LaboList = new Dictionary<string, BaseObject>();
    private Dictionary<BaseObject, int> LaboNumberList = new Dictionary<BaseObject, int>();
    
    public void Register(string key, BaseObject obj, int laboNumber)
    {
        LaboList.Add(key, obj);
        LaboNumberList.Add(obj, laboNumber);
    }

    public void UnRegister(BaseObject obj)
    {

    }

    public BaseObject GetTeamLabo(string key)
    {
        BaseObject? obj = null;

        if (LaboList.TryGetValue(key, out obj))
        {
            return obj;
        }

        return null;
    }

    public int GetLaboNumber(BaseObject obj)
    {
        int naboNumber = 0;

        if (LaboNumberList.TryGetValue(obj, out naboNumber))
        {
            return naboNumber;
        }

        return naboNumber;
    }
}
