using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class IronStabbingSword : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 11;
			Item.knockBack = 1.4f;
			Item.rare = ItemRarityID.White;
			Item.value = 1800;
			Item.shoot = ModContent.ProjectileType<IronStabbingSword_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<IronStabbingSword_Pro_Stab>();
			base.SetDefaults();
		}

		public override void AddRecipes()
		{
			CreateRecipe().
				AddIngredient(ItemID.IronBar, 10).
				AddTile(TileID.Anvils).
				Register();
			base.AddRecipes();
		}
	}
}
