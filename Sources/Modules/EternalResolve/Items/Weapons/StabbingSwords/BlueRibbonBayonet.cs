using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class BlueRibbonBayonet : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 26;
			Item.knockBack = 2.7f;
			Item.rare = ItemRarityID.Blue;
			Item.value = 27000;
			Item.shoot = ModContent.ProjectileType<BlueRibbonBayonet_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<BlueRibbonBayonet_Pro_Stab>();
			base.SetDefaults();
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.MeteoriteBar, 15)
				.AddIngredient(ItemID.Sapphire, 15)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}
