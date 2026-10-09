using Terraria;
using Terraria.GameContent;
using Terraria.Localization;

namespace Everglow.Myth.TheTusk.Projectiles.Weapon;

public class SplieSpineBullet : ModProjectile
{
	// TODO:Splie应为Split翻译
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("SplieSpineBullet");
	}
	public override void SetDefaults()
	{
		Projectile.width = 8;
		Projectile.height = 8;
		Projectile.friendly = true;
		Projectile.alpha = 0;
		Projectile.penetrate = 3;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 300;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.aiStyle = -1;
	}

	private int Tokill = -1;
	public override void AI()
	{
		if (Tokill < 0)
		{
			for (int j = 0; j < 200; j++)
			{
				if ((Main.npc[j].Center - (Projectile.Center + Projectile.velocity * 1.5f)).Length() < 120 && !Main.npc[j].dontTakeDamage && !Main.npc[j].friendly && Main.npc[j].active)
				{
					var v0 = Vector2.Normalize(Projectile.velocity);
					var v1 = Vector2.Normalize(Main.npc[j].Center - Projectile.Center);
					float CosAng = Vector2.Dot(v0, v1); // 夹角余弦值大于0.707,即为45°
					if (CosAng > 0.707)// 爆
					{
						Explosion();
						break;
					}
				}
			}

		}
		if (Tokill is >= 0 and <= 2)
		{
			Projectile.Kill();
		}

		if (Tokill > 0)
		{
			Tokill--;
		}

		if (Tokill is <= 44 and > 0)
		{
			Projectile.position = Projectile.oldPosition;
			Projectile.velocity = Projectile.oldVelocity;
		}
	}
	private void Explosion()
	{
		TuskModPlayer mplayer = Main.player[Projectile.owner].GetModPlayer<TuskModPlayer>();
		mplayer.Shake = 1;
		int NumProjectiles = 6;
		for (int i = 0; i < NumProjectiles; i++)
		{
			Vector2 newVelocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(35));
			newVelocity *= 0.6f - Main.rand.NextFloat(0.2f);
			Projectile.NewProjectileDirect(Terraria.Entity.InheritSource(Projectile), Projectile.Center - Vector2.Normalize(newVelocity) * 60, newVelocity, (int)Projectile.ai[1], Projectile.damage, Projectile.knockBack, Projectile.owner);
		}
		for (int i = 0; i < 26; i++)
		{
			Vector2 v = new Vector2(0, Main.rand.NextFloat(0.4f, 1.4f)).RotatedByRandom(MathHelper.TwoPi);
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, v.X, v.Y, 150, default, Main.rand.NextFloat(0.8f, 4f));
		}
		for (int i = 0; i < 16; i++)
		{
			Vector2 v = new Vector2(0, Main.rand.NextFloat(1, 3)).RotatedByRandom(MathHelper.TwoPi);
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, v.X, v.Y, 0, default, Main.rand.NextFloat(0.8f, 1.3f));
		}
		Projectile.velocity = Projectile.oldVelocity;
		Tokill = 45; // 0.75s后消掉
		Projectile.friendly = false;
		Projectile.damage = 0;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.aiStyle = -1;
	}
	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Explosion();
	}
	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Explosion();
		return false;
	}
	public override void OnKill(int timeLeft)
	{
	}
	public override void PostDraw(Color lightColor)
	{
	}
	public override bool PreDraw(ref Color lightColor)
	{
		if (Tokill > 0)
		{
			return false;
		}
		Texture2D t = TextureAssets.Projectile[Math.Clamp((int)Projectile.ai[1], 0, TextureAssets.Projectile.Length)].Value;
		// 获取贴图中央颜色像素块,后面用于光照
		var Lig = new Color[t.Width * t.Height];
		t.GetData(Lig);
		Color c0 = Lig[(int)(t.Width * t.Height / 2f - 1)];


		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		var bars = new List<Vertex2D>();
		float width = t.Width / 2f;
		var normalDir = Projectile.velocity;
		normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
		Vector2 normalDirLeft = normalDir.RotatedBy(MathHelper.PiOver2);
		float Tr = c0.R / 300f;
		float Tg = c0.G / 300f;
		float Tb = c0.B / 300f;
		float mulLight = 0.2f;
		if (Projectile.timeLeft < 60f)
		{
			mulLight = Projectile.timeLeft / 300f;
		}

		float lightValue = (255 - Projectile.alpha) / 50f * mulLight;
		Lighting.AddLight(Projectile.Center, Tr * lightValue, Tg * lightValue, Tb * lightValue);
		Vector2 drawCenter = Projectile.Center - Main.screenPosition;

		bars.Add(new Vertex2D(drawCenter + normalDir * width, Color.White, new Vector3(1, 0, 0)));
		bars.Add(new Vertex2D(drawCenter + normalDir * -width, Color.White, new Vector3(0, 0, 0)));

		bars.Add(new Vertex2D(drawCenter + normalDirLeft * t.Height + normalDir * width, Color.White, new Vector3(1, 1, 0)));
		bars.Add(new Vertex2D(drawCenter + normalDirLeft * t.Height + normalDir * -width, Color.White, new Vector3(0, 1, 0)));

		Main.graphics.GraphicsDevice.Textures[0] = t;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		return false;
	}
}
