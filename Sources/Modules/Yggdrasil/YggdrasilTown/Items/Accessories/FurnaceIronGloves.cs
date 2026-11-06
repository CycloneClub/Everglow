using Everglow.Commons.Mechanics.ElementalDebuff.Debuffs;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

public class FurnaceIronGloves : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FurnaceIronGlovesPlayer.ElementDebuffBuildUpRate * 100f);

	public override void SetDefaults()
	{
		Item.width = 36;
		Item.height = 40;

		Item.accessory = true;

		Item.rare = ItemRarityID.Orange;
		Item.value = Item.buyPrice(silver: 85);
	}

	public class FurnaceIronGlovesPlayer : ModPlayer
	{
		public const float ElementDebuffBuildUpRate = 2f;

		public bool FurnaceIronGlovesEnable { get; set; } = false;

		public override void ResetEffects()
		{
			FurnaceIronGlovesEnable = false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (FurnaceIronGlovesEnable
				&& hit.DamageType == DamageClass.Magic)
			{
				target.AddElementalDebuffBuildUp(Player, BurnDebuff.ID, (int)(damageDone * ElementDebuffBuildUpRate));
			}
		}
	}
}
