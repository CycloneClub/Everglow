using Everglow.Commons.Netcode.Abstracts;

namespace Everglow.Commons.Netcode;

/// <summary>
/// Specifies the network destination for a packet, relative to the caller's role.
/// <para/> Determined by <see cref="NetUtils"/> based on <see cref="Main.netMode"/> and <see cref="SubworldSystem.Current"/>.
/// </summary>
public enum RouteDestination
{
	/// <summary>
	/// Packet is handled only within the current world.
	/// Useful for vanilla netcode.
	/// </summary>
	WorldOnly,

	/// <summary>
	/// Packet travels upward to the main world server.
	/// <list type="bullet">
	/// <item>
	///     <term>Subworld server</term>
	///     <description>Sends directly to main world.</description>
	/// </item>
	/// <item>
	///     <term>Subworld client</term>
	///     <description>Forwards through its subworld server (server relays transparently, no parsing).</description>
	/// </item>
	/// <item>
	///     <term>Main world client</term>
	///     <description>Sends directly to main world server.</description>
	/// </item>
	/// </list>
	/// Typical use: quest progress reporting, validation requests, data aggregation.
	/// </summary>
	MainServer,

	/// <summary>
	/// Packet is broadcast downstream from the main world server to all endpoints.
	/// Only the main world server is allowed to send this.
	/// <list type="number">
	/// <item>
	///     <term>Main world clients</term>
	///     <description>Direct delivery.</description>
	/// </item>
	/// <item>
	///     <term>All subworld servers</term>
	///     <description>Each subworld server will first execute the packet logic locally, then forward it to its own subworld clients.</description>
	/// </item>
	/// <item>
	///     <term>Subworld clients</term>
	///     <description>Indirectly, via subworld server forwarding.</description>
	/// </item>
	/// </list>
	/// Typical use: global state synchronization, world events, system announcements.
	/// </summary>
	AllDownstream,
}

/// <summary>
/// Manages packet sending, receiving, and routing.
/// </summary>
public partial class PacketResolver
{
	private Mod _mod;
	private Dictionary<int, List<IPacketHandler>> packetHandlerRegistry;
	private Dictionary<Type, int> packetIDMapping;
	private Dictionary<int, Type> packetIDToTypeMapping;
	private int packetIDCounter;

	/// <summary>
	/// Initializes the PacketResolver and registers all packet types and handlers.
	/// </summary>
	public PacketResolver(Mod mod)
	{
		packetIDCounter = 0;
		packetIDMapping = [];
		packetIDToTypeMapping = [];
		packetHandlerRegistry = [];

		_mod = mod;
		RegisterPackets();
	}

	/// <summary>
	/// Queries the packet ID for a given packet type. Returns -1 if not found.
	/// </summary>
	/// <typeparam name="T">The packet type.</typeparam>
	/// <returns>The packet ID, or -1 if not found.</returns>
	public int QueryPacketID<T>()
		where T : IPacket
	{
		return packetIDMapping.TryGetValue(typeof(T), out int packetID) ? packetID : -1;
	}

	/// <summary>
	/// Registers all <see cref="IPacket"/> and <see cref="IPacketHandler"/> implementation types.
	/// </summary>
	private void RegisterPackets()
	{
		var modTypes = Ins.ModuleManager.Types.Where(type => !type.IsAbstract);
		foreach (var type in modTypes.Where(type => type.IsAssignableTo(typeof(IPacket))))
		{
			if (packetIDMapping.TryAdd(type, packetIDCounter))
			{
				packetIDToTypeMapping.Add(packetIDCounter, type);
				packetIDCounter++;
			}
		}

		foreach (var type in modTypes.Where(type => type.IsAssignableTo(typeof(IPacketHandler))))
		{
			// Bind packet to its handler
			if (Attribute.GetCustomAttribute(type, typeof(HandlePacketAttribute)) is HandlePacketAttribute handlePacket)
			{
				if (!packetIDMapping.TryGetValue(handlePacket.PacketType, out int packetID))
				{
					throw new InvalidOperationException("Unknown packet type");
				}

				var handler = Activator.CreateInstance(type) as IPacketHandler;
				if (packetHandlerRegistry.TryGetValue(packetID, out List<IPacketHandler> registeredHandlers))
				{
					registeredHandlers.Add(handler);
				}
				else
				{
					packetHandlerRegistry.Add(packetID, [handler]);
				}
			}
			else
			{
				Ins.Logger.Warn($"Packet Handler {type} does not bind to any packet");
			}
		}

		// Warn if any packet has no handlers bound
		foreach (var packetID in packetIDToTypeMapping)
		{
			if (!packetHandlerRegistry.TryGetValue(packetID.Key, out var registeredHandlers) || registeredHandlers.Count == 0)
			{
				Ins.Logger.Warn($"Packet {packetID.Value} does not have any handler binded");
			}
		}
	}
}
