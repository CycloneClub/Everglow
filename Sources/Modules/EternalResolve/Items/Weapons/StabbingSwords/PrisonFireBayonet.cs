using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class PrisonFireBayonet : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 26;
			Item.knockBack = 2.34f;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(0, 2, 70, 0);
			Item.shoot = ModContent.ProjectileType<PrisonFireBayonet_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<PrisonFireBayonet_Pro_Stab>();
			base.SetDefaults();
		}

		public override void AddRecipes()
		{
			CreateRecipe().
				AddIngredient(ItemID.HellstoneBar, 16).
				AddTile(TileID.Anvils).
				Register();
			base.AddRecipes();
		}
	}
}
