using ServerContent;
using ServerCore;

namespace Server
{
    public class ClientSession : PacketSession
    {
        public ushort SessionId { get; set; }
        public Room GameRoom { get; set; }

        public override void OnConnect()
        {
            Program.g_GameRoom.Enter(this);
            S_SetInitialDataPacket dataPacket = new S_SetInitialDataPacket();
         
            if ((SessionId % 2) == 0)
            {
                dataPacket.myTeam = (ushort)Team.BlueTeam;
            }
            else
            {
                dataPacket.myTeam = (ushort)Team.RedTeam;
            }

            S_LabListPacket labPacket = new S_LabListPacket();

            labPacket.Add(new S_LabListPacket.Laboratory()
            {
                Id = (ushort)Managers.Labo.GetLaboNumber(Managers.Labo.GetTeamLabo("Red")),
                Team = (ushort)Managers.Labo.GetTeamLabo("Red").SelfTeam,
                PosX = Managers.Labo.GetTeamLabo("Red").Position.X,
                PosY = Managers.Labo.GetTeamLabo("Red").Position.Y,
                PosZ = Managers.Labo.GetTeamLabo("Red").Position.Z,
            });
            labPacket.Add(new S_LabListPacket.Laboratory()
            {
                Id = (ushort)Managers.Labo.GetLaboNumber(Managers.Labo.GetTeamLabo("Blue")),
                Team = (ushort)Managers.Labo.GetTeamLabo("Blue").SelfTeam,
                PosX = Managers.Labo.GetTeamLabo("Blue").Position.X,
                PosY = Managers.Labo.GetTeamLabo("Blue").Position.Y,
                PosZ = Managers.Labo.GetTeamLabo("Blue").Position.Z,
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
