namespace Everglow.Yggdrasil.YggdrasilTown.Walls;

public sealed class DarkForestSoilWall_Unsafe : DarkForestSoilWall
{
	public override string Texture => ModAsset.DarkForestSoilWall_Mod;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.wallHouse[Type] = false;
		// TODO: 该物品无贴图
		// ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<Items.Placeables.Walls.DarkForestSoilWall_Item>()] = ModContent.ItemType<Items.Placeables.Walls.DarkForestSoilWall_Unsafe_Item>();
	}
}
