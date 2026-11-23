using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;
using Terraria.Audio;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.Furnitures;

public class ExquisiteTwilightEucalyptusDoorLocked : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileSolid[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileID.Sets.NotReallySolid[Type] = true;
		TileID.Sets.DrawsWalls[Type] = true;
		TileID.Sets.HasOutlines[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;

		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

		DustType = ModContent.DustType<TwilightEucalyptusWoodDust>();
		AdjTiles = new int[] { TileID.ClosedDoor };

		AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Door"));

		TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.ClosedDoor, 0));
		TileObjectData.addTile(Type);
	}

	public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
	{
		return true;
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 1;
	}

	public override void MouseOver(int i, int j)
	{
		Player player = Main.LocalPlayer;
		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
		player.cursorItemIconID = ModContent.ItemType<TwilightCastleKey>();
	}

	public override bool RightClick(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		int top = j - tile.TileFrameY / 18;
		if (!IsLockedColumn(i, top))
		{
			return false;
		}

		Player player = Main.LocalPlayer;
		int keyType = ModContent.ItemType<TwilightCastleKey>();
		int keySlot = -1;
		for (int slot = 0; slot < player.inventory.Length; slot++)
		{
			Item item = player.inventory[slot];
			if (item.type == keyType && item.stack > 0)
			{
				keySlot = slot;
				break;
			}
		}

		if (keySlot < 0)
		{
			return true;
		}

		Item key = player.inventory[keySlot];
		if (key.stack > 1)
		{
			key.stack--;
		}
		else
		{
			key.TurnToAir();
		}

		ushort closedType = (ushort)ModContent.TileType<ExquisiteTwilightEucalyptusDoorClosed>();
		for (int y = 0; y < 3; y++)
		{
			Main.tile[i, top + y].TileType = closedType;
		}

		SoundEngine.PlaySound(SoundID.Unlock, new Vector2(i, top + 1) * 16);
		NetMessage.SendTileSquare(Main.myPlayer, i, top, 1, 3);
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			NetMessage.SendData(MessageID.SyncEquipment, -1, -1, null, player.whoAmI, keySlot);
		}

		return true;
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		// fail = true;
	}

	private bool IsLockedColumn(int x, int top)
	{
		for (int y = 0; y < 3; y++)
		{
			Tile doorTile = Main.tile[x, top + y];
			if (!doorTile.HasTile || doorTile.TileType != Type)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// While a locked castle door remains, players cannot place tiles in the single column immediately left or right of it.
	/// </summary>
	public class ExquisiteTwilightEucalyptusDoorLockedPlacement : GlobalTile
	{
		public override bool CanPlace(int i, int j, int type)
		{
			if (WorldGen.generatingWorld || i < 0 || j < 0 || i >= Main.maxTilesX || j >= Main.maxTilesY)
			{
				return true;
			}

			int lockedType = ModContent.TileType<ExquisiteTwilightEucalyptusDoorLocked>();
			int minX = Math.Max(0, i - 1);
			int maxX = Math.Min(Main.maxTilesX - 1, i + 1);
			for (int x = minX; x <= maxX; x++)
			{
				Tile tile = Main.tile[x, j];
				if (tile.HasTile && tile.TileType == lockedType)
				{
					return false;
				}
			}

			return true;
		}
	}
}
