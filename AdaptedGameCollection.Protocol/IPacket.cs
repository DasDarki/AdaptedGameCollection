using LiteNetwork.Protocol.Abstractions;

namespace AdaptedGameCollection.Protocol;

/// <summary>
/// A packet is a container which holds data which will then sent through the network.
/// </summary>
public interface IPacket
{
    /// <summary>
    /// Gets called when this packet needs to be serialized. The packet data can be written to the given buffer.
    /// The serialization gets executed before the packet is sent through the network.
    /// </summary>
    /// <param name="buffer">The byte buffer which will be sent over the network</param>
    void Serialize(ILitePacketStream buffer);

    /// <summary>
    /// Gets called when this packet needs to be serialized. The packet data can then be read from the given buffer.
    /// The deserialization gets executed after the packet was sent over the network and before it should be handled
    /// by a packet handler.
    /// </summary>
    /// <param name="buffer">The byte buffer which will be sent over the network</param>
    void Deserialize(ILitePacketStream buffer);
}