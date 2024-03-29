using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager
{
    #region Singleton
    static readonly PacketManager instance = new PacketManager();
    public static PacketManager Instance { get { return instance; } }
    #endregion

    PacketManager()
    {
        Register();
    }

    readonly Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
    readonly Dictionary<ushort, Action<PacketSession, IMessage>> handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

    public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

    public void Register()
    {
        onRecv.Add((ushort)MsgId.SEnterGame, MakePacket<S_EnterGame>);
        handler.Add((ushort)MsgId.SEnterGame, PacketHandler.S_EnterGameHandler);
        onRecv.Add((ushort)MsgId.SLeaveGame, MakePacket<S_LeaveGame>);
        handler.Add((ushort)MsgId.SLeaveGame, PacketHandler.S_LeaveGameHandler);
        onRecv.Add((ushort)MsgId.SSpawn, MakePacket<S_Spawn>);
        handler.Add((ushort)MsgId.SSpawn, PacketHandler.S_SpawnHandler);
        onRecv.Add((ushort)MsgId.SDespawn, MakePacket<S_Despawn>);
        handler.Add((ushort)MsgId.SDespawn, PacketHandler.S_DespawnHandler);
        onRecv.Add((ushort)MsgId.SMove, MakePacket<S_Move>);
        handler.Add((ushort)MsgId.SMove, PacketHandler.S_MoveHandler);
        onRecv.Add((ushort)MsgId.SSkill, MakePacket<S_Skill>);
        handler.Add((ushort)MsgId.SSkill, PacketHandler.S_SkillHandler);
        onRecv.Add((ushort)MsgId.SChangeHp, MakePacket<S_ChangeHp>);
        handler.Add((ushort)MsgId.SChangeHp, PacketHandler.S_ChangeHpHandler);
        onRecv.Add((ushort)MsgId.SDie, MakePacket<S_Die>);
        handler.Add((ushort)MsgId.SDie, PacketHandler.S_DieHandler);

    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        if (onRecv.TryGetValue(id, out Action<PacketSession, ArraySegment<byte>, ushort> action))
            action.Invoke(session, buffer, id);
    }

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
    {
        T pkt = new T();
        pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

        if (CustomHandler != null)
        {
            CustomHandler.Invoke(session, pkt, id);
        }
        else
        {
            if (handler.TryGetValue(id, out Action<PacketSession, IMessage> action))
                action.Invoke(session, pkt);
        }
    }

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
        if (handler.TryGetValue(id, out Action<PacketSession, IMessage> action))
            return action;
        return null;
	}
}