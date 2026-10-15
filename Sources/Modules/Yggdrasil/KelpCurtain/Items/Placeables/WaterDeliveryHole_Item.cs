using Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.WaterDeliveryHoles;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Placeables;

public class WaterDeliveryHole_Item : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public int State = 0;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<WaterDeliveryHole>());
		Item.width = 38;
		Item.height = 14;
		Item.value = 5000;
	}

	public override void HoldItem(Player player)
	{
		if (Main.mouseRight && Main.mouseRightRelease)
		{
			State++;
			if (State >= 6)
			{
				State = 0;
			}
			if (State == 0)
			{
				Item.DefaultToPlaceableTile(ModContent.TileType<WaterDeliveryHole>());
			}
			else if (State == 1)
			{
				Item.DefaultToPlaceableTile(ModContent.TileType<WaterDeliveryHole_V>());
			}
			else if (State == 2)
			{
				Item.DefaultToPlaceableTile(ModContent.TileType<WaterDeliveryHole_TopRight>());
				Item.placeStyle = 0;
			}
			else if (State == 3)
			{
				Item.DefaultToPlaceableTile(ModContent.TileType<WaterDeliveryHole_BottomRight>());
				Item.placeStyle = 0;
			}
			else if (State == 4)
			{
				Item.DefaultToPlaceableTile(ModContent.TileType<WaterDeliveryHole_BottomLeft>());
				Item.placeStyle = 0;
			}
			else if (State == 5)
			{
				Item.DefaultToPlaceableTile(ModContent.TileType<WaterDeliveryHole_TopLeft>());
				Item.placeStyle = 0;
			}
		}
		if (State == 2)
		{
			Main.placementPreview = true;
		}
		base.HoldItem(player);
	}

	public override bool CanUseItem(Player player)
	{
		if (State == 2)
		{
			var waterHole_TopLeft = TileLoader.GetTile(ModContent.TileType<WaterDeliveryHole_TopRight>()) as WaterDeliveryHole_TopRight;
			if (waterHole_TopLeft != null)
			{
				int x = (int)(Main.MouseWorld.X / 16);
				int y = (int)(Main.MouseWorld.Y / 16);
				if (waterHole_TopLeft.NoCollideInSolidGrid(x, y))
				{
					waterHole_TopLeft.PlaceAtTileObjectDataOrigin(x, y);
					Item.stack--;
					return false;
				}
			}
		}
		if (State == 3)
		{
			var waterHole_TopLeft = TileLoader.GetTile(ModContent.TileType<WaterDeliveryHole_BottomRight>()) as WaterDeliveryHole_BottomRight;
			if (waterHole_TopLeft != null)
			{
				int x = (int)(Main.MouseWorld.X / 16);
				int y = (int)(Main.MouseWorld.Y / 16);
				if (waterHole_TopLeft.NoCollideInSolidGrid(x, y))
				{
					waterHole_TopLeft.PlaceAtTileObjectDataOrigin(x, y);
					Item.stack--;
					return false;
				}
			}
		}
		if (State == 4)
		{
			var waterHole_TopLeft = TileLoader.GetTile(ModContent.TileType<WaterDeliveryHole_BottomLeft>()) as WaterDeliveryHole_BottomLeft;
			if (waterHole_TopLeft != null)
			{
				int x = (int)(Main.MouseWorld.X / 16);
				int y = (int)(Main.MouseWorld.Y / 16);
				if (waterHole_TopLeft.NoCollideInSolidGrid(x, y))
				{
					waterHole_TopLeft.PlaceAtTileObjectDataOrigin(x, y);
					Item.stack--;
					return false;
				}
			}
		}
		if (State == 5)
		{
			var waterHole_TopLeft = TileLoader.GetTile(ModContent.TileType<WaterDeliveryHole_TopLeft>()) as WaterDeliveryHole_TopLeft;
			if (waterHole_TopLeft != null)
			{
				int x = (int)(Main.MouseWorld.X / 16);
				int y = (int)(Main.MouseWorld.Y / 16);
				if (waterHole_TopLeft.NoCollideInSolidGrid(x, y))
				{
					waterHole_TopLeft.PlaceAtTileObjectDataOrigin(x, y);
					Item.stack--;
					return false;
				}
			}
		}
		return base.CanUseItem(player);
	}
}
