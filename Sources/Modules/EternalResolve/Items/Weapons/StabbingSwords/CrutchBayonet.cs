using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class CrutchBayonet : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 22;
			Item.knockBack = 1.04f;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(0, 10, 0, 0);
			Item.shoot = ModContent.ProjectileType<CrutchBayonet_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<CrutchBayonet_Pro_Stab>();
			base.SetDefaults();
		}
	}
}
