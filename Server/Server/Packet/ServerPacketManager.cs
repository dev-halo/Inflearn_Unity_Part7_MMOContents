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

    public void Register()
    {
        onRecv.Add((ushort)MsgId.CChat, MakePacket<C_Chat>);
        handler.Add((ushort)MsgId.CChat, PacketHandler.C_ChatHandler);

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
        if (handler.TryGetValue(id, out Action<PacketSession, IMessage> action))
            action.Invoke(session, pkt);
    }

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
        if (handler.TryGetValue(id, out Action<PacketSession, IMessage> action))
            return action;
        return null;
	}
}