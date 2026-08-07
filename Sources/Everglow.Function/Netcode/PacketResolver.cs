using Everglow.Commons.FeatureFlags;
using Everglow.Commons.Netcode.Abstracts;
using Everglow.Commons.Utilities;
using SubworldLibrary;

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
	/// Typical use: mission progress reporting, validation requests, data aggregation.
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
public class PacketResolver
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

	private ModPacket GetPacket()
	{
		return _mod.GetPacket();
	}

	private byte[] SerializePacket(IPacket packet, RouteDestination destination, int sourcePlayer)
	{
		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);

		// 1. Write route destination
		bw.Write((int)destination);

		// 2. Write source player ID
		bw.Write(sourcePlayer);

		// 3. Write packet ID
		int id = packetIDMapping[packet.GetType()];
		if (CompileTimeFeatureFlags.NetworkPacketIDUseInt32)
		{
			bw.Write(id);
		}
		else
		{
			bw.Write((byte)id);
		}

		// 4. Write packet data (length + data)
		var lengthPos = ms.Position;
		bw.Write(0);

		var startPos = ms.Position;
		packet.Send(bw);
		var endPos = ms.Position;

		ms.Position = lengthPos;
		bw.Write((int)(endPos - startPos));

		ms.Position = endPos;
		bw.Flush();

		return ms.ToArray();
	}

	private static byte[] SerializePacketWithData(RouteDestination destination, int sourcePlayer, int packetID, byte[] data)
	{
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);

		writer.Write((int)destination);
		writer.Write(sourcePlayer);
		if (CompileTimeFeatureFlags.NetworkPacketIDUseInt32)
		{
			writer.Write(packetID);
		}
		else
		{
			writer.Write((byte)packetID);
		}
		writer.Write(data.Length);
		writer.Write(data);

		writer.Flush();
		return stream.ToArray();
	}

	private static RouteDestination DeserializeRouteDestination(BinaryReader reader)
	{
		return (RouteDestination)reader.ReadInt32();
	}

	private void Send(IPacket packet, RouteDestination destination, int toClient = -1, int ignoreClient = -1)
	{
		if (NetUtils.IsSingle)
		{
			return;
		}

		var sourcePlayer = NetUtils.IsServer ? ignoreClient : Main.myPlayer;
		var data = SerializePacket(packet, destination, sourcePlayer);

		var modPacket = GetPacket();
		modPacket.Write(data);
		modPacket.Send(toClient, ignoreClient);
	}

	/// <summary>
	/// Sends a packet instance to the specified targets.
	/// </summary>
	/// <param name="packet">The packet to send.</param>
	/// <param name="toClient">Target client ID, or -1 for all clients.</param>
	/// <param name="ignoreClient">Client ID to ignore, or -1 to ignore none.</param>
	public void Send(IPacket packet, int toClient = -1, int ignoreClient = -1)
	{
		Send(packet, RouteDestination.WorldOnly, toClient, ignoreClient);
	}

	/// <summary>
	/// Sends a packet with automatic target determination based on the fromServer flag.
	/// <br/> Wrapper around <see cref="Send(IPacket, int, int)"/>.
	/// </summary>
	/// <param name="packet">The packet to send.</param>
	/// <param name="fromServer">If true, sends to all clients except the specified player. If false, sends to all clients.</param>
	/// <param name="player">The player to potentially ignore when fromServer is true.</param>
	public void Send(IPacket packet, bool fromServer, Player player)
	{
		if (fromServer)
		{
			Send(packet, -1, player.whoAmI);
		}
		else
		{
			Send(packet);
		}
	}

	/// <summary>
	/// Routes a packet according to the specified destination.
	/// </summary>
	public void Route(IPacket packet, RouteDestination destination)
	{
		if (NetUtils.IsSingle)
		{
			return;
		}

		Debug.Assert(destination is not RouteDestination.WorldOnly, "Use Send() to send world only packets.");

		switch (destination)
		{
			case RouteDestination.MainServer:
				{
					if (NetUtils.IsMainClient)
					{
						// Main client -> Main world
						Send(packet);
					}
					else if (NetUtils.IsSubServer)
					{
						// Sub world -> Main world
						var data = SerializePacket(packet, RouteDestination.MainServer, -1);
						SubworldSystem.SendToMainServer(_mod, data);
					}
					else if (NetUtils.IsSubClient)
					{
						// Sub client -> Sub world -> Main world
						Send(packet, RouteDestination.MainServer);
					}
				}
				break;
			case RouteDestination.AllDownstream:
				{
					Debug.Assert(NetUtils.IsMainServer, "All downstream can only be sent from main world server!");

					// Send to main clients
					Send(packet);

					// Send to all sub servers
					var data = SerializePacket(packet, RouteDestination.AllDownstream, -1);
					SubworldSystem.SendToAllSubservers(_mod, data);
				}
				break;
		}
	}

	/// <summary>
	/// Handles and resolves incoming packets.
	/// </summary>
	/// <param name="reader">The binary reader containing the packet data.</param>
	/// <param name="_">Passed by <see cref="Mod.HandlePacket"/>. Unused but required for compatibility.</param>
	public void Resolve(BinaryReader reader, int _)
	{
		// Read route destination
		var destination = DeserializeRouteDestination(reader);

		// Read source player ID
		var sourcePlayer = reader.ReadInt32();

		// Read packet ID
		int packetID;
		if (CompileTimeFeatureFlags.NetworkPacketIDUseInt32)
		{
			packetID = reader.ReadInt32();
		}
		else
		{
			packetID = reader.ReadByte();
		}

		// Read data length
		var length = reader.ReadInt32();

		// Forward packets if needed
		bool shouldForward = NetUtils.IsSubServer && destination != RouteDestination.WorldOnly;
		if (shouldForward)
		{
			var headPosition = reader.BaseStream.Position;
			byte[] remainingData = reader.ReadBytes(length);

			var forwardPacket = SerializePacketWithData(destination, sourcePlayer, packetID, remainingData);

			if (destination == RouteDestination.AllDownstream)
			{
				var modPacket = GetPacket();
				modPacket.Write(forwardPacket);
				modPacket.Send();

				reader.BaseStream.Position = headPosition;
			}
			else if (destination == RouteDestination.MainServer)
			{
				// Forward only, no executing packet logic
				Ins.Logger.Debug("Forward packet is sent...");
				SubworldSystem.SendToMainServer(_mod, forwardPacket);
				return;
			}
		}

		if (!packetHandlerRegistry.TryGetValue(packetID, out List<IPacketHandler> registeredHandlers))
		{
			Ins.Logger.Warn($"Received a packet [{packetID}] without handler, automatically ignored");
			return;
		}

		// Read the packet data
		var packet = Activator.CreateInstance(packetIDToTypeMapping[packetID]) as IPacket;
		packet.Receive(reader, sourcePlayer);

		// Invoke handlers to process the packet
		foreach (var handler in registeredHandlers)
		{
			handler.Handle(packet, sourcePlayer);
		}
	}
}
