using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class SilverStabbingSword : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 13;
			Item.knockBack = 1.54f;
			Item.rare = ItemRarityID.White;
			Item.value = 4500;
			Item.shoot = ModContent.ProjectileType<SilverStabbingSword_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<SilverStabbingSword_Pro_Stab>();
			base.SetDefaults();
		}

		public override void AddRecipes()
		{
			CreateRecipe().
				AddIngredient(ItemID.SilverBar, 10).
				AddTile(TileID.Anvils).
				Register();
			base.AddRecipes();
		}
	}
}
