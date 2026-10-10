using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.LightSeeker;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class LightBeamStaff_proj : HandholdProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDef()
	{
		DepartLength = 60;
		TextureRotation = 5 / 18f * MathHelper.Pi;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 5;
		Projectile.ArmorPenetration = 45;
		Projectile.friendly = false;
		base.SetDef();
	}

	public override void OnSpawn(IEntitySource source)
	{
		SoundEngine.PlaySound(SoundID.Shimmer1.WithPitchOffset(1), Projectile.Center);
		Player player = Main.player[Projectile.owner];
		Vector2 mouseToPlayer = Main.MouseWorld - player.Center;
		mouseToPlayer = Vector2.Normalize(mouseToPlayer);
		Projectile.rotation = mouseToPlayer.ToRotation() + MathHelper.PiOver4;
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity * 2, Projectile.velocity, ModContent.ProjectileType<LightBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		base.OnHitNPC(target, hit, damageDone);
	}

	public override void AI()
	{
		HeldProjectileAI();
		Player player = Main.player[Projectile.owner];
	}

	public override void HeldProjectileAI()
	{
		Player player = Main.player[Projectile.owner];
		if (player.HeldItem.type != ModContent.ItemType<LightBeamStaff>())
		{
			Projectile.Kill();
		}
		player.heldProj = Projectile.whoAmI;
		ArmRootPos = player.MountedCenter + new Vector2(-4 * player.direction, -2);

		int timeMax = (int)(player.itemTimeMax / player.meleeSpeed);
		if (Projectile.localNPCHitCooldown > timeMax - 1)
		{
			Projectile.localNPCHitCooldown = timeMax - 1;
		}
		if (player.itemTime == 1)
		{
			Projectile.Kill();
		}
		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Projectile.rotation - MathF.PI * 0.25f) * player.gravDir - MathF.PI * 0.5f);
		DepartLength = 60;

		Projectile.Center = ArmRootPos + new Vector2(0, 1).RotatedBy(Projectile.rotation - Math.PI * 0.75) * DepartLength;
		Projectile.timeLeft = timeMax;
		if (Projectile.Center.X < ArmRootPos.X)
		{
			player.direction = -1;
		}
		else
		{
			player.direction = 1;
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Player player = Main.player[Projectile.owner];
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		SpriteEffects se = SpriteEffects.None;
		if (player.direction == -1)
		{
			se = SpriteEffects.FlipVertically;
		}
		lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		float rot = Projectile.rotation - (float)(Math.PI * 0.25) + TextureRotation * player.direction;
		var texMain_glow = ModAsset.LightBeamStaff_glow.Value;
		float duration = player.itemTime / (float)player.itemTimeMax;
		duration *= 1.5f;
		duration -= 0.5f;
		if (duration < 0)
		{
			duration = 0;
		}
		duration = MathF.Sin(duration * MathHelper.Pi);

		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition + DrawOffset - new Vector2(54, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver4), null, lightColor, rot, texMain.Size() / 2f, 1f, se, 0);
		var powerColor = new Color(duration + 0.3f, duration * duration, duration * duration, 0);
		Main.spriteBatch.Draw(texMain_glow, Projectile.Center - Main.screenPosition + DrawOffset - new Vector2(54, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver4), null, powerColor, rot, texMain_glow.Size() / 2f, 1f, se, 0);
		return false;
	}
}
