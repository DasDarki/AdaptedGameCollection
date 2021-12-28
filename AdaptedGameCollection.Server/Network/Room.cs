using AdaptedGameCollection.Common;
using AdaptedGameCollection.Logging;
using AdaptedGameCollection.Protocol;
using AdaptedGameCollection.Protocol.Server.Room;

namespace AdaptedGameCollection.Server.Network;

internal class Room
{
    private static readonly Logger Logger = LoggerFactory.Create<Room>();
    
    internal string ID { get; }
    
    internal NetworkClient Host { get; }
    
    internal ConcurrentList<NetworkClient> Clients { get; }

    internal Room(string id, NetworkClient host)
    {
        ID = id;
        Host = host;
        Clients = new ConcurrentList<NetworkClient>();
    }

    /// <summary>
    /// Sends the given packet to all clients in the room.
    /// </summary>
    /// <param name="packet">The packet to be sent</param>
    /// <param name="exceptHost">If false the host gets the packet, too</param>
    internal void SendPacket(IPacket packet, bool exceptHost = false)
    {
        if (!exceptHost) Host.SendPacket(packet);
        foreach (NetworkClient client in Clients)
        {
            client.SendPacket(packet);
        }
    }

    /// <summary>
    /// Destroys the room.
    /// </summary>
    internal void Destroy(bool onHostDisconnect = false)
    {
        AGCServer.Instance.Rooms.Remove(this);
        SendPacket(new PacketRoomDestroyed(), onHostDisconnect);
        Logger.Warn("Room {ID} was destroyed cause: {Reason}!", ID, onHostDisconnect ? "HOST_DISCONNECT" : "HOST_MANUALLY");
    }

    /// <summary>
    /// Adds the client to the room.
    /// </summary>
    /// <param name="client">The client to be added</param>
    internal void AddClient(NetworkClient client)
    {
        Clients.Add(client);
        Logger.Info("Client {Name} joined Room {ID}!", client.Name ?? client.Id.ToString(), ID);
        SendPlayerListUpdate();
    }

    /// <summary>
    /// Removes the client from the room.
    /// </summary>
    /// <param name="client">The client to be removed</param>
    internal void RemoveClient(NetworkClient client)
    {
        if (Clients.Contains(client))
        {
            Clients.Remove(client);
            Logger.Warn("Client {Name} left Room {ID}!", client.Name ?? client.Id.ToString(), ID);
            SendPlayerListUpdate();
        }
    }

    private void SendPlayerListUpdate()
    {
        var packet = new PacketRoomPlayerListUpdated();
        foreach (var client in Clients)
        {
            packet.PlayerList.Add(client.Name ?? client.Id.ToString());
        }
        
        SendPacket(packet);
    }
}