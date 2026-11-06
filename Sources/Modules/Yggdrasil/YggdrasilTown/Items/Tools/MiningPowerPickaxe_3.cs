using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools;

public class MiningPowerPickaxe_3 : MiningPowerPickaxe
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Item.pick = 90;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient<MiningPowerPickaxe_2>()
			.AddIngredient<TwilightCrystal>(12)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
