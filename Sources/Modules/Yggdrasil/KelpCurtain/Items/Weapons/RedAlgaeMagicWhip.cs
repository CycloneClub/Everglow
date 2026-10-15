using Everglow.Commons.Templates.Weapons.Whips;
using Everglow.Yggdrasil.KelpCurtain.Items.Materials;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class RedAlgaeMagicWhip : WhipItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.SummonWeapons;

	public override void SetDef()
	{
		Item.width = 40;
		Item.height = 32;
		Item.shoot = ModContent.ProjectileType<RedAlgaeMagicWhip_Proj>();
		Item.shootSpeed = 5.04f;
		Item.value = 35000;
		Item.rare = ItemRarityID.Orange;
		Item.damage = 59;
		Item.useAnimation = 30;
		Item.useTime = 30;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
		.AddIngredient(ModContent.ItemType<JadeLakeRedAlgae_Item>(), 15)
		.AddIngredient(ModContent.ItemType<CrimsonMoonSap>(), 1)
		.AddTile(TileID.WorkBenches)
		.Register();
	}
}
