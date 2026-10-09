using Everglow.Myth.Common;
using Everglow.Myth.TheFirefly.Dusts;
using Everglow.Myth.TheFirefly.Items.Accessories;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class GlowWoodSword : ModProjectile
{
	FireflyBiome fireflyBiome = ModContent.GetInstance<FireflyBiome>();
	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 60;

		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
	}

	public override void AI()
	{
		Projectile.velocity *= 0.97f;
		float k0 = Projectile.timeLeft / 60f;
		Vector2 v0 = new Vector2(Main.rand.NextFloat(0, 0.06f), 0).RotatedByRandom(6.283) * k0;
		var d = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<BlueGlowAppear>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(0.6f, 1.8f) * Projectile.scale * 0.2f * k0);
		if (MothEye.LocalOwner != null && MothEye.LocalOwner.TryGetModPlayer(out MothEyePlayer mothEyePlayer))
		{
			if (mothEyePlayer.MothEyeEquipped && fireflyBiome.IsBiomeActive(Main.LocalPlayer) && Main.hardMode)
			{
				d.velocity = v0 * (int)1.5;
				Projectile.scale = k0 * 3;
				Projectile.damage += (int)k0 * 3;
			}
			else
			{
				d.velocity = v0;
				Projectile.scale = k0 * 2;
			}
		}

		Projectile.rotation = (float)(Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + Math.PI * 0.25);
	}
	public override bool? CanDamage()
	{
		return true;
	}
	public override bool? CanHitNPC(NPC target)
	{
		return true;
	}
	public override bool PreDraw(ref Color lightColor)
	{
		float k0 = Projectile.timeLeft / 60f;
		var c0 = new Color(k0 * k0 * 0.3f, k0 * k0 * 0.8f, k0 * 0.8f + 0.2f, 1 - k0);
		var bars = new List<Vertex2D>();
		float width = 6;
		width *= k0;
		int TrueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
				break;

			TrueL++;
		}
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
				break;

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			var factor = i / (float)TrueL;
			var w = MathHelper.Lerp(1f, 0.05f, factor);
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(8f) - Main.screenPosition, c0, new Vector3(factor, 1, w)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(8f) - Main.screenPosition, c0, new Vector3(factor, 0, w)));
		}
		Texture2D t = ModAsset.MothGreyLine.Value;
		Main.graphics.GraphicsDevice.Textures[0] = t;
		if (bars.Count > 3)
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		return true;
	}
	public override void PostDraw(Color lightColor)
	{
		Texture2D Light = ModAsset.Projectiles_GlowWoodSword.Value;
		float k0 = Projectile.timeLeft / 60f;
		var c0 = new Color(k0 * k0 * 0.3f, k0 * k0 * 0.8f, k0 * 0.8f + 0.2f, 1 - k0);
		Main.spriteBatch.Draw(Light, Projectile.Center - Main.screenPosition, null, c0, Projectile.rotation, Light.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		// 绘制弹幕碰撞箱
		// Rectangle rt = Projectile.Hitbox;
		// rt.X -= (int)Main.screenPosition.X;
		// rt.Y -= (int)Main.screenPosition.Y;
		// Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value,rt,new Color(55,0,0,0));
		base.PostDraw(lightColor);
	}
	public override void OnKill(int timeLeft)
	{

	}
}
