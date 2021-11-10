using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Net;
using UnityEngine;

class ServerSession : PacketSession
{
    public override void OnConnected(EndPoint endPoint)
    {
        Debug.Log($"OnConnected : {endPoint}");

        C_Chat chat = new C_Chat()
        {
            Context = "안녕하세요"
        };

        ushort size = (ushort)chat.CalculateSize();
        byte[] sendBuffer = new byte[size + 4];
        Array.Copy(BitConverter.GetBytes(size + 4), 0, sendBuffer, 0, sizeof(ushort));
        ushort protocolId = (ushort)MsgId.SChat;
        Array.Copy(BitConverter.GetBytes(protocolId), 0, sendBuffer, 2, sizeof(ushort));
        Array.Copy(chat.ToByteArray(), 0, sendBuffer, 4, size);

        Send(new ArraySegment<byte>(sendBuffer));
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Debug.Log($"OnConnected : {endPoint}");
    }

    public override void OnRecvPacket(ushort id, ArraySegment<byte> buffer)
    {
        Debug.Log("ddD");
        PacketManager.Instance.OnRecvPacket(this, buffer, (s, p) => PacketQueue.Instance.Push(id, p));
    }

    public override void OnSend(int numOfBytes)
    {
        //Console.WriteLine($"Transferred bytes : {numOfBytes}");
    }
}
