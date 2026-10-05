using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

public class FurnaceTrapDoor : ModTile
{
	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileSolid[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = false;
		MinPick = int.MaxValue;
		DustType = ModContent.DustType<Heatproof_Furniture_Dust>(); // You should set a kind of dust manually.

		// Placement
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 8;

		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.Origin = new Point16(4, 0);
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(84, 84, 84));
	}

	public override bool CanExplode(int i, int j) => false;

	public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;

	public override void MouseOver(int i, int j)
	{
		Player player = Main.LocalPlayer;
		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
		player.cursorItemIconID = ModContent.ItemType<Items.Placeables.FurnaceTrapDoor_Item>();
		base.MouseOver(i, j);
	}

	public override bool RightClick(int i, int j)
	{
		OpenTrapDoor(i, j);
		return base.RightClick(i, j);
	}

	public void OpenTrapDoor(int i, int j)
	{
		Tile thisTile = TileUtils.SafeGetTile(i, j);
		int startX = i - thisTile.TileFrameX / 18;
		int startY = j - thisTile.TileFrameY / 18;
		var firstTile = TileUtils.SafeGetTile(startX, startY);
		if (firstTile.TileType == Type)
		{
			var trapDoorVFX = new FurnaceTrapDoor_VFX
			{
				Active = true,
				Visible = true,
				Position = new Point(startX, startY).ToWorldCoordinates() + new Vector2(-8),
				MaxTime = 80,
				Open = true,
				ai = new float[] { 300 },
				TileX = startX,
				TileY = startY,
			};
			Ins.VFXManager.Add(trapDoorVFX);
		}
	}
}
