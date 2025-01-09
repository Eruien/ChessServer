using ServerContent;
using ServerCore;

namespace Server
{
    public class ClientSession : PacketSession
    {
        public ushort m_SessionId { get; set; }
        public Team m_SessionTeam { get; set; }
        public Room m_GameRoom { get; set; }

        public override void OnConnect()
        {
            Program.g_GameRoom.Enter(this);
            S_SetInitialDataPacket dataPacket = new S_SetInitialDataPacket();
         
            if ((m_SessionId % 2) == 0)
            {
                this.m_SessionTeam = Team.BlueTeam;
                dataPacket.m_MyTeam = (ushort)this.m_SessionTeam;
            }
            else
            {
                this.m_SessionTeam = Team.RedTeam;
                dataPacket.m_MyTeam = (ushort)this.m_SessionTeam;
            }

            S_LabListPacket labPacket = new S_LabListPacket();

            labPacket.Add(new S_LabListPacket.LaboratoryPacket()
            {
                m_LabId = (ushort)Managers.Lab.GetLabNumber(Managers.Lab.GetTeamLab(Team.RedTeam)),
                m_Team = (ushort)Managers.Lab.GetTeamLab(Team.RedTeam).m_SelfTeam,
                m_PosX = Managers.Lab.GetTeamLab(Team.RedTeam).m_Position.X,
                m_PosY = Managers.Lab.GetTeamLab(Team.RedTeam).m_Position.Y,
                m_PosZ = Managers.Lab.GetTeamLab(Team.RedTeam).m_Position.Z,
            });
            labPacket.Add(new S_LabListPacket.LaboratoryPacket()
            {
                m_LabId = (ushort)Managers.Lab.GetLabNumber(Managers.Lab.GetTeamLab(Team.BlueTeam)),
                m_Team = (ushort)Managers.Lab.GetTeamLab(Team.BlueTeam).m_SelfTeam,
                m_PosX = Managers.Lab.GetTeamLab(Team.BlueTeam).m_Position.X,
                m_PosY = Managers.Lab.GetTeamLab(Team.BlueTeam).m_Position.Y,
                m_PosZ = Managers.Lab.GetTeamLab(Team.BlueTeam).m_Position.Z,
            });
          
            Program.g_GameRoom.Push(() => this.Send(dataPacket.Write()));
            Program.g_GameRoom.Push(() => this.Send(labPacket.Write()));
        }

        public override void OnDisconnect()
        {
            throw new NotImplementedException();
        }

        public override int OnRecvPacket(ArraySegment<byte> buffer)
        {
            return PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnSend(int numOfBytes)
        {
            throw new NotImplementedException();
        }
    }
}
