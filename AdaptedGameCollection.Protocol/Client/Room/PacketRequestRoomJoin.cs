using LiteNetwork.Protocol.Abstractions;

namespace AdaptedGameCollection.Protocol.Client.Room;

/// <summary>
/// The packet to sent to the server to tell him to join a room.
/// </summary>
public class PacketRequestRoomJoin : IPacket
{
    public string RoomID { get; private set; }

    public PacketRequestRoomJoin(string roomID)
    {
        RoomID = roomID;
    }

    public void Serialize(ILitePacketStream buffer)
    {
        buffer.WriteString(RoomID);
    }

    public void Deserialize(ILitePacketStream buffer)
    {
        RoomID = buffer.ReadString();
    }
}