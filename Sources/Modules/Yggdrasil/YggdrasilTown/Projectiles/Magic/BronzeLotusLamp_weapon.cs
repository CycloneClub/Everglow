using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.MidnightBayou;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class BronzeLotusLamp_weapon : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void PostAI()
	{
		bool heldByOwner = Projectile.owner >= 0 && Projectile.owner < Main.maxPlayers
			&& Main.player[Projectile.owner].heldProj == Projectile.whoAmI;
		Projectile.drawLayer = heldByOwner ? ProjectileDrawLayerID.HeldProj : ProjectileDrawLayerID.Default;
		Projectile.usesOwnerLight = heldByOwner;
	}

	public override void SetDefaults()
	{
		Projectile.width = 36;
		Projectile.height = 36;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 360000;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.drawLayer = ProjectileDrawLayerID.HeldProj;
		Projectile.usesOwnerLight = true;
	}

	public Vector2 OldRotationShoot = Vector2.zeroVector;

	public override void OnSpawn(IEntitySource source)
	{
		Player player = Main.player[Projectile.owner];
		Vector2 mouseToPlayer = Main.MouseWorld - player.Center;
		mouseToPlayer = Vector2.Normalize(mouseToPlayer);
		Projectile.rotation = mouseToPlayer.ToRotation() + MathHelper.PiOver4;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		base.OnHitNPC(target, hit, damageDone);
	}

	public override void AI()
	{
		HeldProjectileAI();
	}

	public void HeldProjectileAI()
	{
		Player player = Main.player[Projectile.owner];
		if (player.HeldItem.type != ModContent.ItemType<BronzeLotusLamp>())
		{
			Projectile.Kill();
		}
		player.heldProj = Projectile.whoAmI;
		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.Pi);

		Projectile.Center = player.MountedCenter;

		Projectile.velocity *= 0;
		Vector2 mouseToPlayer = Main.MouseWorld - player.MountedCenter;
		mouseToPlayer = Vector2.Normalize(mouseToPlayer);
		if (player.controlUseItem)
		{
			Projectile.rotation = (float)Math.Atan2(mouseToPlayer.Y, mouseToPlayer.X) - MathF.PI * 1.5f;
			Projectile.Center = player.MountedCenter;
			Projectile.velocity *= 0;
			if (player.itemTime == player.itemTimeMax - 1)
			{
				Shoot();
			}
			if (player.itemTime == 0)
			{
				if (player.CheckMana(player.HeldItem, pay: true))
				{
					player.ItemCheck_ApplyManaRegenDelay(player.HeldItem);
					player.itemTime = player.itemTimeMax;
				}
				else
				{
					Projectile.Kill();
				}
			}
			if (mouseToPlayer.X < 0)
			{
				player.direction = -1;
			}
			else
			{
				player.direction = 1;
			}
		}
		else
		{
			if (player.itemTime == 0)
			{
				Projectile.Kill();
			}
		}
	}

	private void Shoot()
	{
		SoundEngine.PlaySound(SoundID.Item72.WithVolumeScale(0.8f), Projectile.Center);
		Vector2 v0 = Main.MouseWorld - Main.player[Projectile.owner].MountedCenter;
		v0 = Vector2.Normalize(v0);
		Player player = Main.player[Projectile.owner];
		if (OldRotationShoot != Vector2.zeroVector)
		{
			float maxPrecise = 0.15f;
			float deltaSinValue = Vector3.Cross(new Vector3(v0, 0), new Vector3(OldRotationShoot, 0)).Z;
			if (MathF.Abs(deltaSinValue) > maxPrecise)
			{
				float distance = player.HeldItem.useTime * player.HeldItem.shootSpeed;
				for (float k = 0; k < MathF.Abs(deltaSinValue); k += maxPrecise)
				{
					Vector2 v1 = v0.RotatedBy(k * Math.Sign(deltaSinValue));
					Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + v1 * (58 + distance * k / MathF.Abs(deltaSinValue)), v1 * player.HeldItem.shootSpeed + player.velocity, ModContent.ProjectileType<BronzeLotusLamp_Flame>(), Projectile.damage, Projectile.knockBack, player.whoAmI, 20);
				}
			}
		}
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + v0 * 58, v0 * player.HeldItem.shootSpeed + player.velocity, ModContent.ProjectileType<BronzeLotusLamp_Flame>(), Projectile.damage, Projectile.knockBack, player.whoAmI, 20);
		if (Main.rand.NextBool(18))
		{
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + v0 * 36, v0 * player.HeldItem.shootSpeed * 0.5f + player.velocity, ModContent.ProjectileType<BronzeLotusLamp_Blossom>(), (int)(Projectile.damage * 2.5f), Projectile.knockBack, player.whoAmI, 20);
		}
		OldRotationShoot = v0;
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		float rot = Projectile.rotation;
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition + new Vector2(0, -24).RotatedBy(Projectile.rotation), null, lightColor, rot, texMain.Size() / 2f, 1f, SpriteEffects.None, 0);
		return false;
	}
}
