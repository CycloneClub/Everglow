using static Terraria.ModLoader.PlayerDrawLayer;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.TheFirefly.Projectiles;

internal class StaffOfCorruptDust : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 50;
		Projectile.height = 50;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
	}
	public override void OnSpawn(IEntitySource source)
	{
		Projectile.localAI[0] = 0;
		SoundEngine.PlaySound(new SoundStyle("Everglow/Myth/Sounds/CorruptDust_start"), Projectile.Center);
	}
	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		float ProjectileToPlayerDistance = 48f;
		Projectile.velocity *= 0;
		Projectile.Center += player.velocity;
		Vector2 Vdr = Main.MouseWorld - player.MountedCenter;
		Vdr = Vector2.Normalize(Vdr) * ProjectileToPlayerDistance;
		Vector2 aimCenter = player.MountedCenter + Vdr;
		Vector2 MidProjectileNextCenter = Projectile.Center * 0.99f + aimCenter * 0.01f - player.MountedCenter;
		MidProjectileNextCenter = Vector2.Normalize(MidProjectileNextCenter);
		Vector2 PlayerToProjectileNowCenter = Projectile.Center - player.MountedCenter;
		PlayerToProjectileNowCenter = Vector2.Normalize(PlayerToProjectileNowCenter);
		float sinTheta = Vector3.Cross(new Vector3(MidProjectileNextCenter, 0), new Vector3(PlayerToProjectileNowCenter, 0)).Z;
		if (Math.Abs(sinTheta) < 0.18f)
		{
			Projectile.Center = MidProjectileNextCenter * ProjectileToPlayerDistance + player.MountedCenter;
		}
		else
		{
			Projectile.Center = PlayerToProjectileNowCenter.RotatedBy(Math.Asin(0.18 * Math.Sign(sinTheta))) * ProjectileToPlayerDistance + player.MountedCenter;
		}
		Projectile.rotation = (Projectile.Center - player.MountedCenter).ToRotation();
		player.heldProj = Projectile.whoAmI;
		if (player.controlUseItem && player.statMana >= player.HeldItem.mana)
		{
			Projectile.timeLeft = 5;
		}
		else if (Projectile.timeLeft >= 5)
		{
			Projectile.timeLeft = 1;
		}
		Projectile.localAI[0] += 1;
		if (Projectile.localAI[0] % 12 == 2)
		{
			SoundEngine.PlaySound(new SoundStyle("Everglow/Myth/Sounds/CorruptDust_medium").WithPitchOffset(Main.rand.NextFloat(-0.1f, 0.1f)), Projectile.Center);
		}
	}
	public override bool PreDraw(ref Color lightColor)
	{
		Player player = Main.player[Projectile.owner];

		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (float)(Projectile.rotation - Math.PI / 2d));
		Texture2D t = ModAsset.StaffOfCorruptDust.Value;
		Color color = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16.0));
		SpriteEffects S = SpriteEffects.None;
		if (Projectile.Center.X < player.MountedCenter.X)
		{
			player.direction = -1;
		}
		else
		{
			player.direction = 1;
		}

		Main.spriteBatch.Draw(t, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation + MathF.PI * 0.27f, t.Size() / 2f, Projectile.scale, S, 0f);
		return false;
	}
	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(new SoundStyle("Everglow/Myth/Sounds/CorruptDust_end"), Projectile.Center);
		base.OnKill(timeLeft);
	}
}
