using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Monster : GameObject
    {
        public Monster()
        {
            ObjectType = GameObjectType.Monster;

            // TEMP
            Stat.Level = 1;
            Stat.Hp = 100;
            Stat.MaxHp = 100;
            Stat.Speed = 5f;

            State = CreatureState.Idle;
        }

        // FSM (Finite State Machine)
        public override void Update()
        {
            switch (State)
            {
                case CreatureState.Idle:
                    UpdateIdle();
                    break;
                case CreatureState.Moving:
                    UpdateMoving();
                    break;
                case CreatureState.Skill:
                    UpdateSkill();
                    break;
                case CreatureState.Dead:
                    UpdateDead();
                    break;
                default:
                    break;
            }
        }

        Player target;
        int searchCellDist = 10;
        int chaseCellDist = 20;

        long nextSearchTick = 0;
        protected virtual void UpdateIdle()
        {
            if (nextSearchTick > Environment.TickCount64)
                return;

            nextSearchTick = Environment.TickCount64 + 1000;

            Player target = Room.FindPlayer(p =>
            {
                Vector2Int dir = p.CellPos - CellPos;
                return dir.cellDistFromZero <= searchCellDist;
            });

            if (target == null)
                return;

            this.target = target;
            State = CreatureState.Moving;
        }

        long nextMoveTick = 0;
        protected virtual void UpdateMoving()
        {
            if (nextMoveTick > Environment.TickCount64)
                return;

            int moveTick = (int)(1000 / Speed);
            nextMoveTick = Environment.TickCount64 + moveTick;

            if (target == null || target.Room != Room)
            {
                target = null;
                State = CreatureState.Idle;
                return;
            }

            int dist = (target.CellPos - CellPos).cellDistFromZero;
            if (dist == 0 || dist > chaseCellDist)
            {
                target = null;
                State = CreatureState.Idle;
                return;
            }

            List<Vector2Int> path = Room.Map.FindPath(CellPos, target.CellPos, checkObjects: false);
            if (path.Count < 2 || path.Count > chaseCellDist)
            {
                target = null;
                State = CreatureState.Idle;
                return;
            }

            // 이동
            Dir = GetDirFromVec(path[1] - CellPos);
            Room.Map.ApplyMove(this, path[1]);

            // 다른 플레이어한테도 알려준다
            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = PosInfo;
            Room.Broadcast(movePacket);
        }

        protected virtual void UpdateSkill()
        {
        }

        protected virtual void UpdateDead()
        {
        }
    }
}
