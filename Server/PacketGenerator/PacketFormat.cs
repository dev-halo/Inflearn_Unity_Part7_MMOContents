﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketGenerator
{
    class PacketFormat
    {
        // {0} 패킷 등록
        public static string managerFormat =
@"using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager
{{
    #region Singleton
    static readonly PacketManager instance = new PacketManager();
    public static PacketManager Instance {{ get {{ return instance; }} }}
    #endregion

    PacketManager()
    {{
        Register();
    }}

    readonly Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IMessage>> makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IMessage>>();
    readonly Dictionary<ushort, Action<PacketSession, IMessage>> handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

    public void Register()
    {{
{0}
    }}

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IMessage> onRecvCallback = null)
    {{
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        if (makeFunc.TryGetValue(id, out Func<PacketSession, ArraySegment<byte>, IMessage> func))
        {{
            IMessage packet = func.Invoke(session, buffer);

            if (onRecvCallback != null)
                onRecvCallback.Invoke(session, packet);
            else
                HandlePacket(session, packet, id);
        }}
    }}

    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IMessage, new()
    {{
        T pkt = new T();
        pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);
        return pkt;
    }}

    public void HandlePacket(PacketSession session, IMessage packet, ushort id)
    {{
        if (handler.TryGetValue(id, out Action<PacketSession, IMessage> action))
            action.Invoke(session, packet);
    }}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{{
        if (handler.TryGetValue(id, out Action<PacketSession, IMessage> action))
            return action;
        return null;
	}}
}}";

        // {0} MsgId
        // {1} 패킷 이름
        public static string managerRegisterFormat =
@"
        makeFunc.Add((ushort)MsgId.{0}, MakePacket<{1}>);
        handler.Add((ushort)MsgId.{0}, PacketHandler.{1}Handler);";
    }
}
