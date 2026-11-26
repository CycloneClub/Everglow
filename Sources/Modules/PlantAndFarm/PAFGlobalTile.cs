using Everglow.PlantAndFarm.Items.Materials;
using Terraria.DataStructures;

namespace Everglow.PlantAndFarm;

public class PAFGlobalTile : GlobalTile
{
	private const int PlantFrameWidth = 18;
	private const int HorizontalSpread = 4;
	private const int VerticalSpread = 2;

	private readonly Dictionary<int, Flower> flowersByStyle = new();

	public override void SetStaticDefaults()
	{
		// TileID.Plants 的贴图列号，同一行的外观对应同一种野花。
		AddFlower<PurpleTail>([6]);
		AddFlower<DarkPoppy>([7]);
		AddFlower<BlueFreeze>([9]);
		AddFlower<LightChrysanthemum>([10, 11], canSpread: true);
		AddFlower<Lavender>([12]);
		AddFlower<GoldWhip>([13, 18], canSpread: true);
		AddFlower<WhiteStar>([14]);
		AddFlower<SilverClock>([15]);
		AddFlower<BluePedal>([16]);
		AddFlower<PinkSun>([17]);
		AddFlower<OrangeSausage>([19]);
		AddFlower<CyanHyacinth>([20]);
		AddFlower<RedFlame>([21, 22, 23], canSpread: true);
		AddFlower<GoldCup>([24, 25, 26], canSpread: true);
		AddFlower<ShallowNight>([27, 28, 29], canSpread: true);
		AddFlower<PurplePhantom>([30, 31, 32], canSpread: true);
		AddFlower<HotPinkTulip>([33, 34, 35], canSpread: true);
		AddFlower<WhiteTulip>([36, 37, 38], canSpread: true);
		AddFlower<OrangeTulip>([39, 40, 41], canSpread: true);
		AddFlower<LightPurpleBalls>([42, 43, 44], canSpread: true);
	}

	public override void RandomUpdate(int i, int j, int type)
	{
		if (type != TileID.Plants || !flowersByStyle.TryGetValue(Main.tile[i, j].TileFrameX / PlantFrameWidth, out var flower) || !flower.CanSpread)
		{
			return;
		}

		for (int x = i - HorizontalSpread; x <= i + HorizontalSpread; x++)
		{
			for (int y = j - VerticalSpread; y <= j + VerticalSpread; y++)
			{
				Tile target = Main.tile[x, y];
				Tile ground = Main.tile[x, y + 1];
				if (target.HasTile || target.LiquidAmount > 1 || ground.TileType != TileID.Grass || ground.Slope != SlopeType.Solid)
				{
					continue;
				}

				int style = flower.Styles[Main.rand.Next(flower.Styles.Length)];
				target.TileType = TileID.Plants;
				target.TileFrameX = (short)(style * PlantFrameWidth);
				target.HasTile = true;
			}
		}
	}

	public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (type != TileID.Plants || fail || effectOnly || noItem || Main.netMode == NetmodeID.Server)
		{
			return;
		}

		if (flowersByStyle.TryGetValue(Main.tile[i, j].TileFrameX / PlantFrameWidth, out var flower)
			&& WorldGen.GetPlayerForTile(i, j).GetModPlayer<PAFPlayer>().FlowerBrochure)
		{
			EntitySource_TileBreak source = new(i, j);
			Rectangle bounds = new(i * 16, j * 16, 16, 16);
			Item.NewItem(source, bounds, flower.ItemType);
		}
	}

	private void AddFlower<T>(int[] styles, bool canSpread = false)
		where T : ModItem
	{
		var flower = new Flower(ModContent.ItemType<T>(), styles, canSpread);
		foreach (int style in styles)
		{
			flowersByStyle.Add(style, flower);
		}
	}

	private readonly record struct Flower(int ItemType, int[] Styles, bool CanSpread);
}
