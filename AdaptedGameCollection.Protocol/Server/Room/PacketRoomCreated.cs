using LiteNetwork.Protocol.Abstractions;

namespace AdaptedGameCollection.Protocol.Server.Room;

/// <summary>
/// The packet to sent to the client to tell him that the requested room was created.
/// </summary>
public class PacketRoomCreated : IPacket
{
    public string RoomID { get; private set; }

    public PacketRoomCreated(string roomID)
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