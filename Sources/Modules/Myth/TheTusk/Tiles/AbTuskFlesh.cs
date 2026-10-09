using Everglow.Myth.TheTusk.NPCs.BloodTusk;
using Terraria.DataStructures;

namespace Everglow.Myth.TheTusk.Tiles;

public class AbTuskFlesh : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMerge[Type][ModContent.TileType<TuskFlesh>()] = true;
		Main.tileMerge[ModContent.TileType<TuskFlesh>()][Type] = true;
		Main.tileBlockLight[Type] = true;
		DustType = DustID.Blood;
		AddMapEntry(new Color(219, 41, 47));
		HitSound = SoundID.Grass;
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
		if (NPC.CountNPCS(ModContent.NPCType<BloodTusk>()) <= 0)
		{
			NPC tusk = NPC.NewNPCDirect(default, i * 16, j * 16 - 1000, ModContent.NPCType<BloodTusk>());
			tusk.active = true;
		}
	}

	public override void RandomUpdate(int i, int j)
	{
		if (!Main.tile[i, j - 1].HasTile)
		{
			if (Main.rand.NextBool(8))
			{
				switch (Main.rand.Next(1, 4))
				{
					case 1:
						WorldGen.PlaceTile(i, j - 2, (ushort)ModContent.TileType<StoneTuskSmall>());
						short numz = (short)(Main.rand.Next(0, 12) * 24);
						Main.tile[i, j - 2].TileFrameX = numz;
						Main.tile[i, j - 1].TileFrameX = numz;
						break;
					case 2:
						WorldGen.PlaceTile(i, j - 2, (ushort)ModContent.TileType<StoneTuskSmall>());
						short num = (short)(Main.rand.Next(0, 12) * 24);
						Main.tile[i, j - 2].TileFrameX = num;
						Main.tile[i, j - 1].TileFrameX = num;
						break;
					case 3:
						WorldGen.PlaceTile(i, j - 3, (ushort)ModContent.TileType<StoneTusk>());
						short num1 = (short)(Main.rand.Next(0, 12) * 36);
						Main.tile[i, j - 3].TileFrameX = num1;
						Main.tile[i, j - 2].TileFrameX = num1;
						Main.tile[i, j - 1].TileFrameX = num1;
						break;
				}
			}
		}
	}
}
