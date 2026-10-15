using System.Reflection;
using System.Text;
using Everglow.Commons.FeatureFlags;
using Everglow.Commons.Netcode;
using Terraria;

namespace Everglow.UnitTests.Function.Netcode;

[TestClass]
public class PacketResolverTest
{
	[TestMethod]
	public void DeserializePacketHeader_ConsumesOnlyFrameMetadata()
	{
		byte[] payload = [0x12, 0x34, 0x56];
		using MemoryStream stream = new();
		using (BinaryWriter writer = new(stream, Encoding.UTF8, leaveOpen: true))
		{
			writer.Write((int)RouteDestination.MainServer);
			writer.Write(23);
			if (CompileTimeFeatureFlags.NetworkPacketIDUseInt32)
			{
				writer.Write(7);
			}
			else
			{
				writer.Write((byte)7);
			}
			writer.Write(payload.Length);
			writer.Write(payload);
		}

		stream.Position = 0;
		using BinaryReader reader = new(stream);
		MethodInfo? deserializeHeader = typeof(PacketResolver).GetMethod(
			"DeserializePacketHeader",
			BindingFlags.Static | BindingFlags.NonPublic);
		Assert.IsNotNull(deserializeHeader);

		object header = deserializeHeader.Invoke(null, [reader])!;
		T GetHeaderValue<T>(string propertyName) =>
			(T)header.GetType().GetProperty(propertyName)!.GetValue(header)!;

		Assert.AreEqual(RouteDestination.MainServer, GetHeaderValue<RouteDestination>("Destination"));
		Assert.AreEqual(23, GetHeaderValue<int>("LogicalSource"));
		Assert.AreEqual(7, GetHeaderValue<int>("PacketID"));
		Assert.AreEqual(payload.Length, GetHeaderValue<int>("PayloadLength"));

		long expectedPayloadPosition = (sizeof(int) * 3) +
			(CompileTimeFeatureFlags.NetworkPacketIDUseInt32 ? sizeof(int) : sizeof(byte));
		Assert.AreEqual(expectedPayloadPosition, stream.Position);
		CollectionAssert.AreEqual(payload, reader.ReadBytes(payload.Length));
	}

	[TestMethod]
	public void SerializePacketWithData_PreservesWireLayout()
	{
		MethodInfo serializePacket = GetPrivateStaticMethod(
			"SerializePacketWithData",
			typeof(RouteDestination),
			typeof(int),
			typeof(int),
			typeof(byte[]));
		byte[] data = (byte[])serializePacket.Invoke(
			null,
			[RouteDestination.MainServer, 23, 7, new byte[] { 0x12, 0x34, 0x56 }])!;

		byte[] expected = CompileTimeFeatureFlags.NetworkPacketIDUseInt32
			? [0x01, 0x00, 0x00, 0x00, 0x17, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56]
			: [0x01, 0x00, 0x00, 0x00, 0x17, 0x00, 0x00, 0x00, 0x07, 0x03, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56];
		CollectionAssert.AreEqual(expected, data);
	}

	[TestMethod]
	public void WritePayload_WithCallbackBackfillsLength()
	{
		using MemoryStream stream = new();
		using BinaryWriter writer = new(stream);
		MethodInfo writePayload = GetPrivateStaticMethod(
			"WritePayload",
			typeof(BinaryWriter),
			typeof(Action<BinaryWriter>));
		Action<BinaryWriter> write = payloadWriter =>
		{
			payloadWriter.Write((byte)0x12);
			payloadWriter.Write((byte)0x34);
			payloadWriter.Write((byte)0x56);
		};

		writePayload.Invoke(null, [writer, write]);
		writer.Flush();

		CollectionAssert.AreEqual(
			new byte[] { 0x03, 0x00, 0x00, 0x00, 0x12, 0x34, 0x56 },
			stream.ToArray());
	}

	[TestMethod]
	public void EvaluateRoute_MainServerRouteFromSubworldBridge_ExecutesAsLogicalServer()
	{
		object decision = EvaluateRoute(
			RouteDestination.MainServer,
			logicalSource: -1,
			transportSender: Netplay.MaxConnections,
			receiverRole: "MainServer");

		Assert.IsTrue(GetDecisionValue<bool>(decision, "Accepted"));
		Assert.IsFalse(GetDecisionValue<bool>(decision, "Forward"));
		Assert.IsTrue(GetDecisionValue<bool>(decision, "Execute"));
		Assert.AreEqual(-1, GetDecisionValue<int>(decision, "SourcePlayer"));
	}

	[TestMethod]
	public void EvaluateRoute_WorldOnlyRouteFromSubworldBridge_IsRejected()
	{
		object decision = EvaluateRoute(
			RouteDestination.WorldOnly,
			logicalSource: -1,
			transportSender: Netplay.MaxConnections,
			receiverRole: "MainServer");

		Assert.IsFalse(GetDecisionValue<bool>(decision, "Accepted"));
		Assert.IsFalse(GetDecisionValue<bool>(decision, "Forward"));
		Assert.IsFalse(GetDecisionValue<bool>(decision, "Execute"));
	}

	[TestMethod]
	[DataRow("MainServer", 4, 99, 4)]
	[DataRow("Subserver", 4, 99, 4)]
	[DataRow("Client", Netplay.MaxConnections, -1, -1)]
	public void EvaluateRoute_WorldOnlyRouteOnDirectTransport_ExecutesWithTrustedSource(
		string receiverRole,
		int transportSender,
		int logicalSource,
		int expectedSource)
	{
		object decision = EvaluateRoute(
			RouteDestination.WorldOnly,
			logicalSource,
			transportSender,
			receiverRole);

		Assert.IsTrue(GetDecisionValue<bool>(decision, "Accepted"));
		Assert.IsFalse(GetDecisionValue<bool>(decision, "Forward"));
		Assert.IsTrue(GetDecisionValue<bool>(decision, "Execute"));
		Assert.AreEqual(expectedSource, GetDecisionValue<int>(decision, "SourcePlayer"));
	}

	[TestMethod]
	public void EvaluateRoute_MainServerRouteFromSubworldClient_ForwardsWithTransportSource()
	{
		object decision = EvaluateRoute(
			RouteDestination.MainServer,
			logicalSource: 99,
			transportSender: 4,
			receiverRole: "Subserver");

		Assert.IsTrue(GetDecisionValue<bool>(decision, "Accepted"));
		Assert.IsTrue(GetDecisionValue<bool>(decision, "Forward"));
		Assert.IsFalse(GetDecisionValue<bool>(decision, "Execute"));
		Assert.AreEqual(4, GetDecisionValue<int>(decision, "SourcePlayer"));
	}

	[TestMethod]
	public void EvaluateRoute_AllDownstreamRouteFromServerBridge_ExecutesAsLogicalServer()
	{
		object decision = EvaluateRoute(
			RouteDestination.AllDownstream,
			logicalSource: -1,
			transportSender: Netplay.MaxConnections,
			receiverRole: "Client");

		Assert.IsTrue(GetDecisionValue<bool>(decision, "Accepted"));
		Assert.IsFalse(GetDecisionValue<bool>(decision, "Forward"));
		Assert.IsTrue(GetDecisionValue<bool>(decision, "Execute"));
		Assert.AreEqual(-1, GetDecisionValue<int>(decision, "SourcePlayer"));
	}

	private static object EvaluateRoute(
		RouteDestination destination,
		int logicalSource,
		int transportSender,
		string receiverRole)
	{
		Type resolverType = typeof(PacketResolver);
		Type receiverRoleType = resolverType.GetNestedType("PacketReceiverRole", BindingFlags.NonPublic)!;
		object role = Enum.Parse(receiverRoleType, receiverRole);
		return resolverType
			.GetMethod("EvaluateRoute", BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, [destination, logicalSource, transportSender, role])!;
	}

	private static MethodInfo GetPrivateStaticMethod(string name, params Type[] parameterTypes)
	{
		MethodInfo? method = typeof(PacketResolver).GetMethod(
			name,
			BindingFlags.Static | BindingFlags.NonPublic,
			binder: null,
			types: parameterTypes,
			modifiers: null);
		Assert.IsNotNull(method);
		return method;
	}

	private static T GetDecisionValue<T>(object decision, string propertyName) =>
		(T)decision.GetType().GetProperty(propertyName)!.GetValue(decision)!;
}
