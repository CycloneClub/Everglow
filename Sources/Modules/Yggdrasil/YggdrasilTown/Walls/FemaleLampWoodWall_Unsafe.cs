namespace Everglow.Yggdrasil.YggdrasilTown.Walls;

public sealed class FemaleLampWoodWall_Unsafe : FemaleLampWoodWall
{
	public override string Texture => ModAsset.FemaleLampWoodWall_Mod;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.wallHouse[Type] = false;
		// TODO: 该物品无贴图
		// ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<Items.Placeables.Walls.FemaleLampWoodWall_Item>()] = ModContent.ItemType<Items.Placeables.Walls.FemaleLampWoodWall_Unsafe_Item>();
	}
}
