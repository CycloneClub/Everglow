using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	// TODO:翻译：拐剑\n第一次命中敌人后如果暴击则造成270%的伤害\n特务使用的武器，非常适合暗杀
	public class CrutchBayonet : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 9;
			Item.knockBack = 1.04f;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 2, 24, 0);
			Item.shoot = ModContent.ProjectileType<CrutchBayonet_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<CrutchBayonet_Pro_Stab>();
			base.SetDefaults();
		}
	}
}
