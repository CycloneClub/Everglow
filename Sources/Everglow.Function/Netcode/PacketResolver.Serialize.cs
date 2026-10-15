using Everglow.Commons.FeatureFlags;
using Everglow.Commons.Netcode.Abstracts;

namespace Everglow.Commons.Netcode;

public partial class PacketResolver
{
	private readonly record struct PacketFrameHeader(
		RouteDestination Destination,
		int LogicalSource,
		int PacketID,
		int PayloadLength);

	private byte[] SerializePacket(IPacket packet, RouteDestination destination, int sourcePlayer)
	{
		int packetID = packetIDMapping[packet.GetType()];
		return Serialize(writer =>
		{
			WriteRouteDestination(writer, destination);
			WriteLogicalSource(writer, sourcePlayer);
			WritePacketID(writer, packetID);
			WritePayload(writer, packet.Send);
		});
	}

	private static byte[] SerializePacketWithData(RouteDestination destination, int sourcePlayer, int packetID, byte[] data)
	{
		return Serialize(writer =>
		{
			WriteRouteDestination(writer, destination);
			WriteLogicalSource(writer, sourcePlayer);
			WritePacketID(writer, packetID);
			WritePayload(writer, data);
		});
	}

	private static byte[] Serialize(Action<BinaryWriter> write)
	{
		using MemoryStream stream = new();
		using BinaryWriter writer = new(stream);

		write(writer);
		writer.Flush();
		return stream.ToArray();
	}

	private static PacketFrameHeader DeserializePacketHeader(BinaryReader reader)
	{
		var destination = ReadRouteDestination(reader);
		var logicalSource = ReadLogicalSource(reader);
		var packetID = ReadPacketID(reader);
		var payloadLength = ReadPayloadLength(reader);

		return new(destination, logicalSource, packetID, payloadLength);
	}

	private static void WriteRouteDestination(BinaryWriter writer, RouteDestination destination) =>
		writer.Write((int)destination);

	private static RouteDestination ReadRouteDestination(BinaryReader reader) =>
		(RouteDestination)reader.ReadInt32();

	private static void WriteLogicalSource(BinaryWriter writer, int logicalSource) =>
		writer.Write(logicalSource);

	private static int ReadLogicalSource(BinaryReader reader) =>
		reader.ReadInt32();

	private static void WritePacketID(BinaryWriter writer, int packetID)
	{
		if (CompileTimeFeatureFlags.NetworkPacketIDUseInt32)
		{
			writer.Write(packetID);
		}
		else
		{
			writer.Write((byte)packetID);
		}
	}

	private static int ReadPacketID(BinaryReader reader)
	{
		return CompileTimeFeatureFlags.NetworkPacketIDUseInt32
			? reader.ReadInt32()
			: reader.ReadByte();
	}

	private static void WritePayloadLength(BinaryWriter writer, int payloadLength) =>
		writer.Write(payloadLength);

	private static int ReadPayloadLength(BinaryReader reader) =>
		reader.ReadInt32();

	private static void WritePayload(BinaryWriter writer, Action<BinaryWriter> write)
	{
		long lengthPosition = writer.BaseStream.Position;
		WritePayloadLength(writer, 0); // Write a placeholder for the payload length, will be overwritten later

		long payloadPosition = writer.BaseStream.Position;
		write(writer); // Write the actual payload data
		long endPosition = writer.BaseStream.Position;

		writer.BaseStream.Position = lengthPosition;
		WritePayloadLength(writer, (int)(endPosition - payloadPosition)); // Write the actual payload length
		writer.BaseStream.Position = endPosition;
	}

	private static void WritePayload(BinaryWriter writer, byte[] data)
	{
		WritePayloadLength(writer, data.Length);
		writer.Write(data);
	}
}
