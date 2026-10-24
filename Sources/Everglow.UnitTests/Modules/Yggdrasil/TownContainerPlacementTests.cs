using Everglow.Yggdrasil.WorldGeneration;
using Terraria;
using Terraria.ID;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class TownContainerPlacementTests
{
	private int[] indices = [];
	private TileTypeData[] savedTypes = [];
	private TileWallWireStateData[] savedStates = [];

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		indices = Enumerable.Range(35, 40)
			.SelectMany(x => Enumerable.Range(35, 35).Select(y => x * Main.tile.Height + y))
			.ToArray();
		var types = Main.tile.GetData<TileTypeData>();
		var states = Main.tile.GetData<TileWallWireStateData>();
		savedTypes = indices.Select(index => types[index]).ToArray();
		savedStates = indices.Select(index => states[index]).ToArray();
		foreach (int index in indices)
		{
			types[index] = default;
			states[index] = default;
		}
	}

	[TestCleanup]
	public void Cleanup()
	{
		var types = Main.tile.GetData<TileTypeData>();
		var states = Main.tile.GetData<TileWallWireStateData>();
		for (int i = 0; i < indices.Length; i++)
		{
			types[indices[i]] = savedTypes[i];
			states[indices[i]] = savedStates[i];
		}
	}

	[TestMethod]
	[DataRow(TileID.Containers, 45, 45)]
	[DataRow(TileID.Dressers, 45, 45)]
	[DataRow(TileID.Containers, 66, 59)]
	[DataRow(TileID.Dressers, 66, 59)]
	public void RejectsContainersAwayFromTheAreaOrigin(int type, int x, int y)
	{
		PlaceContainer((ushort)type, x, y);

		Assert.IsFalse(YggdrasilWorldGeneration.ChestSafeArea(40, 40, 26, 19));
	}

	[TestMethod]
	[DataRow(TileID.Containers)]
	[DataRow(TileID.Dressers)]
	public void RejectsReplacingContainerSupports(int type)
	{
		PlaceContainer((ushort)type, 45, 38);

		Assert.IsFalse(YggdrasilWorldGeneration.ChestSafeArea(40, 40, 26, 19));
	}

	[TestMethod]
	public void RejectsPartialDresserOverlapWithItsOriginOutsideTheNewRoom()
	{
		// The 26x23 twilight room's dresser occupies (58..60, 55..56).
		// A second room starting at (60, 40) would overwrite its right column.
		PlaceContainer(TileID.Dressers, 58, 55);

		Assert.IsFalse(YggdrasilWorldGeneration.ChestSafeArea(60, 40, 10, 19));
	}

	[TestMethod]
	[DataRow(TileID.Containers)]
	[DataRow(TileID.Dressers)]
	public void AllowsAreaBesideAnIntactContainer(int type)
	{
		PlaceContainer((ushort)type, 67, 45);

		Assert.IsTrue(YggdrasilWorldGeneration.ChestSafeArea(40, 40, 26, 19));
	}

	[TestMethod]
	public void AllowsEmptyAreaWithStaleContainerTypes()
	{
		Main.tile[40, 40].TileType = TileID.Containers;
		Main.tile[40, 39].TileType = TileID.Dressers;

		Assert.IsTrue(YggdrasilWorldGeneration.ChestSafeArea(40, 40, 26, 19));
	}

	private static void PlaceContainer(ushort type, int x, int y)
	{
		int width = type == TileID.Dressers ? 3 : 2;
		for (int dx = 0; dx < width; dx++)
		{
			for (int dy = 0; dy < 2; dy++)
			{
				var tile = Main.tile[x + dx, y + dy];
				tile.HasTile = true;
				tile.TileType = type;
				tile.TileFrameX = (short)(dx * 18);
				tile.TileFrameY = (short)(dy * 18);
			}
		}
	}
}
