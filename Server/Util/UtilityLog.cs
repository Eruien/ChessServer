
namespace ServerContent
{
    public enum Team
    {
        None = 0,
        BlueTeam = 8,
        RedTeam = 9,
    };

    public enum ObjectType
    {
        None,
        Machine,
        Monster,
    }

    public enum MonsterType
    {
        None = 0,
        Melee = 1,
        Range = 2,
    }

    public enum MonsterState
    {
        None,
        Idle,
        Move,
        Attack,
    }

    public enum PanelState
    {
        None,
        Select,
        SelectMove,
    }
}
