using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	// TODO:翻译：红梅落\n产生方向随机的二级刺锋\n兼具美观与锋芒
	public class BlossomThorn : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 9;
			Item.knockBack = 0.97f;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.sellPrice(0, 2, 38, 0);
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
