using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Minortopography.GiantPinetree.Projectiles;

namespace Everglow.Minortopography.GiantPinetree.Items
{
	public class PineStab : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			StaminaCost = 0.72f;
			Item.damage = 12;
			Item.knockBack = 1f;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 2, 45, 0);
			Item.shoot = ModContent.ProjectileType<PineStab_Pro>();
			PowerfulStabDamageFlat = 1.81f;
			PowerfulStabProj = ModContent.ProjectileType<PineStab_Pro_Stab>();
			base.SetDefaults();
		}
	}
}
