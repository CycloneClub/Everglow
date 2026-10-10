using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.EternalResolve.Projectiles
{
	public class HolyBayonet_Slash : ModProjectile, IWarpProjectile
	{
		public override string Texture => Commons.ModAsset.StabbingProjectile_Mod;

		private Vector2 startCenter;

		public override void OnSpawn(IEntitySource source)
		{
			startCenter = Projectile.Center;
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 60;
			Projectile.timeLeft = 120;
			Projectile.extraUpdates = 3;
		}

		public override void AI()
		{
			Projectile.velocity *= 0.93f;
			if (Main.rand.NextBool(7) && Projectile.timeLeft < 90)
			{
				Vector2 vel = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f)) * Main.rand.NextFloat(3f, 5f);
				var dust = Dust.NewDustDirect(Projectile.position - Vector2.Normalize(Projectile.velocity) * Main.rand.NextFloat(0f, (120 - Projectile.timeLeft) * 3f) - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<HolyDust>(), 0, 0, 0, default, Main.rand.NextFloat(0.95f, 1.2f));
				dust.velocity = vel;
				dust.noGravity = true;
			}
			if (Projectile.timeLeft == 114)
			{
				SoundEngine.PlaySound(new SoundStyle("Everglow/EternalResolve/Sounds/Slash").WithVolumeScale(0.33f), Projectile.Center);
			}
		}

		public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
		{
			float value0 = (120 - Projectile.timeLeft) / 120f;
			float value1 = MathF.Pow(value0, 0.5f);
			float width = (1 - MathF.Cos(value1 * 2f * MathF.PI)) * 10f;
			Vector2 normalizedVelocity = Projectile.velocity.SafeNormalize(Vector2.zeroVector);
			Vector2 normalize = normalizedVelocity.RotatedBy(Math.PI / 2d) * width;
			Color shadow = Color.White * 0.7f;
			var bars = new List<Vertex2D>
			{
				new Vertex2D(startCenter + normalize - Main.screenPosition, shadow, new Vector3(0, 0, 0)),
				new Vertex2D(startCenter - normalize - Main.screenPosition, shadow, new Vector3(0, 1, 0)),
				new Vertex2D(Projectile.Center + normalize - Main.screenPosition, shadow, new Vector3(1, 0, 0)),
				new Vertex2D(Projectile.Center - normalize - Main.screenPosition, shadow, new Vector3(1, 1, 0)),
			};
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Star2_black.Value;
			if (bars.Count > 3)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}

			var light = new Color(1f, 0.85f, 0.3f, 0);
			light *= width / 10f;
			bars = new List<Vertex2D>
			{
				new Vertex2D(startCenter + normalize - Main.screenPosition, light, new Vector3(0, 0, 0)),
				new Vertex2D(startCenter - normalize - Main.screenPosition, light, new Vector3(0, 1, 0)),
				new Vertex2D(Projectile.Center + normalize - Main.screenPosition, light, new Vector3(1, 0, 0)),
				new Vertex2D(Projectile.Center - normalize - Main.screenPosition, light, new Vector3(1, 1, 0)),
			};
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.StabbingProjectile.Value;
			if (bars.Count > 3)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}

			return false;
		}

		public void DrawWarp(VFXBatch sb)
		{
			float time = (float)(Main.time * 0.03);
			float value0 = (120 - Projectile.timeLeft) / 120f;
			float value1 = MathF.Pow(value0, 0.5f);
			float width = (1 - MathF.Cos(value1 * 2f * MathF.PI)) * 5f;
			Vector2 normalizedVelocity = Projectile.velocity.SafeNormalize(Vector2.zeroVector);
			Vector2 normalize = normalizedVelocity.RotatedBy(Math.PI / 2d) * width;
			Vector2 start = startCenter - Main.screenPosition;
			Vector2 end = Projectile.Center - Main.screenPosition;
			var middle = Vector2.Lerp(start, end, 0.5f);
			float rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
			Color alphaColor = Color.White;
			alphaColor.A = 0;
			alphaColor.R = (byte)((rotation + Math.PI) % 6.283 / 6.283 * 255);
			alphaColor.G = 15;
			var bars = new List<Vertex2D>
			{
				new Vertex2D(start - normalize, new Color(alphaColor.R, alphaColor.G / 9, 0, 0), new Vector3(1 + time, 0.3f, 0)),
				new Vertex2D(start + normalize, new Color(alphaColor.R, alphaColor.G / 9, 0, 0), new Vector3(1 + time, 0.7f, 0)),
				new Vertex2D(middle - normalize, new Color(alphaColor.R, alphaColor.G / 3, 0, 0), new Vector3(0.5f + time, 0.3f, 0.5f)),
				new Vertex2D(middle + normalize, new Color(alphaColor.R, alphaColor.G / 3, 0, 0), new Vector3(0.5f + time, 0.7f, 0.5f)),
				new Vertex2D(end, alphaColor, new Vector3(0f + time, 0.5f, 1)),
				new Vertex2D(end, alphaColor, new Vector3(0f + time, 0.5f, 1)),
			};
			sb.Draw(Commons.ModAsset.Trail_1.Value, bars, PrimitiveType.TriangleStrip);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Projectile.damage /= 2;
			base.OnHitNPC(target, hit, damageDone);
		}
	}
}
