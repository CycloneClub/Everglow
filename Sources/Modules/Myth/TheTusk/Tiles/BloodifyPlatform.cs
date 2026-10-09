using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Myth.TheTusk.Tiles;

public class BloodifyPlatform : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileLighted[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileSolidTop[Type] = true;
		Main.tileSolid[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileTable[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileID.Sets.Platforms[Type] = true;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
		};
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.CoordinatePadding = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.StyleMultiplier = 27;
		TileObjectData.newTile.StyleWrapLimit = 27;
		TileObjectData.newTile.UsesCustomCanPlace = false;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.addTile(Type);
		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
		var modTranslation = CreateMapEntryName();
		AddMapEntry(new Color(168, 11, 0), modTranslation);
		HitSound = SoundID.Grass;
	}
	public override bool CreateDust(int i, int j, ref int type)
	{
		Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.Blood, 0f, 0f, 1, Color.White, 1f);
		return false;
	}
	public override void PostSetDefaults()
	{
		Main.tileNoSunLight[Type] = false;
	}
	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (Main.rand.NextBool(30))
			Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, DustID.Blood, 0f, 0f, 1, Color.White, 1f);
		if (Main.rand.NextBool(300))
			WorldGen.KillTile(i, j);
		return base.PreDraw(i, j, spriteBatch);
	}
	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = fail ? 1 : 3;
	}
}
