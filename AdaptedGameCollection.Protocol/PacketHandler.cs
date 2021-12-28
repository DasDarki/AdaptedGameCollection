using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace AdaptedGameCollection.Protocol;

/// <summary>
/// Marks a method as packet handler. If on server-side the first argument of the packet handling method must be the
/// client which sent the packet. The second parameter (on server-side) or the first parameter (on client-side) is
/// the packet which gets handled by the method.
/// The method gets called when the server or client received the handled packet.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class PacketHandler : Attribute
{
    private static readonly ConcurrentDictionary<Type, MethodHandle> Handlers = new();
    private static readonly Type PacketType = typeof(IPacket);

    /// <summary>
    /// Scans through the object instance and registers all non-static <see cref="PacketHandler"/>.
    /// The registration will be done by assuming the handlers are for the server, so the first argument
    /// must be server-handled client.
    /// </summary>
    /// <param name="instance">The instance to scan through</param>
    public static void RegisterForServer(object instance)
    {
        foreach (MethodInfo method in instance.GetType().GetMethods(BindingFlags.Static))
        {
            if (method.GetCustomAttribute<PacketHandler>() != null)
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 2)
                {
                    var packetParameter = parameters[1].ParameterType;
                    if (PacketType.IsAssignableFrom(packetParameter))
                    {
                        Handlers.TryAdd(packetParameter, new MethodHandle(instance, method));
                    }
                }
            } 
        }
    }

    /// <summary>
    /// Scans through the given type and registers all static <see cref="PacketHandler"/>.
    /// The registration will be done by assuming the handlers are for the server, so the first argument
    /// must be server-handled client.
    /// </summary>
    public static void RegisterForServer<T>()
    {
        foreach (MethodInfo method in typeof(T).GetMethods(BindingFlags.Static))
        {
            if (method.GetCustomAttribute<PacketHandler>() != null)
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 2)
                {
                    var packetParameter = parameters[1].ParameterType;
                    if (PacketType.IsAssignableFrom(packetParameter))
                    {
                        Handlers.TryAdd(packetParameter, new MethodHandle(null, method));
                    }
                }
            } 
        }
    }

    /// <summary>
    /// Scans through the object instance and registers all non-static <see cref="PacketHandler"/>.
    /// The registration will be done by assuming the handlers are for the client, so the first argument
    /// must be a <see cref="IPacket"/>.
    /// </summary>
    /// <param name="instance">The instance to scan through</param>
    public static void RegisterForClient(object instance)
    {
        foreach (MethodInfo method in instance.GetType().GetMethods(BindingFlags.Static))
        {
            if (method.GetCustomAttribute<PacketHandler>() != null)
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 1)
                {
                    var packetParameter = parameters[0].ParameterType;
                    if (PacketType.IsAssignableFrom(packetParameter))
                    {
                        Handlers.TryAdd(packetParameter, new MethodHandle(instance, method));
                    }
                }
            } 
        }
    }

    /// <summary>
    /// Scans through the given type and registers all static <see cref="PacketHandler"/>.
    /// The registration will be done by assuming the handlers are for the client, so the first argument
    /// must be a <see cref="IPacket"/>.
    /// </summary>
    public static void RegisterForClient<T>()
    {
        foreach (MethodInfo method in typeof(T).GetMethods(BindingFlags.Static))
        {
            if (method.GetCustomAttribute<PacketHandler>() != null)
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 1)
                {
                    var packetParameter = parameters[0].ParameterType;
                    if (PacketType.IsAssignableFrom(packetParameter))
                    {
                        Handlers.TryAdd(packetParameter, new MethodHandle(null, method));
                    }
                }
            } 
        }
    }

    /// <summary>
    /// Calls the registered packet handler for the given packet assuming that the handler is on the server
    /// and the first argument is a client which also needs to be passed.
    /// </summary>
    /// <param name="client">The client from where the packet comes from</param>
    /// <param name="packet">The packet which was received</param>
    public static void CallForServer(object client, IPacket packet)
    {
        Type type = packet.GetType();
        if (Handlers.ContainsKey(type))
        {
            Handlers[type].Invoke(client, packet);
        }
    }

    /// <summary>
    /// Calls the registered packet handler for the given packet assuming that the handler is on the client
    /// and the first argument is the packet.
    /// </summary>
    /// <param name="packet">The packet which was received</param>
    public static void CallForClient(IPacket packet)
    {
        Type type = packet.GetType();
        if (Handlers.ContainsKey(type))
        {
            Handlers[type].Invoke(packet);
        }
    }
    
    private class MethodHandle
    {
        private readonly object? _owner;
        private readonly MethodInfo _method;

        internal MethodHandle(object? owner, MethodInfo method)
        {
            _owner = owner;
            _method = method;
        }

        internal void Invoke(params object[] args)
        {
            _method.Invoke(_owner, args);
        }
    }
}