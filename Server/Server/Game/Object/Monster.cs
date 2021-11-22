using Google.Protobuf.Protocol;
using Server.Data;
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

        int skillRange = 1;
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
                BroadcastMove();
                return;
            }

            Vector2Int dir = target.CellPos - CellPos;
            int dist = dir.cellDistFromZero;
            if (dist == 0 || dist > chaseCellDist)
            {
                target = null;
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

            List<Vector2Int> path = Room.Map.FindPath(CellPos, target.CellPos, checkObjects: false);
            if (path.Count < 2 || path.Count > chaseCellDist)
            {
                target = null;
                State = CreatureState.Idle;
                BroadcastMove();
                return;
            }

            // 스킬 넘어갈지 체크
            if (dist <= skillRange && (dir.x == 0 || dir.y == 0))
            {
                coolTick = 0;
                State = CreatureState.Skill;
                return;
            }

            // 이동
            Dir = GetDirFromVec(path[1] - CellPos);
            Room.Map.ApplyMove(this, path[1]);

            BroadcastMove();
        }

        void BroadcastMove()
        {
            // 다른 플레이어한테도 알려준다
            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = PosInfo;
            Room.Broadcast(movePacket);
        }

        long coolTick = 0;
        protected virtual void UpdateSkill()
        {
            if (coolTick == 0)
            {
                // 유효한 타겟인지
                if (target == null || target.Room != Room || target.Hp == 0)
                {
                    target = null;
                    State = CreatureState.Moving;
                    BroadcastMove();
                    return;
                }

                // 스킬이 아직 사용 가능한지
                Vector2Int dir = (target.CellPos - CellPos);
                int dist = dir.cellDistFromZero;
                bool canUseSkill = (dist <= skillRange && (dir.x == 0 || dir.y == 0));
                if (canUseSkill == false)
                {
                    State = CreatureState.Moving;
                    BroadcastMove();
                    return;
                }

                // 타게팅 방향 주시
                MoveDir lookDir = GetDirFromVec(dir);
                if (Dir != lookDir)
                {
                    Dir = lookDir;
                    BroadcastMove();
                }

                DataManager.SkillDict.TryGetValue(1, out Skill skillData);

                // 데미지 판정
                target.OnDamaged(this, skillData.damage + Stat.Attack);

                // 스킬 사용 Broadcast
                S_Skill skill = new S_Skill() { Info = new SkillInfo() };
                skill.ObjectId = Id;
                skill.Info.SkillId = skillData.id;
                Room.Broadcast(skill);

                // 스킬 쿨타임 적용
                int coolTick = (int)(1000 * skillData.cooldown);
                this.coolTick = Environment.TickCount64 + coolTick;
            }

            if (this.coolTick > Environment.TickCount64)
                return;

            this.coolTick = 0;
        }

        protected virtual void UpdateDead()
        {
        }
    }
}
