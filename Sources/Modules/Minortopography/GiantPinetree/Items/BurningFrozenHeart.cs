using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Everglow.Minortopography.GiantPinetree.Items;

public class BurningFrozenHeart : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public static readonly int ResourceBoost = 100;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ResourceBoost);

	public override void SetStaticDefaults()
	{
		Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 7));
	}

	public override void SetDefaults()
	{
		Item.width = 26;
		Item.height = 52;
		Item.value = 4090;
		Item.accessory = true;
		Item.rare = ItemRarityID.Blue;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		Lighting.AddLight(player.Center, 0, 0.4f, 0.8f);
		player.buffImmune[BuffID.Frostburn] = true;
		player.buffImmune[BuffID.Frostburn2] = true;
		player.buffImmune[BuffID.Frozen] = true;
		player.buffImmune[BuffID.Chilled] = true;
	}

	public override void Update(WorldItem item, ref float gravity, ref float maxFallSpeed)
	{
		Lighting.AddLight(item.Center, 0, 0.6f, 1.2f);
		base.Update(item, ref gravity, ref maxFallSpeed);
	}
}
