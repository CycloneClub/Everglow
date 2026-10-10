namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class MeteorClub : ClubProj
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
		int type = DustID.Flare;
		for (float d = 0.1f; d < Omega; d += 0.04f)
		{
			var D = Dust.NewDustDirect(target.Center - new Vector2(4)/*Dust?Size=8x8*/, 0, 0, type, 0, 0, 150, default, Main.rand.NextFloat(0.4f, 1.1f));
			D.noGravity = true;
			D.velocity = new Vector2(0, Main.rand.NextFloat(Omega * 25f)).RotatedByRandom(6.283);
		}
		target.AddBuff(BuffID.OnFire, 300);
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

			Texture2D texture = ModAsset.MeteorClub_glow.Value;
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
		var color = new Color(MathF.Sqrt(Omega), Omega * Omega * 0.8f, Omega * Omega * 0.2f, Omega * 0.3f);
		var bars = CreateTrailVertices(useSpecialAplha: true, trailColor: color);
		if (bars == null)
		{
			return;
		}

		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		var sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, model);

		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.MeteorClub_trail.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}
}
