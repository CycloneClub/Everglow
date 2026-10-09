using Everglow.Commons.DataStructures;
using Everglow.Commons.Utilities;
using SubworldLibrary;

namespace Everglow.SubSpace;

public class CreateRoom : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.maxStack = 1;
		Item.useTurn = true;
		Item.autoReuse = true;
		Item.useAnimation = 15;
		Item.useTime = 7;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.consumable = false;
	}
	public override bool? UseItem(Player player)
	{
		if (player.itemAnimation == player.itemAnimationMax)
		{



		}
		return base.UseItem(player);
	}
	public bool Holding = false;
	public Point MousePoint = new Point(0, 0);
	public override void HoldItem(Player player)
	{
		MousePoint = new Point((int)(Main.MouseWorld.X / 16f), (int)(Main.MouseWorld.Y / 16f));
		if (RoomWorld.OriginalWorld != null)
		{
			// Main.NewText(Main.worldName);
		}
		if (!SubworldSystem.IsActive<RoomWorld>())
		{
			// RoomWorld.LayerDepth = 0;
		}
		if (Main.mouseLeft && Main.mouseLeftRelease)
		{
			RoomManager.EnterNextLevelRoom(MousePoint);
		}
		if (Main.mouseRight && Main.mouseRightRelease)
		{
			RoomManager.ExitALevelOfRoom();
		}
		if (Main.mouseMiddle && Main.mouseMiddleRelease)
		{
			RoomManager.ExitToOriginalWorld();
		}
		Holding = true;
		base.HoldItem(player);
	}
	public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		if (Holding)
		{
			SpriteBatchState sBS = GraphicsUtils.GetState(spriteBatch).Value;
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			Color drawColor1 = new Color(0.4f, 0, 0, 1);
			string mouseText = MousePoint.ToString() + "\nLeft click to create a room at this point";
			int mouseTextValue = 0;
			string path = Path.Combine(Main.SavePath, "Mods", "ModDatas", "Everglow_RoomWorlds", Main.worldID.ToString());
			if (SubworldSystem.AnyActive())
			{
				path = Path.Combine(Main.SavePath, "Mods", "ModDatas", "Everglow_RoomWorlds", Main.worldID.ToString(), SubworldSystem.Current.Name.ToString());
			}
			string readPath = path + "\\RoomMapIO" + MousePoint.X.ToString() + "_" + MousePoint.Y.ToString() + "_" + (RoomWorld.LayerDepth + 1) + ".mapio";
			if (File.Exists(readPath))
			{
				mouseText = MousePoint.ToString() + "\nHad Room!";
				mouseTextValue = 3;
				drawColor1 = new Color(1f, 0.6f, 0, 1);
			}
			if (SubworldSystem.IsActive<RoomWorld>())
			{
				mouseText = "Left click to go next layer\nRight click to go previous layer\nCurrent layer : " + RoomWorld.LayerDepth;
				mouseTextValue = 2;
				drawColor1 = new Color(0f, 0.2f, 1, 1);
			}

			Main.instance.MouseText(mouseText, mouseTextValue);

			Texture2D block = Commons.ModAsset.TileBlock.Value;
			spriteBatch.Draw(block, MousePoint.ToVector2() * 16 - Main.screenPosition, null, drawColor1, 0, new Vector2(0), 1f, SpriteEffects.None, 0);

			spriteBatch.End();
			spriteBatch.Begin(sBS);
		}
		Holding = false;
		base.PostDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
	}
}
