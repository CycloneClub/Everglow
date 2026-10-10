using Everglow.Myth.TheTusk.NPCs.BloodTusk;
using Terraria.DataStructures;
using Terraria.Utilities;

namespace Everglow.Myth.TheTusk.Projectiles;

public class BloodTuskLongSpice : ModProjectile
{
	public override string Texture => ModAsset.Empty_Mod;

	public NPC Tusk;

	public Vector2 StartPos = default;
	public Vector2 StartDeltaPos = default;

	public override void SetDefaults()
	{
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 240;
		Projectile.penetrate = -1;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.velocity *= 0;
		StartPos = Projectile.Center;
	}

	public override void AI()
	{
		if (Tusk != null)
		{
			if (StartDeltaPos == default)
			{
				StartDeltaPos = StartPos - Tusk.Center;
			}
			StartPos = Tusk.Center + StartDeltaPos;
			BloodTusk bloodTusk = Tusk.ModNPC as BloodTusk;
			Projectile.Center = StartPos + new Vector2(Projectile.ai[0], Projectile.ai[1]) * bloodTusk.CowerValue;
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float k = 0;
		if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, StartPos + new Vector2(Projectile.ai[0], Projectile.ai[1]), 10, ref k))
		{
			return true;
		}
		return false;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}
}
