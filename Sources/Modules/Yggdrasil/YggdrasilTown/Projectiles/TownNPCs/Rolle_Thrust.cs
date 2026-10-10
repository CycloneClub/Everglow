using Everglow.Commons.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;

public class Rolle_Thrust : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public NPC Owner;

	public int Timer;

	public float ThrustValue = 0;

	public float StarScale = 0;

	public Vector2 StarPos = Vector2.zeroVector;

	public Vector2 ArmThrustPos = Vector2.zeroVector;

	public override void SetDefaults()
	{
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60;
		Projectile.ArmorPenetration = 0;
		Projectile.friendly = true;
		Projectile.timeLeft = 54;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		ThrustValue = 0;
		ArmThrustPos = Vector2.zeroVector;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Timer = 0;
		Projectile.velocity = Projectile.velocity.RotatedBy(-MathHelper.PiOver2);
		int checkDirection = 1;
		if (Projectile.velocity.Y < 0)
		{
			checkDirection = -1;
		}
		Projectile.spriteDirection = checkDirection;

		StarPos = new Vector2(0, -16);
		if (Projectile.ai[0] is >= 0 and < 200)
		{
			Owner = Main.npc[(int)Projectile.ai[0]];
		}
		if (Owner == null || !Owner.active)
		{
			Projectile.active = false;
		}
	}

	public override bool ShouldUpdatePosition() => false;

	public override void AI()
	{
		if (Owner == null || !Owner.active)
		{
			Projectile.active = false;
			return;
		}
		Projectile.Center = Owner.Center + new Vector2(-Owner.spriteDirection * 6, -10);
		Timer++;
		if (Timer < 6)
		{
			Projectile.rotation = Projectile.rotation * 0.6f + Projectile.velocity.ToRotation() * 0.4f;
		}
		if (Timer == 6)
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
		}
		if (Timer >= 6 && Timer < 12)
		{
			ThrustValue += 4;
		}
		if (Timer >= 9 && Timer < 21)
		{
			StarScale = (MathF.Cos((Timer - 9) / 11f * MathHelper.TwoPi - MathHelper.Pi) + 1) * 0.3f;
			StarPos.Y -= 6;
		}
		if (Timer >= 21 && Timer < 30)
		{
			ArmThrustPos.Y += 4;
		}
		if (Timer == 30)
		{
			ArmThrustPos.Y = -48;
		}
		if (Timer > 30 && Timer <= 40)
		{
			ArmThrustPos.Y += 4.8f;
		}
		if (Timer > 40 && Timer <= 50)
		{
			Projectile.rotation *= (50 - Timer) / 10f;
		}
		if (Timer == 50)
		{
			float knifeRot = Projectile.rotation + MathHelper.Pi;
			for (int i = 0; i < 6; i++)
			{
				Vector2 pos = Projectile.Center + new Vector2(-2, -26 - i * 8).RotatedBy(knifeRot);
				var dust = Dust.NewDustDirect(pos, 0, 0, DustID.Smoke);
				dust.position = pos;
				dust.velocity *= 0.1f;
			}
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (Timer < 30 || Timer > 40)
		{
			return false;
		}
		float knifeRot = Projectile.rotation + MathHelper.Pi;
		Vector2 hitPos = Projectile.Center + new Vector2(1 * Projectile.spriteDirection, -48).RotatedBy(knifeRot) * 2.4f;
		float point = 0;
		if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, hitPos, 20, ref point))
		{
			return true;
		}
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		// target.AddBuff(BuffID.OnFire, 300);
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		Texture2D tex = ModAsset.Rolle_Thrust.Value;
		float knifeRot = Projectile.rotation + MathHelper.Pi;
		Vector2 drawPos = Projectile.Center - Main.screenPosition + ArmThrustPos.RotatedBy(knifeRot);
		if (Timer >= 6 && Timer <= 50)
		{
			var knifeFrame = new Rectangle(0, 0, 10, 50);
			if (Projectile.spriteDirection == -1)
			{
				knifeFrame = new Rectangle(12, 0, 10, 50);
			}
			Color knifeColor = lightColor;
			Main.spriteBatch.Draw(tex, drawPos + new Vector2(-2 * Projectile.spriteDirection, -12).RotatedBy(knifeRot), knifeFrame, knifeColor, knifeRot, new Vector2(5, 50), Projectile.scale, SpriteEffects.None, 0);

			Rectangle knifeFrameShine = knifeFrame;
			knifeFrameShine.X += 24;
			if (Timer >= 6 && Timer < 9)
			{
				Main.spriteBatch.Draw(tex, drawPos + new Vector2(-2 * Projectile.spriteDirection, -12).RotatedBy(knifeRot), knifeFrameShine, lightColor, knifeRot, new Vector2(5, 50), Projectile.scale, SpriteEffects.None, 0);
			}

			if (Timer >= 30 && Timer < 33)
			{
				Main.spriteBatch.Draw(tex, drawPos + new Vector2(-2 * Projectile.spriteDirection, -12).RotatedBy(knifeRot), knifeFrameShine, lightColor, knifeRot, new Vector2(5, 50), Projectile.scale, SpriteEffects.None, 0);
			}
		}
		if (Timer >= 9 && Timer < 21)
		{
			Texture2D star = Commons.ModAsset.StarSlash.Value;
			Color drawStarColor = lightColor;
			drawStarColor.A = 0;
			Main.spriteBatch.Draw(star, drawPos + StarPos.RotatedBy(knifeRot), null, drawStarColor, (float)Main.timeForVisualEffects * 0.08f, star.Size() * 0.5f, Projectile.scale * StarScale, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(star, drawPos + StarPos.RotatedBy(knifeRot), null, drawStarColor, (float)Main.timeForVisualEffects * 0.08f + MathHelper.PiOver2, star.Size() * 0.5f, Projectile.scale * StarScale, SpriteEffects.None, 0);
		}
		var armFrame = new Rectangle(0, 52, 10, 20);
		if (Projectile.spriteDirection == -1)
		{
			armFrame = new Rectangle(12, 52, 10, 20);
		}
		Main.spriteBatch.Draw(tex, drawPos, armFrame, lightColor, Projectile.rotation, new Vector2(5, 0), Projectile.scale, SpriteEffects.None, 0);

		if (Timer >= 30 && Timer <= 40)
		{
			SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			float duration = (Timer - 30) / 10f;
			Vector2 normalized = new Vector2(-2, -12).NormalizeSafe().RotatedBy(knifeRot + MathHelper.PiOver2) * 40 * (1 - duration) * Projectile.spriteDirection;
			var startPos = Projectile.Center - Main.screenPosition + new Vector2(1 * Projectile.spriteDirection, -48).RotatedBy(knifeRot) * (duration * 0.5f);
			var middlePos = Projectile.Center - Main.screenPosition + new Vector2(1 * Projectile.spriteDirection, -48).RotatedBy(knifeRot) * (1f + duration);
			var endPos = Projectile.Center - Main.screenPosition + new Vector2(1 * Projectile.spriteDirection, -48).RotatedBy(knifeRot) * 2.4f;
			var effectColor = lightColor;
			effectColor.A = 0;
			effectColor *= 1 - duration;
			var bars = new List<Vertex2D>();
			bars.Add(startPos, Color.Transparent, new Vector3(1f, 0.5f, 0f));
			bars.Add(startPos - normalized, Color.Transparent, new Vector3(1f, 1, 0f));
			bars.Add(middlePos, effectColor * 0.5f, new Vector3(0.5f, 0.5f, 0.5f));
			bars.Add(middlePos - normalized, effectColor * 0.5f, new Vector3(0.5f, 1, 0.5f));
			bars.Add(endPos, effectColor, new Vector3(0f, 0.5f, 1f));
			bars.Add(endPos - normalized, effectColor, new Vector3(0f, 1, 1f));
			if (bars.Count >= 3)
			{
				Effect effect = Commons.ModAsset.StabSwordEffect.Value;
				var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
				var model = Matrix.CreateTranslation(new Vector3(0)) * Main.GameViewMatrix.TransformationMatrix;
				effect.Parameters["uTransform"].SetValue(model * projection);
				effect.Parameters["uProcession"].SetValue(0.5f);
				effect.CurrentTechnique.Passes[0].Apply();
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_1.Value;
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}
		return false;
	}
}
