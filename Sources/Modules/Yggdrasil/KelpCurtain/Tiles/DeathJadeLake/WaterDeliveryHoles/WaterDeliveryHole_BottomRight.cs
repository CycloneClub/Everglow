using Everglow.Commons.TileHelper;
using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.WaterDeliveryHoles;

public class WaterDeliveryHole_BottomRight : ShapeDataTile, ISceneTile
{
	public override void SetStaticDefaults()
	{
		TotalHeight = 4;
		TotalWidth = 4;
		CustomItemType = ModContent.ItemType<WaterDeliveryHole_Item>();

		Main.tileSolid[Type] = false;
		Main.tileBlendAll[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileWaterDeath[Type] = false;
		Main.tileCut[Type] = false;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Origin = new Point16(2, 2);
		TileObjectData.newTile.Height = TotalHeight;
		TileObjectData.newTile.Width = TotalWidth;
		TileObjectData.newTile.CoordinateHeights = new int[TotalHeight];
		for (int i = 0; i < TotalHeight; i++)
		{
			TileObjectData.newTile.CoordinateHeights[i] = 16;
		}
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.addTile(Type);

		DustType = ModContent.DustType<WaterDeliveryHoleDust>();
		AddMapEntry(new Color(78, 162, 255));
	}

	public void AddScene(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX == 36 && tile.TileFrameY == 36)
		{
			var vfx = new WaterDeliveryHole_VFX
			{
				Active = true,
				Visible = true,
				Position = new Point(i, j).ToWorldCoordinates(),
				OriginTilePos = new Point(i, j),
				OriginTileType = Type,
				Direction = 1,
				Rotation = MathHelper.PiOver4,
			};
			Ins.VFXManager.Add(vfx);
			var warp = new WaterDeliveryHole_VFX_warp
			{
				Active = true,
				Visible = true,
				Position = new Point(i, j).ToWorldCoordinates(),
				OriginTilePos = new Point(i, j),
				OriginTileType = Type,
				Direction = 1,
				Rotation = MathHelper.PiOver4,
			};
			Ins.VFXManager.Add(warp);
			var foreground = new WaterDeliveryHole_Slope_foreground
			{
				Active = true,
				Visible = true,
				Position = new Point(i, j).ToWorldCoordinates(),
				OriginTilePos = new Point(i, j),
				OriginTileType = Type,
				Direction = 1,
				Rotation = MathHelper.PiOver4,
			};
			Ins.VFXManager.Add(foreground);
		}
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}
		Tile tile = Main.tile[i, j];
		Texture2D tex = ModAsset.WaterDeliveryHole.Value;
		Vector2 pos = new Vector2(i * 16, j * 16) - Main.screenPosition + zero;
		spriteBatch.Draw(tex, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY + 36, 16, 16), new Color(0f, 0f, 0.7f, 0));
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		if (Main.dedServ)
		{
			base.NearbyEffects(i, j, closer);
			return;
		}
		Lighting.AddLight(new Point(i, j).ToWorldCoordinates(), new Vector3(0.4f, 0.8f, 2f));
		WaterDeliveryHole_TeleportPlayer.GetDestinationAndTeleport(i, j);
		base.NearbyEffects(i, j, closer);
	}
}
