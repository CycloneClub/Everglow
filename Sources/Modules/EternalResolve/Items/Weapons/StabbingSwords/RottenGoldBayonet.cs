using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	// TODO:翻译
	// i:命中后削弱目标1点防御力
	// ii:延时一秒在命中处生成三道200%倍率的魔金剑光
	// 对于1个魔金刺剑,i效果和ii效果一秒最多触发一次,但是总数没有上限
	// 它会腐化你的骨髓
	public class RottenGoldBayonet : StabbingSwordItem
	{
		internal int specialDelay = 0;

		public override void SetDefaults()
		{
			Item.damage = 10;
			Item.knockBack = 1.52f;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.shoot = ModContent.ProjectileType<RottenGoldBayonet_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<RottenGoldBayonet_Pro_Stab>();
			base.SetDefaults();
		}

		public override void AddRecipes()
		{
			CreateRecipe().
				AddIngredient(ItemID.DemoniteBar, 10).
				AddIngredient(ItemID.ShadowScale, 5).
				AddTile(TileID.Anvils).
				Register();
			base.AddRecipes();
		}

		public override bool AltFunctionUse(Player player)
		{
			return NPC.downedBoss1 && base.AltFunctionUse(player);
		}

		public override void UpdateInventory(Player player)
		{
			if (specialDelay > 0)
			{
				specialDelay--;
			}
			base.UpdateInventory(player);
		}
	}
}
