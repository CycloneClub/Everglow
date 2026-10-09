using Everglow.Myth.Common;
using Everglow.Myth.Misc.Dusts.Slingshots;
using Newtonsoft.Json.Linq;
using Terraria.Audio;
using Everglow.Commons.Templates.Weapons.Slingshots;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Ranged.Slingshots;

public abstract class GemAmmo : SlingshotAmmo
{
	/// <summary>
	/// 拖尾的颜色
	/// </summary>
	internal Color TrailColor = new Color(255, 255, 255, 0);
	/// <summary>
	/// 拖尾的路径
	/// </summary>
	internal string TrailTexPath = "";
	/// <summary>
	/// Dust(粒子)种类,默认钻石粉尘
	/// </summary>
	internal int dustType = ModContent.DustType<DiamondDust>();
	public override void AI()
	{
		if (TimeTokill >= 0 && TimeTokill <= 2)
			Projectile.Kill();
		if (TimeTokill <= 15 && TimeTokill > 0)
			Projectile.velocity = Projectile.oldVelocity;
		TimeTokill--;
		if (TimeTokill < 0)
		{
			Projectile.velocity.Y += 0.17f;
			Dust.NewDustDirect(Projectile.Center - new Vector2(4, 3)/*Half Dust Size*/, 0, 0, dustType, 0, 0, 0, default, Main.rand.NextFloat(0.85f, 1.15f));
		}
		else
		{
			if (TimeTokill < 30)
			{
				Projectile.damage = 0;
				Projectile.friendly = false;
			}
			Projectile.velocity *= 0f;
		}
	}
	public override void DrawTrail()
	{
		DrawShade();
		var bars = new List<Vertex2D>();
		int TrueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				if (i == 1)
					return;
				break;
			}

			TrueL = i;
		}
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
				break;

			float width = 4;
			if (Projectile.timeLeft <= 30)
				width *= Projectile.timeLeft / 30f;
			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);

			var factor = i / (float)TrueL;
			var color = Color.Lerp(TrailColor * 0.6f, new Color(0, 0, 0, 0), factor);

			float fac1 = factor * 3 + (float)(-Main.timeForVisualEffects * 0.03) + 100000;
			float fac2 = (i + 1) / (float)TrueL * 3 + (float)(-Main.timeForVisualEffects * 0.03) + 100000;
			// TODO:925分钟之后会炸

			fac1 %= 1f;
			fac2 %= 1f;
			if (fac1 > fac2)
			{
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(fac1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(fac1, 1, 0)));
				if (i < Projectile.oldPos.Length - 1)
				{
					float fac3 = 1 - fac1;
					fac3 /= 3 / (float)TrueL;
					normalDir = Projectile.oldPos[i] - Projectile.oldPos[i + 1];
					normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);
					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(1, 0, 0)));
					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(1, 1, 0)));

					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(0, 0, 0)));
					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(0, 1, 0)));
				}
			}
			else
			{
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(fac1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(fac1, 1, 0)));
			}
		}

		if (bars.Count > 2)
		{
			Texture2D t = Commons.ModAsset.Trail_1.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}
	private void DrawShade()
	{
		var bars = new List<Vertex2D>();
		int TrueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				if (i == 1)
					return;
				break;
			}

			TrueL = i;
		}
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
				break;

			float width = 4;
			if (Projectile.timeLeft <= 30)
				width *= Projectile.timeLeft / 30f;
			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);

			var factor = i / (float)TrueL;
			var color = Color.Lerp(Color.White * 0.6f, new Color(0, 0, 0, 0), factor);

			float fac1 = factor * 3 + (float)(-Main.timeForVisualEffects * 0.03) + 100000;
			float fac2 = (i + 1) / (float)TrueL * 3 + (float)(-Main.timeForVisualEffects * 0.03) + 100000;

			fac1 %= 1f;
			fac2 %= 1f;
			if (fac1 > fac2)
			{
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(fac1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(fac1, 1, 0)));
				if (i < Projectile.oldPos.Length - 1)
				{
					float fac3 = 1 - fac1;
					fac3 /= 3 / (float)TrueL;
					normalDir = Projectile.oldPos[i] - Projectile.oldPos[i + 1];
					normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);
					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(1, 0, 0)));
					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(1, 1, 0)));

					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(0, 0, 0)));
					bars.Add(new Vertex2D(Projectile.oldPos[i + 1] * (1 - fac3) + Projectile.oldPos[i] * fac3 + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(0, 1, 0)));
				}
			}
			else
			{
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(fac1, 0, 0)));
				bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(10, 10) - Main.screenPosition, color, new Vector3(fac1, 1, 0)));
			}
		}
		if (bars.Count > 2)
		{
			Texture2D t = Commons.ModAsset.Trail_black.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}
	public override void AmmoHit()
	{
		SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
		Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center + Projectile.velocity, Vector2.Zero, ModContent.ProjectileType<NormalHit>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.velocity.Length(), Main.rand.NextFloat(6.283f));
		TimeTokill = 30;
		float Power = Projectile.ai[0] + 0.5f;
		Projectile.velocity = Projectile.oldVelocity;
		Player player = Main.player[Projectile.owner];
		for (int x = 0; x < 50; x++)
		{
			var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType, 0, 0, 150, default, Main.rand.NextFloat(0.35f, 1.35f) * Power * 3);
			d.velocity = new Vector2(0, Main.rand.NextFloat(Main.rand.NextFloat(1f, 4f), 16f) / d.scale).RotatedByRandom(6.283) * Power;
		}
		foreach (NPC target in Main.npc)
		{
			if ((target.Center - Projectile.Center).Length() < 25 * (1 + Power * 3) && !target.dontTakeDamage && !target.friendly)
			{
				NPC.HitModifiers npcHitM = new NPC.HitModifiers();
				NPC.HitInfo hit = npcHitM.ToHitInfo(Projectile.damage * Main.rand.NextFloat(0.85f, 1.15f), Main.rand.NextFloat(100f) < player.GetTotalCritChance(Projectile.DamageType), 0);
				target.StrikeNPC(hit, true, true);
				NetMessage.SendStrikeNPC(target, hit);
			}
		}
		Projectile.velocity *= 0f;
	}
}
