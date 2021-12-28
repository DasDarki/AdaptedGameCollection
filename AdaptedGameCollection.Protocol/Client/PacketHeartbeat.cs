using LiteNetwork.Protocol.Abstractions;

namespace AdaptedGameCollection.Protocol.Client;

/// <summary>
/// The keep-alive packet which should be sent to the server.
/// </summary>
public class PacketHeartbeat : IPacket
{
    public void Serialize(ILitePacketStream buffer)
    {
    }

    public void Deserialize(ILitePacketStream buffer)
    {
    }
}