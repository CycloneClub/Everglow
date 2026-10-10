using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class TinStabbingSword : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 3;
			Item.knockBack = 1.24f;
			Item.rare = ItemRarityID.White;
			Item.value = Item.sellPrice(0, 0, 16, 0);
			Item.shoot = ModContent.ProjectileType<TinStabbingSword_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<TinStabbingSword_Pro_Stab>();
			base.SetDefaults();
		}

		public override void AddRecipes()
		{
			CreateRecipe().
				AddIngredient(ItemID.TinBar, 7).
				AddTile(TileID.Anvils).
				Register();
			base.AddRecipes();
		}
	}
}
