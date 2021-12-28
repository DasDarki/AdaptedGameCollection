using System.Collections.Generic;
using LiteNetwork.Protocol.Abstractions;

namespace AdaptedGameCollection.Protocol.Server.Room;

/// <summary>
/// The packet to sent to the client to update his room's player list.
/// </summary>
public class PacketRoomPlayerListUpdated : IPacket
{
    public List<string> PlayerList { get; private set; }

    public PacketRoomPlayerListUpdated(List<string>? playerList = null)
    {
        PlayerList = playerList ?? new List<string>();
    }

    public void Serialize(ILitePacketStream buffer)
    {
        buffer.WriteInt32(PlayerList.Count);
        foreach (var player in PlayerList)
        {
            buffer.WriteString(player);
        }
    }

    public void Deserialize(ILitePacketStream buffer)
    {
        int playersCount = buffer.ReadInt32();
        PlayerList = new List<string>();
        for (int i = 0; i < playersCount; i++)
        {
            PlayerList.Add(buffer.ReadString());
        }
    }
}