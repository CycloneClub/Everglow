using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class CopperBayonet : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			StaminaCost += 0.3f;
			Item.damage = 3;
			Item.knockBack = 1f;
			Item.rare = ItemRarityID.White;
			Item.value = Item.sellPrice(0, 0, 12, 0);
			Item.shoot = ModContent.ProjectileType<CopperStabbingSword_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<CopperStabbingSword_Pro_Stab>();
			base.SetDefaults();
		}

		public override void AddRecipes()
		{
			CreateRecipe().
				AddIngredient(ItemID.CopperBar, 7).
				AddTile(TileID.Anvils).
				Register();
			base.AddRecipes();
		}
	}
}
