using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.CyanVine;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class TownOreFramingTests
{
	[TestInitialize]
	public void Initialize() => Program.SavePath = string.Empty;

	[TestMethod]
	[DataRow(typeof(CyanVineStone))]
	[DataRow(typeof(YggdrasilAmber))]
	public void OreUsesMergeFramesAtHostEdgesAndKeepsExposedEdges(Type contentType)
	{
		const ushort oreType = TileID.Iron;
		const int x = 10;
		const int y = 10;
		ushort host = (ushort)ModContent.TileType<StoneScaleWood>();
		var content = (ModTile)Activator.CreateInstance(contentType)!;
		typeof(ModBlockType).GetProperty(nameof(ModBlockType.Type))!.SetValue(content, oreType);
		var types = Main.tile.GetData<TileTypeData>();
		var states = Main.tile.GetData<TileWallWireStateData>();
		int[] indices = Enumerable.Range(x - 1, 3).SelectMany(column => Enumerable.Range(y - 1, 3).Select(row => column * Main.tile.Height + row)).ToArray();
		var oldTypes = indices.Select(index => types[index]).ToArray();
		var oldStates = indices.Select(index => states[index]).ToArray();
		bool oldOreStone = Main.tileStone[oreType];
		bool oldHostStone = Main.tileStone[host];
		try
		{
			Main.tileStone[oreType] = false;
			Main.tileStone[host] = false;
			for (int layout = 0; layout < 6; layout++)
			{
				for (int variant = 0; variant < 3; variant++)
				{
					foreach (int index in indices)
					{
						types[index] = default;
						states[index] = default;
					}
					for (int dx = -1; dx <= 1; dx++)
					{
						for (int dy = -1; dy <= 1; dy++)
						{
							var neighbor = Main.tile[x + dx, y + dy];
							neighbor.HasTile = layout is 0 or 2 or 4;
							neighbor.TileType = layout == 0 ? host : oreType;
						}
					}
					var tile = Main.tile[x, y];
					tile.HasTile = true;
					tile.TileType = oreType;
					tile.TileFrameNumber = variant;
					if (layout == 2)
					{
						Main.tile[x + 1, y].TileType = host;
					}
					if (layout is 3 or 5)
					{
						var below = Main.tile[x, y + 1];
						var left = Main.tile[x - 1, y];
						var right = Main.tile[x + 1, y];
						below.HasTile = true;
						Main.tile[x, y + 1].TileType = host;
						left.HasTile = layout == 3;
						right.HasTile = layout == 3;
						below.Slope = layout == 5 ? SlopeType.SlopeDownRight : SlopeType.Solid;
					}
					if (layout == 4)
					{
						Main.tile[x + 1, y + 1].TileType = host;
					}
					bool reset = false;
					bool noBreak = false;
					Assert.IsFalse(content.TileFrame(x, y, ref reset, ref noBreak));
					var expected = layout switch
					{
						0 => (108 + variant * 18, 198),
						2 => (144, 126 + variant * 18),
						3 => (234 + variant * 18, 0),
						4 => (0, 90 + variant * 36),
						_ => (162 + variant * 18, 54),
					};
					Assert.AreEqual(expected.Item1, (int)tile.TileFrameX, $"{contentType.Name}, layout {layout}, variant {variant}");
					Assert.AreEqual(expected.Item2, (int)tile.TileFrameY, $"{contentType.Name}, layout {layout}, variant {variant}");
					Assert.AreEqual(variant, tile.TileFrameNumber);
				}
			}
		}
		finally
		{
			Main.tileStone[oreType] = oldOreStone;
			Main.tileStone[host] = oldHostStone;
			for (int i = 0; i < indices.Length; i++)
			{
				types[indices[i]] = oldTypes[i];
				states[indices[i]] = oldStates[i];
			}
		}
	}
}
