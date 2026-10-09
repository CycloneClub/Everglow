using Everglow.Myth.Acytaea.Projectiles;
using Everglow.Myth.LanternMoon.Buffs;
using Terraria;

namespace Everglow.Myth.LanternMoon.Projectiles.DashCore;

class GreenFlame1Split : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 34;
		Projectile.height = 34;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 180;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Magic;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
	}
	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
	}
	public override Color? GetAlpha(Color lightColor)
	{
		return new Color(0, 0, 0, 0);
	}
	float ka = 1;
	public override void AI()
	{
		Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
		if (Projectile.ai[0] != 15)
		{
			Projectile.velocity *= 0.995f;
		}

		if (Projectile.timeLeft < 60f)
		{
			ka *= 0.97f;
		}

		Lighting.AddLight(Projectile.Center, (byte)(color0.R * ka) / 100f, (byte)(color0.G * ka) / 100f, (byte)(color0.B * ka) / 100f);
		if (Projectile.timeLeft < 60)
		{
			Projectile.scale *= 0.97f;
		}

		color0.R = (byte)(color0.R * 0.94f + Aimcolor.R * 0.06f);
		color0.G = (byte)(color0.G * 0.94f + Aimcolor.G * 0.06f);
		color0.B = (byte)(color0.B * 0.94f + Aimcolor.B * 0.06f);
		color0.A = (byte)(color0.A * 0.94f + Aimcolor.A * 0.06f);
		ProjOldColor[0] = color0;
		for (int f = ProjOldColor.Length - 1; f > 0; f--)
		{
			ProjOldColor[f] = ProjOldColor[f - 1];
		}
		if (Projectile.timeLeft % 15 == 0)
		{
			Vector2 vm = Projectile.velocity.RotatedBy(1.57) * 0.35f;
			Projectile.NewProjectile(null, Projectile.Center, vm, ModContent.ProjectileType<GreenFlame0Little>(), Projectile.damage, 0, Main.myPlayer, 0, 0);
			vm = Projectile.velocity.RotatedBy(-1.57) * 0.35f;
			Projectile.NewProjectile(null, Projectile.Center, vm, ModContent.ProjectileType<GreenFlame0Little>(), Projectile.damage, 0, Main.myPlayer, 0, 0);
		}
		kb *= 0.97f;
	}
	Color color0 = new Color(0, 255, 17);
	Color Aimcolor = new Color(0, 255, 17);
	Color[] ProjOldColor = new Color[70];
	float kb = 1;
	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}
	int TrueL = 1;
	public override void PostDraw(Color lightColor)
	{
		var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(color0.R, color0.G, color0.B, 0), Projectile.rotation, new Vector2(17, 17), Projectile.scale * 2.2f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Everglow/Myth/LanternMoon/Projectiles/DashCore/DashCoreLight").Value, Projectile.Center - Main.screenPosition, null, new Color(color0.R / 155f * kb, color0.G / 155f * kb, color0.B / 155f * kb, 0), Projectile.rotation, new Vector2(56, 56), Projectile.scale * 2f, SpriteEffects.None, 0);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		var bars = new List<Vertex2D>();
		float width = 20;
		if (Projectile.timeLeft < 60)
		{
			width = Projectile.timeLeft / 3f;
		}

		TrueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			TrueL++;
		}
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));

			var factor = i / (float)TrueL;
			var w = MathHelper.Lerp(1f, 0.05f, factor);

			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(17, 17) - Main.screenPosition, new Color(84, 53, 46, 0), new Vector3(factor, 1, w)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(17, 17) - Main.screenPosition, new Color(84, 53, 46, 0), new Vector3(factor, 0, w)));
		}
		var Vx = new List<Vertex2D>();
		if (bars.Count > 2)
		{
			Vx.Add(bars[0]);
			var vertex = new Vertex2D((bars[0].position + bars[1].position) * 0.5f + Vector2.Normalize(Projectile.velocity) * 30, new Color(84, 53, 46, 0), new Vector3(0, 0.5f, 1));
			Vx.Add(bars[1]);
			Vx.Add(vertex);
			for (int i = 0; i < bars.Count - 2; i += 2)
			{
				Vx.Add(bars[i]);
				Vx.Add(bars[i + 2]);
				Vx.Add(bars[i + 1]);

				Vx.Add(bars[i + 1]);
				Vx.Add(bars[i + 2]);
				Vx.Add(bars[i + 3]);
			}
		}
		Texture2D t = Commons.ModAsset.Metero.Value;
		Main.graphics.GraphicsDevice.Textures[0] = t; // GlodenBloodScaleMirror
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx.ToArray(), 0, Vx.Count / 3);

		/*List<Vertex2D> bars2 = new List<Vertex2D>();
            for (int i = 1; i < Projectile.oldPos.Length; ++i)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) break;
                var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
                normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));

                var factor = i / (float)TrueL;
                var w = MathHelper.Lerp(1f, 0.05f, factor);

                bars2.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(10, 10) - Main.screenPosition, new Color(ProjOldColor[i].R, ProjOldColor[i].G, ProjOldColor[i].B, 0), new Vector3(factor, 1, w)));
                bars2.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, new Color(ProjOldColor[i].R, ProjOldColor[i].G, ProjOldColor[i].B, 0), new Vector3(factor, 0, w)));
            }
            List<Vertex2D> Vx2 = new List<Vertex2D>();
            if (bars.Count > 2)
            {
                Vx2.Add(bars[0]);
                var vertex = new Vertex2D((bars[0].position + bars[1].position) * 0.5f + Vector2.Normalize(Projectile.velocity) * 30, new Color(color0.R, color0.G, color0.B, 0), new Vector3(0, 0.5f, 1));
                Vx2.Add(bars[1]);
                Vx2.Add(vertex);
                for (int i = 0; i < bars.Count - 2; i += 2)
                {
                    Vx2.Add(bars[i]);
                    Vx2.Add(bars[i + 2]);
                    Vx2.Add(bars[i + 1]);

                    Vx2.Add(bars[i + 1]);
                    Vx2.Add(bars[i + 2]);
                    Vx2.Add(bars[i + 3]);
                }
            }
            t = ModContent.Request<Texture2D>("Everglow/Myth/LanternMoon/Projectiles/DashCore/Flame").Value;
            Main.graphics.GraphicsDevice.Textures[0] = t;//GlodenBloodScaleMirror
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vx2.ToArray(), 0, Vx2.Count / 3);*/
	}
}
