using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class RedRibbonBayonet : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 26;
			Item.knockBack = 2.7f;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(0, 2, 70, 0);
			Item.shoot = ModContent.ProjectileType<RedRibbonBayonet_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<RedRibbonBayonet_Pro_Stab>();
			base.SetDefaults();
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.MeteoriteBar, 15)
				.AddIngredient(ItemID.Ruby, 15)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}
