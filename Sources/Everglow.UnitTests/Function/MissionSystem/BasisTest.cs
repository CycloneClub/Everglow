using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Function.MissionSystem;

[TestClass]
public class BasisTest
{
	public TestContext TestContext { get; set; } = default!;

	[TestMethod]
	public void TagCompoundExceptionTest()
	{
		var keyStringList = "testList";
		var keyTagList = "testList2";
		var baseTag = new TagCompound
		{
			{ keyStringList, new List<string> { "a", "b", "c" } },
			{ keyTagList, new List<TagCompound> { new(), new() } },
		};

		Assert.ThrowsExactly<IOException>(() =>
		{
			var list = baseTag.GetCompound(keyStringList);
		});

		Assert.ThrowsExactly<IOException>(() =>
		{
			var list = baseTag.GetCompound(keyTagList);
		});

		TagCompound? value1 = null;
		TagCompound? value2 = null;

		Assert.IsNull(value1);
		Assert.IsNull(value2);

		try
		{
			value1 = baseTag.GetCompound(keyStringList);
			value2 = baseTag.GetCompound(keyTagList);
		}
		catch (IOException)
		{
			TestContext.WriteLine("Caught IOException as expected.");
		}
		finally
		{
			value1 = new TagCompound();
			value2 = new TagCompound();
			TestContext.WriteLine("Test completed.");
		}

		Assert.IsNotNull(value1);
		Assert.IsNotNull(value2);
	}

	[TestMethod]
	public void RemainingData_Should_BeForwardedWithoutParsing()
	{
		// Arrange: Client builds original progress packet
		int expectedPacketId = 132;
		int expectedData1 = 2;
		int expectedData2 = 3;
		int expectedData3 = 4;

		byte[] originalPacket = BuildProgressPacket(expectedPacketId, expectedData1, expectedData2, expectedData3);

		// Act: Sub-world forwards packet (reads only ID, business data remains opaque)
		byte[] forwardedPacket = ForwardWithoutParsing(originalPacket);

		// Assert: Main-world can fully parse the forwarded packet
		(int packetId, int data1, int data2, int data3) = ParseProgressPacket(forwardedPacket);

		Assert.AreEqual(expectedPacketId, packetId);
		Assert.AreEqual(expectedData2, data2);
		Assert.AreEqual(expectedData1, data1);
		Assert.AreEqual(expectedData3, data3);
	}

	/// <summary>
	/// Client: Builds a complete progress packet
	/// </summary>
	private static byte[] BuildProgressPacket(int packetId, int data1, int data2, int data3)
	{
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);

		writer.Write(packetId);
		writer.Write(data1);
		writer.Write(data2);
		writer.Write(data3);

		return stream.ToArray();
	}

	/// <summary>
	/// Sub-world: Reads only PacketId, forwards remaining data as opaque byte[]
	/// </summary>
	private static byte[] ForwardWithoutParsing(byte[] receivedPacket)
	{
		using var reader = new BinaryReader(new MemoryStream(receivedPacket));

		int packetId = reader.ReadInt32();
		byte[] remainingData = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));

		// Repackage: PacketId + opaque business data
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);

		writer.Write(packetId);
		writer.Write(remainingData);

		return stream.ToArray();
	}

	/// <summary>
	/// Main-world: Fully parses the progress packet
	/// </summary>
	private static (int PacketId, int Data1, int Data2, int Data3) ParseProgressPacket(byte[] packet)
	{
		using var reader = new BinaryReader(new MemoryStream(packet));

		int packetId = reader.ReadInt32();
		int data1 = reader.ReadInt32();
		int data2 = reader.ReadInt32();
		int data3 = reader.ReadInt32();

		return (packetId, data1, data2, data3);
	}
}
