using Everglow.Yggdrasil.YggdrasilTown.VFXs.TownNPCAttack;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs;

public class Georg_Hammer_Flame : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public NPC Owner;

	public int Timer;

	public float StartBottom = 0;

	public override string Texture => Commons.ModAsset.Empty_Mod;

	public override void SetDefaults()
	{
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60;
		Projectile.ArmorPenetration = 0;
		Projectile.friendly = false;
		Projectile.timeLeft = 180;
		Projectile.tileCollide = false;
		Projectile.hostile = true;
		Projectile.penetrate = -1;
		Projectile.width = 30;
		Projectile.height = 210;
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
		Projectile.Bottom = Projectile.Center;
		StartBottom = Projectile.Bottom.Y;
	}

	public override bool ShouldUpdatePosition() => false;

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => base.Colliding(projHitbox, targetHitbox);

	public override void AI()
	{
		Projectile.velocity *= 0;
		int times = 10;
		for (int x = 0; x < times; x++)
		{
			float size = Main.rand.NextFloat(0.4f, 0.96f);
			var flame = new HammerFlamePostDust
			{
				Velocity = (new Vector2(0, -12) + new Vector2(0, Main.rand.NextFloat(1f, 6f)).RotatedByRandom(MathHelper.TwoPi)) * Main.rand.NextFloat(Projectile.timeLeft / 50f + 0.3f) * Projectile.ai[0],
				Active = true,
				Visible = true,
				Position = Projectile.Bottom,
				MaxTime = Main.rand.Next(24, 36),
				Scale = 20f * size,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				Frame = Main.rand.Next(3),
				ai = new float[] { Projectile.Bottom.X, Main.rand.NextFloat(-0.8f, 0.8f) },
			};
			Ins.VFXManager.Add(flame);
		}
		if (Projectile.height > 0)
		{
			Projectile.height--;
			Projectile.Bottom = new Vector2(Projectile.Bottom.X, StartBottom);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		float value = Projectile.timeLeft / 50f;
		Vector2 newScale = new Vector2(1f, value * 1.1f) * MathF.Sin(value * MathF.PI) * 1.2f * Projectile.ai[0];
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
		Main.spriteBatch.Draw(star, Projectile.Bottom - Main.screenPosition, null, flame, 0, star.Size() / 2f, newScale * 0.75f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Bottom - Main.screenPosition, null, flame, MathHelper.PiOver2, star.Size() / 2f, newScale, SpriteEffects.None, 0);
		return false;
	}
}
