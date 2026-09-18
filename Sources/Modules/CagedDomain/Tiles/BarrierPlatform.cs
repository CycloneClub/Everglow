using Terraria.ObjectData;

namespace Everglow.CagedDomain.Tiles;

public class BarrierPlatform : ModTile
{
	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileLighted[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileSolidTop[Type] = true;
		Main.tileSolid[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileTable[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileID.Sets.Platforms[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;

		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
		AddMapEntry(Color.Transparent);
		AdjTiles = new int[] { TileID.Platforms };

		// Placement
		TileObjectData.newTile.CoordinateHeights = new[] { 16 };
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.CoordinatePadding = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.StyleMultiplier = 27;
		TileObjectData.newTile.StyleWrapLimit = 27;
		TileObjectData.newTile.UsesCustomCanPlace = false;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.addTile(Type);
		MinPick = int.MaxValue;
	}

	public override void PlaceInWorld(int i, int j, Item item)
	{
		var tile = Main.tile[i, j];
		tile.IsTileInvisible = true;
	}

	public override void PostSetDefaults() => Main.tileNoSunLight[Type] = false;

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 0;

	public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;

	public override bool CanExplode(int i, int j) => false;

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		noBreak = true;
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (BarrierBlock.PlayerHeldBarrierItem(Main.LocalPlayer))
		{
			var tile = Main.tile[i, j];
			tile.IsTileInvisible = true;
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			var frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
			spriteBatch.Draw(ModAsset.BarrierPlatform_Visable.Value, new Vector2(i, j) * 16 - Main.screenPosition + zero, frame, Color.White, 0, new Vector2(0), 1f, SpriteEffects.None, 0);
		}
		return base.PreDraw(i, j, spriteBatch);
	}
}
