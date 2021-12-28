using LiteNetwork.Protocol.Abstractions;

namespace AdaptedGameCollection.Protocol.Client.Room;

/// <summary>
/// The packet which is sent to tell the server to create a new room.
/// </summary>
public class PacketRequestRoomHost : IPacket
{
    public void Serialize(ILitePacketStream buffer)
    {
    }

    public void Deserialize(ILitePacketStream buffer)
    {
    }
}