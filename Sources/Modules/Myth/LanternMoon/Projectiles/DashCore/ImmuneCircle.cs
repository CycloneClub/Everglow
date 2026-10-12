using Everglow.Myth.LanternMoon.Buffs;
using Terraria;

namespace Everglow.Myth.LanternMoon.Projectiles.DashCore;

internal class ImmuneCircle : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 34;
		Projectile.height = 34;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 180000;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Magic;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
	}

	public override Color? GetAlpha(Color lightColor)
	{
		return new Color(0, 0, 0, 0);
	}

	private float ka = 1;

	public override void AI()
	{
		if (Projectile.timeLeft < 60f)
		{
			ka *= 0.97f;
		}

		Lighting.AddLight(Projectile.Center, (byte)(color0.R * ka) / 300f, (byte)(color0.G * ka) / 300f, (byte)(color0.B * ka) / 300f);
		int AimPlayer = Projectile.owner;
		if (Main.player[AimPlayer].active)
		{
			Projectile.Center = Main.player[AimPlayer].Center + new Vector2(0, -24);
			aimcolor = new Color(0, 0, 0);
			if (Main.player[AimPlayer].HasBuff(ModContent.BuffType<WhiteImmune>()))
			{
				aimcolor = new Color(255, 255, 255);
			}

			if (Main.player[AimPlayer].HasBuff(ModContent.BuffType<RedImmune>()))
			{
				aimcolor = new Color(255, 0, 0);
			}

			if (Main.player[AimPlayer].HasBuff(ModContent.BuffType<GreenImmune>()))
			{
				aimcolor = new Color(0, 255, 17);
			}

			if (Main.player[AimPlayer].HasBuff(ModContent.BuffType<BlueImmune>()))
			{
				aimcolor = new Color(0, 131, 255);
			}

			if (Main.player[AimPlayer].HasBuff(ModContent.BuffType<BrownImmune>()))
			{
				aimcolor = new Color(107, 53, 0);
			}

			if (Main.player[AimPlayer].HasBuff(ModContent.BuffType<PurpleImmune>()))
			{
				aimcolor = new Color(129, 4, 224);
			}

			if (Main.player[AimPlayer].HasBuff(ModContent.BuffType<PinkImmune>()))
			{
				aimcolor = new Color(255, 0, 191);
			}

			if (Main.player[AimPlayer].HasBuff(ModContent.BuffType<YellowImmune>()))
			{
				aimcolor = new Color(255, 204, 0);
			}
		}
		else
		{
			if (Projectile.timeLeft > 65)
			{
				Projectile.timeLeft = 60;
			}
		}
		if (aimcolor == new Color(0, 0, 0) && Projectile.timeLeft > 65)
		{
			Projectile.timeLeft = 60;
		}

		color0.R = (byte)(color0.R * 0.94f + aimcolor.R * 0.06f);
		color0.G = (byte)(color0.G * 0.94f + aimcolor.G * 0.06f);
		color0.B = (byte)(color0.B * 0.94f + aimcolor.B * 0.06f);
		color0.A = (byte)(color0.A * 0.94f + aimcolor.A * 0.06f);
		kb *= 0.97f;
	}

	private Color color0 = new Color(0, 0, 0);
	private Color aimcolor = new Color(0, 0, 0);
	private float kb = 1;

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}

	private float cirR0 = 0;
	private float cirPro0 = 0;

	public override void PostDraw(Color lightColor)
	{
		cirR0 += 0.007f;
		cirPro0 += 0.1f;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		var Vx = new List<Vertex2D>();

		Vector2 vf = Projectile.Center - Main.screenPosition;
		var color3 = new Color(color0.R, color0.G, color0.B, 0);
		for (int h = 0; h < 90; h++)
		{
			color3.A = 0;
			color3.R = (byte)(color3.R * (255 - Projectile.alpha) / 255f);
			color3.G = (byte)(color3.G * (255 - Projectile.alpha) / 255f);
			color3.B = (byte)(color3.B * (255 - Projectile.alpha) / 255f);
			Vector2 v0 = new Vector2(0, 50).RotatedBy(h / 45d * Math.PI + cirR0);
			Vector2 v1 = new Vector2(0, 50).RotatedBy((h + 1) / 45d * Math.PI + cirR0);
			Vx.Add(new Vertex2D(vf + v0, color3, new Vector3(h / 30f % 1f, 0, 0)));
			Vx.Add(new Vertex2D(vf + v1, color3, new Vector3((0.999f + h) / 30f % 1f, 0, 0)));
			Vx.Add(new Vertex2D(vf, color3, new Vector3((0.5f + h) / 30f % 1f, 1, 0)));
		}
		Texture2D t = ModContent.Request<Texture2D>("Everglow/Myth/LanternMoon/Projectiles/DashCore/ImmuneCircle").Value;
		Main.graphics.GraphicsDevice.Textures[0] = t;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx.ToArray(), 0, Vx.Count / 3);

		var Vx2 = new List<Vertex2D>();
		for (int h = 0; h < 90; h++)
		{
			color3.A = 0;
			color3.R = (byte)(color3.R * (255 - Projectile.alpha) / 255f);
			color3.G = (byte)(color3.G * (255 - Projectile.alpha) / 255f);
			color3.B = (byte)(color3.B * (255 - Projectile.alpha) / 255f);
			Vector2 v0 = new Vector2(0, 50).RotatedBy(h / 45d * Math.PI + cirR0);
			Vector2 v1 = new Vector2(0, 50).RotatedBy((h + 1) / 45d * Math.PI + cirR0);
			Vx2.Add(new Vertex2D(vf + v0, color3, new Vector3(h / 30f % 1f, 0, 0)));
			Vx2.Add(new Vertex2D(vf + v1, color3, new Vector3((0.999f + h) / 30f % 1f, 0, 0)));
			Vx2.Add(new Vertex2D(vf, color3, new Vector3((0.5f + h) / 30f % 1f, 1, 0)));
		}
		t = ModContent.Request<Texture2D>("Everglow/Myth/LanternMoon/Projectiles/DashCore/ImmuneCircle3").Value;
		Main.graphics.GraphicsDevice.Textures[0] = t;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx2.ToArray(), 0, Vx2.Count / 3);

		var Vx3 = new List<Vertex2D>();
		for (int h = 0; h < 90; h++)
		{
			color3.A = 0;
			color3.R = (byte)(color3.R * (255 - Projectile.alpha) / 255f);
			color3.G = (byte)(color3.G * (255 - Projectile.alpha) / 255f);
			color3.B = (byte)(color3.B * (255 - Projectile.alpha) / 255f);
			Vector2 v0 = new Vector2(0, 50).RotatedBy(h / 45d * Math.PI + cirR0);
			Vector2 v1 = new Vector2(0, 50).RotatedBy((h + 1) / 45d * Math.PI + cirR0);
			if (h % 2 == 1)
			{
				Vx3.Add(new Vertex2D(vf + v0, color3, new Vector3(h / 30f % 1f, 0, 0)));
				Vx3.Add(new Vertex2D(vf + v1, color3, new Vector3((0.999f + h) / 30f % 1f, 0, 0)));
				Vx3.Add(new Vertex2D(vf, color3, new Vector3((0.5f + h) / 30f % 1f, 1, 0)));
			}
			else
			{
				Vx3.Add(new Vertex2D(vf + v0, color3, new Vector3((0.999f + h) / 30f % 1f, 0, 0)));
				Vx3.Add(new Vertex2D(vf + v1, color3, new Vector3(h / 30f % 1f, 0, 0)));
				Vx3.Add(new Vertex2D(vf, color3, new Vector3((0.5f + h) / 30f % 1f, 1, 0)));
			}
		}
		t = ModContent.Request<Texture2D>("Everglow/Myth/LanternMoon/Projectiles/DashCore/ImmuneCircle2").Value;
		Main.graphics.GraphicsDevice.Textures[0] = t;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx3.ToArray(), 0, Vx3.Count / 3);
	}
}
