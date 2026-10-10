using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Bosses.KingJellyBall;

public class JellyBallGelStream : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override void SetCustomDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.scale = 1;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 3600;

		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;
		TrailLength = 20;
		TrailColor = new Color(0.1f, 0.3f, 1, 0f);
		TrailWidth = 40f;
		SelfLuminous = true;
		TrailTexture = Commons.ModAsset.Trail_2_thick.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_2_black.Value;
		TrailShader = Commons.ModAsset.Trailing.Value;
	}

	public override void Behaviors()
	{
		if (TimeAfterEntityDestroy < 0)
		{
			if (Projectile.velocity.Y <= 12)
			{
				Projectile.velocity.Y += 0.2f;
			}
			Projectile.rotation += Projectile.ai[0];
			if (Projectile.timeLeft > 540)
			{
				GenerateSmog(1);
			}
		}
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.rotation = Main.rand.NextFloat(6.283f);
		Projectile.ai[0] = Main.rand.NextFloat(-0.15f, 0.15f);
	}

	public override void DestroyEntityEffect()
	{
		for (int g = 0; g < 36; g++)
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(6, 24)).RotatedByRandom(MathHelper.TwoPi);
			float mulScale = Main.rand.NextFloat(7f, 20f);
			var blood = new JellyBallGelDrop
			{
				Velocity = afterVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(42, 84),
				Scale = mulScale,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(blood);
		}
		NPC.NewNPCDirect(Projectile.GetSource_FromAI(), Projectile.Center - Projectile.oldVelocity, ModContent.NPCType<JellyBall>(), default, 0, 127);
		SoundEngine.PlaySound(SoundID.Item127.WithVolume(1f), Projectile.Center);
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		if (Projectile.penetrate == 1)
		{
			DestroyEntity();
		}
		int maxTime = 60;
		if (Main.expertMode)
		{
			maxTime = 120;
		}
		if (Main.masterMode)
		{
			maxTime = 150;
		}
		target.AddBuff(ModContent.BuffType<JellyBallStick>(), maxTime);
	}

	public void GenerateSmog(int Frequency)
	{
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(2, 24)).RotatedByRandom(MathHelper.TwoPi);
			float mulScale = Main.rand.NextFloat(6f, 12f);
			var blood = new JellyBallGelDrop
			{
				Velocity = afterVelocity / mulScale,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(42, 84),
				Scale = mulScale,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(blood);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float value = (Projectile.timeLeft - 540) / 60f;
		if (value > 1)
		{
			value = 1;
		}
		TrailColor = new Color(0.1f, 0.3f, 1, 0f) * value;
		return base.PreDraw(ref lightColor);
	}

	public override void DrawSelf()
	{
		Vector2 drawCenter = Projectile.Center - Main.screenPosition;
		var drop = new List<Vertex2D>();
		int step = 30;
		float timeValue = (float)Main.time * 0.03f;
		var drawColor = new Color(0.1f, 0.3f, 1f, 0.6f);
		for (int theta = 0; theta <= step; theta++)
		{
			float a = 14;
			float b = 7 + 0.7f * MathF.Sin(timeValue);
			float rot = theta / (float)step * MathHelper.TwoPi;
			float r = a - b * MathF.Sin(rot);

			r *= Projectile.scale;
			Vector2 toDistance = new Vector2(-r, 0).RotatedBy(rot);
			toDistance.Y *= 1.3f;

			drop.Add(drawCenter + toDistance.RotatedBy(Projectile.velocity.ToRotation() - MathHelper.PiOver2), drawColor, new Vector3(theta / (float)step, 0, 0));
			drop.Add(drawCenter, drawColor, new Vector3(theta / (float)step, 1, 0));
		}
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		if (drop.Count > 2)
		{
			Main.graphics.graphicsDevice.Textures[0] = ModAsset.JellyBallGelStream.Value;
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, drop.ToArray(), 0, drop.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		var normal = Vector2.Normalize(Projectile.velocity);
		Vector2 normalLeft = Vector2.Normalize(Projectile.velocity).RotatedBy(MathHelper.PiOver2);
		var dropTrail = new List<Vertex2D>();
		for (int k = 0; k <= 6; k++)
		{
			Color dropColor = drawColor;
			if (k == 0)
			{
				dropColor = Color.Transparent;
			}
			dropTrail.Add(drawCenter + normalLeft * 14 - normal * 7 * (k - 2), dropColor, new Vector3(-k / 10f + timeValue, 0.8f, 1 - k / 6f));
			dropTrail.Add(drawCenter - normalLeft * 14 - normal * 7 * (k - 2), dropColor, new Vector3(-k / 10f + timeValue, 0.2f, 1 - k / 6f));
		}
		Effect effect = ModAsset.JellyBallGelStream_Trail_dissolve.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uThredshold"].SetValue(0.05f);
		effect.CurrentTechnique.Passes["Test"].Apply();
		if (dropTrail.Count > 2)
		{
			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_thick.Value;
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, dropTrail.ToArray(), 0, dropTrail.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		timeValue *= 0.5f;
		factor *= 0.7f;
		return base.ModifyTrailTextureCoordinate(factor, timeValue, phase, widthValue);
	}

	public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
	{
		modifiers.HitDirectionOverride = Projectile.velocity.X > 0 ? 1 : -1;
		base.ModifyHitPlayer(target, ref modifiers);
	}
}
