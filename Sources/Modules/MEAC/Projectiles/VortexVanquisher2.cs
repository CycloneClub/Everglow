using Terraria.GameContent.Shaders;

namespace Everglow.MEAC.Projectiles;

public class VortexVanquisher2 : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public override string Texture => "Everglow/MEAC/Projectiles/VortexVanquisher";

	public override void SetStaticDefaults()
	{
		Main.projFrames[Projectile.type] = 3;
	}

	public override void SetDefaults()
	{
		Projectile.width = 24;
		Projectile.height = 24;
		Projectile.netImportant = true;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 15;
		Projectile.tileCollide = false;
	}

	private Vector2 mainVec = Vector2.One;

	public override void AI()
	{
		Lighting.AddLight(Projectile.Center, 0.9f, 0.6f, 0f);
		Projectile.velocity *= 0.94f;
		mainVec = Projectile.velocity;
		ProduceWaterRipples(new Vector2(mainVec.Length(), 30));
	}

	private void ProduceWaterRipples(Vector2 beamDims)
	{
		mainVec = Projectile.velocity;
		var shaderData = (WaterShaderData)Terraria.Graphics.Effects.Filters.Scene["WaterDistortion"].GetShader();
		float waveSine = 1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 20f);
		Vector2 ripplePos = Projectile.Center + new Vector2(beamDims.X * 0.5f, 0f).RotatedBy(mainVec.ToRotation());
		Color waveData = new Color(0.5f, 0.1f * Math.Sign(waveSine) + 0.5f, 0f, 1f) * Math.Abs(waveSine);
		shaderData.QueueRipple(ripplePos, waveData, beamDims, RippleShape.Square, mainVec.ToRotation());
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.velocity.ToRotation() + 0.7854f, tex.Size() / 2, Projectile.scale, 0, 0);
		Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Everglow/MEAC/Projectiles/VortexVanquisherGlow").Value, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.velocity.ToRotation() + 0.7854f, tex.Size() / 2, Projectile.scale, 0, 0);
		return false;
	}
}
