using ServerContent;
using ServerCore;

namespace Server
{
    public class ClientSession : PacketSession
    {
        public ushort SessionId { get; set; }
        public Team SessionTeam { get; set; }
        public Room GameRoom { get; set; }

        public override void OnConnect()
        {
            Program.g_GameRoom.Enter(this);
            S_SetInitialDataPacket dataPacket = new S_SetInitialDataPacket();
         
            if ((SessionId % 2) == 0)
            {
                this.SessionTeam = Team.BlueTeam;
                dataPacket.myTeam = (ushort)this.SessionTeam;
            }
            else
            {
                this.SessionTeam = Team.RedTeam;
                dataPacket.myTeam = (ushort)this.SessionTeam;
            }

            S_LabListPacket labPacket = new S_LabListPacket();

            labPacket.Add(new S_LabListPacket.Laboratory()
            {
                Id = (ushort)Managers.Lab.GetLabNumber(Managers.Lab.GetTeamLab(Team.RedTeam)),
                Team = (ushort)Managers.Lab.GetTeamLab(Team.RedTeam).SelfTeam,
                PosX = Managers.Lab.GetTeamLab(Team.RedTeam).Position.X,
                PosY = Managers.Lab.GetTeamLab(Team.RedTeam).Position.Y,
                PosZ = Managers.Lab.GetTeamLab(Team.RedTeam).Position.Z,
            });
            labPacket.Add(new S_LabListPacket.Laboratory()
            {
                Id = (ushort)Managers.Lab.GetLabNumber(Managers.Lab.GetTeamLab(Team.BlueTeam)),
                Team = (ushort)Managers.Lab.GetTeamLab(Team.BlueTeam).SelfTeam,
                PosX = Managers.Lab.GetTeamLab(Team.BlueTeam).Position.X,
                PosY = Managers.Lab.GetTeamLab(Team.BlueTeam).Position.Y,
                PosZ = Managers.Lab.GetTeamLab(Team.BlueTeam).Position.Z,
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
