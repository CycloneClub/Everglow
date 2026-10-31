using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.SquamousShell;

public class EyeOfAnabiosis : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public const int UseTime = 30;

	private int WeaponProjectileIndex { get; set; } = 0;

	private Projectile WeaponProjectile => Main.projectile[WeaponProjectileIndex];

	public override void SetDefaults()
	{
		Item.width = 72;
		Item.height = 66;

		Item.DamageType = DamageClass.Magic;
		Item.damage = 30;
		Item.knockBack = 3f;
		Item.crit = 14;
		Item.mana = 9;

		Item.useTime = Item.useAnimation = UseTime;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.UseSound = SoundID.Item117;
		Item.autoReuse = true;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.channel = true;

		Item.value = Item.buyPrice(gold: 1, silver: 3);
		Item.rare = ItemRarityID.Green;

		Item.shoot = ModContent.ProjectileType<EyeOfAnabiosis_Weapon>();
		Item.shootSpeed = 7;
	}

	public override void HoldItem(Player player)
	{
		if (Main.myPlayer == player.whoAmI)
		{
			player.ListenMouseWorld();

			bool hasGeneratedWeaponProj = WeaponProjectileIndex > -1 && WeaponProjectile is not null && WeaponProjectile.type == ModContent.ProjectileType<EyeOfAnabiosis_Weapon>() && WeaponProjectile.owner == player.whoAmI && WeaponProjectile.timeLeft > 0;

			Vector2 position = player.Center;
			if (player.HeldItem == Item && hasGeneratedWeaponProj is false)
			{
				WeaponProjectileIndex = Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, Vector2.Zero, ModContent.ProjectileType<EyeOfAnabiosis_Weapon>(), 0, 0, player.whoAmI, Main.MouseWorld.X, Main.MouseWorld.Y);
			}
		}
	}

	public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;
}
