using Google.Protobuf;
using System.Collections.Generic;

public class PacketMessage
{
    public ushort Id { get; set; }
    public IMessage Message { get; set; }
}

public class PacketQueue
{
    public static PacketQueue Instance { get; } = new PacketQueue();

    readonly Queue<PacketMessage> packetQueue = new Queue<PacketMessage>();
    readonly object _lock = new object();

    public void Push(ushort id, IMessage packet)
    {
        lock (_lock)
        {
            packetQueue.Enqueue(new PacketMessage() { Id = id, Message = packet });
        }
    }

    public PacketMessage Pop()
    {
        lock (_lock)
        {
            if (packetQueue.Count == 0)
                return null;

            return packetQueue.Dequeue();
        }
    }

    public List<PacketMessage> PopAll()
    {
        List<PacketMessage> list = new List<PacketMessage>();

        lock (_lock)
        {
            while (packetQueue.Count > 0)
                list.Add(packetQueue.Dequeue());
        }

        return list;
    }
}
