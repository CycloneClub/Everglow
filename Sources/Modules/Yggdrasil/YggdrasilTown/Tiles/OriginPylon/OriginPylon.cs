using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.Common.Projectiles;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using SubworldLibrary;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.OriginPylon;

public class OriginPylon : ModTile, ISceneTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;

		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 12;
		TileObjectData.newTile.Width = 8;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			16,
			20,
		};
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.addTile(Type);
		AddMapEntry(new Color(188, 189, 185));
		MinPick = int.MaxValue;
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		noBreak = true;
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX < 108)
		{
			r = 1f;
			g = 1f;
			b = 1f;
		}
		else
		{
			r = 1f;
			g = 1f;
			b = 1f;
		}
		base.ModifyLight(i, j, ref r, ref g, ref b);
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
		base.NumDust(i, j, fail, ref num);
	}

	public override bool CanKillTile(int i, int j, ref bool blockDamaged)
	{
		return false;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Main.tile[i, j];
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

		if (Ins.VisualQuality.Low)
		{
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			if (tile.TileFrameX == 4 * 18 && tile.TileFrameY == 6 * 18)
			{
				spriteBatch.Draw(ModAsset.OriginPylon.Value, new Vector2(i, j) * 16 - Main.screenPosition + zero + new Vector2(16, -36), null, Color.White, 0, ModAsset.OriginPylon.Value.Size() * 0.5f, 1, SpriteEffects.None, 0);
			}
		}
		return false;
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		if (!Main.gamePaused && YggdrasilWorld.YggdrasilTimer > 5 && !Ins.VisualQuality.Low)
		{
			if (Main.rand.NextBool(600))
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 1f)).RotatedByRandom(MathHelper.TwoPi);
				Vector2 pos = new Vector2(i, j) * 16;
				Vector2 addPos = new Vector2(Main.rand.NextFloat(1f) * 120f, 0).RotatedByRandom(6.283);
				addPos.Y = -Math.Abs(addPos.Y) * 4 + 100;
				float size = Math.Max(addPos.Y + 500, 0) / 500f;
				size *= (130 - Math.Abs(addPos.X)) / 120f;
				addPos.X += 16;
				pos += addPos;
				if (Collision.SolidCollision(pos, 0, 0))
				{
					return;
				}
				var dust = new WhiteTriangle
				{
					velocity = newVelocity + new Vector2(0, addPos.Y * 0.01f),
					Active = true,
					Visible = true,
					position = pos,
					maxTime = Main.rand.Next(50, 192),
					scale = 0,
					rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(4.0f, 14.5f) * size, Main.rand.NextFloat(-0.03f, 0.03f) },
				};
				Ins.VFXManager.Add(dust);
			}
		}
	}

	public void AddScene(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
		{
			var oPVFX = new OriginalPylon_VFX { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = ModContent.TileType<OriginPylon>() };
			Ins.VFXManager.Add(oPVFX);
		}
	}

	public override bool RightClick(int i, int j)
	{
		Player player = Main.LocalPlayer;
		int projectileType = ModContent.ProjectileType<TeleportToYggdrasil>();
		if (player.ownedProjectileCounts[projectileType] <= 0)
		{
			player.AddBuff(BuffID.Shimmer, 30);
			Projectile.NewProjectileDirect(WorldGen.GetProjectileSource_PlayerOrWires(i, j, false, player), Main.MouseWorld, Vector2.zeroVector, projectileType, 0, 0, player.whoAmI);
		}
		return base.RightClick(i, j);
	}
}
