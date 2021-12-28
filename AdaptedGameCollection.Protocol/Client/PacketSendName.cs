using LiteNetwork.Protocol.Abstractions;

namespace AdaptedGameCollection.Protocol.Client;

/// <summary>
/// The packet to sent to the server to tell him the name of the sender.
/// </summary>
public class PacketSendName : IPacket
{
    public string Name { get; private set; }

    public PacketSendName(string name)
    {
        Name = name;
    }
    
    public void Serialize(ILitePacketStream buffer)
    {
        buffer.WriteString(Name);
    }

    public void Deserialize(ILitePacketStream buffer)
    {
        Name = buffer.ReadString();
    }
}