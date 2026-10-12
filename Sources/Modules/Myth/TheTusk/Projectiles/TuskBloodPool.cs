using Everglow.Commons.DataStructures;
using Everglow.Myth.TheTusk.Buffs;
using Everglow.Myth.TheTusk.NPCs.BloodTusk;
using Everglow.Myth.TheTusk.Tiles;
using Terraria.DataStructures;

namespace Everglow.Myth.TheTusk.Projectiles;

public class TuskBloodPool : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override string Texture => ModAsset.Empty_Mod;

	public List<Point> DissolvingTile;

	public override void SetDefaults()
	{
		Projectile.tileCollide = false;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.timeLeft = 480;
		Projectile.width = 40;
		Projectile.height = 40;
		DissolvingTile = new List<Point>();
	}

	public override void OnSpawn(IEntitySource source)
	{
		Point point = Projectile.Center.ToTileCoordinates();
		if (point.X < 20 || point.X > Main.maxTilesX - 20)
		{
			Projectile.active = false;
			return;
		}
		if (point.Y < 20 || point.Y > Main.maxTilesY - 20)
		{
			Projectile.active = false;
			return;
		}

		for (int i = -8; i < 9; i++)
		{
			for (int j = -8; j < 9; j++)
			{
				Point checkPoint = new Point(i, j) + point;
				Tile tile = Main.tile[checkPoint];
				if (tile.HasTile && tile.TileType == ModContent.TileType<TuskFlesh>())
				{
					if (new Vector2(i, j).Length() < Main.rand.NextFloat(5.5f, 8f))
					{
						tile.ClearTile();
						DissolvingTile.Add(checkPoint);
					}
				}
			}
		}
		Projectile projectile = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<TuskBloodPool_open>(), 0, 0, Projectile.owner, Main.rand.NextFloat(1), Main.rand.NextFloat(1));
		TuskBloodPool_open tbpo = projectile.ModProjectile as TuskBloodPool_open;
		tbpo.DissolvingTile = DissolvingTile;
		base.OnSpawn(source);
	}

	public override void AI()
	{
		Projectile.ai[0] = 36;
		if (Projectile.timeLeft < 120)
		{
			Projectile.ai[0] = 36 + MathF.Pow((120 - Projectile.timeLeft) / 120f, 3) * 100;
		}
		else if (Projectile.timeLeft < 400)
		{
			if (Projectile.timeLeft % 60 == 0)
			{
				NPC jaw = NPC.NewNPCDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(Main.rand.NextFloat(-90, 90f), 50), ModContent.NPCType<Living_Jawbone>());
				jaw.scale = Main.rand.NextFloat(0.5f, 1f);
				jaw.velocity = new Vector2(0, -Main.rand.NextFloat(7f, 12f)).RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f));
				jaw.damage = 33;
				if (Main.expertMode)
				{
					jaw.damage = 44;
				}
				if (Main.masterMode)
				{
					jaw.damage = 57;
				}
			}
		}
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		Projectile.hide = true;
		behindNPCsAndTiles.Add(index);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, null);
		Effect dissolve = ModAsset.TuskBloodPool.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		float dissolveDuration = 1.2f;
		if (Projectile.timeLeft < 120)
		{
			dissolveDuration = MathF.Pow(Projectile.timeLeft / 120f, 0.5f) * 1.2f;
		}
		dissolve.Parameters["uTransform"].SetValue(model * projection);
		dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_melting.Value);
		dissolve.Parameters["duration"].SetValue(dissolveDuration);
		dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(0.1f, 0.0f, 0.2f, 0.7f));
		dissolve.Parameters["uNoiseSize"].SetValue(2.6f);
		dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
		dissolve.CurrentTechnique.Passes[0].Apply();

		List<Vertex2D> bars = new List<Vertex2D>();
		for (int x = -60; x < 61; x++)
		{
			AddBars(bars, x);
		}
		Main.graphics.graphicsDevice.Textures[0] = ModAsset.TuskBloodPool_Liquid.Value;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		bars = new List<Vertex2D>();
		for (int x = -60; x < 61; x++)
		{
			AddBars(bars, x, 1);
		}
		Main.graphics.graphicsDevice.Textures[0] = ModAsset.TuskBloodPool_Liquid.Value;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (DissolvingTile != null)
		{
			foreach (var tilePos in DissolvingTile)
			{
				Rectangle check = new Rectangle(tilePos.X * 16, tilePos.Y * 16, 16, 16);
				if (Rectangle.Intersect(check, targetHitbox) != Rectangle.emptyRectangle)
				{
					return true;
				}
			}
		}
		return false;
	}

	public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
	{
		target.AddBuff(ModContent.BuffType<TuskCursed>(), 450);
	}

	private void AddBars(List<Vertex2D> bars, float x, int style = 0)
	{
		float timeValue = (float)(Main.time * 0.03f) + Projectile.whoAmI * 2.74f;
		float mulTimeValue = 1f;
		if (Projectile.timeLeft < 990)
		{
			mulTimeValue = 3f;
		}
		float upY = MathF.Sin(timeValue * mulTimeValue) * MathF.Sin(x / 10f * mulTimeValue) * 2f;
		upY += MathF.Sin(timeValue * 1.4f * mulTimeValue) * MathF.Sin(x / 4f * mulTimeValue + 2 - timeValue * 1.3f) * 2f;
		upY += MathF.Sin(timeValue * 0.6f * mulTimeValue) * MathF.Sin(x / 13f * mulTimeValue - 1) * 2f;
		Vector2 drawPosUp = new Vector2(x * 4, upY + Projectile.ai[0]);

		float upDistance = (drawPosUp.Length() - 120) / 60f;
		upDistance = Math.Clamp(upDistance, 0, 1);
		drawPosUp += Projectile.Center;
		Vector2 drawPosDown = new Vector2(x * 4, MathF.Sqrt(Math.Abs(x * x * 16 - 240 * 240)) + Projectile.ai[0]);
		if (style == 1)
		{
			drawPosDown = new Vector2(x * 4, upY + Projectile.ai[0] + 8);
		}
		float downDistance = drawPosDown.Length() / 190f;
		float coordY = drawPosDown.Y / 30f;
		if (style == 1)
		{
			coordY = 0.5f;
		}
		drawPosDown += Projectile.Center;
		if (drawPosDown.Y < drawPosUp.Y)
		{
			drawPosDown.Y = drawPosUp.Y;
		}

		float coordX = (x + 60) / 90f + timeValue * 0.05f + 50;
		if (style == 1)
		{
			coordX = (x + 60) / 90f - timeValue * 0.03f + 50000;
		}
		Color colorUp = Lighting.GetColor(drawPosUp.ToTileCoordinates());
		Color colorDown = Lighting.GetColor(drawPosDown.ToTileCoordinates());
		if (style == 1)
		{
			colorUp.A = 120;
			colorDown.A = 120;
		}
		bars.Add(drawPosUp, colorUp, new Vector3(coordX, 0, upDistance));
		bars.Add(drawPosDown, colorDown, new Vector3(coordX, coordY, downDistance));
	}

	public override void OnKill(int timeLeft)
	{
		Projectile projectile = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<TuskBloodPool_close>(), 0, 0, Projectile.owner, Main.rand.NextFloat(1), Main.rand.NextFloat(1));
		TuskBloodPool_close tbpc = projectile.ModProjectile as TuskBloodPool_close;
		tbpc.DissolvingTile = DissolvingTile;
	}
}
