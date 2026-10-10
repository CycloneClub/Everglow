using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class RuinTorch : ModTile
{
	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileLighted[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileNoAttach[Type] = true;
		Main.tileNoFail[Type] = true;
		Main.tileWaterDeath[Type] = true;
		TileID.Sets.FramesOnKillWall[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;
		TileID.Sets.DisableSmartInteract[Type] = true;
		TileID.Sets.Torches[Type] = true;

		DustType = ModContent.DustType<RuinTorchDust>();
		AdjTiles = new int[] { TileID.Torches };

		TileID.Sets.RoomNeeds.CountsAsTorch[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Torches, 0));

		TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
		TileObjectData.newSubTile.LinkedAlternates = true;
		TileObjectData.newSubTile.WaterDeath = false;
		TileObjectData.newSubTile.LavaDeath = false;
		TileObjectData.newSubTile.WaterPlacement = LiquidPlacement.Allowed;
		TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
		TileObjectData.addSubTile(1);

		TileObjectData.addTile(Type);
	}

	public override void MouseOver(int i, int j)
	{
		Player player = Main.LocalPlayer;
		player.noThrow = 2;
		player.cursorItemIconEnabled = true;

		// We can determine the item to show on the cursor by getting the tile style and looking up the corresponding item drop.
		int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
		player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, style);
	}

	public override float GetTorchLuck(Player player)
	{
		bool inYggdrasilUndergroundBiome = player.InModBiome<YggdrasilTownBiome>();
		return inYggdrasilUndergroundBiome ? 1f : -0.1f;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = Main.rand.Next(1, 3);

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		Tile tile = Main.tile[i, j];

		// If the torch is on
		if (tile.TileFrameX < 66)
		{
			r = 0.5f;
			g = 1.1f;
			b = 1.1f;
		}
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
				int numForDust = Dust.NewDust(dustBox.TopLeft(), dustBox.Width, dustBox.Height, ModContent.DustType<RuinTorchDust>(), 0f, 0f, 254, default, Main.rand.NextFloat(0.95f, 1.75f));
				Dust obj = Main.dust[numForDust];
				obj.velocity *= 0.4f;
				Main.dust[numForDust].velocity.Y -= 0.4f;
			}
		}
	}

	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
	{
		// This code slightly lowers the draw position if there is a solid tile above, so the flame doesn't overlap that tile. Terraria torches do this same logic.
		offsetY = 0;

		if (WorldGen.SolidTile(i, j - 1))
		{
			offsetY = 4;
		}
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];

		if (!TileDrawing.IsVisible(tile))
		{
			return;
		}

		// The following code draws multiple flames on top our placed torch.
		int offsetY = 0;

		if (WorldGen.SolidTile(i, j - 1))
		{
			offsetY = 4;
		}

		Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}

		ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)i); // Don't remove any casts.
		Color color = new Color(100, 100, 100, 60);
		int width = 20;
		int height = 20;
		int frameX = tile.TileFrameX;
		int frameY = tile.TileFrameY;
		for (int k = 0; k < 7; k++)
		{
			float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
			float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;

			spriteBatch.Draw(ModAsset.RuinTorch_Item_Flame.Value, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + xx, j * 16 - (int)Main.screenPosition.Y + offsetY + yy) + zero, new Rectangle(frameX, frameY, width, height), color, 0f, default, 1f, SpriteEffects.None, 0f);
		}
	}
}
