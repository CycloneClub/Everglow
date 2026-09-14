using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Melee;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons.DevilHeart;

public class DevilHeartBayonet : StabbingSwordItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.damage = 22;
		Item.knockBack = 1.5f;
		Item.rare = ItemRarityID.Green;
		Item.value = Item.sellPrice(0, 2, 0, 0);
		Item.shoot = ModContent.ProjectileType<DevilHeartBayonet_proj>();
		PowerfulStabDamageFlat = 4f;
		PowerfulStabProj = ModContent.ProjectileType<DevilHeartBayonet_proj_stab>();

		base.SetDefaults();
	}

	public override bool AltFunctionUse(Player player)
	{
		if (player.statLife <= 15)
		{
			return false;
		}

		return base.AltFunctionUse(player);
	}
}
