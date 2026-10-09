using Everglow.Myth.TheFirefly.Dusts;
using ReLogic.Content;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Myth.TheFirefly.Tiles.Furnitures;

public class GlowWoodCandle : ModTile
{
	private Asset<Texture2D> flameTexture;

	public override void SetStaticDefaults()
	{
		Main.tileTable[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileID.Sets.HasOutlines[Type] = true;
		TileID.Sets.InteractibleByNPCs[Type] = true;
		TileID.Sets.IsValidSpawnPoint[Type] = true;

		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

		DustType = ModContent.DustType<BlueGlow>();
		AdjTiles = new int[] { TileID.Candles };
		TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
		TileObjectData.addTile(Type);


		if (!Main.dedServ)
		{
			if (!Main.dedServ)
			{
				flameTexture = ModContent.Request<Texture2D>("Everglow/Myth/TheFirefly/Tiles/Furnitures/GlowWoodCandle_Flame");
			}
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
		r = 0.1f;
		g = 0.9f;
		b = 1f;
	}

	public override void HitWire(int i, int j)
	{
		FurnitureUtils.LightHitwire(i, j, Type, 1, 1);
	}
	public override void NearbyEffects(int i, int j, bool closer)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX < 54)
		{
			int frequency = 20;
			if (!Main.gamePaused && Main.instance.IsActive && (!Lighting.UpdateEveryFrame || Main.rand.NextBool(4)) && Main.rand.NextBool(frequency))
			{
				Rectangle dustBox = Utils.CenteredRectangle(new Vector2(i * 16 + 8, j * 16 + 4), new Vector2(16, 16));
				int numForDust = Dust.NewDust(dustBox.TopLeft(), dustBox.Width, dustBox.Height, ModContent.DustType<Dusts.BlueToPurpleSpark>(), 0f, 0f, 254, default, Main.rand.NextFloat(0.95f, 1.75f));
				Dust obj = Main.dust[numForDust];
				obj.velocity *= 0.4f;
				Main.dust[numForDust].velocity.Y -= 0.4f;
			}
		}
	}
	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}

		ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)i); // Don't remove any casts.
		var color = new Color(55, 5, 255, 0);
		int width = 20;
		int height = 20;
		var tile = Main.tile[i, j];
		int frameX = tile.TileFrameX;
		int frameY = tile.TileFrameY;
		color.A = 40;
		for (int k = 0; k < 7; k++)
		{
			float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
			float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;

			spriteBatch.Draw(flameTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + xx, j * 16 - (int)Main.screenPosition.Y + yy + k * 0.2f) + zero, new Rectangle(frameX, frameY, width, height), color, 0f, default, 1f, SpriteEffects.None, 0f);
		}
		color = new Color(22, 22, 22, 0);
		for (int k = 0; k < 7; k++)
		{
			float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
			float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;

			spriteBatch.Draw(flameTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + xx, j * 16 - (int)Main.screenPosition.Y + yy + 3 - k * 0.3f) + zero, new Rectangle(frameX, frameY, width, height), color, 0f, default, 1f, SpriteEffects.None, 0f);
		}
	}

	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
	{
		offsetY = 2;
	}
}
