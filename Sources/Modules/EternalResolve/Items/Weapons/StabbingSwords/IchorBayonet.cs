using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class IchorBayonet : StabbingSwordItem
	{
		// TODO:翻译
		// 灵液刺剑
		public override void SetDefaults()
		{
			Item.damage = 44;
			Item.knockBack = 1.87f;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.sellPrice(0, 15, 27, 86);
			Item.shoot = ModContent.ProjectileType<IchorBayonet_Pro>();
			PowerfulStabDamageFlat = 4f;
			StaminaCost = 0.82f;
			PowerfulStabProj = ModContent.ProjectileType<IchorBayonet_Pro_Stab>();
			base.SetDefaults();
		}

		public override void AddRecipes()
		{
			CreateRecipe().
				AddIngredient(ItemID.Ichor, 14).
				AddIngredient(ModContent.ItemType<BloodGoldBayonet>()).
				AddTile(TileID.MythrilAnvil).
				Register();
			base.AddRecipes();
		}
	}
}
