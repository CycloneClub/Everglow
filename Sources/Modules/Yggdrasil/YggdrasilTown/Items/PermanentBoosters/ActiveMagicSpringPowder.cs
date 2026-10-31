using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.PermanentBoosters;

public class ActiveMagicSpringPowder : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.PermanentBoosters;

	public const int MaxUse = 4;
	public const int ManaPerUse = 5;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ManaPerUse, MaxUse);

	public override void SetDefaults()
	{
		Item.width = 34;
		Item.height = 24;
		Item.useStyle = ItemUseStyleID.DrinkLiquid;
		Item.useTime = Item.useAnimation = 30;
		Item.UseSound = SoundID.Item4;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
		Item.rare = ItemRarityID.Green;
	}

	public override bool CanUseItem(Player player) => player.GetModPlayer<YggdrasilPlayer>().ConsumedActiveMagicSpringPowder < MaxUse;

	public override void Update(ref float gravity, ref float maxFallSpeed)
	{
		Lighting.AddLight(Item.Center, new Vector3(0f, 0.45f, 0.6f));
	}

	public override bool? UseItem(Player player)
	{
		if (player.whoAmI == Main.myPlayer)
		{
			return player.GetModPlayer<YggdrasilPlayer>().UseActiveMagicSpringPowder();
		}
		return true;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<ManaSpringOre>(6);
		recipe.AddIngredient<AuburnRodSkeleton>();
		recipe.AddIngredient<LightBulbOvule>();
		recipe.AddTile(TileID.Bottles);
		recipe.Register();
	}
}
