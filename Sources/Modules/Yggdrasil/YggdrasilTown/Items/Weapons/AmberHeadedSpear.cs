using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

public class AmberHeadedSpear : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetStaticDefaults()
	{
		ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
		ItemID.Sets.Spears[Item.type] = true;
		Item.SetNameOverride("Amber-headed Spear");
	}

	public override void SetDefaults()
	{
		Item.DamageType = DamageClass.Melee;
		Item.damage = 18;
		Item.knockBack = 6f;
		Item.noUseGraphic = true;
		Item.noMelee = true;

		Item.useStyle = ItemUseStyleID.Shoot;
		Item.useTime = 31;
		Item.useAnimation = 31;
		Item.UseSound = SoundID.Item71;
		Item.autoReuse = true;

		Item.shootSpeed = 3.7f;
		Item.shoot = ModContent.ProjectileType<Projectiles.Melee.AmberHeadedSpear>();

		Item.rare = ItemRarityID.Blue;
		Item.value = Item.buyPrice(silver: 20);
	}

	public override bool CanUseItem(Player player)
	{
		return player.ownedProjectileCounts[Item.shoot] < 1;
	}

	public override bool? UseItem(Player player)
	{
		if (!Main.dedServ && Item.UseSound.HasValue)
		{
			SoundEngine.PlaySound(Item.UseSound.Value, player.Center);
		}

		return null;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Amber, 10)
			.AddIngredient(ItemID.StoneBlock, 30)
			.AddIngredient(ItemID.IronBar, 15)
			.Register();
		CreateRecipe()
			.AddIngredient(ItemID.Amber, 10)
			.AddIngredient(ItemID.StoneBlock, 30)
			.AddIngredient(ItemID.LeadBar, 15)
			.Register();
	}
}
