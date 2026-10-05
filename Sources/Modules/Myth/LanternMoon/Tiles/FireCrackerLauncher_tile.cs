using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Myth.LanternMoon.Projectiles.Item_Shoot;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Everglow.Myth.LanternMoon.Tiles;

public class FireCrackerLauncher_tile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 5;
		TileObjectData.newTile.Width = 3;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			16,
			16,
			18
		};
		TileObjectData.newTile.Origin = new Point16(0, 3);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(200, 10, 10));
	}
	public override void HitWire(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		int frameX = tile.TileFrameX;
		int frameY = tile.TileFrameY;
		if (frameX == 18 && frameY == 54)
		{
			Activate(i, j);
		}
	}
	private void Activate(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		int frameX = tile.TileFrameX;
		int frameY = tile.TileFrameY;
		Vector2 aimPos = new Vector2(i, j) * 16 + new Vector2(18 - frameX, 0 - frameY) / 18f * 16f + new Vector2(8);
		GenerateFire(60, aimPos);
		GenerateSmog(30, aimPos);
		GenerateSpark(60, aimPos);
		Projectile p = Projectile.NewProjectileDirect(null, aimPos, new Vector2(Main.rand.NextFloat(-1f, 1f), -45) * Main.rand.NextFloat(0.95f, 1.05f), ModContent.ProjectileType<RisingFirework>(), 50, 0f, Main.LocalPlayer.whoAmI, 0.2f, 0);
		if (p != null)
		{
			RisingFirework risingFirework = p.ModProjectile as RisingFirework;
			if (risingFirework != null)
			{
				risingFirework.MoveSight = false;
			}
		}
	}
	public void GenerateSmog(int Frequency, Vector2 pos)
	{
		float mulVelocity = 1f;
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(2f, 4f)).RotatedByRandom(MathHelper.TwoPi) + new Vector2(0, Main.rand.NextFloat(-40f, -5f));
			var somg = new FireSmogDust
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = pos + newVelocity * 0.3f,
				maxTime = Main.rand.Next(37, 45),
				scale = Main.rand.NextFloat(20f, 35f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 }
			};
			Ins.VFXManager.Add(somg);
		}
	}
	public void GenerateFire(int Frequency, Vector2 pos)
	{
		float mulVelocity = 1f;

		for (int g = 0; g < Frequency; g++)
		{
			float scale = Main.rand.NextFloat(4f, 15f);
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(0f, 1f)).RotatedByRandom(MathHelper.TwoPi) + new Vector2(0, Main.rand.NextFloat(-40f, -5f));
			var fire = new FireDust
			{
				Velocity = newVelocity / scale * 4f,
				Active = true,
				Visible = true,
				Position = pos,
				MaxTime = Main.rand.Next(9, 75),
				Scale = Main.rand.NextFloat(7f, 15f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 }
			};
			Ins.VFXManager.Add(fire);
		}
	}
	public void GenerateSpark(int Frequency, Vector2 pos)
	{
		float mulVelocity = 1f;
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(0f, 1f)).RotatedByRandom(MathHelper.TwoPi) + new Vector2(0, Main.rand.NextFloat(-60f, -5f));
			var spark = new FireSparkDust
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = pos,
				maxTime = Main.rand.Next(17, 125),
				scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(0.1f, 17.0f)),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.02f, 0.02f) }
			};
			Ins.VFXManager.Add(spark);
		}
	}
	public override bool RightClick(int i, int j)
	{
		Activate(i, j);
		return base.RightClick(i, j);
	}
	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public override bool CanKillTile(int i, int j, ref bool blockDamaged)
	{
		return false;
	}
}
