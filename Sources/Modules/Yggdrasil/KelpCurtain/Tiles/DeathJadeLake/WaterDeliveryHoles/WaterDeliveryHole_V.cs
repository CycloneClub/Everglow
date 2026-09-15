using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.WaterDeliveryHoles;

public class WaterDeliveryHole_V : ModTile, ISceneTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileWaterDeath[Type] = false;
		Main.tileBlendAll[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 5;
		TileObjectData.newTile.Width = 2;
		var SolidOrSolidSideAnchor1TilesLong = new AnchorData(AnchorType.SolidTile, 5, 0);
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			16,
			16,
			16,
		};
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = true;

		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newTile.Origin = new Point16(0, 2);
		TileObjectData.newAlternate.AnchorLeft = SolidOrSolidSideAnchor1TilesLong;
		TileObjectData.newAlternate.Style = 1;
		TileObjectData.addAlternate(1);

		TileObjectData.newTile.Origin = new Point16(1, 2);
		TileObjectData.newTile.AnchorRight = SolidOrSolidSideAnchor1TilesLong;
		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<WaterDeliveryHoleDust>();
		AddMapEntry(new Color(78, 162, 255));
	}

	public void AddScene(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameY == 36 && (tile.TileFrameX == 18 || tile.TileFrameX == 36))
		{
			int dir = -1;
			if (tile.TileFrameX == 36)
			{
				dir = 1;
			}
			var vfx = new WaterDeliveryHole_VFX
			{
				Active = true,
				Visible = true,
				Position = new Vector2(i, j).ToWorldCoordinates(8, 8),
				OriginTilePos = new Point(i, j),
				OriginTileType = Type,
				Direction = 1,
				Rotation = (dir - 1) * MathHelper.PiOver2,
			};
			Ins.VFXManager.Add(vfx);
			var warp = new WaterDeliveryHole_VFX_warp
			{
				Active = true,
				Visible = true,
				Position = new Vector2(i, j).ToWorldCoordinates(8, 8),
				OriginTilePos = new Point(i, j),
				OriginTileType = Type,
				Direction = 1,
				Rotation = (dir - 1) * MathHelper.PiOver2,
			};
			Ins.VFXManager.Add(warp);
			var foreground = new WaterDeliveryHole_foreground
			{
				Active = true,
				Visible = true,
				Position = new Vector2(i, j).ToWorldCoordinates(8, 8),
				OriginTilePos = new Point(i, j),
				OriginTileType = Type,
				Direction = 1,
				Rotation = (dir - 1) * MathHelper.PiOver2,
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
		spriteBatch.Draw(tex, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY + 90, 16, 16), new Color(0f, 0f, 0.7f, 0));
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
