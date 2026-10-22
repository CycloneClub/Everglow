using Everglow.Yggdrasil.YggdrasilTown.Dusts;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class YggdrasilAmber : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = false;
		Main.tileBlendAll[Type] = false;
		Main.tileBlockLight[Type] = false;
		Main.tileShine2[Type] = false;
		DustType = ModContent.DustType<YggdrasilAmberDust>();
		MinPick = 0;
		RegisterItemDrop(ModContent.ItemType<Items.Materials.YggdrasilAmber>());
		HitSound = SoundID.Shimmer1;
		AddMapEntry(new Color(198, 124, 19, 163));
	}

	public override bool CanExplode(int i, int j)
	{
		return true;
	}
}
