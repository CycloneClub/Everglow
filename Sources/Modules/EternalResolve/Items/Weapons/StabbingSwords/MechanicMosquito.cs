using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class MechanicMosquito : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 16;
			Item.knockBack = 1.14f;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(0, 2, 50, 0);
			Item.shoot = ModContent.ProjectileType<MechanicMosquito_Pro>();
			PowerfulStabDamageFlat = 4f;
			StaminaCost = 0.65f; // 机械剑省力，很合理（）
			PowerfulStabProj = ModContent.ProjectileType<MechanicMosquito_Pro_Stab>();
			base.SetDefaults();
		}
	}
}
