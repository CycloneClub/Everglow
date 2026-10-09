using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class PlatinumStabbingSword : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 7;
			Item.knockBack = 1.91f;
			Item.rare = ItemRarityID.White;
			Item.value = Item.sellPrice(0, 0, 75, 0);
			Item.shoot = ModContent.ProjectileType<PlatinumStabbingSword_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<PlatinumStabbingSword_Pro_Stab>();
			base.SetDefaults();
		}

		public override void AddRecipes()
		{
			CreateRecipe().
				AddIngredient(ItemID.PlatinumBar, 10).
				AddTile(TileID.Anvils).
				Register();
			base.AddRecipes();
		}
	}
}
