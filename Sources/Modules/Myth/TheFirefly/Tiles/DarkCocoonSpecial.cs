using Everglow.Myth.TheFirefly.NPCs.Bosses;

namespace Everglow.Myth.TheFirefly.Tiles;

public class DarkCocoonSpecial : ModTile// 用来生成魔茧
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileMerge[Type][ModContent.TileType<DarkCocoon>()] = true;
		MinPick = 17500;
		DustType = DustID.SpookyWood;
		AddMapEntry(new Color(17, 16, 17));
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public override bool CanKillTile(int i, int j, ref bool blockDamaged)
	{
		return false;
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		if (NPC.CountNPCS(ModContent.NPCType<EvilPack>()) < 1)
		{
			NPC.NewNPC(null, i * 16, j * 16 + 244, ModContent.NPCType<EvilPack>());
		}
	}
}
