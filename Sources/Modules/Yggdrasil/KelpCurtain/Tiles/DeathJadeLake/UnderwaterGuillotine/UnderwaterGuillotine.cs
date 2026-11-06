using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.TileEffect;
using Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.LightningMechanism;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.UnderwaterGuillotine;

public class UnderwaterGuillotine : ShapeDataTile
{
	public override void SetStaticDefaults()
	{
		CustomItemType = ModContent.ItemType<UnderwaterGuillotine_Item>();
		DustType = ModContent.DustType<UnderwaterGuillotineDust>();
		TotalWidth = 8;
		TotalHeight = 3;
		Main.tileSolid[Type] = false;
		Main.tileBlendAll[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.Width = 8;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			16,
		};
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.Origin = new Point16(4, 0);
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 8, 0);
		TileObjectData.addTile(Type);
		AddMapEntry(new Color(127, 97, 36));
	}

	public override void HitWire(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		Point topLeft = new Point(i - tile.TileFrameX / 18, j - tile.TileFrameY / 18);
		foreach (var proj in Main.projectile)
		{
			if (proj != null && proj.active && proj.type == ModContent.ProjectileType<UnderwaterGuillotine_Projectile>())
			{
				UnderwaterGuillotine_Projectile uGP = proj.ModProjectile as UnderwaterGuillotine_Projectile;
				if (uGP != null && uGP.TileTopLeft == topLeft)
				{
					return;
				}
			}
		}
		var projectile = Projectile.NewProjectileDirect(WorldGen.GetProjectileSource_PlayerOrWires(i, j, true, Main.LocalPlayer), (topLeft + new Point(4, 0)).ToWorldCoordinates(), Vector2.zeroVector, ModContent.ProjectileType<UnderwaterGuillotine_Projectile>(), 1000, 5, Main.maxPlayers);
		UnderwaterGuillotine_Projectile uGP0 = projectile.ModProjectile as UnderwaterGuillotine_Projectile;
		if (uGP0 != null)
		{
			uGP0.TileTopLeft = topLeft;
		}
		base.HitWire(i, j);
	}

	public override void PlaceOriginAtTopLeft(int x, int y)
	{
		if (x > Main.maxTilesX - TotalWidth || x < 0 || y > Main.maxTilesY - TotalHeight || y < 0)
		{
			return;
		}

		for (int i = 0; i < TotalWidth; i++)
		{
			for (int j = 0; j < TotalHeight; j++)
			{
				if (PixelHasTile[i, j] >= 200)
				{
					ushort type = Type;
					Tile tile = Main.tile[x + i, y + j];
					tile.TileType = type;
					tile.TileFrameX = (short)(i * 18);
					tile.TileFrameY = (short)(j * 18);
					tile.HasTile = true;
				}
			}
		}
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Main.tile[i, j];
		Vector2 zero = new Vector2(Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.zeroVector;
		}
		Texture2D tex = ModAsset.UnderwaterGuillotine.Value;
		spriteBatch.Draw(tex, new Point(i, j).ToWorldCoordinates() - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Lighting.GetColor(i, j), 0, new Vector2(8), 1f, SpriteEffects.None, 0);
		base.PostDraw(i, j, spriteBatch);
	}
}
