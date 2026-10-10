using Everglow.Myth.LanternMoon.NPCs;

namespace Everglow.Myth.LanternMoon.Projectiles.PerWave15;

public class BloodLanternGhost_PowerBall : ModProjectile
{
	public float Timer = 0;

	public NPC OwnerNPC;

	public override void SetDefaults()
	{
		Projectile.width = 4;
		Projectile.height = 4;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 500;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.scale = 1f;
	}

	public override void AI()
	{
		Timer++;
		if (Timer % 4 == 0)
		{
			Projectile.frame++;
			Projectile.frame %= 4;
		}
		if (Timer < 60)
		{
			if (OwnerNPC != null && OwnerNPC.active && OwnerNPC.type == ModContent.NPCType<BloodLanternGhost>())
			{
				Projectile.Center = OwnerNPC.Center + new Vector2(0, 60);
			}
		}
		if (Timer == 60)
		{
			Vector2 targetPos = Vector2.zeroVector;
			int targetIndex = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
			Player player = null;
			if (targetIndex != -1)
			{
				player = Main.player[targetIndex];
			}
			if (player != null)
			{
				targetPos = player.Center;
			}
			if (targetPos != Vector2.zeroVector)
			{
				var toTarget = targetPos - Projectile.Center - Projectile.velocity;
				Projectile.velocity = toTarget.SafeNormalize(Vector2.Zero) * 3f;
			}
		}
		float fade = 1f;
		if (Projectile.timeLeft < 60f)
		{
			fade = Projectile.timeLeft / 60f;
			Projectile.hostile = false;
		}
		Projectile.rotation = MathF.Sin(Timer * 0.07f + Projectile.whoAmI) * 0.4f;
		Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.3f * MathF.Sin(Timer * 0.03f + Projectile.whoAmI) + 0.3f, 0.3f * MathF.Cos(Timer * 0.03f + Projectile.whoAmI) + 0.3f) * 1.6f * fade);
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (Projectile.timeLeft > 20)
		{
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<BloodLanternGhost_PowerBall_Explosion>(), 30, 0f, Main.myPlayer, 1);
			Projectile.timeLeft = 20;
		}
		Projectile.velocity *= 0;
		return false;
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		if (Projectile.timeLeft > 20)
		{
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<BloodLanternGhost_PowerBall_Explosion>(), 30, 0f, Main.myPlayer, 1);
			Projectile.timeLeft = 20;
		}
		Projectile.velocity *= 0;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float maxDistance = 40f;
		bool CheckCenter(Vector2 pos)
		{
			return (pos - projHitbox.Center()).Length() < maxDistance / 0.9f;
		}
		return CheckCenter(targetHitbox.TopLeft()) || CheckCenter(targetHitbox.TopRight()) || CheckCenter(targetHitbox.BottomLeft()) || CheckCenter(targetHitbox.BottomRight());
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		float mulScale = MathF.Sin(Timer * 0.03f) * 0.15f + 1f;
		if (Projectile.timeLeft < 20f)
		{
			mulScale *= Projectile.timeLeft / 20f;
		}
		if (Timer < 60)
		{
			mulScale *= Timer / 60f;
		}

		Rectangle frame = new Rectangle(0, 100 * Projectile.frame, 100, 100);
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, new Color(1f, 1f, 1f, 0), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale * mulScale, SpriteEffects.None, 0);
		return false;
	}
}
