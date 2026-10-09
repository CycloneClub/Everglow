using Everglow.Myth.TheFirefly.Buffs;
using Everglow.Myth.TheFirefly.Dusts;

namespace Everglow.Myth.TheFirefly.Projectiles;

internal class EvilChrysalis : ModProjectile
{
	public override string Texture => "Everglow/Myth/TheFirefly/Projectiles/EvilChrysalisTex/EvilChrysalis0";
	public override void SetDefaults()
	{
		Projectile.width = 50;
		Projectile.height = 50;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 90;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Summon;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Player player = Main.player[Projectile.owner];
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(Math.PI / 2d));
		if (Projectile.timeLeft == 75)
		{
			player.AddBuff(ModContent.BuffType<GlowMothBuff>(), 18000);
			var p = Projectile.NewProjectileDirect(Terraria.Entity.InheritSource(Projectile), player.Bottom + new Vector2(Main.rand.NextFloat(-60, 60), -5), new Vector2(0, Main.rand.NextFloat(-8, -2)), ModContent.ProjectileType<GlowMoth>(), Projectile.damage, Projectile.knockBack, player.whoAmI, Main.rand.NextFloat(0, 200f), Main.rand.NextFloat(0, 200f));
			p.damage /= 400;

			for (int f = 0; f < 12; f++)
			{
				// Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), player.Bottom + new Vector2(Main.rand.NextFloat((f - 5.5f) * 10, (f - 4.5f) * 10), Main.rand.NextFloat(-5, 15)), new Vector2(0, Main.rand.NextFloat(-12, -4)), ModContent.ProjectileType<DarkEffect>(), Projectile.damage, Projectile.knockBack, player.whoAmI, Main.rand.NextFloat(7f, 15f), 0);

				for (int z = 0; z < 4; z++)
				{
					int ds = Dust.NewDust(player.Bottom + new Vector2(Main.rand.NextFloat(-60, 60), Main.rand.NextFloat(-5, 15)), 0, 0, ModContent.DustType<MothBlue2>(), 0, Main.rand.NextFloat(-8, -4), 0, default, Main.rand.NextFloat(0.6f, 1.8f));
					Main.dust[ds].velocity = new Vector2(0, Main.rand.NextFloat(-8, -4));
					int es = Dust.NewDust(player.Bottom + new Vector2(Main.rand.NextFloat(-60, 60), Main.rand.NextFloat(-5, 15)), 0, 0, DustID.SpookyWood, 0, Main.rand.NextFloat(-8, -4), 0, default, Main.rand.NextFloat(0.3f, 1.0f));
					Main.dust[es].velocity = new Vector2(0, Main.rand.NextFloat(-8, -4));
				}
			}
		}
		if (Projectile.timeLeft == 78)
		{
			ScreenShaker Splayer = player.GetModPlayer<ScreenShaker>();
			Splayer.FlyCamPosition = new Vector2(0, 32).RotatedByRandom(6.283);

			foreach (NPC target in Main.npc)
			{
				if ((target.Center - Projectile.Center).Length() < 60 && !target.dontTakeDamage && !target.friendly)
				{
					NPC.HitModifiers npcHitM = new NPC.HitModifiers();
					NPC.HitInfo hit = npcHitM.ToHitInfo(Projectile.damage * Main.rand.NextFloat(0.85f, 1.15f) * 4, Main.rand.NextFloat(100f) < player.GetTotalCritChance(Projectile.DamageType), 2);
					target.StrikeNPC(hit, true, true);
					NetMessage.SendStrikeNPC(target, hit);
				}
			}
		}
	}

	private float Dy = 0;

	public override bool PreDraw(ref Color lightColor)
	{
		Player player = Main.player[Projectile.owner];
		Texture2D t = ModAsset.EvilChrysalis.Value;
		Texture2D tG = ModAsset.EvilChrysalisG.Value;
		var drawOrigin = new Vector2(t.Width * 0.5f, t.Height * 0.5f);
		Color c0 = Lighting.GetColor((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f));
		SpriteEffects sp = SpriteEffects.None;
		if (player.direction == -1)
			sp = SpriteEffects.FlipHorizontally;

		if (Projectile.timeLeft >= 75)
			Dy += 0.5f;
		else
		{
			Dy = 0;
		}
		if (Projectile.timeLeft < 72)
		{
			t = ModAsset.EvilChrysalis1.Value;
			tG = ModAsset.EvilChrysalis1G.Value;
		}
		if (Projectile.timeLeft < 66)
		{
			t = ModAsset.EvilChrysalis2.Value;
			tG = ModAsset.EvilChrysalis2G.Value;
		}
		if (Projectile.timeLeft < 60)
		{
			t = ModAsset.EvilChrysalis3.Value;
			tG = ModAsset.EvilChrysalis3G.Value;
		}
		if (Projectile.timeLeft < 54)
		{
			t = ModAsset.EvilChrysalis.Value;
			tG = ModAsset.EvilChrysalisG.Value;
		}
		if (Projectile.timeLeft < 48)
		{
			t = ModAsset.EvilChrysalis2.Value;
			tG = ModAsset.EvilChrysalis2G.Value;
		}
		if (Projectile.timeLeft < 42)
		{
			t = ModAsset.EvilChrysalis.Value;
			tG = ModAsset.EvilChrysalisG.Value;
		}
		if (Projectile.timeLeft < 36)
		{
			t = ModAsset.EvilChrysalis3.Value;
			tG = ModAsset.EvilChrysalis3G.Value;
		}
		if (Projectile.timeLeft < 30)
		{
			t = ModAsset.EvilChrysalis.Value;
			tG = ModAsset.EvilChrysalisG.Value;
		}
		if (Projectile.timeLeft < 24)
		{
			t = ModAsset.EvilChrysalis4.Value;
			tG = ModAsset.EvilChrysalis4G.Value;
		}
		if (Projectile.timeLeft < 18)
		{
			t = ModAsset.EvilChrysalis.Value;
			tG = ModAsset.EvilChrysalisG.Value;
		}
		if (Projectile.timeLeft < 12)
		{
			t = ModAsset.EvilChrysalis5.Value;
			tG = ModAsset.EvilChrysalis5G.Value;
		}
		if (Projectile.timeLeft < 6)
		{
			t = ModAsset.EvilChrysalis.Value;
			tG = ModAsset.EvilChrysalisG.Value;
		}
		Main.spriteBatch.Draw(t, player.Center + new Vector2(20 * player.direction, -5 - Dy) - Main.screenPosition, null, c0, (float)(-0.25 * Math.PI * player.direction), drawOrigin, 1, sp, 0);
		Main.spriteBatch.Draw(tG, player.Center + new Vector2(20 * player.direction, -5 - Dy) - Main.screenPosition, null, new Color(255, 255, 255, 0), (float)(-0.25 * Math.PI * player.direction), drawOrigin, 1, sp, 0);
		return false;
	}

	public override void OnKill(int timeLeft)
	{
	}

	private Vector3[] CirclePoint = new Vector3[120];
	private float Rad = 0;
	private Vector2[] Circle2D = new Vector2[120];
	private float Cy2 = -37.5f;
	private float cirpro = 0;

	public override void PostDraw(Color lightColor)
	{
		Player player = Main.player[Projectile.owner];
		if (Projectile.timeLeft < 75)
		{
			Cy2 = 27.5f - Projectile.timeLeft;
			Rad = (float)Math.Sin(Projectile.timeLeft / 75d * Math.PI) * 90f;
		}
		cirpro += 0.5f;
		if (Projectile.timeLeft < 75)
		{
			for (int d = 0; d < 120; d++)
			{
				Circle2D[d] = new Vector2(30, 0).RotatedBy(d * Math.PI / 60d);
				CirclePoint[d] = new Vector3(Circle2D[d].X, -15, 50 + Circle2D[d].Y);
			}
			for (int d = 0; d < 120; d++)
			{
				Circle2D[d] = new Vector2(CirclePoint[d].X / CirclePoint[d].Z, CirclePoint[d].Y / CirclePoint[d].Z) * Rad;
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Vector2 Vbase = player.Center - Main.screenPosition;

			var Vx3 = new List<Vertex2D>();
			for (int h = 0; h < 120; h++)
			{
				Vx3.Add(new Vertex2D(Vbase + Circle2D[h % 120] - new Vector2(0, Cy2), Color.White, new Vector3((h + cirpro) / 30f % 1f, 0, 0)));
				Vx3.Add(new Vertex2D(Vbase + Circle2D[(h + 1) % 120] - new Vector2(0, Cy2), Color.White, new Vector3((0.999f + h + cirpro) / 30f % 1f, 0, 0)));
				Vx3.Add(new Vertex2D(Vbase + new Vector2(0, -0.3f * Rad) - new Vector2(0, Cy2), Color.White, new Vector3((0.5f + h + cirpro) / 30f % 1f, 1, 0)));
			}

			Texture2D t = ModAsset.BlackHalo.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx3.ToArray(), 0, Vx3.Count / 3);
		}
	}
}
