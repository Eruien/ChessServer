using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    public class Room
    {
        List<ClientSession> SessionList = new List<ClientSession>();
        JobQueue JobQueue = new JobQueue();
        List<ArraySegment<byte>> PendingList = new List<ArraySegment<byte>>();

        // 서버쪽에도 등록, 클라쪽에도 등록

        public void Push(Action job)
        {
            JobQueue.Push(job);
        }

        public void Flush()
        {
            foreach (var session in SessionList)
            {
                session.Send(PendingList);
            }
            PendingList.Clear();
        }

        public void BroadCast(ArraySegment<byte> segment)
        {
            PendingList.Add(segment);
        }

        public void Enter(ClientSession session)
        {
            session.GameRoom = this;
            // 새로운 세션이 들어오면 기존애 있던 애들한테 알림
            SessionList.Add(session);

            //PlayerList playerList = new PlayerList();

          /*  foreach (var arg in SessionList)
            {
                playerList.Add(new PlayerList.Player
                {
                    PlayerId = session.SessionId,
                    IsSelf = (session == arg),
                    PosX = 0,
                    PosY = 0,
                    PosZ = 0,
                });
            }*/

            // 아예 새로운 정보 갱신
            //session.Send(playerList.Write());

           /* S_BroadcastEnterGame enterPacket = new S_BroadcastEnterGame();

            enterPacket.playerId = session.SessionId;
            enterPacket.posX = 0;
            enterPacket.posY = 0;
            enterPacket.posZ = 0;
            // 기존에 있던 애들한테도 알림
            // 한 명만 추가
            // 받을 때 본인 자신의 정보가 있으면 거르기
            BroadCast(enterPacket.Write());*/
        }

        public void Leave()
        {

        }

    }
}
