using System.Reflection;
using Everglow.Commons.TileHelper;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.UnitTests.Function.TileHelper;

[TestClass]
[DoNotParallelize]
public class MapIOVersionTest
{
	private static readonly FieldInfo ChestLookup = typeof(Chest).GetField("_chestsByCoords", BindingFlags.NonPublic | BindingFlags.Static)!;
	private object originalChestLookup = null!;
	private Chest[] originalChests = null!;
	private Dictionary<int, TileEntity> originalById = null!;
	private Dictionary<Point16, TileEntity> originalByPosition = null!;
	private List<TileEntity> originalUpdates = null!;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		originalChests = Main.chest;
		originalChestLookup = ChestLookup.GetValue(null)!;
		Main.chest = new Chest[2];
		ChestLookup.SetValue(null, new Dictionary<Point, Chest>());
		originalById = TileEntity.ByID;
		originalByPosition = TileEntity.ByPosition;
		originalUpdates = TileEntity.UpdateEntities;
		TileEntity.ByID = [];
		TileEntity.ByPosition = [];
		TileEntity.UpdateEntities = [];
	}

	[TestCleanup]
	public void Cleanup()
	{
		Main.chest = originalChests;
		ChestLookup.SetValue(null, originalChestLookup);
		TileEntity.ByID = originalById;
		TileEntity.ByPosition = originalByPosition;
		TileEntity.UpdateEntities = originalUpdates;
	}

	[TestMethod]
	public void ChestCapacity_RoundTripsAndRegistersRelocatedChest()
	{
		var chest = Chest.CreateWorldChest(0, 2, 3);
		chest.Resize(80);
		chest.name = "Expanded chest";
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
		MapIO.WriteChest(writer, new Rectangle(0, 0, 10, 10), withoutItem: true);
		Chest.RemoveChest(0);
		stream.Position = 0;
		using var reader = new BinaryReader(stream);
		MapIO.ReadChest(reader, new Rectangle(20, 30, 10, 10), withoutItem: true);
		Assert.AreEqual(80, Main.chest[0].maxItems);
		Assert.AreEqual("Expanded chest", Main.chest[0].name);
		Assert.AreEqual(0, Chest.FindChest(22, 33));
		Assert.AreEqual(-1, Chest.FindChest(2, 3));
		Assert.AreEqual(stream.Length, stream.Position);
	}

	[TestMethod]
	public void LegacyChestSection_RetainsFortySlots()
	{
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
		writer.Write(1);
		writer.Write(2);
		writer.Write(3);
		writer.Write("Legacy chest");
		writer.Write(0);
		writer.Write(1234567);
		stream.Position = 0;
		using var reader = new BinaryReader(stream);
		MapIO.ReadChest(reader, new Rectangle(0, 0, 10, 10), withoutItem: true);
		Assert.AreEqual(40, Main.chest[0].maxItems);
		Assert.AreEqual(0, Chest.FindChest(2, 3));
		Assert.AreEqual(1234567, reader.ReadInt32());
	}
	[TestMethod]
	public void WriteEntitySection_RecordsTerrariaPayloadVersion()
	{
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
		MapIO.WriteTileEntity(writer, new Rectangle(0, 0, 10, 10), new ModEntry());
		stream.Position = 0;
		using var reader = new BinaryReader(stream);
		Assert.AreEqual(-1, reader.ReadInt32());
		Assert.AreEqual(Main.curRelease, reader.ReadInt32());
		Assert.AreEqual(0, reader.ReadInt32());
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ReadEntitySection_AcceptsLegacyAndVersionedSectionsWithoutConsumingFollowingData(bool versioned)
	{
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
		if (versioned)
		{
			writer.Write(-1);
			writer.Write(Main.curRelease);
		}
		writer.Write(0);
		writer.Write(1234567);
		stream.Position = 0;
		using var reader = new BinaryReader(stream);
		MapIO.ReadTileEntity(reader, new Rectangle(0, 0, 10, 10), new ModEntry());
		Assert.AreEqual(1234567, reader.ReadInt32());
		Assert.AreEqual(0, TileEntity.ByID.Count);
		Assert.AreEqual(0, TileEntity.UpdateEntities.Count);
	}
}
