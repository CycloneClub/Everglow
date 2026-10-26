using Everglow.Commons.Utilities;
using Terraria.ObjectData;

namespace Everglow.Myth.TheFirefly.Tiles;

public class DarkFlowerPlant : ModTile
{
	internal const int StyleCount = 9;
	internal const int FrameStride = 18;
	private const int VariantsPerStage = 3;
	private const int BloomStyleStart = 6;
	private const int BloomFrameWidth = 22;
	private const int FlowerFrameHeight = 34;
	private const int BloomFrameStride = 24;
	private const int WorldGenSpawnChanceDenominator = 250;
	private const int NaturalSpawnChanceDenominator = 1750;
	private const double TicksPerDay = 86400;

	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileCut[Type] = true;
		Main.tileNoAttach[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.CoordinateWidth = FrameStride;
		TileObjectData.newTile.CoordinateHeights = new[] { FlowerFrameHeight };
		TileObjectData.newTile.CoordinatePadding = 0;
		TileObjectData.newTile.DrawYOffset = -16;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.RandomStyleRange = VariantsPerStage;
		TileObjectData.newTile.AnchorValidTiles = new[] { ModContent.TileType<DarkCocoon>() };
		TileObjectData.addTile(Type);
		RegisterItemDrop(ModContent.ItemType<Misc.Items.Weapons.DarkFlower>());
		AddMapEntry(new Color(100, 80, 160), ModContent.GetInstance<Misc.Items.Weapons.DarkFlower>().DisplayName);
		HitSound = SoundID.Grass;
		DustType = DustID.SpookyWood;
	}

	public override bool CanDrop(int i, int j)
	{
		// 只在开花阶段掉落(阶段3)
		int style = Main.tile[i, j].TileFrameX / FrameStride;
		return style >= BloomStyleStart;
	}

	public override void RandomUpdate(int i, int j)
	{
		Tile flower = Main.tile[i, j];
		if (flower.TileFrameX >= BloomStyleStart * FrameStride)
		{
			return;
		}

		double chance = GetStageGrowthChance(Main.maxTilesX, Main.maxTilesY, (int)Main.worldSurface, j, Main.remixWorld);
		if (WorldGen.genRand.NextDouble() >= chance)
		{
			return;
		}

		// 每次只升一级，保留组内变体；阶段随 Tile 帧数据保存。
		flower.TileFrameX += VariantsPerStage * FrameStride;
		if (Main.netMode == NetmodeID.Server)
		{
			NetMessage.SendTileSquare(-1, i, j);
		}
	}

	internal static double GetStageGrowthChance(int worldWidth, int worldHeight, int worldSurface, int tileY, bool remixWorld)
	{
		// 对应 WorldGen.UpdateWorld_Inner 的抽样范围和普通流速下的抽样次数。
		bool underground = tileY >= worldSurface - 1;
		int sampledHeight = underground ? worldHeight - worldSurface - 19 : worldSurface - 11;
		float samplingRate = underground ? (remixWorld ? 2.5E-05f : 1.5E-05f) : 3E-05f;
		double samplesPerTick = Math.Ceiling((double)worldWidth * worldHeight * samplingRate);
		// Remix 世界地下每个抽样点依次执行地下、地表两次随机更新。
		int callbacksPerSample = underground && remixWorld ? 2 : 1;
		double updatesPerDay = samplesPerTick * callbacksPerSample * TicksPerDay / ((double)(worldWidth - 20) * sampledHeight);
		return 1 / updatesPerDay;
	}

	internal static bool TryGrow(int i, int groundY)
	{
		Tile ground = Main.tile[i, groundY];
		Tile flower = Main.tile[i, groundY - 1];
		Tile flowerTop = Main.tile[i, groundY - 2];
		if (!ground.HasTile || ground.TileType != ModContent.TileType<DarkCocoon>() || ground.IsActuated || ground.IsHalfBlock || ground.Slope != SlopeType.Solid || flower.HasTile || flower.LiquidAmount > 0 || flowerTop.HasTile || flowerTop.LiquidAmount > 0)
		{
			return false;
		}

		// 近似原版自然恩赐的放置分支；自然补生还包含约 1/7 的地面生长机会。
		int spawnChance = WorldGen.gen ? WorldGenSpawnChanceDenominator : NaturalSpawnChanceDenominator;
		if (!WorldGen.genRand.NextBool(spawnChance))
		{
			return false;
		}

		flower.ClearTile();
		flower.HasTile = true;
		flower.TileType = (ushort)ModContent.TileType<DarkFlowerPlant>();
		int styleCount = WorldGen.gen ? StyleCount : VariantsPerStage;
		flower.TileFrameX = (short)(WorldGen.genRand.Next(styleCount) * FrameStride);
		flower.TileFrameY = 0;
		if (!WorldGen.gen && Main.netMode == NetmodeID.Server)
		{
			NetMessage.SendTileSquare(-1, i, groundY - 1);
		}
		return true;
	}

	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
	{
		// 世界帧统一按 18 像素存储；图集中第三组改为 22 像素宽、2 像素间隔。
		int style = tileFrameX / FrameStride;
		if (style >= BloomStyleStart)
		{
			width = BloomFrameWidth;
			tileFrameX = (short)(BloomStyleStart * FrameStride + (style - BloomStyleStart) * BloomFrameStride);
		}
		tileFrameY = 0;
	}
}
