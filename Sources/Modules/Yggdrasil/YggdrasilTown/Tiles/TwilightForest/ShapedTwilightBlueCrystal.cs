using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;

public abstract class ShapedTwilightBlueCrystal : ShapeDataTile
{
	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.tileLighted[Type] = true;
		AddMapEntry(new Color(40, 80, 148));
		DustType = ModContent.DustType<TwilightCrystalDust>();
		MinPick = 75;
		MineResist = TwilightBlueCrystalTile.MiningResistance;
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (fail || effectOnly)
		{
			return;
		}

		base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
		if (!noItem && !WorldGen.gen && Main.netMode != NetmodeID.MultiplayerClient)
		{
			Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, TwilightBlueCrystalTile.CreateCrystalDrop());
		}
	}
}
