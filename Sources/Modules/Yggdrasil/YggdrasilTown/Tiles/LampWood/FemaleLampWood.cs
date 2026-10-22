using Everglow.Yggdrasil.YggdrasilTown.Dusts;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;

public class FemaleLampWood : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileBlockLight[Type] = true;
		DustType = ModContent.DustType<LampWood_Dust>();
		HitSound = SoundID.Dig;

		AddMapEntry(new Color(69, 71, 71));
	}
}
