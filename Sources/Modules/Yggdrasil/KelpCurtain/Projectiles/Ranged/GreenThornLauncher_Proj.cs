using Terraria.Audio;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;

public class GreenThornLauncher_Proj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public int TileCollideCounter { get; set; }

	public override void SetDefaults()
	{
		Projectile.width = 32;
		Projectile.height = 32;
		Projectile.scale = 1.1875f;

		Projectile.DamageType = DamageClass.Ranged;
		Projectile.friendly = true;
		Projectile.penetrate = 1;
	}

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.TrailCacheLength[Type] = 15;
		ProjectileID.Sets.TrailingMode[Type] = 0;
	}

	public override void AI()
	{
		Projectile.velocity.Y = Projectile.velocity.Y + 0.4f;
		if (Projectile.velocity.Y > 16f)
		{
			Projectile.velocity.Y = 16f;
		}
		Projectile.rotation += 0.2f * Projectile.direction;
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (++TileCollideCounter >= 5)
		{
			return true;
		}
		else
		{
			SoundEngine.PlaySound(SoundID.Item56, Projectile.Center);
			for (int i = 0; i < 5; i++)
			{
				Vector2 velocity = Main.rand.NextVector2Circular(2f, 2f);
				var dust = Dust.NewDustPerfect(Projectile.Center, DustID.GrassBlades, velocity);
				dust.scale = Main.rand.NextFloat(0.8f, 1.4f);
				dust.noGravity = true;
			}
			for (int i = 0; i < 1; i++)
			{
				var dust = Dust.NewDustPerfect(Projectile.Center, DustID.GemEmerald, -Projectile.velocity * 0.3f);
				dust.scale = 1.5f;
				dust.noGravity = true;
				dust.fadeIn = 1.2f;
			}
			Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Circular(1f, 1f), GoreID.TreeLeaf_Normal, 0.6f);
		}
		if (Projectile.velocity.X != oldVelocity.X)
		{
			Projectile.velocity.X = -oldVelocity.X;
		}

		if (Projectile.velocity.Y != oldVelocity.Y)
		{
			Projectile.velocity.Y = -oldVelocity.Y;
		}

		Projectile.velocity *= 0.69f;
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Projectile.Kill();
	}

	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(SoundID.Grass, Projectile.Center);
		for (int i = 0; i < 20; i++)
		{
			var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GrassBlades);
			dust.noGravity = true;
			dust.velocity *= 0.5f;
			dust.scale = Main.rand.NextFloat(0.8f, 1.4f);
		}
		SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

		// 爆炸草叶粒子
		for (int i = 0; i < 20; i++)
		{
			Vector2 velocity = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(4f, 8f);
			var dust = Dust.NewDustPerfect(Projectile.Center, DustID.GrassBlades, velocity);
			dust.scale = Main.rand.NextFloat(1.2f, 2f);
			dust.noGravity = true;
		}

		// 光点粒子
		for (int i = 0; i < 10; i++)
		{
			Vector2 velocity = Main.rand.NextVector2Circular(1.5f, 1.5f) * Main.rand.NextFloat(2f, 4f);
			var dust = Dust.NewDustPerfect(Projectile.Center, DustID.GemEmerald, velocity);
			dust.scale = Main.rand.NextFloat(0.8f, 1.4f);
			dust.noGravity = true;
			dust.fadeIn = 1.2f;
		}

		// 叶片碎屑
		for (int i = 0; i < 2; i++)
		{
			int goreType = GoreID.TreeLeaf_Normal;
			var velocity = Main.rand.NextVector2Circular(3f, 3f);
			Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.Center, velocity, goreType, Main.rand.NextFloat(0.8f, 1.2f));
		}

		float knockback = 1f;

		foreach (NPC npc in Main.npc)
		{
			if (Projectile.Distance(npc.Center) <= 75)
			{
				Player owner = Main.player[Projectile.owner];

				owner.ApplyDamageToNPC(npc, Projectile.damage, Projectile.knockBack, 0);
			}
		}

		if (Main.myPlayer == Projectile.owner)
		{
			var projNum = Main.rand.Next(2, 5);
			for (int i = 0; i < projNum; i++)
			{
				var projVelo = new Vector2(0, 10f).RotatedByRandom(MathHelper.TwoPi);
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, projVelo, ModContent.ProjectileType<GreenThornLauncher_SubProj>(), (int)(Projectile.damage * 0.4f), knockback, Projectile.owner);
			}
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var texture = ModContent.Request<Texture2D>(Texture).Value;
		Vector2 origin = texture.Size() / 2f;

		for (int i = 0; i < Projectile.oldPos.Length; i++)
		{
			Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
			float fade = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
			Color color = Lighting.GetColor(Projectile.Center.ToTileCoordinates(), new Color(131, 219, 0, 100));
			color.A = 100;
			color = Color.Lerp(color, Color.Transparent, 1f - fade);
			color *= 0.5f * fade;

			Main.spriteBatch.Draw(texture, drawPos, null, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
		}

		Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2, 1f, SpriteEffects.None, 0);
		return false;
	}
}
