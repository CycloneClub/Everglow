using Terraria.DataStructures;

namespace Everglow.Yggdrasil.CorruptWormHive.Projectiles.Melee.TrueDeathSickle;

public class TrueDeathSickleManager : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public override string Texture => ModAsset.TrueDeathSickleHit_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 120;
		Projectile.localNPCHitCooldown = 60;
		Projectile.usesLocalNPCImmunity = true;
	}

	public bool[] AttackState = new bool[8];

	public override void OnSpawn(IEntitySource source)
	{
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.ai[0] += player.meleeSpeed;
		Item item = player.HeldItem;
		if (Projectile.ai[0] >= 0 && !AttackState[0])
		{
			SetPlayerDirection(player);
			Projectile.NewProjectile(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<TrueDeathSickle_Blade>(), (int)(player.GetWeaponDamage(item) * 2.6f), item.knockBack, player.whoAmI, 240f, 0, -Main.rand.NextFloat(0.0f, 0.3f));
			AttackState[0] = true;
		}
		if (Projectile.ai[0] >= 30 && !AttackState[1])
		{
			SetPlayerDirection(player);
			Projectile.NewProjectile(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<TrueDeathSickle_Blade>(), (int)(player.GetWeaponDamage(item) * 2.9f), item.knockBack, player.whoAmI, 240f, 0, -Main.rand.NextFloat(0.4f, 0.5f));
			AttackState[1] = true;
		}
		if (Projectile.ai[0] >= 48 && !AttackState[2])
		{
			SetPlayerDirection(player);
			var proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<TrueDeathSickle_Blade>(), (int)(player.GetWeaponDamage(item) * 1.4f), item.knockBack, player.whoAmI, 240f, 0, -Main.rand.NextFloat(0f, 0.05f));
			var tDSB = proj.ModProjectile as TrueDeathSickle_Blade;
			tDSB.NoSickleSelf = true;
			AttackState[2] = true;
		}
		if (Projectile.ai[0] >= 54 && !AttackState[3])
		{
			SetPlayerDirection(player);
			var proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<TrueDeathSickle_Blade>(), (int)(player.GetWeaponDamage(item) * 1.1f), item.knockBack, player.whoAmI, 240f, 0.7f, -Main.rand.NextFloat(0.1f, 0.2f));
			var tDSB = proj.ModProjectile as TrueDeathSickle_Blade;
			tDSB.NoSickleSelf = true;
			AttackState[3] = true;
		}
		if (Projectile.ai[0] >= 60 && !AttackState[4])
		{
			SetPlayerDirection(player);
			Projectile.NewProjectile(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<TrueDeathSickle_Blade>(), (int)(player.GetWeaponDamage(item) * 1.1f), item.knockBack, player.whoAmI, 240f, -0.7f, -Main.rand.NextFloat(0.1f, 0.2f));
			AttackState[4] = true;
		}
		if (Projectile.ai[0] >= 95 && !AttackState[5])
		{
			SetPlayerDirection(player);
			var proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<TrueDeathSickle_Blade>(), (int)(player.GetWeaponDamage(item) * 3.4f), item.knockBack, player.whoAmI, 240f, 0, -5);
			AttackState[5] = true;
		}
		if (player.controlUseItem)
		{
			Projectile.timeLeft = player.itemAnimation + 2;
		}
		if (Projectile.ai[0] > 120)
		{
			Reset();
		}
		if (player.HeldItem.type != ModContent.ItemType<Items.Weapons.TrueDeathSickle>())
		{
			Projectile.Kill();
		}
	}

	public void SetPlayerDirection(Player player)
	{
		if (Main.MouseWorld.X > player.Center.X)
		{
			player.direction = 1;
		}
		else
		{
			player.direction = -1;
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}

	public void Reset()
	{
		AttackState = new bool[8];
		Projectile.ai[0] = 0;
	}
}
