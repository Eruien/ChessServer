using ServerContent;

public class LabManager
{
    public int m_LabCount { get; set; } 

    private Dictionary<Team, BaseObject> m_LabDict = new Dictionary<Team, BaseObject>();
    private Dictionary<BaseObject, int> m_LabNumberDict = new Dictionary<BaseObject, int>();
    
    public void Register(Team key, BaseObject obj, int labNumber)
    {
        m_LabDict.Add(key, obj);
        m_LabNumberDict.Add(obj, labNumber);
        m_LabCount++;
    }

    public void UnRegister(BaseObject obj)
    {
        m_LabDict.Remove(obj.m_SelfTeam);
        m_LabNumberDict.Remove(obj);
    }

    public void RemoveAll()
    {
        m_LabDict.Clear();
        m_LabNumberDict.Clear();
    }

    public BaseObject GetTeamLab(Team key)
    {
        BaseObject? obj = null;

        if (m_LabDict.TryGetValue(key, out obj))
        {
            return obj;
        }

        return null;
    }

    public int GetLabNumber(BaseObject obj)
    {
        int labNumber = 0;

        if (m_LabNumberDict.TryGetValue(obj, out labNumber))
        {
            return labNumber;
        }

        return labNumber;
    }
}
