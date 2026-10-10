using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class PrisonFireBayonet : StabbingSwordItem
	{
		// TODO:翻译
		// 点燃敌人！
		// 在水里无法点燃,但是在熔岩里造成两倍伤害
		// 切勿伤敌一万自损八千
		public override void SetDefaults()
		{
			Item.damage = 12;
			Item.knockBack = 2.34f;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.sellPrice(0, 1, 12, 0);
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
