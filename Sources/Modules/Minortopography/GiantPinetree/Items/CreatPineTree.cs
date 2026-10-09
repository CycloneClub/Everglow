using Everglow.Commons.TileHelper;
using Everglow.Minortopography.GiantPinetree.TilesAndWalls;
using Terraria;

namespace Everglow.Minortopography.GiantPinetree.Items;

public class CreatPineTree : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.width = 10;
		Item.height = 10;
		Item.useTime = 7;
		Item.useAnimation = 7;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.createTile = ModContent.TileType<GiantPineCone_1>();
	}
	public override bool? UseItem(Player player)
	{
		return true;
	}
	public override void HoldItem(Player player)
	{
		if (Main.mouseRight && Main.mouseRightRelease)
		{
			int x = (int)(Main.MouseWorld.X / 16);
			int y = (int)(Main.MouseWorld.Y / 16);
			List<Item> chestContents = new List<Item>();

			int maxCount = Main.rand.Next(8, 14);
			switch (Main.rand.Next(5))
			{
				case 0:
					chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<FrostBomb>(), 1));
					break;
				case 1:
					chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<BurningFrozenHeart>(), 1));
					break;
				case 2:
					chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<PearlOfCyan>(), 1));
					break;
				case 3:
					chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<SnowPineLockBox>(), 1));
					break;
				case 4:
					chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<HarvestingClaw>(), 1));
					break;
			}
			if (Main.rand.NextBool(3))
			{
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<SnowPineLeaveStaff>(), 1));
				chestContents.Add(new Item(setDefaultsToType: ModContent.ItemType<SnowPineWoodStaff>(), 1));
			}
			if (Main.rand.NextBool(7))
			{
				chestContents.Add(new Item(setDefaultsToType: ItemID.HerbBag, Main.rand.Next(1, 4)));
			}
			if (Main.rand.NextBool(7))
			{
				chestContents.Add(new Item(setDefaultsToType: ItemID.CanOfWorms, Main.rand.Next(2, 5)));
			}
			if (Main.rand.NextBool(2))
			{
				chestContents.Add(new Item(setDefaultsToType: ItemID.BorealWood, Main.rand.Next(50, 151)));
			}
			if (Main.rand.NextBool(7))
			{
				chestContents.Add(new Item(setDefaultsToType: ItemID.FrostDaggerfish, Main.rand.Next(80, 201)));
			}
			if (Main.rand.NextBool(7))
			{
				chestContents.Add(new Item(setDefaultsToType: ItemID.FrostburnArrow, Main.rand.Next(80, 201)));
			}
			if (Main.rand.NextBool(5))
			{
				chestContents.Add(new Item(setDefaultsToType: ItemID.GoldCoin, Main.rand.Next(1, 3)));
			}
			WorldGenMisc.PlaceChest(x, y, (ushort)ModContent.TileType<TilesAndWalls.SnowPineChest>(), chestContents);
		}
		base.HoldItem(player);
	}
}
