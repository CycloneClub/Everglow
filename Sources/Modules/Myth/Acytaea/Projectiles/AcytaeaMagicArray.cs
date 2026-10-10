using Everglow.Commons.DataStructures;
using Everglow.Myth.Acytaea.Buffs;
using Everglow.Myth.Acytaea.VFXs;
using Terraria.Audio;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;

namespace Everglow.Myth.Acytaea.Projectiles;

public class AcytaeaMagicArray : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override string Texture => "Everglow/Myth/Acytaea/Projectiles/AcytaeaSword_projectile";

	public override void SetDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 60;
		Projectile.scale = 1f;
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.width = 40;
		Projectile.height = 40;
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Color lightColor)
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
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
		dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(1f, 0f, 0f, 1f));
		dissolve.Parameters["uNoiseSize"].SetValue(2f);
		dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
		dissolve.CurrentTechnique.Passes[0].Apply();
		float range = MathF.Sin((60 - Projectile.timeLeft) / 35f * MathF.PI);
		if (Projectile.timeLeft < 40)
		{
			range = 1;
		}
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int t = 0; t <= 30; t++)
		{
			float mulWid = MathF.Sin(t / 30f * MathHelper.Pi);
			Vector2 radius = new Vector2(90 * range, 0).RotatedBy((t - 15f) / 50d * MathHelper.TwoPi + Projectile.rotation);
			bars.Add(new Vertex2D(Projectile.Center + radius, new Color(255, 0, 215, 155), new Vector3(t / 10f, 0, 0)));
			bars.Add(new Vertex2D(Projectile.Center + radius * (1 + mulWid * 0.5f), new Color(125, 0, 0, 0), new Vector3(t / 10f, 0.3f, 0)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.AcytaeaCircle14.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Texture2D lightSword = ModAsset.AcytaeaCircle11.Value;
		float mulTime = 1f;
		if (Projectile.timeLeft < 30f)
		{
			mulTime = Projectile.timeLeft / 30f;
		}
		for (int t = 0; t < 5; t++)
		{
			Vector2 radius = new Vector2(190 * range, 0).RotatedBy((t - 2f) / 10d * MathHelper.TwoPi + Projectile.rotation);
			Main.spriteBatch.Draw(lightSword, Projectile.Center + radius, null, new Color(255, 0, 215, 155), Projectile.rotation + (t - 2f) / 1.57f, lightSword.Size() / 2f, new Vector2(1.4f, 0.5f * mulTime), SpriteEffects.None, 0);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}
}
