using LiteNetwork.Protocol.Abstractions;

namespace AdaptedGameCollection.Protocol.Server.Room;

/// <summary>
/// The packet to sent to the client to tell him that he joined the requested room.
/// </summary>
public class PacketRoomJoined : IPacket
{
    public void Serialize(ILitePacketStream buffer)
    {
    }

    public void Deserialize(ILitePacketStream buffer)
    {
    }
}