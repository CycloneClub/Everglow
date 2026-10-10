using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class TungstenStabbingSword : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 5;
			Item.knockBack = 1.74f;
			Item.rare = ItemRarityID.White;
			Item.value = Item.sellPrice(0, 0, 48, 0);
			Item.shoot = ModContent.ProjectileType<TungstenStabbingSword_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<TungstenStabbingSword_Pro_Stab>();
			base.SetDefaults();
		}

		public override void AddRecipes()
		{
			CreateRecipe().
				AddIngredient(ItemID.TungstenBar, 10).
				AddTile(TileID.Anvils).
				Register();
			base.AddRecipes();
		}
	}
}
