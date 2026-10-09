using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class CurseFlameStabbingSword : StabbingSwordItem
	{
		// TODO:翻译
		// 用诅咒火焰点燃敌人！
		public override void SetDefaults()
		{
			Item.damage = 40;
			Item.knockBack = 1.79f;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.sellPrice(0, 14, 56, 75);
			Item.shoot = ModContent.ProjectileType<CurseFlameStabbingSword_Pro>();
			PowerfulStabDamageFlat = 4f;
			StaminaCost = 0.82f;
			PowerfulStabProj = ModContent.ProjectileType<CurseFlameStabbingSword_Pro_Stab>();
			base.SetDefaults();
		}
		public override void AddRecipes()
		{
			CreateRecipe().
				AddIngredient(ItemID.CursedFlame, 14).
				AddIngredient(ModContent.ItemType<RottenGoldBayonet>()).
				AddTile(TileID.Anvils).
				Register();
			base.AddRecipes();
		}
	}
}
