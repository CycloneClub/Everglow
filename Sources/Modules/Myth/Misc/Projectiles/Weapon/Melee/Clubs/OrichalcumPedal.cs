using Everglow.Commons.DataStructures;
using Everglow.Commons.VFX.CommonVFXDusts;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class OrichalcumPedal : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = true;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = false;
		Projectile.DamageType = DamageClass.Magic;

		Projectile.timeLeft = 360;
	}

	public override void OnSpawn(IEntitySource source)
	{
		for (int g = 0; g < 1; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 7f)).RotatedByRandom(MathHelper.TwoPi);
			var fire = new BlossomFlameDust
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) + newVelocity * 3,
				maxTime = Main.rand.Next(6, 15),
				scale = Main.rand.NextFloat(10f, 60f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(fire);
		}
	}

	public override void AI()
	{
		if (TileUtils.PlatformCollision(Projectile.Center))
		{
			Projectile.velocity = Vector2.Zero;
			Projectile.alpha += 10;
			return;
		}
		Projectile.frameCounter++;
		if (Projectile.frameCounter > 5)
		{
			Projectile.frame++;
			Projectile.frameCounter = 0;
		}
		if (Projectile.frame > 2)
		{
			Projectile.frame = 0;
		}
		Projectile.rotation += Projectile.ai[0];
		Projectile.ai[0] *= 0.98f;
		Projectile.velocity += new Vector2(Main.windSpeedCurrent * 0.2f, 0.25f * Projectile.scale);
		Projectile.velocity *= MathF.Pow(0.98f, Projectile.velocity.Length() * 0.54f);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		SpriteEffects spriteEffects = SpriteEffects.None;
		if (Projectile.spriteDirection == -1)
		{
			spriteEffects = SpriteEffects.FlipHorizontally;
		}

		var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect dissolve = Commons.ModAsset.Dissolve.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		float dissolveDuration = 1;
		if (Projectile.alpha > 0)
		{
			dissolveDuration = 1 - Projectile.alpha / 255f - 0.2f;
		}
		dissolve.Parameters["uTransform"].SetValue(model * projection);
		dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_spiderNet.Value);
		dissolve.Parameters["duration"].SetValue(dissolveDuration);
		dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(1f, 0, 0.7f, 1f));
		dissolve.Parameters["uNoiseSize"].SetValue(2f);
		dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
		dissolve.CurrentTechnique.Passes[0].Apply();

		Main.spriteBatch.Draw(texture, Projectile.Center, new Rectangle(Projectile.frame * 20, (int)(Projectile.ai[1] * 20), 20, 20), lightColor * ((255 - Projectile.alpha) / 255f), Projectile.rotation, new Vector2(5), Projectile.scale, spriteEffects, 0);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}
}
