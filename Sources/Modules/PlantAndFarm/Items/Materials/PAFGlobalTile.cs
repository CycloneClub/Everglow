using Terraria.DataStructures;

namespace Everglow.PlantAndFarm.Items.Materials;

public class PAFGlobalTile : GlobalTile
{
	public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (Main.netMode != NetmodeID.Server &&
			type == TileID.Plants &&
			!fail &&
			!effectOnly &&
			!noItem && WorldGen.GetPlayerForTile(i, j).GetModPlayer<PAFPlayer>().FlowerBrochure)
		{
			EntitySource_TileBreak source = new(i, j);
			Rectangle rec = new(i * 16, j * 16, 16, 16);
			switch (Main.tile[i, j].frameX / 18)
			{
				// grass
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
					{
						break;
					}
				case 6:
					{
						Item.NewItem(source, rec, ModContent.ItemType<PurpleTail>());
						break;
					}
				case 7:
					{
						Item.NewItem(source, rec, ModContent.ItemType<DarkPoppy>());
						break;
					}
				// mushroom
				case 8:
					{
						break;
					}
				case 9:
					{
						Item.NewItem(source, rec, ModContent.ItemType<BlueFreeze>());
						break;
					}
				case 10:
				case 11:
					{
						Item.NewItem(source, rec, ModContent.ItemType<LightChrysanthemum>());
						break;
					}
				case 12:
					{
						Item.NewItem(source, rec, ModContent.ItemType<Lavender>());
						break;
					}
				case 13:
				case 18:
					{
						Item.NewItem(source, rec, ModContent.ItemType<GoldWhip>());
						break;
					}
				case 14:
					{
						Item.NewItem(source, rec, ModContent.ItemType<WhiteStar>());
						break;
					}
				case 15:
					{
						Item.NewItem(source, rec, ModContent.ItemType<SilverClock>());
						break;
					}
				case 16:
					{
						Item.NewItem(source, rec, ModContent.ItemType<BluePedal>());
						break;
					}
				case 17:
					{
						Item.NewItem(source, rec, ModContent.ItemType<PinkSun>());
						break;
					}
				case 19:
					{
						Item.NewItem(source, rec, ModContent.ItemType<OrangeSausage>());
						break;
					}
				case 20:
					{
						Item.NewItem(source, rec, ModContent.ItemType<CyanHyacinth>());
						break;
					}
				case 21:
				case 22:
				case 23:
					{
						Item.NewItem(source, rec, ModContent.ItemType<RedFlame>());
						break;
					}
				case 24:
				case 25:
				case 26:
					{
						Item.NewItem(source, rec, ModContent.ItemType<GoldCup>());
						break;
					}
				case 27:
				case 28:
				case 29:
					{
						Item.NewItem(source, rec, ModContent.ItemType<ShallowNight>());
						break;
					}
				case 30:
				case 31:
				case 32:
					{
						Item.NewItem(source, rec, ModContent.ItemType<PurplePhantom>());
						break;
					}
				case 33:
				case 34:
				case 35:
					{
						Item.NewItem(source, rec, ModContent.ItemType<HotPinkTulip>());
						break;
					}
				case 36:
				case 37:
				case 38:
					{
						Item.NewItem(source, rec, ModContent.ItemType<WhiteTulip>());
						break;
					}
				case 39:
				case 40:
				case 41:
					{
						Item.NewItem(source, rec, ModContent.ItemType<OrangeTulip>());
						break;
					}
				case 42:
				case 43:
				case 44:
					{
						Item.NewItem(source, rec, ModContent.ItemType<LightPurpleBalls>());
						break;
					}
			}
		}
	}
}
