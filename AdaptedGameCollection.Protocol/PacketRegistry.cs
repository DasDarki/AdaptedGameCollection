using System;
using System.Collections.Generic;
using AdaptedGameCollection.Logging;
using AdaptedGameCollection.Protocol.Client;
using AdaptedGameCollection.Protocol.Client.Room;
using AdaptedGameCollection.Protocol.Server.Room;
using Microsoft.Extensions.Logging;

namespace AdaptedGameCollection.Protocol;

/// <summary>
/// The singleton registry which holds every registered packet and its ID associated with it type.
/// </summary>
public class PacketRegistry
{
    private static readonly Logger Logger = LoggerFactory.Create<PacketRegistry>();
    
    private static readonly Dictionary<int, Type> IdToType = new();
    private static readonly Dictionary<Type, int> TypeToId = new();
    private static bool _isInitialized;
    private static int _currentId = 0;

    /// <summary>
    /// Initializes the registry and registers all packets.
    /// </summary>
    public static void Initialize()
    {
        if (_isInitialized) return;
        _isInitialized = true;
        Register<PacketHeartbeat>();
        Register<PacketRequestRoomHost>();
        Register<PacketRequestRoomJoin>();
        Register<PacketRoomCreated>();
        Register<PacketRoomJoined>();
        Register<PacketRoomPlayerListUpdated>();
        Register<PacketRoomError>();
    }

    /// <summary>
    /// Returns the packet ID of the given packet type.
    /// </summary>
    /// <param name="type">The type of the wanted packet</param>
    /// <returns>The packet ID associated to the given type or -1 if the type is not found</returns>
    public static int GetPacketId(Type type)
    {
        if (TypeToId.ContainsKey(type)) return TypeToId[type];
        return -1;
    }
    
    /// <summary>
    /// Returns a new packet instance by the given ID.
    /// </summary>
    /// <param name="id">The ID of the wanted packet</param>
    /// <returns>The packet instance or null if the ID was not found</returns>
    public static IPacket? GetPacketById(int id)
    {
        if (!IdToType.ContainsKey(id)) return null;
        return Activator.CreateInstance(IdToType[id]) as IPacket;
    }

    /// <summary>
    /// Sets the internal logger debug mode.
    /// </summary>
    /// <param name="debug">The debug state</param>
    public static void SetLoggerDebug(bool debug)
    {
        Logger.OverwriteDebugMode(debug);
    }

    /// <summary>
    /// Registers the packet of the given type in this packet registry.
    /// </summary>
    /// <typeparam name="T">The type of the wanted packet</typeparam>
    private static void Register<T>() where T : IPacket
    {
        Type type = typeof(T);
        if (TypeToId.ContainsKey(type)) return;
        int id = ++_currentId;
        TypeToId.Add(type, id);
        IdToType.Add(id, type);
        Logger.Debug("Packet {Type} registered and identified by {ID}!", type.Name, id);
    }
}