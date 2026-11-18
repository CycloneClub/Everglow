using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class BlossomThorn : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 14;
			Item.knockBack = 0.97f;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(0, 2, 70, 0);
			Item.shoot = ModContent.ProjectileType<BlossomThorn_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<BlossomThorn_Pro_Stab>();
			base.SetDefaults();
		}

		public override void AddRecipes()
		{
			CreateRecipe().
				AddIngredient(ItemID.JungleRose).
				AddIngredient(ItemID.Stinger, 4).
				AddIngredient(ItemID.Vine, 10).
				AddIngredient(ItemID.JungleSpores, 4).
				AddTile(TileID.Anvils).
				Register();
			base.AddRecipes();
		}
	}
}
