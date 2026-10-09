using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class LeadStabbingSword : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 4;
			Item.knockBack = 1.45f;
			Item.rare = ItemRarityID.White;
			Item.value = Item.sellPrice(0, 0, 24, 0);
			Item.shoot = ModContent.ProjectileType<LeadStabbingSword_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<LeadStabbingSword_Pro_Stab>();
			base.SetDefaults();
		}

		public override void AddRecipes()
		{
			CreateRecipe().
				AddIngredient(ItemID.LeadBar, 10).
				AddTile(TileID.Anvils).
				Register();
			base.AddRecipes();
		}
	}
}
