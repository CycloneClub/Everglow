using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Trainee;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;

public class TraineeEquipmentsShelf : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileNoAttach[Type] = false;
		Main.tileBlendAll[Type] = false;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Width = 11;
		TileObjectData.newTile.Height = 4;
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.CoordinatePadding = 2;
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.CoordinateHeights = new int[4];
		Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
		TileObjectData.newTile.CoordinateHeights[^1] = 18;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.Origin = new(5, 2);
		TileObjectData.newTile.AnchorTop = new AnchorData(0, 0, 0);
		TileObjectData.newTile.AnchorBottom = new AnchorData(0, 0, 0);
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(58, 48, 44));
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		resetFrame = false;
		return false;
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public override void NearbyEffects(int i, int j, bool closer) => base.NearbyEffects(i, j, closer);

	public override bool RightClick(int i, int j)
	{
		Main.LocalPlayer.QuickSpawnItem(WorldGen.GetItemSource_FromTileBreak(i, j), ModContent.ItemType<SafetyHelmets>());
		Main.LocalPlayer.QuickSpawnItem(WorldGen.GetItemSource_FromTileBreak(i, j), ModContent.ItemType<ConventionalEquipment>());
		Main.LocalPlayer.QuickSpawnItem(WorldGen.GetItemSource_FromTileBreak(i, j), ModContent.ItemType<StandardLeggings>());
		return base.RightClick(i, j);
	}

	public override void MouseOver(int i, int j)
	{
		base.MouseOver(i, j);
		string text = "Claim free [i:" + ModContent.ItemType<SafetyHelmets>() + "]+" + " [i:" + ModContent.ItemType<ConventionalEquipment>() + "]+" + " [i:" + ModContent.ItemType<StandardLeggings>() + "].";
		Main.instance.MouseText(text, ItemRarityID.White);
	}
}
