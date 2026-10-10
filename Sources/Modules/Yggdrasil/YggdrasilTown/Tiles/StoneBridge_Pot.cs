using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Microsoft.Xna.Framework;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class StoneBridge_Pot : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileCut[Type] = true;
		Main.tileOreFinderPriority[Type] = 100;
		Main.tileSpelunker[Type] = true;
		DustType = ModContent.DustType<StoneBridgeDust>();

		// Placement
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2); // this style already takes care of direction for us
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
		TileObjectData.addTile(Type);

		HitSound = SoundID.Shatter;

		AddMapEntry(new Color(82, 76, 118));
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 6;
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield break;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameY == 0 && tile.TileFrameX % 36 == 0 && tile.TileFrameX < 108)
		{
			TileFluentDrawManager.AddFluentPoint(this, i, j);
		}
		return true;
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		var tile = TileUtils.SafeGetTile(i, j);
		switch (Main.rand.Next(7))
		{
			case 0:
				Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.Rope, Main.rand.Next(9, 85)));
				break;
			case 1:
				if (Main.rand.NextBool(3))
				{
					Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.GoldCoin, Main.rand.Next(1, 3)));
				}
				else
				{
					Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.SilverCoin, Main.rand.Next(1, 80)));
					Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.CopperCoin, Main.rand.Next(0, 99)));
				}
				break;
			case 2:
				if (Main.rand.NextBool(3))
				{
					Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.HealingPotion));
				}
				else
				{
					Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.LesserHealingPotion, Main.rand.Next(1, 5)));
				}
				break;
			case 3:
				switch (Main.rand.Next(46))
				{
					case 0:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.RestorationPotion));
						break;
					case 1:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.ObsidianSkinPotion));
						break;
					case 2:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.AmmoReservationPotion));
						break;
					case 3:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.ArcheryPotion));
						break;
					case 4:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.BattlePotion));
						break;
					case 5:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.BiomeSightPotion));
						break;
					case 6:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.BuilderPotion));
						break;
					case 7:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.CalmingPotion));
						break;
					case 8:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.CratePotion));
						break;
					case 9:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.EndurancePotion));
						break;
					case 10:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.FeatherfallPotion));
						break;
					case 11:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.FishingPotion));
						break;
					case 12:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.FlipperPotion));
						break;
					case 13:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.GenderChangePotion));
						break;
					case 14:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.GillsPotion));
						break;
					case 15:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.GravitationPotion));
						break;
					case 16:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.GreaterHealingPotion));
						break;
					case 17:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.GreaterManaPotion));
						break;
					case 18:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.HeartreachPotion));
						break;
					case 19:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.HunterPotion));
						break;
					case 20:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.InfernoPotion));
						break;
					case 21:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.InvisibilityPotion));
						break;
					case 22:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.IronskinPotion));
						break;
					case 23:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.LifeforcePotion));
						break;
					case 24:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.LovePotion));
						break;
					case 25:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.LuckPotion));
						break;
					case 26:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.MagicPowerPotion));
						break;
					case 27:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.ManaRegenerationPotion));
						break;
					case 28:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.MiningPotion));
						break;
					case 29:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.NightOwlPotion));
						break;
					case 30:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.RagePotion));
						break;
					case 31:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.RecallPotion));
						break;
					case 32:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.RegenerationPotion));
						break;
					case 33:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.ShinePotion));
						break;
					case 34:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.SonarPotion));
						break;
					case 35:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.SpelunkerPotion));
						break;
					case 36:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.StinkPotion));
						break;
					case 37:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.SummoningPotion));
						break;
					case 38:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.SwiftnessPotion));
						break;
					case 39:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.TeleportationPotion));
						break;
					case 40:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.ThornsPotion));
						break;
					case 41:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.TitanPotion));
						break;
					case 42:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.TrapsightPotion));
						break;
					case 43:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.WaterWalkingPotion));
						break;
					case 44:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.WormholePotion));
						break;
					case 45:
						Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.WrathPotion));
						break;
				}
				break;
			case 4:
				Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.Bomb, Main.rand.Next(2, 20)));
				break;
			case 5:
				Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.Torch, Main.rand.Next(2, 35)));
				break;
			case 6:
				Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), new Point(i, j).ToWorldCoordinates(24, 24), new Item(ItemID.Glowstick, Main.rand.Next(2, 25)));
				break;
		}
		for (int h = 0; h < 4; h++)
		{
			Gore.NewGore(WorldGen.GetItemSource_FromTileBreak(i, j), new Vector2(i * 16, j * 16), new Vector2(0, Main.rand.NextFloat(0f, 3f)).RotatedByRandom(MathHelper.TwoPi), ModContent.Find<ModGore>("Everglow/StoneBridge_Pot_Gore_" + Main.rand.Next(13)).Type, 1);
		}
		base.KillMultiTile(i, j, frameX, frameY);
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		noItem = true;
		base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
	}
}
