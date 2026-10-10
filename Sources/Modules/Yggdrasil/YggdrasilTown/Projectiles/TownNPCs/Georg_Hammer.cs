using Everglow.Commons.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;

public class Georg_Hammer : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public NPC Owner;

	public int Timer;

	public override void SetDefaults()
	{
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60;
		Projectile.ArmorPenetration = 0;
		Projectile.friendly = true;
		Projectile.timeLeft = 60;
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
		Projectile.friendly = true;
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
		Projectile.Center = Owner.Center + new Vector2(-Owner.spriteDirection * 10, -10);
		Timer++;
		if (Timer < 6)
		{
			Projectile.rotation = Projectile.rotation * 0.6f + 4 * 0.4f;
			Projectile.ai[1] = 0.4f;
		}
		if (Timer > 12 && Timer < 16)
		{
			Projectile.ai[1] -= 0.03f;
			Projectile.rotation += Projectile.ai[1];
		}
		if (Timer >= 16 && Timer < 24)
		{
			Projectile.ai[1] -= 0.19f;
			Projectile.rotation += Projectile.ai[1];
		}
		if (Timer == 48)
		{
			Projectile.ai[1] = -Projectile.rotation * 0.15f;
		}
		if (Timer >= 50)
		{
			Projectile.ai[1] = -Projectile.rotation * 0.15f;
			Projectile.rotation += Projectile.ai[1];
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (Timer < 20 || Timer > 40)
		{
			return false;
		}
		Vector2 hitPos = new Vector2(0, 40).RotatedBy(Projectile.rotation * Projectile.spriteDirection) + Projectile.Center;
		return Rectangle.Intersect(new Rectangle((int)hitPos.X - 24, (int)hitPos.Y - 24, 48, 48), targetHitbox) != Rectangle.emptyRectangle;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.OnFire, 300);
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		var bars = new List<Vertex2D>();
		var barsDark = new List<Vertex2D>();
		int times = 0;
		float fadeCool = 1f;
		if (Timer >= 24)
		{
			fadeCool = (27 - Timer) / 3f;
		}
		if (Timer >= 27 || Timer < 19)
		{
			fadeCool = 0;
		}
		var bloomColor = new Color(1f, 0.3f, 0.1f, 0);
		if (Timer > 21 && Timer < 23)
		{
			bloomColor = new Color(1f, 0.9f, 0.6f, 0);
		}
		if (Timer > 26 && Timer < 33)
		{
			bloomColor = Color.Lerp(new Color(1f, 0.3f, 0.1f, 0), new Color(0.5f, 0f, 0f, 0), (33 - Timer) / 7f);
		}
		if (Timer > 33)
		{
			bloomColor = new Color(0.5f, 0f, 0f, 0);
		}
		for (float r = 4.8296f; r > Projectile.rotation; r -= 0.1f)
		{
			times++;
			float fadeT = times / 8f;
			if (times >= 8)
			{
				fadeT = 1;
			}
			if (r - 0.1 <= Projectile.rotation)
			{
				fadeT = 0;
			}
			barsDark.Add(new Vector2(0, 60).RotatedBy(r * Projectile.spriteDirection) + Projectile.Center - Main.screenPosition, Color.White * fadeT * fadeCool, new Vector3(r * 0.3f, 0.5f, 0));
			barsDark.Add(new Vector2(0, 5).RotatedBy(r * Projectile.spriteDirection) + Projectile.Center - Main.screenPosition, Color.White * fadeT * fadeCool, new Vector3(r * 0.3f, 0.1f, 0));
			bars.Add(new Vector2(0, 60).RotatedBy(r * Projectile.spriteDirection) + Projectile.Center - Main.screenPosition, bloomColor * fadeT * fadeCool, new Vector3(r * 0.3f, 0.5f, 0));
			bars.Add(new Vector2(0, 5).RotatedBy(r * Projectile.spriteDirection) + Projectile.Center - Main.screenPosition, bloomColor * fadeT * fadeCool, new Vector3(r * 0.3f, 0.1f, 0));
		}
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_1_black.Value;
		if (barsDark.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, barsDark.ToArray(), 0, barsDark.Count - 2);
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_1.Value;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		Texture2D tex = ModAsset.Georg_Hammer.Value;
		var armFrame = new Rectangle(0, 0, 12, 28);
		if (Projectile.spriteDirection == -1)
		{
			armFrame = new Rectangle(118, 0, 12, 28);
		}
		if (Timer >= 6 && Timer < 9)
		{
			armFrame = new Rectangle(14, 0, 12, 28);
			if (Projectile.spriteDirection == -1)
			{
				armFrame = new Rectangle(104, 0, 12, 28);
			}
		}
		if (Timer >= 9 && Timer < 46)
		{
			armFrame = new Rectangle(28, 0, 12, 28);
			if (Projectile.spriteDirection == -1)
			{
				armFrame = new Rectangle(90, 0, 12, 28);
			}
		}
		if (Timer >= 46 && Timer < 49)
		{
			armFrame = new Rectangle(14, 0, 12, 28);
			if (Projectile.spriteDirection == -1)
			{
				armFrame = new Rectangle(104, 0, 12, 28);
			}
		}
		if (Timer >= 49)
		{
			armFrame = new Rectangle(0, 0, 12, 28);
			if (Projectile.spriteDirection == -1)
			{
				armFrame = new Rectangle(118, 0, 12, 28);
			}
		}
		Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, armFrame, lightColor, Projectile.rotation * Projectile.spriteDirection, new Vector2(6, 4), Projectile.scale, SpriteEffects.None, 0);

		var hammerFrame = new Rectangle(0, 30, 42, 40);
		float hammerScale = 0f;
		float hammerRot = Projectile.rotation * Projectile.spriteDirection + MathHelper.Pi;
		if (Timer >= 6 && Timer <= 9)
		{
			hammerScale = (Timer - 6) / 3f;
		}
		if (Timer >= 9 && Timer < 46)
		{
			hammerScale = 1;
		}
		if (Timer >= 44 && Timer <= 49)
		{
			hammerScale = (49 - Timer) / 5f;
		}
		if (Timer >= 49)
		{
			hammerScale = 0;
		}

		if (Timer >= 16 && Timer < 19)
		{
			hammerFrame = new Rectangle(44, 30, 42, 40);
		}
		if (Timer >= 19 && Timer < 36)
		{
			hammerFrame = new Rectangle(88, 30, 42, 40);
		}
		if (Timer >= 36 && Timer < 40)
		{
			hammerFrame = new Rectangle(44, 30, 42, 40);
		}
		Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(0, -14).RotatedBy(hammerRot) - Main.screenPosition, hammerFrame, lightColor, hammerRot, new Vector2(21, 40), Projectile.scale * hammerScale, SpriteEffects.None, 0);

		Rectangle hammerGlow = hammerFrame;
		hammerGlow.Y += 42;
		float glowColor = 1f;
		if (Timer > 24)
		{
			glowColor = Math.Max((44 - Timer) / 20f, 0);
		}
		Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(0, -14).RotatedBy(hammerRot) - Main.screenPosition, hammerGlow, new Color(1f, 1f, 1f, 0) * glowColor, hammerRot, new Vector2(21, 40), Projectile.scale * hammerScale, SpriteEffects.None, 0);
		return false;
	}
}
