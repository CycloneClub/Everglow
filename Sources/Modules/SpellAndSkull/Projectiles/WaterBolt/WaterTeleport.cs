using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.SpellAndSkull.Buffs;
using Everglow.SpellAndSkull.Dusts;
using static Everglow.SpellAndSkull.Common.SpellAndSkullUtils;

namespace Everglow.SpellAndSkull.Projectiles.WaterBolt;

public class WaterTeleport : ModProjectile, IWarpProjectile
{
	public override void SetDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCs;
		Projectile.width = 28;
		Projectile.height = 28;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 10000;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.tileCollide = false;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Vector2 AIM0 = player.Center + new Vector2(0, 12 * player.gravDir * (float)(0.2 + Math.Sin(Main.timeForVisualEffects / 18d) / 2d)) + new Vector2(-60 * player.direction, 30).RotatedBy((Projectile.ai[0] - 1) / 4.5 * Math.PI * player.direction);
		if (player.itemTime > 0 && player.active)
		{
			AIM0 = player.Center + new Vector2(player.direction * -12, -24 * player.gravDir * (float)(0.2 + Math.Sin(Main.timeForVisualEffects / 18d) / 2d)) + new Vector2(-120 * player.direction, 60).RotatedBy((Projectile.ai[0] - 1) / 4.5 * Math.PI * player.direction);
		}

		Projectile.Center = Projectile.Center * (-Projectile.ai[0] / 50f + 0.97f) + AIM0 * (Projectile.ai[0] / 50f + 0.03f);
		Projectile.spriteDirection = player.direction;
		Projectile.velocity *= 0;
		if (player.HeldItem.type == ItemID.WaterBolt)
		{
			Projectile.timeLeft = player.itemTime + 60;
			if (timer < 30)
			{
				timer++;
			}
		}
		else
		{
			timer--;
			if (timer < 0)
			{
				Projectile.Kill();
			}
		}

		if (Main.mouseRight && Main.mouseRightRelease)
		{
			int AimAi0 = player.ownedProjectileCounts[Projectile.type];

			while (AimAi0 > 0)
			{
				foreach (Projectile p in Main.projectile)
				{
					if (p.owner == player.whoAmI && p.type == Projectile.type)
					{
						if (p.ai[0] == AimAi0)
						{
							player.Center = Main.MouseWorld;
							Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<RipplingWave>(), 0, 0, Projectile.owner, 1);

							float k1 = 0.3f;
							float k2 = 8;
							float k0 = 1f / (4 + 2) * 2 * k2;
							for (int j = 0; j < 80; j++)
							{
								Vector2 v0 = new Vector2(Main.rand.NextFloat(9, 11f), 0).RotatedByRandom(6.283) * k1;
								int dust0 = Dust.NewDust(player.Center - new Vector2(4), 0, 0, ModContent.DustType<ShatterDrop_1>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(0.6f, 1.8f) * 0.4f * k0);
								Main.dust[dust0].noGravity = true;
							}
							for (int j = 0; j < 160; j++)
							{
								Vector2 v0 = new Vector2(Main.rand.NextFloat(9, 11f), 0).RotatedByRandom(6.283) * k1;
								int dust1 = Dust.NewDust(player.Center - new Vector2(4), 0, 0, ModContent.DustType<ShatterDrop_0>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(3.7f, 5.1f) * k0);
								Main.dust[dust1].alpha = (int)(Main.dust[dust1].scale * 50 / k0);
								Main.dust[dust1].rotation = Main.rand.NextFloat(0, 6.283f);
							}
							AimAi0 = -1;

							p.Kill();
						}
					}
				}
				AimAi0--;
			}
		}

		if (Main.mouseMiddle && Main.mouseMiddleRelease)
		{
			if (player.ownedProjectileCounts[Projectile.type] == 6)
			{
				foreach (Projectile p in Main.projectile)
				{
					if (p.owner == player.whoAmI && p.type == Projectile.type)
					{
						p.Kill();
						player.AddBuff(ModContent.BuffType<WaterBoltII>(), 300);
						foreach (Projectile p0 in Main.projectile)
						{
							if (p0.owner == player.whoAmI && p0.type == Projectile.type && p0.active)
							{
								p0.Kill();
								Projectile.NewProjectile(p0.GetSource_FromAI(), p0.Center, Vector2.Zero, ModContent.ProjectileType<RipplingWave>(), 0, 0, 255, 1);
								p0.active = false;
							}
						}
					}
				}
			}
		}
	}


	public override bool PreDraw(Player player, ref Color lightColor)
	{
		DrawMagicArray(Commons.ModAsset.Trail_5_black.Value, new Color(1f, 1f, 1f, 1f));

		// DrawMagicArray(Commons.ModAsset.Trail_5_black.Value, new Color(1f, 1f, 1f, 1f));
		DrawMagicArray(Commons.ModAsset.Trail_5.Value, new Color(0, 0.45f, 1f, 0));
		Main.spriteBatch.Draw(ModAsset.WaterTeleport.Value, Projectile.Center - Main.screenPosition, null, new Color(0, 30, 255, 0), 0, new Vector2(34), timer / 30f, SpriteEffects.None, 0);
		if ((Main.MouseWorld - Projectile.Center).Length() < 30)
		{
			Utils.DrawBorderString(Main.spriteBatch, Projectile.ai[0].ToString(), Projectile.Center - Main.screenPosition, Color.AliceBlue);
		}

		return false;
	}

	internal int timer = 0;

	public void DrawMagicArray(Texture2D tex, Color c0)
	{
		float Size = (float)((Main.timeForVisualEffects / 2d + Projectile.ai[0] * 4) % 40d);
		float SizeII = (float)((Main.timeForVisualEffects / 2d + 20 + Projectile.ai[0] * 4) % 40d);
		DrawTexCircle(Size, (40 - Size) * timer / 30f, c0, Projectile.Center - Main.screenPosition, tex, Main.timeForVisualEffects / 17);
		DrawTexCircle(SizeII, (40 - SizeII) * timer / 30f, c0, Projectile.Center - Main.screenPosition, tex, Main.timeForVisualEffects / 17);
	}

	public static void DrawTexLine(Vector2 StartPos, Vector2 EndPos, Color color1, Color color2, Texture2D tex)
	{
		float Wid = 6f;
		Vector2 Width = Vector2.Normalize(StartPos - EndPos).RotatedBy(Math.PI / 2d) * Wid;

		var vertex2Ds = new List<Vertex2D>();

		for (int x = 0; x < 3; x++)
		{
			float Value0 = (float)(Main.timeForVisualEffects / 291d + 20) % 1f;
			float Value1 = (float)(Main.timeForVisualEffects / 291d + 20.03) % 1f;
			vertex2Ds.Add(new Vertex2D(StartPos + Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 1, 0)));

			vertex2Ds.Add(new Vertex2D(EndPos + Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 0, 0)));
			vertex2Ds.Add(new Vertex2D(EndPos - Width + new Vector2(x / 3f).RotatedBy(x), color2, new Vector3(Value1, 1, 0)));
			vertex2Ds.Add(new Vertex2D(StartPos - Width + new Vector2(x / 3f).RotatedBy(x), color1, new Vector3(Value0, 1, 0)));
		}

		Main.graphics.GraphicsDevice.Textures[0] = tex;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertex2Ds.ToArray(), 0, vertex2Ds.Count / 3);
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float Size = (float)((Main.timeForVisualEffects / 2d + Projectile.ai[0] * 4) % 40d);
		float SizeII = (float)((Main.timeForVisualEffects / 2d + 20 + Projectile.ai[0] * 4) % 40d);
		DrawTexCircle(spriteBatch, Size, (40 - Size) * timer / 30f, new Color(64, 7, 255, 0), Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_5.Value, Main.timeForVisualEffects / 17);
		DrawTexCircle(spriteBatch, SizeII, (40 - SizeII) * timer / 30f, new Color(64, 7, 255, 0), Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_5.Value, Main.timeForVisualEffects / 17);
	}

	public override void OnKill(int timeLeft)
	{
		if (timer < 1)
		{
			return;
		}

		float k1 = 0.3f;
		float k2 = 8;
		float k0 = 1f / (4 + 2) * 2 * k2;
		for (int j = 0; j < 80; j++)
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(9, 11f), 0).RotatedByRandom(6.283) * k1;
			int dust0 = Dust.NewDust(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<ShatterDrop_1>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(0.6f, 1.8f) * 0.4f * k0);
			Main.dust[dust0].noGravity = true;
		}
		for (int j = 0; j < 160; j++)
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(9, 11f), 0).RotatedByRandom(6.283) * k1;
			int dust1 = Dust.NewDust(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<ShatterDrop_0>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(3.7f, 5.1f) * k0);
			Main.dust[dust1].alpha = (int)(Main.dust[dust1].scale * 50 / k0);
			Main.dust[dust1].rotation = Main.rand.NextFloat(0, 6.283f);
		}
		base.OnKill(timeLeft);
	}
}
