using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools;

public class MiningPowerPickaxe_2 : MiningPowerPickaxe
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Item.pick = 75;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient<MiningPowerPickaxe>()
			.AddIngredient<YggdrasilAmber>(12)
			.AddIngredient<CrystalNucleusOfJellyKing>(4)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
