using Everglow.Commons.Utilities;
using ModLiquidLib.ID;
using ModLiquidLib.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Liquids;

public class JellyBallNutritionBucket : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Tools;

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 5;
		ItemID.Sets.IsLavaImmuneRegardlessOfRarity[Type] = true;
		ItemID.Sets.AlsoABuildingItem[Type] = true;
		ItemID.Sets.DuplicationMenuToolsFilter[Type] = true;
		LiquidID_TLmod.Sets.CreateLiquidBucketItem[LiquidLoader.LiquidType<JellyBallNutritionLiquid>()] = Type;
	}

	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.WaterBucket);
	}

	public override void HoldItem(Player player)
	{
		if (player.whoAmI != Main.myPlayer || player.JustDroppedAnItem || player.noBuilding)
		{
			return;
		}

		int x = Player.tileTargetX;
		int y = Player.tileTargetY;
		if (!WorldGen.InWorld(x, y, 1) || !player.IsInTileInteractionRange(x, y, TileReachCheckSettings.Simple))
		{
			return;
		}

		if (!Main.GamepadDisableCursorItemIcon)
		{
			player.cursorItemIconEnabled = true;
			Main.ItemIconCacheUpdate(Type);
		}
		if (!player.controlUseItem || player.itemAnimation <= 0 || !player.ItemTimeIsZero)
		{
			return;
		}

		Tile tile = Main.tile[x, y];
		int liquidType = LiquidLoader.LiquidType<JellyBallNutritionLiquid>();
		if (tile.LiquidAmount >= 200 || (tile.LiquidAmount > 0 && tile.LiquidType != liquidType))
		{
			return;
		}
		if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType] && tile.TileType != TileID.Grate)
		{
			return;
		}

		tile.LiquidType = liquidType;
		tile.LiquidAmount = byte.MaxValue;
		WorldGen.SquareTileFrame(x, y);
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			NetMessage.sendWater(x, y);
		}

		SoundEngine.PlaySound(SoundID.SplashWeak, player.position);
		Item.stack--;
		player.PutItemInInventoryFromItemUsage(ItemID.EmptyBucket, player.selectedItem);
		player.ApplyItemTime(Item);
	}
}
