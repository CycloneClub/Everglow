using Everglow.Commons.VFX.CommonVFXDusts;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.TheTusk.Projectiles;

public class Living_Jawbone_Huge : ModProjectile
{
	public bool OpenMouth = false;

	public override void SetDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCsAndTiles;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
		Projectile.width = 100;
		Projectile.height = 100;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 1000;
	}

	public override void OnSpawn(IEntitySource source)
	{
		for (int g = 0; g < 4; g++)
		{
			var blood = new BloodDrop
			{
				Velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(0.4f, 1.1f),
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(54, 74),
				Scale = Main.rand.NextFloat(6f, 25f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(blood);
		}
		for (int g = 0; g < 2; g++)
		{
			var blood = new BloodSplash
			{
				Velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(0.4f, 1.1f),
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(54, 74),
				Scale = Main.rand.NextFloat(6f, 18f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(20.0f, 40.0f) },
			};
			Ins.VFXManager.Add(blood);
		}
	}

	public override void AI()
	{
		Projectile.ai[1] = (float)Utils.Lerp(Projectile.ai[1], Projectile.ai[0], 0.5f);
		Projectile.ai[2] = (float)Utils.Lerp(Projectile.ai[2], Projectile.ai[1], 0.5f);
		Projectile.rotation = Projectile.velocity.ToRotation();
		Projectile.velocity.Y += 0.2f;
		if (!OpenMouth)
		{
			Projectile.ai[0] = 0;
			if (Main.rand.NextBool(24))
			{
				OpenMouth = true;
			}
		}
		else
		{
			Projectile.ai[0] = (float)Utils.Lerp(Projectile.ai[0], 2, 0.06f);
			if (Projectile.ai[0] > 1.8f)
			{
				Projectile.ai[0] = 0;
				OpenMouth = false;
			}
		}
		if (Projectile.timeLeft < 470 && Projectile.timeLeft > 30)
		{
			if (Collision.SolidCollision(Projectile.Center, 0, 0))
			{
				Projectile.timeLeft = 10;
				Projectile.velocity *= 0.6f;
			}
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float k = 0;
		bool b0 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + new Vector2(430 * Projectile.scale, 0).RotatedBy(Projectile.rotation + Projectile.ai[0]), 60, ref k);
		bool b1 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + new Vector2(430 * Projectile.scale, 0).RotatedBy(Projectile.rotation - Projectile.ai[0]), 60, ref k);
		return b0 || b1;
	}


	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D upJaw = ModAsset.Living_Jawbone_Huge_up.Value;
		Texture2D downJaw = ModAsset.Living_Jawbone_Huge_down.Value;
		SpriteEffects spriteEffects = SpriteEffects.None;
		if (Projectile.spriteDirection == -1)
		{
			spriteEffects = SpriteEffects.FlipVertically;
		}
		lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		Main.EntitySpriteDraw(upJaw, Projectile.Center - Main.screenPosition, null, lightColor * 0.25f, Projectile.rotation - Projectile.ai[2], new Vector2(16f, 92f), Projectile.scale, spriteEffects, 0);
		Main.EntitySpriteDraw(downJaw, Projectile.Center - Main.screenPosition, null, lightColor * 0.25f, Projectile.rotation + Projectile.ai[2], new Vector2(16f, 92f), Projectile.scale, spriteEffects, 0);

		Main.EntitySpriteDraw(upJaw, Projectile.Center - Main.screenPosition, null, lightColor * 0.5f, Projectile.rotation - Projectile.ai[1], new Vector2(16f, 92f), Projectile.scale, spriteEffects, 0);
		Main.EntitySpriteDraw(downJaw, Projectile.Center - Main.screenPosition, null, lightColor * 0.5f, Projectile.rotation + Projectile.ai[1], new Vector2(16f, 92f), Projectile.scale, spriteEffects, 0);

		Main.EntitySpriteDraw(upJaw, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation - Projectile.ai[0], new Vector2(16f, 92f), Projectile.scale, spriteEffects, 0);
		Main.EntitySpriteDraw(downJaw, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + Projectile.ai[0], new Vector2(16f, 92f), Projectile.scale, spriteEffects, 0);
		return false;
	}
}
