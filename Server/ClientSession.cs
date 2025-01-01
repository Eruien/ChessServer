using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            S_SetInitialLaboPacket laboPacket = new S_SetInitialLaboPacket();
            Labo labo = new Labo();
            labo.Init();

            if ((SessionId % 2) == 0)
            {
                dataPacket.myTeam = (ushort)Team.RedTeam;
                labo.SetPosition(0.0f, 2.904f, -22.0f);
            }
            else
            {
                dataPacket.myTeam = (ushort)Team.BlueTeam;
                labo.SetPosition(0.0f, 2.904f, 22.0f);
            }

            labo.SelfTeam = (Team)dataPacket.myTeam;

            laboPacket.laboId = (ushort)Managers.Object.Register(labo);
            laboPacket.laboPosX = labo.Position.X;
            laboPacket.laboPosY = labo.Position.Y;
            laboPacket.laboPosZ = labo.Position.Z;
            Program.g_GameRoom.Push(() => this.Send(dataPacket.Write()));
            Program.g_GameRoom.BroadCast(laboPacket.Write());
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
