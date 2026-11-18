using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	public class YoenLeZed : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 14;
			Item.knockBack = 1.08f;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.sellPrice(0, 0, 90, 0);
			Item.shoot = ModContent.ProjectileType<YoenLeZed_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<YoenLeZed_Pro_Stab>();
			base.SetDefaults();
		}
	}
}
