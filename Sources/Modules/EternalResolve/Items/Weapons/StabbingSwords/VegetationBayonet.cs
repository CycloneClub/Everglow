using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class VegetationBayonet : StabbingSwordItem
	{
		// TODO:翻译
		// 会造成中毒
		// 使用6秒之后自己也会变得狂野
		// 看到那只蝙蝠了吗，它的蛋白质含量时牛肉的六倍
		internal int specialDelay = 0;

		public override void SetDefaults()
		{
			Item.damage = 9;
			Item.knockBack = 1.08f;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.sellPrice(0, 0, 90, 0);
			Item.shoot = ModContent.ProjectileType<VegetationBayonet_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<VegetationBayonet_Pro_Stab>();
			base.SetDefaults();
		}

		public override void AddRecipes()
		{
			CreateRecipe().
				AddIngredient(ItemID.Stinger, 2).
				AddIngredient(ItemID.Vine, 8).
				AddIngredient(ItemID.JungleSpores, 2).
				AddRecipeGroup(RecipeGroups.Wood, 14).
				AddTile(TileID.Anvils).
				Register();
			base.AddRecipes();
		}

		public override void UpdateInventory(Player player)
		{
			base.UpdateInventory(player);
			if (specialDelay > 360)
			{
				player.AddBuff(BuffID.Rabies, 2);
			}
			if (player.ownedProjectileCounts[Item.shoot] <= 0)
			{
				specialDelay = 0;
			}
		}
	}
}
