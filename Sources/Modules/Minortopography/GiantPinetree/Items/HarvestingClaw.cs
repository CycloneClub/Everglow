using Terraria.DataStructures;
using static Everglow.Commons.TileHelper.ShakeTreeTweak;

namespace Everglow.Minortopography.GiantPinetree.Items;

// TODO:翻译
// 摇树掉落水果
public class HarvestingClaw : FruitPickerTool
{
	public override void SetDefaults()
	{
		Item.damage = 6;
		Item.DamageType = DamageClass.Melee;
		Item.width = 46;
		Item.height = 46;
		Item.useTime = 15;
		Item.useAnimation = 15;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.knockBack = 3;
		Item.value = 1400;
		Item.rare = ItemRarityID.Green;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;
		Item.axe = 1;
	}
}
