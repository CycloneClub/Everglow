using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class DreamStar : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 18;
			Item.knockBack = 1.5f;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(0, 5, 0, 0);
			Item.shoot = ModContent.ProjectileType<DreamStar_Pro>();
			PowerfulStabDamageFlat = 2.2f;
			PowerfulStabProj = ModContent.ProjectileType<DreamStar_Pro_Stab>();
			StaminaCost -= 0.1f;
			base.SetDefaults();
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.FallenStar, 18)
				.AddIngredient(ItemID.SunplateBlock, 45)
				.AddTile(TileID.SkyMill)
				.Register();
		}
	}
}
