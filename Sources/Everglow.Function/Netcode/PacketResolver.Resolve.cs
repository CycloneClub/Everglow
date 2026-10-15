using Everglow.Commons.Netcode.Abstracts;
using Everglow.Commons.Utilities;
using SubworldLibrary;

namespace Everglow.Commons.Netcode;

public partial class PacketResolver
{
	private enum PacketReceiverRole
	{
		MainServer,
		Subserver,
		Client,
	}

	private readonly record struct PacketRouteDecision(
		bool Accepted,
		bool Forward,
		bool Execute,
		int SourcePlayer);

	/// <summary>
	/// Handles and resolves incoming packets.
	/// </summary>
	/// <param name="reader">The binary reader containing the packet data.</param>
	/// <param name="whoAmI">The transport-level sender supplied by <see cref="Mod.HandlePacket"/>.</param>
	public void Resolve(BinaryReader reader, int whoAmI)
	{
		var header = DeserializePacketHeader(reader);
		PacketRouteDecision route = EvaluateRoute(
			header.Destination,
			header.LogicalSource,
			whoAmI,
			GetReceiverRole());
		if (!route.Accepted)
		{
			Ins.Logger.Warn($"Rejected packet route [{header.Destination}] from transport sender [{whoAmI}]");
			return;
		}

		if (route.Forward)
		{
			var payloadPosition = reader.BaseStream.Position;
			byte[] remainingData = reader.ReadBytes(header.PayloadLength);

			var forwardPacket = SerializePacketWithData(header.Destination, route.SourcePlayer, header.PacketID, remainingData);

			if (header.Destination == RouteDestination.AllDownstream)
			{
				// Forward and execute the packet logic
				var modPacket = GetPacket();
				modPacket.Write(forwardPacket);
				modPacket.Send();

				reader.BaseStream.Position = payloadPosition;
			}
			else if (header.Destination == RouteDestination.MainServer)
			{
				// Forward but not execute packet logic
				Ins.Logger.Debug("Forward packet is sent...");
				SubworldSystem.SendToMainServer(_mod, forwardPacket);
				return;
			}
		}

		if (!route.Execute)
		{
			return;
		}

		if (!packetHandlerRegistry.TryGetValue(header.PacketID, out List<IPacketHandler> registeredHandlers))
		{
			Ins.Logger.Warn($"Received a packet [{header.PacketID}] without handler, automatically ignored");
			return;
		}

		// Read the packet data
		var packet = Activator.CreateInstance(packetIDToTypeMapping[header.PacketID]) as IPacket;
		packet.Receive(reader, route.SourcePlayer);

		// Invoke handlers to process the packet
		foreach (var handler in registeredHandlers)
		{
			handler.Handle(packet, route.SourcePlayer);
		}
	}

	private static PacketRouteDecision EvaluateRoute(
		RouteDestination destination,
		int logicalSource,
		int transportSender,
		PacketReceiverRole receiverRole)
	{
		int sourcePlayer = IsClientSlot(transportSender) ? transportSender : logicalSource;
		return destination switch
		{
			RouteDestination.WorldOnly => EvaluateWorldOnlyRoute(transportSender, receiverRole, sourcePlayer),
			RouteDestination.MainServer => EvaluateMainServerRoute(transportSender, receiverRole, sourcePlayer),
			RouteDestination.AllDownstream => EvaluateAllDownstreamRoute(transportSender, receiverRole, sourcePlayer),
			_ => new(false, false, false, sourcePlayer),
		};
	}

	private static PacketRouteDecision EvaluateWorldOnlyRoute(
		int transportSender,
		PacketReceiverRole receiverRole,
		int sourcePlayer)
	{
		if (receiverRole == PacketReceiverRole.Client && transportSender == Netplay.MaxConnections)
		{
			return new(true, false, true, sourcePlayer);
		}
		else if (receiverRole != PacketReceiverRole.Client && IsClientSlot(transportSender))
		{
			return new(true, false, true, sourcePlayer);
		}
		else
		{
			return new(false, false, false, sourcePlayer);
		}
	}

	private static PacketRouteDecision EvaluateMainServerRoute(
		int transportSender,
		PacketReceiverRole receiverRole,
		int sourcePlayer)
	{
		if (receiverRole == PacketReceiverRole.Subserver && IsClientSlot(transportSender))
		{
			return new(true, true, false, sourcePlayer);
		}
		else if (receiverRole == PacketReceiverRole.MainServer
			&& (IsClientSlot(transportSender) || transportSender == Netplay.MaxConnections))
		{
			return new(true, false, true, sourcePlayer);
		}
		else
		{
			return new(false, false, false, sourcePlayer);
		}
	}

	private static PacketRouteDecision EvaluateAllDownstreamRoute(
		int transportSender,
		PacketReceiverRole receiverRole,
		int sourcePlayer)
	{
		if (receiverRole == PacketReceiverRole.Subserver && transportSender == Netplay.MaxConnections)
		{
			return new(true, true, true, sourcePlayer);
		}
		else if (receiverRole == PacketReceiverRole.Client && transportSender == Netplay.MaxConnections)
		{
			return new(true, false, true, sourcePlayer);
		}
		else
		{
			return new(false, false, false, sourcePlayer);
		}
	}

	private static bool IsClientSlot(int transportSender) =>
		transportSender >= 0 && transportSender < Main.maxPlayers;

	private static PacketReceiverRole GetReceiverRole()
	{
		if (NetUtils.IsMainServer)
		{
			return PacketReceiverRole.MainServer;
		}
		else if (NetUtils.IsSubServer)
		{
			return PacketReceiverRole.Subserver;
		}
		else
		{
			return PacketReceiverRole.Client;
		}
	}
}
