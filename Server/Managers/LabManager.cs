using ServerContent;

public class LabManager
{
    public int LabCount { get; set; } 

    private Dictionary<Team, BaseObject> LabList = new Dictionary<Team, BaseObject>();
    private Dictionary<BaseObject, int> LabNumberList = new Dictionary<BaseObject, int>();
    
    public void Register(Team key, BaseObject obj, int labNumber)
    {
        LabList.Add(key, obj);
        LabNumberList.Add(obj, labNumber);
        LabCount++;
    }

    public void UnRegister(BaseObject obj)
    {

    }

    public BaseObject GetTeamLab(Team key)
    {
        BaseObject? obj = null;

        if (LabList.TryGetValue(key, out obj))
        {
            return obj;
        }

        return null;
    }

    public int GetLabNumber(BaseObject obj)
    {
        int labNumber = 0;

        if (LabNumberList.TryGetValue(obj, out labNumber))
        {
            return labNumber;
        }

        return labNumber;
    }
}
