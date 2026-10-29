using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.PermanentBoosters;

public class MagicOrganExtract : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.PermanentBoosters;

	public const int MaxUse = 6;
	public const int ManaPerUse = 5;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ManaPerUse, MaxUse);

	public override void SetDefaults()
	{
		Item.width = 26;
		Item.height = 22;
		Item.useStyle = ItemUseStyleID.DrinkLiquid;
		Item.useTime = Item.useAnimation = 30;
		Item.UseSound = SoundID.Item4;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
		Item.rare = ItemRarityID.Blue;
	}

	public override bool CanUseItem(Player player) => player.GetModPlayer<YggdrasilPlayer>().ConsumedMagicOrganExtract < MaxUse;

	public override bool? UseItem(Player player)
	{
		if (player.whoAmI == Main.myPlayer)
		{
			return player.GetModPlayer<YggdrasilPlayer>().UseMagicOrganExtract();
		}
		return true;
	}

	public override void Update(ref float gravity, ref float maxFallSpeed)
	{
		Lighting.AddLight(Item.Center, new Vector3(0.43f, 0.06f, 0.13f));
		base.Update(ref gravity, ref maxFallSpeed);
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<RodWing>(6);
		recipe.AddIngredient<CaterpillarJuice>(6);
		recipe.AddIngredient<UnstablePollen>(6);
		recipe.Register();
	}
}
