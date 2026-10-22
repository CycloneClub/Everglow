using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies;

public class AncientPhantom_Spear_Spear : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	public float HoldoutRangeMin => 24f;

	public float HoldoutRangeMax = 110f;

	public NPC OwnerEnemy;

	public Vector2 NPCCenter;

	public override void SetDefaults()
	{
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.tileCollide = false;
		Projectile.maxPenetrate = 128;
		Projectile.timeLeft = 23;
		Projectile.extraUpdates = 3;
	}

	public override void OnSpawn(IEntitySource source)
	{
		HoldoutRangeMax = Main.rand.NextFloat(96f, 124f);
		base.OnSpawn(source);
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return base.Colliding(projHitbox, targetHitbox);
	}

	public override bool PreAI()
	{
		if (OwnerEnemy is not null && OwnerEnemy.active && OwnerEnemy.type == ModContent.NPCType<AncientPhantom_Spear>())
		{
			NPCCenter = OwnerEnemy.Center;
		}
		int duration = 22;

		if (Projectile.timeLeft > duration)
		{
			Projectile.timeLeft = duration;
		}

		float halfDuration = duration * 0.5f;
		float progress = Projectile.timeLeft < halfDuration ? Projectile.timeLeft / halfDuration : (duration - Projectile.timeLeft) / halfDuration;
		Projectile.Center = NPCCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);
		if (Projectile.timeLeft > 11)
		{
			Lighting.AddLight(Projectile.Center, new Vector3(0.6f, 0.8f, 1f));
			Vector2 velocity = Projectile.velocity * 0.5f;
			var dustVFX = new TwilightCrystalVFXDust
			{
				Velocity = velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = (26 - Projectile.timeLeft) * 1.8f,
				Scale = Main.rand.NextFloat(3, 12),
				Rotation = Projectile.rotation - MathHelper.PiOver4,
				ai = new float[] { 0, 0, 0 },
			};
			Ins.VFXManager.Add(dustVFX);
		}
		else
		{
			Projectile.extraUpdates = 0;
			Lighting.AddLight(Projectile.Center, Vector3.Lerp(new Vector3(0f, 0f, 0.5f), new Vector3(0.6f, 0.8f, 1f), Projectile.timeLeft / 11f));
		}
		return false;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var tex = ModAsset.AncientPhantom_Spear_Spear.Value;
		var tex_glow = ModAsset.AncientPhantom_Spear_Spear_Glow.Value;
		lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor * 0.6f, Projectile.rotation, new Vector2(100, 7), 1f, SpriteEffects.None, 0);
		if (Projectile.timeLeft > 11)
		{
			Main.EntitySpriteDraw(tex_glow, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0f), Projectile.rotation, new Vector2(100, 7), 1f, SpriteEffects.None, 0);
		}
		if (Projectile.timeLeft <= 11)
		{
			var color = Color.Lerp(new Color(0f, 0f, 1f, 0f), new Color(1f, 1f, 1f, 0f), Projectile.timeLeft / 11f);
			Main.EntitySpriteDraw(tex_glow, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, new Vector2(100, 7), 1f, SpriteEffects.None, 0);
		}
		return false;
	}
}
