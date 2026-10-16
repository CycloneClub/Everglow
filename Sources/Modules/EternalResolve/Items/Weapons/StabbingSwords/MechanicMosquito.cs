using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class MechanicMosquito : StabbingSwordItem
	{
		// TODO:翻译：械蚊\n召唤三个会吸血的机械蚊子协助攻击,蚊子每次攻击会获得1点吸血，最多累积3点。使用结束之后，治疗吸血累积的点数。
		public override void SetDefaults()
		{
			Item.damage = 16;
			Item.knockBack = 1.14f;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.sellPrice(0, 0, 50, 0);
			Item.shoot = ModContent.ProjectileType<MechanicMosquito_Pro>();
			PowerfulStabDamageFlat = 4f;
			StaminaCost = 0.65f; // 机械剑省力，很合理（）
			PowerfulStabProj = ModContent.ProjectileType<MechanicMosquito_Pro_Stab>();
			base.SetDefaults();
		}
	}
}
