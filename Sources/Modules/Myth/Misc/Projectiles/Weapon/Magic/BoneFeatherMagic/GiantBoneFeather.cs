using Terraria.Audio;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Magic.BoneFeatherMagic;

public class GiantBoneFeather : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 40;
		Projectile.height = 40;
		Projectile.friendly = true;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = false;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 360;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.extraUpdates = 3;
		Projectile.localNPCHitCooldown = 2;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 80;
	}

	internal int timeTokill = -1;

	public override void AI()
	{
		if (timeTokill >= 0 && timeTokill <= 2)
		{
			Projectile.Kill();
		}

		if (timeTokill <= 80 && timeTokill > 0)
		{
			Projectile.velocity = Projectile.oldVelocity;
		}

		timeTokill--;
		if (timeTokill >= 0)
		{
			if (timeTokill < 10)
			{
				Projectile.damage = 0;
				Projectile.friendly = false;
			}
			Projectile.velocity *= 0f;
		}
		else
		{
			Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
			if (Projectile.position.Y < Main.MouseWorld.Y)
			{
				Projectile.tileCollide = false;
			}
			else
			{
				Projectile.tileCollide = true;
			}

			if (Projectile.Center.X > Main.screenPosition.X - 100 && Projectile.Center.X < Main.screenPosition.X + Main.screenWidth + 100 && Projectile.Center.Y > Main.screenPosition.Y - 100 && Projectile.Center.Y < Main.screenPosition.Y + Main.screenWidth + 100)
			{
			}
		}
		if (Main.rand.NextBool(6))
		{
			Vector2 v = new Vector2(0, Main.rand.NextFloat(0, 2)).RotatedByRandom(MathHelper.TwoPi);
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.BoneFeather>(), v.X, v.Y, 150, default, Main.rand.NextFloat(0.8f, 1.7f));
		}
		if (Projectile.position.Y <= 320 || Projectile.position.Y >= Main.maxTilesY * 16 - 320)
		{
			Projectile.Kill();
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Color lightColor)
	{
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		if (timeTokill >= 0)
		{
			float timeValue = (80 - timeTokill) / 80f;
			DrawTrail(Commons.ModAsset.Trail_2_black_thick.Value, Color.White, 36);
			DrawTrail(Commons.ModAsset.Noise_spine.Value, new Color(0.9f * (1 - timeValue), 0.9f * (1 - timeValue), 0.6f * (1 - timeValue), 0f), Math.Max(timeTokill - 44, 0));
			return;
		}
		else
		{
			DrawTrail(Commons.ModAsset.Trail_2_black_thick.Value, Color.White);
			DrawTrail(Commons.ModAsset.Noise_spine.Value, new Color(0.9f, 0.9f, 0.5f, 0f));
		}
		SpriteEffects spriteEffects = SpriteEffects.None;
		if (Projectile.spriteDirection == -1)
		{
			spriteEffects = SpriteEffects.FlipHorizontally;
		}

		var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
		int frameHeight = texture.Height / Main.projFrames[Projectile.type];
		int startY = frameHeight * Projectile.frame;
		var sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
		Vector2 origin = sourceRectangle.Size() / 2f;
		float offsetX = 20f;
		origin.X = Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX;
		float amount = 1f;
		if (Projectile.timeLeft >= 1040)
		{
			amount = (1080 - Projectile.timeLeft) / 40f;
		}
		Color aimColor = new Color(1f, 1f, 1f, 1f);
		Color drawColor = Color.Lerp(lightColor, aimColor, amount);
		if (Projectile.wet)
		{
			float value = 0.6f;
			if (Projectile.timeLeft < 700)
			{
				value = (Projectile.timeLeft - 100) / 1000f;
			}
			aimColor = new Color(value, value / 12f, 0f, 1f);
			drawColor = aimColor;
		}
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
	}

	public void DrawTrail(Texture2D tex, Color color, float width = 36)
	{
		var c0 = color;
		var bars = new List<Vertex2D>();

		int trueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			trueL++;
		}
		if (timeTokill > 0)
		{
			trueL = Math.Max(trueL, timeTokill);
			if (trueL == 0)
			{
				return;
			}
		}
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			float width2 = width;
			if (Projectile.timeLeft <= 40)
			{
				width2 = Projectile.timeLeft * 0.9f;
			}

			if (i < 10)
			{
				width2 *= i / 10f;
			}

			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			if (normalDir == Vector2.zeroVector)
			{
				continue;
			}
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			var factor = i / (float)trueL;
			var w = MathHelper.Lerp(1f, 0.05f, factor);
			float x0 = factor * 0.6f - (float)(Main.timeForVisualEffects / 35d) + 10000;
			if (i > 2)
			{
				if ((Projectile.oldPos[i - 2] - Projectile.oldPos[i - 1]) == Vector2.zeroVector)
				{
					bars.Add(new Vertex2D(Projectile.oldPos[i - 1] + new Vector2(Projectile.width / 2f) - Main.screenPosition + Projectile.velocity, c0, new Vector3(x0, 0.5f, w)));
					bars.Add(new Vertex2D(Projectile.oldPos[i - 1] + new Vector2(Projectile.width / 2f) - Main.screenPosition + Projectile.velocity, c0, new Vector3(x0, 0.5f, w)));
				}
			}
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width2 * (1 - factor) + new Vector2(Projectile.width / 2f) - Main.screenPosition + Projectile.velocity, c0, new Vector3(x0, 1, w)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width2 * (1 - factor) + new Vector2(Projectile.width / 2f) - Main.screenPosition + Projectile.velocity, c0, new Vector3(x0, 0, w)));
		}

		Texture2D t = tex;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.Textures[0] = t;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		AmmoHit();
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GiantBoneFeatherExplosion>(), Projectile.damage, 10, Projectile.owner);
		return false;
	}

	public void AmmoHit()
	{
		SoundEngine.PlaySound(SoundID.Shatter.WithVolumeScale(0.8f), Projectile.Center);
		if (timeTokill > 0)
		{
			return;
		}
		Player player = Main.player[Projectile.owner];
		timeTokill = 80;
		Projectile.tileCollide = false;
		Projectile.friendly = false;
		Projectile.ignoreWater = true;
		Projectile.velocity = Projectile.oldVelocity;
		SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact.WithVolume(0.3f).WithPitchOffset(Main.rand.NextFloat(-0.4f, 0.4f)), Projectile.Center);
		for (int j = 0; j < 80; j++)
		{
			Vector2 v = new Vector2(0, Main.rand.NextFloat(7, 160)).RotatedByRandom(MathHelper.TwoPi);
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.BoneFeather>(), v.X, v.Y, 0, default, Main.rand.NextFloat(1.8f, 3.7f));
		}
		for (int j = 0; j < 40; j++)
		{
			Vector2 v = new Vector2(0, Main.rand.NextFloat(0.8f, 9)).RotatedByRandom(MathHelper.TwoPi);
			Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.Bones2>(), v.X, v.Y, 0, default, Main.rand.NextFloat(0.8f, 1.7f));
			d.rotation = Main.rand.NextFloat(6.283f);
		}
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GiantBoneFeatherExplosion>(), Projectile.damage, Projectile.knockBack, player.whoAmI, 8);
		for (int x = 0; x < 6; x++)
		{
			Vector2 v0 = new Vector2(0, 15).RotatedBy(x / 6d * Math.Tau);
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center - v0 * 2, v0, ModContent.ProjectileType<BoneFeather_spine>(), Projectile.damage / 8, Projectile.knockBack, player.whoAmI, Main.rand.NextFloat(1f), Main.rand.NextFloat(1f));
		}
		Projectile.position -= Projectile.velocity;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		AmmoHit();
	}
}
