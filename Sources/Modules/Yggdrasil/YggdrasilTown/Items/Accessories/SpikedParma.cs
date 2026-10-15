namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

public class SpikedParma : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public const int ThornBuffDuration = 1800;

	public override void SetDefaults()
	{
		Item.width = 48;
		Item.height = 46;
		Item.accessory = true;
		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(silver: 75);
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// 1. + 3 Defense
		player.statDefense += 4;

		// 2. + 3% All damage
		player.allDamage += 0.04f;

		// 3. Add a 30s thorn buff after hurt
		player.GetModPlayer<SpikedParmaPlayer>().HasSpikedParma = true;
	}
}

public class SpikedParmaPlayer : ModPlayer
{
	public bool HasSpikedParma = false;

	public override void ResetEffects()
	{
		HasSpikedParma = false;
	}

	public override void PostHurt(Player.HurtInfo info)
	{
		if (HasSpikedParma)
		{
			if (!Player.HasBuff(BuffID.Thorns))
			{
				Player.AddBuff(BuffID.Thorns, SpikedParma.ThornBuffDuration);
			}
			if (Player.FindBuffIndex(BuffID.Thorns) >= 0)
			{
				if (Player.buffTime[Player.FindBuffIndex(BuffID.Thorns)] < SpikedParma.ThornBuffDuration)
				{
					Player.AddBuff(BuffID.Thorns, SpikedParma.ThornBuffDuration);
				}
			}
		}
		base.PostHurt(info);
	}
}
