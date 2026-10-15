using Everglow.CagedDomain.Items;
using Everglow.Commons.VFX.Scene;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.CagedDomain.Tiles.Escalator;

public class NormalEscalator : ModTile, ISceneTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileFrameImportant[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.addTile(Type);
		DustType = DustID.WoodFurniture;
		AddMapEntry(new Color(153, 152, 151));
		HitSound = SoundID.DD2_SkeletonHurt;
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
		base.NumDust(i, j, fail, ref num);
	}

	public void AddScene(int i, int j)
	{
		int dir = 1;
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX == 18)
		{
			dir = -1;
		}
		var escalator_VFX = new NormalEscalator_VFX()
		{
			OriginTilePos = new Point(i, j),
			OriginTileType = Type,
			Position = new Vector2(i * 16, j * 16),
			Direction = dir,
			State = tile.TileFrameY / 18,
			Active = true,
			Visible = true,
		};
		Ins.VFXManager.Add(escalator_VFX);
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Player player = Main.LocalPlayer;
		if (player.HeldItem.type == ModContent.ItemType<NormalEscalator_Item>())
		{
			Vector2 zero = new Vector2(Main.offScreenRange);
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			Tile tile = Main.tile[i, j];
			int dir = 1;
			if (tile.TileFrameX == 18)
			{
				dir = -1;
			}
			Texture2D tex = Commons.ModAsset.TileBlock.Value;
			for (int k = 1; k < 120; k++)
			{
				spriteBatch.Draw(tex, new Vector2(i + k * dir, j - k) * 16 - Main.screenPosition + zero, null, new Color(0f, 1f, 0f, 0f), 0, Vector2.zeroVector, 1f, SpriteEffects.None, 0);
			}
		}
	}
}
