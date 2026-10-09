using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class EternalNight : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 12;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.sellPrice(0, 1, 88, 48);
			Item.shoot = ModContent.ProjectileType<EternalNight_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<EternalNight_Pro_Stab>();
			base.SetDefaults();
		}
		// TODO: bayonet counterpart of murasama should be added to use in this recipe
		// public override void AddRecipes()
		// {
		// CreateRecipe()
		// .AddIngredient(ModContent.ItemType<VegetationBayonet>()) // or Blossom Thorn
		// .AddIngredient(ModContent.ItemType<RottenGoldBayonet>())
		// .AddIngredient(ModContent.ItemType<PrisonFireBayonet>())
		// .AddTile(TileID.SkyMill)
		// .Register();
		// }
	}
}
