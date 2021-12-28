using LiteNetwork.Protocol.Abstractions;

namespace AdaptedGameCollection.Protocol.Server.Room;

/// <summary>
/// The packet to sent to the client to tell him that the current room of him was destroysed.
/// </summary>
public class PacketRoomDestroyed : IPacket
{
    public PacketRoomDestroyed()
    {
    }

    public void Serialize(ILitePacketStream buffer)
    {
    }

    public void Deserialize(ILitePacketStream buffer)
    {
    }
}