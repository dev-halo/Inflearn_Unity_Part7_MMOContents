﻿using System;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;

class PacketHandler
{
    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
        C_Move movePacket = packet as C_Move;
        ClientSession clientSession = session as ClientSession;
    }
}
