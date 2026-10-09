using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords
{
	// TODO:翻译：椎刺骨\n在与目标相对速度较大的情况下提升伤害，最多不超过500%\n粗糙的接触面造成的毁坏随速度的增加急剧提高
	public class VertebralSpur : StabbingSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 10;
			Item.knockBack = 1.37f;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 1, 16, 0);
			Item.shoot = ModContent.ProjectileType<VertebralSpur_Pro>();
			PowerfulStabDamageFlat = 4f;
			PowerfulStabProj = ModContent.ProjectileType<VertebralSpur_Pro_Stab>();
			base.SetDefaults();
		}
	}
}
