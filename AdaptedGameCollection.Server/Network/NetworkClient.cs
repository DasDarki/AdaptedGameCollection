using System;
using System.Threading.Tasks;
using AdaptedGameCollection.Logging;
using AdaptedGameCollection.Protocol;
using LiteNetwork.Protocol;
using LiteNetwork.Protocol.Abstractions;
using LiteNetwork.Server;

namespace AdaptedGameCollection.Server.Network;

/// <summary>
/// The network client is an abstracted entity handling the network flow for AGC. It wraps the default send method
/// and adds the top-level <see cref="SendPacket"/> method.
/// </summary>
internal class NetworkClient : LiteServerUser
{
    private static readonly Logger Logger = LoggerFactory.Create<NetworkClient>();

    /// <summary>
    /// The name of the client.
    /// </summary>
    internal string? Name { get; set; }

    /// <summary>
    /// The room the client is currently in.
    /// </summary>
    internal Room? CurrentRoom { get; set; } = null;
    
    /// <summary>
    /// Gets called when this player sent a packet from its client-side to this server.
    /// </summary>
    /// <param name="buffer">The stream which contains the packet data</param>
    /// <returns></returns>
    public override Task HandleMessageAsync(ILitePacketStream buffer)
    {
        try
        {
            var packetId = buffer.ReadInt32();
            IPacket? packet = PacketRegistry.GetPacketById(packetId);
            if (packet == null)
            {
                Logger.Warn("Received a Packet by the ID #{ID} but it is not registered!", packetId);
                return base.HandleMessageAsync(buffer);
            }
            
            packet.Deserialize(buffer);
            PacketHandler.CallForServer(this, packet);
        }
        catch (Exception e)
        {
            Logger.Error(e, "An error occurred while receiving a Packet!");
        }
        
        return base.HandleMessageAsync(buffer);
    }

    /// <summary>
    /// Sends a packet to this player's client-side.
    /// </summary>
    /// <param name="packet">The packet to be sent</param>
    public void SendPacket(IPacket packet)
    {
        try
        {
            using var buffer = new LitePacket();
            var packetId = PacketRegistry.GetPacketId(packet.GetType());
            if (packetId == -1)
            {
                Logger.Warn("Tried to send Packet {Type} but it is not registered!", packet.GetType().FullName ?? "undefined");
                return;
            }
        
            buffer.WriteInt32(packetId);
            packet.Serialize(buffer);
            Send(buffer);
        }
        catch(Exception e)
        {
            Logger.Error(e, "An error occurred while sending Packet {Type}!", packet.GetType().FullName ?? "undefined");
        }
    }

    protected override void OnConnected()
    {
        Logger.Info("New Client connected <-> {IP}!", Socket.RemoteEndPoint?.ToString() ?? "undefined");
        base.OnConnected();
    }

    protected override void OnDisconnected()
    {
        Logger.Warn("Client {IP} is disconnected!", Socket.RemoteEndPoint?.ToString() ?? "undefined");
        if (CurrentRoom != null)
        {
            if (CurrentRoom.Host.Id == Id)
            {
                CurrentRoom.Destroy(true);
            }
            else
            {
                CurrentRoom.RemoveClient(this);
            }
        }
        base.OnDisconnected();
    }
}