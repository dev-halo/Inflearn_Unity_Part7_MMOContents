﻿using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Arrow : Projectile
    {
        public GameObject Owner { get; set; }

        long nextMoveTick = 0;

        public override void Update()
        {
            if (Data == null || Data.projectile == null || Owner == null || Room == null)
                return;

            if (nextMoveTick >= Environment.TickCount64)
                return;

            long tick = (long)(1000 / Data.projectile.speed);
            nextMoveTick = Environment.TickCount64 + tick;

            Vector2Int destPos = GetFrontCellPos();
            if (Room.Map.CanGo(destPos))
            {
                CellPos = destPos;

                S_Move movePacket = new S_Move();
                movePacket.ObjectId = Id;
                movePacket.PosInfo = PosInfo;
                Room.Broadcast(movePacket);

                Console.WriteLine("Move Arrow");
            }
            else
            {
                GameObject target = Room.Map.Find(destPos);
                if (target != null)
                {
                    target.OnDamaged(this, Data.damage + Owner.Stat.Attack);
                }

                // 소멸
                Room.Push(Room.LeaveGame, Id);
            }
        }
    }
}
