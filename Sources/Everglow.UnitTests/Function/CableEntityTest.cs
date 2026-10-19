using Everglow.Commons.TileHelper;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Function;

[TestClass]
[DoNotParallelize]
public class CableEntityTest
{
	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
	}

	[DataTestMethod]
	[DataRow(37, -12)]
	[DataRow(-125, 125)]
	[DataRow(0, 120)]
	[DataRow(-120, 0)]
	[DataRow(0, 0)]
	public void NetRoundTrip_PreservesEndpointOffset(int x, int y)
	{
		var source = new CableEneity { ToTail = new Point(x, y) };
		var destination = new CableEneity { ToTail = new Point(9, 9) };

		Transfer(source, destination);

		Assert.AreEqual(source.ToTail, destination.ToTail);
	}

	[TestMethod]
	public void SavedCable_NetRoundTrip_PreservesWorldConnection()
	{
		var source = new CableEneity();
		source.LoadData(new TagCompound
		{
			["CableToTailX"] = -80,
			["CableToTailY"] = 24,
		});
		var destination = new CableEneity();

		Transfer(source, destination);

		Assert.AreEqual(new Point(-80, 24), destination.ToTail);
		var saved = new TagCompound();
		destination.SaveData(saved);
		Assert.AreEqual(-80, saved.GetInt("CableToTailX"));
		Assert.AreEqual(24, saved.GetInt("CableToTailY"));
	}

	[TestMethod]
	public void UnconnectedNode_DoesNotRequireWorldOrGraphicsToSkipSelfConnection()
	{
		var cable = new CableCarJoint();

		Assert.IsNull(cable.ConnectRope(0, 0, 0, 0));
	}

	private static void Transfer(CableEneity source, CableEneity destination)
	{
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		source.NetSend(writer);
		writer.Flush();
		stream.Position = 0;
		using var reader = new BinaryReader(stream);
		destination.NetReceive(reader);
		Assert.AreEqual(stream.Length, stream.Position);
	}
}
