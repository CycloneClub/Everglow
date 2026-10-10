using Terraria.Audio;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class StarDancer : ClubProj
{
	public override void SetCustomDefaults()
	{
		EnableReflection = true;
		Beta = 0.005f;
		MaxOmega = 0.45f;
		WarpValue = 0.05f;
		ReflectionStrength = 1.2f;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (Main.rand.NextBool(7))
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(1f, 1.2f), 0).RotatedByRandom(6.283);
			for (int i = 0; i < 5; i++)
			{
				Vector2 v1 = v0.RotatedBy(i / 2.5 * Math.PI);
				Vector2 v2 = v0.RotatedBy((i + 0.5) / 2.5 * Math.PI) * 3;
				Vector2 v3 = v0.RotatedBy((i + 1) / 2.5 * Math.PI);
				for (int j = 0; j < 15; j++)
				{
					Vector2 v4 = (v1 * j + v2 * (14 - j)) / 14f;
					Vector2 v5 = (v3 * j + v2 * (14 - j)) / 14f;
					Vector2 v6 = v2 * (14 - j) / 14f;
					var D = Dust.NewDustDirect(target.Center + v4 - new Vector2(4)/*Dust的Size=8x8*/, 0, 0, DustID.GoldCoin, 0, 0, 0, default, 1.5f);
					D.noGravity = true;
					D.velocity = v4;

					var D1 = Dust.NewDustDirect(target.Center + v5 - new Vector2(4)/*Dust的Size=8x8*/, 0, 0, DustID.GoldCoin, 0, 0, 0, default, 1.5f);
					D1.noGravity = true;
					D1.velocity = v5;

					var D2 = Dust.NewDustDirect(target.Center + v6 - new Vector2(4)/*Dust的Size=8x8*/, 0, 0, DustID.GoldCoin, 0, 0, 0, default, 1.3f);
					D2.noGravity = true;
					D2.velocity = v6;
				}
			}
			Vector2 v7 = new Vector2(0, -Main.rand.NextFloat(1000f, 1200f)).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));
			Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center + v7, -v7 / 40f, ProjectileID.FallingStar, (int)(Projectile.damage * 8.3f), Projectile.knockBack, Projectile.owner);
			SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, target.Center);
		}
	}

	public override void AI()
	{
		base.AI();
		if (Omega > 0.1f)
		{
			GenerateDust();
		}
	}

	private void GenerateDust()
	{
		var v0 = new Vector2(1, 1);
		v0 *= Main.rand.NextFloat(Main.rand.NextFloat(1, HitLength), HitLength);
		v0.X *= Projectile.spriteDirection;
		if (Main.rand.NextBool(2))
		{
			v0 *= -1;
		}

		v0 = v0.RotatedBy(Projectile.rotation);
		float Speed = Math.Min(Omega * 0.5f, 0.181f);
		int type = DustID.GoldCoin;
		for (float d = 0.1f; d < Omega; d += 0.04f)
		{
			var D = Dust.NewDustDirect(Projectile.Center + v0 - new Vector2(4)/*Dust的Size=8x8*/, 0, 0, type, -v0.Y * Speed, v0.X * Speed, 150, default, Main.rand.NextFloat(0.1f, 0.2f));
			D.noGravity = true;
			D.velocity = new Vector2(-v0.Y * Speed, v0.X * Speed);
		}
	}

	public override void PostDraw(Player player, Color lightColor)
	{
		Player previousDrawPlayer = DrawPlayer;
		DrawPlayer = player;
		try
		{
			SpriteEffects effects = SpriteEffects.None;
			if (Projectile.spriteDirection == 1)
			{
				effects = SpriteEffects.FlipHorizontally;
			}

			Texture2D texture = ModAsset.StarDancer_glow.Value;
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, Projectile.scale, effects, 0f);
			for (int i = 0; i < 5; i++)
			{
				float fade = Omega * 2f + 0.2f;
				fade *= (5 - i) / 5f;
				var color2 = new Color(fade, fade, fade, 0);
				Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color2, Projectile.rotation - i * 0.75f * Omega, texture.Size() / 2f, Projectile.scale, effects, 0f);
			}
		}
		finally
		{
			DrawPlayer = previousDrawPlayer;
		}
	}

	public override void PostPreDraw()
	{
		var bars = CreateTrailVertices(useSpecialAplha: true);
		if (bars == null)
		{
			return;
		}

		var lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16)).ToVector4();
		lightColor.W = 0.7f * Omega;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;

		var sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, TrailBlendState(), SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);

		Effect MeleeTrail = ModAsset.CrystalClubTrail.Value;
		MeleeTrail.Parameters["uTransform"].SetValue(model * projection);
		MeleeTrail.Parameters["tex1"].SetValue(ModAsset.CrystalClub_trail.Value);
		MeleeTrail.Parameters["Light"].SetValue(lightColor);
		MeleeTrail.CurrentTechnique.Passes["TrailByOrigTex"].Apply();

		Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(TrailShapeTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}
}
