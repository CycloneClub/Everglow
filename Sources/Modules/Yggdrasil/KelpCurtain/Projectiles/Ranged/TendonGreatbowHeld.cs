using Everglow.Yggdrasil.KelpCurtain.Items.Weapons;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;

public class TendonGreatbowHeld : ModProjectile
{
	private enum AIState
	{
		ChargeFrames,
	}

	private int ChargeFrames
	{
		get => (int)Projectile.ai[(int)AIState.ChargeFrames];
		set => Projectile.ai[(int)AIState.ChargeFrames] = value;
	}

	public override string LocalizationCategory => LocalizationUtils.Categories.RangedProjectiles;

	public override string Texture => $"Terraria/Images/Item_{ItemID.Marrow}";

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 34;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.friendly = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 2;
	}

	public override bool ShouldUpdatePosition() => false;

	public override bool? CanDamage() => false;

	public override void AI()
	{
		Player owner = Main.player[Projectile.owner];
		if (!owner.active || owner.dead || owner.noItems || owner.CCed || owner.HeldItem.ModItem is not TendonGreatbow)
		{
			Projectile.Kill();
			return;
		}
		Projectile.timeLeft = 2;
		if (Projectile.owner == Main.myPlayer)
		{
			Vector2 aim = (Main.MouseWorld - owner.MountedCenter).SafeNormalize(Vector2.UnitX * owner.direction);
			if (Vector2.DistanceSquared(aim, Projectile.velocity) > 0.0001f)
			{
				Projectile.velocity = aim;
				Projectile.netUpdate = true;
			}
			if (!owner.channel)
			{
				if (TendonGreatbow.ShouldRelease(ChargeFrames, owner.channel))
				{
					Release(owner);
				}
				Projectile.Kill();
				return;
			}
		}

		if (ChargeFrames < TendonGreatbow.ChargeDuration)
		{
			ChargeFrames++;
			if (ChargeFrames == TendonGreatbow.ChargeDuration && Projectile.owner == Main.myPlayer)
			{
				Projectile.netUpdate = true;
			}
		}
		owner.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
		owner.heldProj = Projectile.whoAmI;
		owner.itemTime = 2;
		owner.itemAnimation = 2;
		owner.itemRotation = (Projectile.velocity * owner.direction).ToRotation();
		Projectile.rotation = Projectile.velocity.ToRotation();
		Projectile.Center = owner.MountedCenter + Projectile.velocity * 18f;
		owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
	}

	private void Release(Player owner)
	{
		Item weapon = owner.HeldItem;
		if (!owner.PickAmmo(weapon, out int type, out float speed, out int damage, out float knockback, out int ammoId))
		{
			return;
		}
		Vector2 position = owner.MountedCenter + Projectile.velocity * 24f;
		Vector2 velocity = Projectile.velocity * speed;
		ItemLoader.ModifyShootStats(weapon, owner, ref position, ref velocity, ref type, ref damage, ref knockback);
		var source = new EntitySource_ItemUse_WithAmmo(owner, weapon, ammoId);
		Projectile.NewProjectile(source, position, velocity, type, damage, knockback, owner.whoAmI);
		if (!Main.dedServ)
		{
			SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		if (!Main.dedServ)
		{
			var texture = TextureAssets.Item[ItemID.Marrow].Value;
			float scale = 1f + 0.35f * ChargeFrames / TendonGreatbow.ChargeDuration;
			var effects = Projectile.velocity.X < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor,
				Projectile.rotation, texture.Size() * 0.5f, scale, effects);
		}
		return false;
	}
}
