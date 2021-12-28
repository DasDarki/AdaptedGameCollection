using System;
using System.Threading.Tasks;
using AdaptedGameCollection.Logging;
using AdaptedGameCollection.Protocol;
using AdaptedGameCollection.Protocol.Client;
using LiteNetwork.Client;
using LiteNetwork.Protocol;
using LiteNetwork.Protocol.Abstractions;

namespace AdaptedGameCollection.Game.Network;

/// <summary>
/// The network client is an abstracted entity handling the network flow for AGC. It wraps the default send method
/// and adds the top-level <see cref="SendPacket"/> method.
/// </summary>
internal class NetworkClient : LiteClient
{
    /// <summary>
    /// The current instance of the running network client.
    /// </summary>
    internal static NetworkClient Instance { get; private set; } = null!;
    
    /// <summary>
    /// The name of this client.
    /// </summary>
    internal string Name { get; private set; }
    
    private static readonly Logger Logger = LoggerFactory.Create<NetworkClient>();

    internal NetworkClient(LiteClientOptions options, IServiceProvider serviceProvider = null) : base(options, serviceProvider)
    {
        Instance = this;
    }
    
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
            PacketHandler.CallForClient(packet);
        }
        catch (Exception e)
        {
            Logger.Error(e, "An error occurred while receiving a Packet!");
        }
        
        return base.HandleMessageAsync(buffer);
    }

    /// <summary>
    /// Sends a packet to the server.
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

    /// <summary>
    /// Sets the name of this client to the given one.
    /// </summary>
    /// <param name="name">The new name</param>
    public void SetName(string name)
    {
        Name = name;
        SendPacket(new PacketSendName(name));
    }

    protected override void OnConnected()
    {
        Logger.Info("A connection to the server could be successfully established!");
        base.OnConnected();
    }

    protected override void OnDisconnected()
    {
        Logger.Warn("The connection to the server was closed!");
        base.OnDisconnected();
    }
}