using Everglow.Commons.DataStructures;
using Everglow.Myth.Acytaea.VFXs;
using Terraria.Audio;
using static Terraria.GameContent.Bestiary.IL_BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions;

namespace Everglow.Myth.Acytaea.Projectiles;

public class Acytaea_SmashGround : ModProjectile
{
	public override string Texture => "Everglow/Myth/Acytaea/Projectiles/AcytaeaSword_projectile";

	public override void SetDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 60;
		Projectile.extraUpdates = 3;
		Projectile.scale = 1f;
		Projectile.hostile = false;
		Projectile.friendly = true;
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
		if (Projectile.timeLeft == 59)
		{
			ShakerManager.AddShaker(Projectile.Center, new Vector2(0, -1), 40, 30f, 160, 0.9f, 0.8f, 120);
			for (int x = 0; x < 600; x++)
			{
				float size = Main.rand.NextFloat(0.4f, 0.96f);
				var acytaeaFlame = new DragonFlameDust
				{
					Velocity = new Vector2(0, Main.rand.NextFloat(12, 17f)).RotatedByRandom(MathHelper.TwoPi),
					Active = true,
					Visible = true,
					Position = Projectile.Bottom,
					MaxTime = Main.rand.Next(24, 36),
					Scale = 25f * size,
					Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
					Frame = Main.rand.Next(3),
					ai = new float[] { Projectile.Bottom.X, Main.rand.NextFloat(-0.8f, 0.8f) },
				};
				Ins.VFXManager.Add(acytaeaFlame);
			}

			SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.Center);
			Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<Acytaea_FlamePost>(), 110, 3, Projectile.owner, 1);
			p0.Bottom = Projectile.Center;
		}

		if (Projectile.timeLeft == 30)
		{
			Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(120, 0), Vector2.zeroVector, ModContent.ProjectileType<Acytaea_FlamePost>(), 110, 3, Projectile.owner, 0.8f);
			Projectile p1 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(-120, 0), Vector2.zeroVector, ModContent.ProjectileType<Acytaea_FlamePost>(), 110, 3, Projectile.owner, 0.8f);
			p0.Bottom = Projectile.Center + new Vector2(120, 0);
			p1.Bottom = Projectile.Center + new Vector2(-120, 0);
		}
		if (Projectile.timeLeft == 10)
		{
			Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(240, 0), Vector2.zeroVector, ModContent.ProjectileType<Acytaea_FlamePost>(), 110, 3, Projectile.owner, 0.5f);
			Projectile p1 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(-240, 0), Vector2.zeroVector, ModContent.ProjectileType<Acytaea_FlamePost>(), 110, 3, Projectile.owner, 0.5f);
			p0.Bottom = Projectile.Center + new Vector2(240, 0);
			p1.Bottom = Projectile.Center + new Vector2(-240, 0);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Player player, Color lightColor)
	{
		float value = Projectile.timeLeft / 60f;
		Vector2 newScale = new Vector2(1f, value * 2) * MathF.Sin(value * MathF.PI) * 2f;
		Color flame;
		if (value > 0.5f)
		{
			flame = Color.Lerp(new Color(1f, 1f, 1f, 0), new Color(1f, 0.6f, 0f, 0), (value - 0.5f) * 2f);
		}
		else
		{
			flame = Color.Lerp(new Color(1f, 0.6f, 0f, 0), new Color(0.5f, 0f, 0f, 0), value * 2f);
		}
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		Texture2D dark = Commons.ModAsset.Point_black.Value;
		Main.spriteBatch.Draw(dark, Projectile.Center - Main.screenPosition, null, Color.White * value, 0, dark.Size() / 2f, 1f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, flame, 0, star.Size() / 2f, newScale * 0.75f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, flame, MathHelper.PiOver2, star.Size() / 2f, newScale, SpriteEffects.None, 0);
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float range = (60 - Projectile.timeLeft) / 60f;
		range = MathF.Pow(range, 0.3f);
		range *= 270f;
		flame = Color.Lerp(flame, new Color(0.5f, 0f, 0f, 0), 0.7f);
		flame *= MathF.Pow(value, 0.5f);
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int t = 0; t <= 30; t++)
		{
			Vector2 radius = new Vector2(0, -range).RotatedBy(t / 30d * MathHelper.TwoPi);
			float colorValue = (MathF.Sin((t + 11.25f) / 15f * MathHelper.TwoPi) + 0.5f) * 0.5f;
			bars.Add(new Vertex2D(Projectile.Center + radius - Main.screenPosition, flame * colorValue, new Vector3(t / 30f, 0, 0)));
			bars.Add(new Vertex2D(Projectile.Center + radius * 0.5f - Main.screenPosition, Color.Transparent, new Vector3(t / 30f, 1, 0)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_thick.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}
}
