using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs.Arena;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class ArenaChallengeSettingTile : ModTile, ISceneTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileNoAttach[Type] = false;
		Main.tileWaterDeath[Type] = false;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Origin = new(0, 0);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.CoordinateHeights = new int[2];
		Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.Origin = new Point16(0, 0);

		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<UnionMarblePost_Dust_Khaki>();
		MinPick = int.MaxValue;
		AddMapEntry(new Color(96, 96, 96));
	}

	public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;

	public override bool CanExplode(int i, int j) => false;

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) => base.PreDraw(i, j, spriteBatch);

	public void AddScene(int i, int j)
	{
		Tile tile = TileUtils.SafeGetTile(i, j);
		if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
		{
			NPCBossTagsSystem nCTS = new NPCBossTagsSystem { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
			Ins.VFXManager.Add(nCTS);
		}
	}
}
