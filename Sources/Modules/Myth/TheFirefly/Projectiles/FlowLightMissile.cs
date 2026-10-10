using Everglow.Myth.TheFirefly.Dusts;
using Terraria.Audio;

namespace Everglow.Myth.TheFirefly.Projectiles;

internal class FlowLightMissile : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 36;
		Projectile.height = 36;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 360000;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Magic;
	}

	public override Color? GetAlpha(Color lightColor)
	{
		return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0);
	}

	private bool release = true;
	private Vector2 oldPo = Vector2.Zero;
	private int energy = 0;

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		int holdMana = player.statManaMax - 8;
		player.heldProj = Projectile.whoAmI;
		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathF.PI * 0.75f);
		energy += 2;
		Vector2 MouseToPlayer = Main.MouseWorld - player.MountedCenter;
		MouseToPlayer = Vector2.Normalize(MouseToPlayer);
		if (player.controlUseItem && release)
		{
			Projectile.ai[0] *= 0.9f;
			Projectile.ai[1] -= 1f;
			Projectile.rotation = (float)(Math.Atan2(MouseToPlayer.Y, MouseToPlayer.X) + Math.PI * 0.25);
			Projectile.Center = player.MountedCenter + Vector2.Normalize(MouseToPlayer).RotatedBy(Projectile.ai[0] / 0.8d) * (8f - Projectile.ai[0] * 8) + new Vector2(0, 0);
			oldPo = Projectile.Center;
			Projectile.Center = oldPo;
			Projectile.velocity *= 0;
			player.itemAnimation = 1;
		}
		if (!player.controlUseItem && release)
		{
			if (Projectile.ai[1] > 0)
			{
				Projectile.ai[0] *= 0.9f;
				Projectile.ai[1] -= 1f;
				Projectile.Center = player.MountedCenter + Vector2.Normalize(MouseToPlayer).RotatedBy(Projectile.ai[0] / 4d) * (8f - Projectile.ai[0] * 4);
			}
			else
			{
				Shoot();
			}
		}
		if (energy > 450)
		{
			energy = 450;
		}

		if (player.statMana <= 0)
		{
			Shoot();
		}

		if (energy >= 180)
		{
			// player.velocity += MouseToPlayer;
			// if (player.velocity.Length() > 20f)
			// player.velocity *= 20f / player.velocity.Length();
			Vector2 HitPoint = player.Center + MouseToPlayer * 105;
			if (Collision.SolidCollision(HitPoint, 0, 0))
			{
				HitToAnything();
			}

			foreach (var target in Main.ActiveNPCs)
			{
				if (!target.dontTakeDamage && !target.friendly && target.CanBeChasedBy())
				{
					if (Rectangle.Intersect(target.Hitbox, new Rectangle((int)HitPoint.X, (int)HitPoint.Y, 0, 0)) != new Rectangle(0, 0, 0, 0))
					{
						HitToAnything();
					}
				}
			}
		}

		if (Projectile.Center.X < player.MountedCenter.X)
		{
			player.direction = -1;
		}
		else
		{
			player.direction = 1;
		}
	}

	private void HitToAnything()
	{
		Projectile.velocity = Projectile.oldVelocity;
		Projectile.friendly = false;
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<MissileExplosion>(), (int)(Projectile.damage * (energy + 150) / 100f), Projectile.knockBack * 0.4f, Projectile.owner, energy / 15f + 5f);
		Projectile.Kill();
	}

	private void Shoot()
	{
		SoundEngine.PlaySound(SoundID.Item72.WithVolumeScale(0.5f), Projectile.Center);
		Vector2 v0 = Main.MouseWorld - Main.player[Projectile.owner].MountedCenter;
		v0 = Vector2.Normalize(v0);
		Player player = Main.player[Projectile.owner];
		float ai1 = 1.57f;
		if (Main.rand.NextBool(2))
		{
			ai1 *= -1;
		}
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + v0 * 62, v0 * (energy + 20) / 9f, ModContent.ProjectileType<MissileProj>(), (int)(Projectile.damage * (energy + 150) / 100f), Projectile.knockBack, player.whoAmI, energy / 5f + 10, ai1);

		Vector2 newVelocity = v0;
		newVelocity *= 1f - Main.rand.NextFloat(0.3f);
		newVelocity *= Math.Clamp(energy / 18f, 0.2f, 10f);
		Vector2 basePos = Projectile.Center + newVelocity * 3.7f + v0 * 62 - new Vector2(0, 6);

		for (int j = 0; j < energy; j++)
		{
			Vector2 v = newVelocity / 108f * j;
			Vector2 v1 = new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) * 0.3f + v;
			int num20 = Dust.NewDust(basePos, 0, 0, ModContent.DustType<BlueGlowAppearStoppedByTile>(), v1.X, v1.Y, 100, default, Main.rand.NextFloat(0.6f, 1.8f) * 0.4f);
			Main.dust[num20].noGravity = true;
		}
		for (int j = 0; j < energy; j++)
		{
			Vector2 v = newVelocity / 216f * j;
			Vector2 v1 = new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) * 0.3f + v;
			float Scale = Main.rand.NextFloat(3.7f, 5.1f);
			int num21 = Dust.NewDust(basePos + new Vector2(4, 4.5f), 0, 0, ModContent.DustType<BlueParticleDark2StoppedByTile>(), v1.X, v1.Y, 100, default, Scale);
			Main.dust[num21].alpha = (int)(Main.dust[num21].scale * 50);
		}
		for (int j = 0; j < 16; j++)
		{
			Vector2 v = newVelocity / 216f * j;
			Vector2 v1 = new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) * 0.3f + v;
			v1 *= 0.2f;
			float Scale = Main.rand.NextFloat(3.7f, 5.1f);
			int num21 = Dust.NewDust(basePos + new Vector2(4, 4.5f), 0, 0, ModContent.DustType<MothSmog>(), v1.X, v1.Y, 100, default, Scale);
			Main.dust[num21].alpha = (int)(Main.dust[num21].scale * 50);
		}

		// player.velocity -= newVelocity;
		Projectile.Kill();
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Player player, Color lightColor)
	{
		if (!release)
		{
			return;
		}

		player.heldProj = Projectile.whoAmI;
		Vector2 v0 = Projectile.Center - player.MountedCenter;
		if (player.controlUseTile)
		{
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(Math.Atan2(v0.Y, v0.X) - Math.PI / 2d));
		}

		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Texture2D texMainG = ModAsset.FlowLightMissileGlow.Value;
		Texture2D texMainBloom = ModAsset.FixCoinLight3.Value;

		Projectile.frame = (int)(energy % 45 / 5f);
		Color drawColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16.0));
		SpriteEffects se = SpriteEffects.None;
		if (player.direction == -1)
		{
			se = SpriteEffects.FlipVertically;
		}

		float rot0 = Projectile.rotation - (float)(Math.PI * 0.25) + MathF.PI * 0.36f * player.direction;
		float rot1 = Projectile.rotation - (float)(Math.PI * 0.25);
		// Main.spriteBatch.Draw(texMainBloom, Projectile.Center + new Vector2(56, 0).RotatedBy(rot1) - Main.screenPosition - new Vector2(0, 6), null, new Color(energy, energy, energy, 0) * MathF.Sin(0.4f * (float)Main.time), rot0, texMainBloom.Size() / 2f, 0.6f, se, 0);
		DrawPowerEffect();
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition - new Vector2(0, 6), null, drawColor, rot0, texMain.Size() / 2f, 1f, se, 0);
		Main.spriteBatch.Draw(texMainG, Projectile.Center - Main.screenPosition - new Vector2(0, 6), null, new Color(energy, energy, energy, 0), rot0, texMainG.Size() / 2f, 1f, se, 0);
	}

	public void DrawPowerEffect()
	{
		float rot0 = Projectile.rotation - (float)(Math.PI * 0.25);
		Vector2 bulbPos = Projectile.Center + new Vector2(56, 0).RotatedBy(rot0) - new Vector2(0, 6);
		float energyValue = energy / 250f;
		Color c0 = new Color(0, energyValue * energyValue * 0.2f, energyValue, 0);
		float timeValue = (float)Main.time * 0.004f;
		List<Vertex2D> bars = new List<Vertex2D>();
		float accuracy = 16;
		List<Vertex2D> bars2 = new List<Vertex2D>();
		for (int t = 0; t <= accuracy; t++)
		{
			Vector2 v0 = new Vector2(0, MathF.Sqrt(energy + 15) * 3).RotatedBy(t / (accuracy * 0.5) * Math.PI);
			bars2.Add(new Vertex2D(bulbPos - Main.screenPosition, Color.White * energyValue, new Vector3(timeValue * 0.7f, 0.5f, 0)));
			bars2.Add(new Vertex2D(bulbPos + v0 * 0.7f - Main.screenPosition, c0, new Vector3(timeValue * 0.7f + 0.9f, 0.5f + (t % 2 - 1) * 0.4f, 0)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_cell_black.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		if (bars2.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
		}

		for (int t = 0; t <= accuracy; t++)
		{
			Vector2 v0 = new Vector2(0, MathF.Sqrt(energy + 15) * 3).RotatedBy(t / (accuracy * 0.5) * Math.PI);
			bars.Add(new Vertex2D(bulbPos - Main.screenPosition, c0, new Vector3(timeValue, 0.5f, 0)));
			bars.Add(new Vertex2D(bulbPos + v0 - Main.screenPosition, Color.Transparent, new Vector3(timeValue + 0.5f, 0.5f + (t % 2 - 1) * 0.1f, 0)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_hiveCyber.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}
}
