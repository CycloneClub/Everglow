using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using ReLogic.Content;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood.Furniture;

public class LampWoodCandelabra : ModTile
{
	private Asset<Texture2D> flameTexture;

	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileTable[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

		DustType = ModContent.DustType<LampWood_Dust>();
		AdjTiles = new int[] { TileID.Candelabras };

		// Placement
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2); // this style already takes care of direction for us
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
		TileObjectData.addTile(Type);
		if (!Main.dedServ)
		{
			flameTexture = ModAsset.LampWoodCandelabra_Flame;
		}

		LocalizedText name = CreateMapEntryName();
		AddMapEntry(new Color(69, 36, 78), name);
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		r = 1f;
		g = 0.7f;
		b = 0f;
	}

	public override void HitWire(int i, int j)
	{
		FurnitureUtils.LightHitwire(i, j, Type, 2, 2);
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}

		var color = new Color(55, 5, 255, 0);
		int width = 20;
		int height = 20;
		var tile = Main.tile[i, j];
		int frameX = tile.TileFrameX;
		int frameY = tile.TileFrameY;

		spriteBatch.Draw(flameTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f, j * 16 - (int)Main.screenPosition.Y + 3) + zero, new Rectangle(frameX, frameY, width, height), color, 0f, default, 1f, SpriteEffects.None, 0f);
	}

	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
	{
		offsetY = 2;
	}
}
