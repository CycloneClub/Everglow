using Microsoft.Xna.Framework;
using Terraria;
namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class FeatheredStaff_staff : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override string Texture => ModAsset.Auburn_FeatheredStaff_Mod;

	private Player Owner => Main.player[Projectile.owner];

	private Vector2 OwnerMouseWorld => Owner.MouseWorld();

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
		Projectile.penetrate = -1;
		Projectile.drawLayer = ProjectileDrawLayerID.HeldProj;
		Projectile.usesOwnerLight = true;
	}

	public override void AI()
	{
		Owner.heldProj = Projectile.whoAmI;
		Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathF.PI * 0.75f);

		Vector2 MouseToPlayer = OwnerMouseWorld - Owner.MountedCenter;
		MouseToPlayer = Vector2.Normalize(MouseToPlayer);
		if (Owner.controlUseItem)
		{
			Projectile.rotation = (float)(Math.Atan2(MouseToPlayer.Y, MouseToPlayer.X) + Math.PI * 0.25);
			Projectile.Center = Owner.MountedCenter + Vector2.Normalize(MouseToPlayer) * 28f + new Vector2(0, 0);
			Projectile.velocity *= 0;
			if (Owner.itemTime == Owner.itemTimeMax - 1)
			{
				Shoot();
			}
			if (Owner.itemTime == 0)
			{
				if (Owner.CheckMana(Owner.HeldItem, pay: true))
				{
					Owner.ItemCheck_ApplyManaRegenDelay(Owner.HeldItem);
					Owner.itemTime = Owner.itemTimeMax;
				}
				else
				{
					Projectile.Kill();
				}
			}

			Owner.direction = Projectile.Center.X < Owner.MountedCenter.X ? -1 : 1;
		}
		else
		{
			Projectile.Kill();
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity) => false;

	private void Shoot()
	{
		int projectileNumber = 3;
		Item item = Owner.HeldItem;
		for (int i = 0; i < projectileNumber; i++)
		{
			Vector2 shootDirection = Vector2.Normalize(Main.MouseWorld - Owner.position) * item.shootSpeed;
			Projectile.NewProjectile(
				Owner.GetSource_ItemUse(item),
				Owner.Center + shootDirection * 3 * Projectile.scale,
				shootDirection.RotatedByRandom(MathHelper.ToRadians(15)),
				ModContent.ProjectileType<FeatheredStaff>(),
				item.damage,
				item.knockBack,
				Owner.whoAmI,
				0,
				Main.rand.Next(20));
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */ => false;

	public override void PostDraw(Player player, Color lightColor)
	{
		player.heldProj = Projectile.whoAmI;
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Color drawColor = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16.0));
		SpriteEffects se = SpriteEffects.None;
		if (player.direction == -1)
		{
			se = SpriteEffects.FlipVertically;
		}

		float rot0 = Projectile.rotation - (float)(Math.PI * 0.25) + MathF.PI * 0.3f * player.direction;
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, null, drawColor, rot0, texMain.Size() / 2f, 1f, se, 0);
	}
}
