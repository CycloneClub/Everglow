using Everglow.Commons.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class MythrilClub_smash_explosion : ModProjectile
{
	public override string Texture => "Everglow/" + ModAsset.CobaltClub_Path;

	public override void SetDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCsAndTiles;
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 60;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 0;
	}

	public override bool ShouldUpdatePosition()
	{
		return false;
	}

	public override bool PreAI()
	{
		if (Projectile.timeLeft > 60)
		{
			return false;
		}
		return base.PreAI();
	}

	public override void AI()
	{
	}


	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Player player, Color lightColor)
	{
		if (Projectile.timeLeft > 60)
		{
			return;
		}
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect dissolve = Commons.ModAsset.Dissolve.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		float dissolveDuration = Projectile.timeLeft / 30f - 0.2f;
		if (Projectile.timeLeft > 30)
		{
			dissolveDuration = 1.2f;
		}
		dissolve.Parameters["uTransform"].SetValue(model * projection);
		dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_spiderNet.Value);
		dissolve.Parameters["duration"].SetValue(dissolveDuration);
		dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(0f, 0.3f, 0.3f, 1f));
		dissolve.Parameters["uNoiseSize"].SetValue(2f);
		dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
		dissolve.CurrentTechnique.Passes[0].Apply();
		float range = MathF.Sin((60 - Projectile.timeLeft) / 120f * MathF.PI);

		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.MythrilClub_smash0.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

		float mulColor = MathF.Pow(MathF.Sin((60 - Projectile.timeLeft) / 60f * MathF.PI), 4) * 1.2f;
		var bars = new List<Vertex2D>();
		for (int t = 0; t <= 30; t++)
		{
			Vector2 radius = new Vector2(0, -180 * range * Projectile.ai[0]).RotatedBy(t / 30d * MathHelper.TwoPi + MathHelper.Pi + Projectile.ai[1]);
			bars.Add(new Vertex2D(Projectile.Center + radius, new Color(0, 172, 255, 0) * mulColor, new Vector3(t / 10f, 0, 0)));
			bars.Add(new Vertex2D(Projectile.Center + radius * 0.1f, new Color(0, 172, 255, 0) * mulColor, new Vector3(t / 10f, 1f, 0)));
		}
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		bars = new List<Vertex2D>();
		for (int t = 0; t <= 30; t++)
		{
			Vector2 radius = new Vector2(0, -90 * range * Projectile.ai[0]).RotatedBy(t / 30d * MathHelper.TwoPi);
			bars.Add(new Vertex2D(Projectile.Center + radius, new Color(0, 72, 145, 0) * mulColor, new Vector3(t / 10f, 0, 0)));
			bars.Add(new Vertex2D(Projectile.Center + radius * 0.1f, new Color(0, 72, 145, 0) * mulColor, new Vector3(t / 10f, 1f, 0)));
		}
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}
}
