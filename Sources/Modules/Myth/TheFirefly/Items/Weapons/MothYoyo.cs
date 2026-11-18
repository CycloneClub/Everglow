using Everglow.Commons.Templates.Weapons.Yoyos;
using Everglow.Myth.TheFirefly.Items.Accessories;
using Everglow.Myth.TheFirefly.Projectiles;
using Terraria.Localization;

namespace Everglow.Myth.TheFirefly.Items.Weapons;

public class MothYoyo : YoyoItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetCustomDefaults()
	{
		Item.damage = 25;
		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(0, 0, 23, 0);
		Item.shoot = ModContent.ProjectileType<MothYoyoProjectile>();
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		if (MothEye.LocalOwner != null && MothEye.LocalOwner.TryGetModPlayer(out MothEyePlayer mothEyePlayer))
		{
			if (mothEyePlayer.MothEyeEquipped && ModContent.GetInstance<FireflyBiome>().IsBiomeActive(Main.LocalPlayer) && Main.hardMode)
			{
				tooltips.AddRange(new TooltipLine[]
				{
					new(ModIns.Mod, "MothEyeBonusText", MothEye.BonusHeader),
					new(ModIns.Mod, "MothEyeYoyoBonus", this.GetLocalizedValue("MothEyeBonus")),
				});
			}
		}
	}
}
