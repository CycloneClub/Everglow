using Everglow.Commons.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;

public class Rolle_Swing : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public NPC Owner;

	public int Timer;

	public float StarScale = 0;

	public Vector2 StarPos = Vector2.zeroVector;

	public override void SetDefaults()
	{
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60;
		Projectile.ArmorPenetration = 0;
		Projectile.friendly = true;
		Projectile.timeLeft = 42;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Timer = 0;
		Projectile.direction = 1;
		if (Projectile.velocity.X < 0)
		{
			Projectile.direction = -1;
		}
		Projectile.spriteDirection = Projectile.direction;

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
			Projectile.rotation = Projectile.rotation * 0.6f + 4f * 0.4f;
		}
		if (Timer >= 9 && Timer < 21)
		{
			StarScale = (MathF.Cos((Timer - 9) / 11f * MathHelper.TwoPi - MathHelper.Pi) + 1) * 0.3f;
			StarPos.Y -= 6;
		}
		if (Timer >= 21 && Timer < 30)
		{
			float value = (Timer - 21) / 8f;
			Projectile.rotation = Projectile.rotation * (1 - value) + -0.7f * value;
		}
		if (Timer == 33)
		{
			float knifeRot = Projectile.rotation * Projectile.spriteDirection + MathHelper.Pi;
			for (int i = 0; i < 6; i++)
			{
				Vector2 pos = Projectile.Center + new Vector2(-2, -26 - i * 8).RotatedBy(knifeRot);
				var dust = Dust.NewDustDirect(pos, 0, 0, DustID.Smoke);
				dust.position = pos;
				dust.velocity *= 0.1f;
			}
		}
		if (Timer >= 33 && Timer < 38)
		{
			Projectile.rotation *= (38 - Timer) / 5f;
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (Timer < 21 || Timer > 30)
		{
			return false;
		}

		float startRot = 4f;
		for (float r = startRot; r > Projectile.rotation; r -= 0.1f)
		{
			Vector2 hitPos = Projectile.Center + new Vector2(0, 70).RotatedBy(r * Projectile.spriteDirection);
			float point = 0;
			if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, hitPos, 20, ref point))
			{
				return true;
			}
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
		float knifeRot = Projectile.rotation * Projectile.spriteDirection + MathHelper.Pi;
		Vector2 drawPos = Projectile.Center - Main.screenPosition;
		if (Timer >= 6 && Timer <= 33)
		{
			var knifeFrame = new Rectangle(0, 0, 10, 50);
			if (Projectile.spriteDirection == -1)
			{
				knifeFrame = new Rectangle(12, 0, 10, 50);
			}
			Color knifeColor = lightColor;
			if (Timer >= 21 && Timer < 30)
			{
				knifeColor *= 0.2f;
			}
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
		if (Timer >= 21 && Timer < 30)
		{
			SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			float startRot = 4f;

			var bars_dark = new List<Vertex2D>();
			var bars = new List<Vertex2D>();
			int count = 0;
			for (float r = startRot; r > Projectile.rotation; r -= 0.1f)
			{
				count++;
				Color drawStarColor = lightColor;
				drawStarColor.A = 0;
				Color drawDarkColor = Color.White * 1f;
				float colorValue = 1f;
				if (r - 0.1f <= Projectile.rotation)
				{
					colorValue *= 0;
				}
				if (count < 10)
				{
					colorValue *= count / 10f;
				}
				float value = (Timer - 21) / 8f;
				colorValue *= MathF.Pow((MathF.Cos(value * MathHelper.TwoPi - MathHelper.Pi) + 1f) / 2f, 0.5f);
				float value2 = r;
				colorValue *= MathF.Pow((MathF.Cos(value2 - 3 + value * 4) + 1f) / 2f, 8);
				drawDarkColor *= colorValue * 5;
				drawStarColor *= colorValue;
				bars_dark.Add(drawPos + new Vector2(0, 70).RotatedBy(r * Projectile.spriteDirection), drawDarkColor, new Vector3(r * 0.1f, 0.5f, 0));
				bars_dark.Add(drawPos + new Vector2(0, 0).RotatedBy(r * Projectile.spriteDirection), drawDarkColor * 0, new Vector3(r * 0.1f, 0.8f, 0));

				bars.Add(drawPos + new Vector2(0, 70).RotatedBy(r * Projectile.spriteDirection), drawStarColor, new Vector3(r * 0.1f, 0.5f, 0));
				bars.Add(drawPos + new Vector2(0, 0).RotatedBy(r * Projectile.spriteDirection), drawStarColor * 0, new Vector3(r * 0.1f, 0.8f, 0));
			}
			if (bars_dark.Count >= 3)
			{
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_1_black.Value;
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_dark.ToArray(), 0, bars_dark.Count - 2);
			}
			if (bars.Count >= 3)
			{
				Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_1.Value;
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}
		var armFrame = new Rectangle(0, 52, 10, 20);
		if (Projectile.spriteDirection == -1)
		{
			armFrame = new Rectangle(12, 52, 10, 20);
		}
		Main.spriteBatch.Draw(tex, drawPos, armFrame, lightColor, Projectile.rotation * Projectile.spriteDirection, new Vector2(5, 0), Projectile.scale, SpriteEffects.None, 0);
		return false;
	}
}
