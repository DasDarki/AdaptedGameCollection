using System;
using System.Linq;
using AdaptedGameCollection.Common;
using AdaptedGameCollection.Logging;
using AdaptedGameCollection.Protocol;
using AdaptedGameCollection.Protocol.Client;
using AdaptedGameCollection.Protocol.Client.Room;
using AdaptedGameCollection.Protocol.Server.Room;
using AdaptedGameCollection.Server.Network;
using LiteNetwork;
using LiteNetwork.Server;

namespace AdaptedGameCollection.Server;

/// <summary>
/// The main component of the server. It manages the network flow as well as all other components for the
/// server-side.
/// </summary>
internal class AGCServer : LiteServer<NetworkClient>
{
    /// <summary>
    /// The current instance to this server.
    /// </summary>
    internal static AGCServer Instance { get; private set; } = null!;
    
    /// <summary>
    /// The current config instance loaded.
    /// </summary>
    internal static Config Config { get; private set; } = null!;
    
    /// <summary>
    /// The rooms created on this server.
    /// </summary>
    internal ConcurrentList<Room> Rooms { get; }

    private static readonly Logger Logger = LoggerFactory.Create<AGCServer>();

    private AGCServer(LiteServerOptions options) : base(options)
    {
        Rooms = new ConcurrentList<Room>();
    }

    /// <summary>
    /// Blocks the main thread until "!exit" is being entered which leads to the server's shutdown.
    /// </summary>
    public void BlockThread()
    {
        while (true)
        {
            if (Console.ReadLine() == "!exit")
            {
                break;
            }
        }
    }

    /// <summary>
    /// Initializes and starts the server.
    /// </summary>
    internal static bool Run()
    {
        Logger.Info("Loading configuration file...");
        var config = Config.Load();
        if (config == null)
        {
            Logger.Fatal("The configuration file is corrupted!");
            Logger.Warn("Press any key to exit...");
            Console.ReadKey();
            return false;
        }

        Config = config;
        LoggerFactory.IsDebug = config.IsDebug;
        Logger.Info("Registering network packets...");
        PacketRegistry.SetLoggerDebug(config.IsDebug);
        PacketRegistry.Initialize();
        var configuration = new LiteServerOptions
        {
            Host = Config.BindingHost,
            Port = Config.Port,
            ReceiveStrategy = ReceiveStrategyType.Queued
        };
        Logger.Info("Setting up server...");
        Instance = new AGCServer(configuration);
        PacketHandler.RegisterForServer(Instance);
        Logger.Info("Starting server...");
        Instance.Start();
        Logger.Info("Server successfully started!");
        return true;
    }

    private Room? GetRoomByID(string id)
    {
        return Rooms.FirstOrDefault(room => room.ID == id);
    }
    
    [PacketHandler]
    public void OnPacketSendNameReceived(NetworkClient client, PacketSendName packet)
    {
        client.Name = packet.Name;
        Logger.Debug("Client {ID} changed his Name to {NAME}", client.Id, client.Name);
    }

    [PacketHandler]
    public void OnPacketRequestRoomHost(NetworkClient host, PacketRequestRoomHost packet)
    {
        var room = new Room(GetRoomID(), host);
        Rooms.Add(room);
        host.CurrentRoom = room;
        host.SendPacket(new PacketRoomCreated(room.ID));
        Logger.Verbose("Room {ID} was created by {Client}!", host.Name ?? host.Id.ToString());
    }

    [PacketHandler]
    public void OnPacketRequestRoomJoin(NetworkClient client, PacketRequestRoomJoin packet)
    {
        var room = GetRoomByID(packet.RoomID);
        if (room == null)
        {
            client.SendPacket(new PacketRoomError(PacketRoomError.RoomError.NotExisting));
            return;
        }

        client.CurrentRoom = room;
        client.SendPacket(new PacketRoomJoined());
        room.AddClient(client);
    }

    private string GetRoomID()
    {
        string Generate()
        {
            return Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
        }
        
        string id = Generate();
        while (GetRoomByID(id) != null)
        {
            id = Generate();
        }

        return id;
    }
}