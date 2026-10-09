using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class DreamStar : StabbingSwordItem
	{
		// TODO:翻译
		// 每次命中都会给敌人标上1层角星标
		// 5层角星标合成一个星标，获得星标的敌人不会再获得更多角星标
		// 对于一把DreamStar，场上最多存在两个被星标或角星标标记的敌人
		// 两个星标的出现会在两处同时引发220%倍率的星辉爆炸，并摧毁先被标记的敌人的星标
		// 目标敌人若没有星标，且场上已存在被你命中过但是角星标不足5个的敌人，也会在目标敌人和被角星标标记的角色处引发较小的星辉爆炸，威力取决于角星标层数
		// 带有星标的敌人被击杀会掉落星之种，拾取后下次造成伤害必定暴击且伤害*264%
		// 读懂星空需要复杂深邃的理论知识，观赏星空只需要一只眼睛
		public override void SetDefaults()
		{
			Item.damage = 10;
			Item.knockBack = 1.5f;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 2, 27, 15);
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
