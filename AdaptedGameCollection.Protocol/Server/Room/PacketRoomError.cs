using LiteNetwork.Protocol.Abstractions;

namespace AdaptedGameCollection.Protocol.Server.Room;

/// <summary>
/// The packet to sent to the client to tell him that there was an error with his room.
/// </summary>
public class PacketRoomError : IPacket
{
    public RoomError Error { get; private set; }

    public PacketRoomError(RoomError error)
    {
        Error = error;
    }

    public void Serialize(ILitePacketStream buffer)
    {
        buffer.WriteInt32((int) Error);
    }

    public void Deserialize(ILitePacketStream buffer)
    {
        Error = (RoomError) buffer.ReadInt32();
    }
    
    public enum RoomError
    {
        NotExisting
    }
}