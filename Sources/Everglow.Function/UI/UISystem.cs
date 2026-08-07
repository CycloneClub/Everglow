using Terraria.UI;

namespace Everglow.Commons.UI
{
	public class UISystem : ModSystem
	{
		public static EverglowUISystem EverglowUISystem
		{
			get => Instance.system;
		}

		public static UISystem Instance
		{
			get => instance;
		}

		private EverglowUISystem system;
		private static UISystem instance;
		private Point screenSize;

		public bool LeftResizing = false;
		public bool RightResizing = false;
		public bool TopResizing = false;
		public bool BottomResizing = false;

		public UISystem()
		{
			system = new EverglowUISystem();
			instance = this;
		}

		public override void Load()
		{
			base.Load();
			On_Main.DrawCursor += ModifyUIBlockResizeCursor;
			On_Main.DrawThickCursor += ModifyUIBlockResizeThickCursor;
			if (Main.netMode != NetmodeID.Server)
			{
				system.Load();
			}
		}

		public override void UpdateUI(GameTime gameTime)
		{
			LeftResizing = false;
			RightResizing = false;
			TopResizing = false;
			BottomResizing = false;
			base.UpdateUI(gameTime);

			if (Main.netMode != NetmodeID.Server)
			{
				if (screenSize != Main.ScreenSize)
				{
					screenSize = Main.ScreenSize;
					system.Calculation();
				}
				system.Update(gameTime);
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			base.ModifyInterfaceLayers(layers);
			int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseTextIndex != -1)
			{
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
					"Everglow: Everglow UI System",
					() =>
					{
						if (Main.netMode != NetmodeID.Server)
						{
							system.Draw(Main.spriteBatch);
						}

						return true;
					},
					InterfaceScaleType.UI));
			}
		}

		private void ModifyUIBlockResizeCursor(On_Main.orig_DrawCursor orig, Vector2 bonus, bool smart)
		{
			Vector2 resizeDir = GetResizeDirection();
			if (resizeDir == Vector2.zeroVector)
			{
				orig(bonus, smart);
			}
			else
			{
				Texture2D tex = ModAsset.Cursor_Resize_TL_BR_Slash.Value;
				int dirValue = (int)(resizeDir.X * resizeDir.Y);
				if (dirValue == 1)
				{
					tex = ModAsset.Cursor_Resize_TL_BR_Slash.Value;
				}
				else if (dirValue == -1)
				{
					tex = ModAsset.Cursor_Resize_BL_TR_Slash.Value;
				}
				else if (dirValue == 0)
				{
					if (resizeDir.Y == 0)
					{
						tex = ModAsset.Cursor_Resize_H.Value;
					}
					if (resizeDir.X == 0)
					{
						tex = ModAsset.Cursor_Resize_V.Value;
					}
				}
				Main.spriteBatch.Draw(tex, new Vector2(Main.mouseX, Main.mouseY) + Vector2.One * -11, null, Main.cursorColor, 0f, default(Vector2), Main.cursorScale, SpriteEffects.None, 0f);
			}
		}

		private Vector2 ModifyUIBlockResizeThickCursor(On_Main.orig_DrawThickCursor orig, bool smart)
		{
			Vector2 resizeDir = GetResizeDirection();
			if (resizeDir == Vector2.zeroVector)
			{
				return orig(smart);
			}
			else
			{
				int dirValue = (int)(resizeDir.X * resizeDir.Y);
				Texture2D tex = ModAsset.Cursor_Resize_TL_BR_Slash_Bound.Value;
				if (dirValue == 1)
				{
					tex = ModAsset.Cursor_Resize_TL_BR_Slash_Bound.Value;
				}
				else if (dirValue == -1)
				{
					tex = ModAsset.Cursor_Resize_BL_TR_Slash_Bound.Value;
				}
				else if (dirValue == 0)
				{
					if (resizeDir.Y == 0)
					{
						tex = ModAsset.Cursor_Resize_H_Bound.Value;
					}
					if (resizeDir.X == 0)
					{
						tex = ModAsset.Cursor_Resize_V_Bound.Value;
					}
				}
				for (int k = 0;k < 4;k++)
				{
					Vector2 offset = new Vector2(1, 0).RotatedBy(MathHelper.PiOver2 * k);
					Main.spriteBatch.Draw(tex, new Vector2(Main.mouseX, Main.mouseY) + Vector2.One * -11 + offset, null, Main.MouseBorderColor, 0f, default(Vector2), Main.cursorScale, SpriteEffects.None, 0f);
				}
				return Vector2.zeroVector;
			}
		}

		private Vector2 GetResizeDirection()
		{
			Vector2 resizeDir = Vector2.Zero;
			if (LeftResizing)
			{
				resizeDir.X = -1;
			}
			if (RightResizing)
			{
				resizeDir.X = 1;
			}
			if (TopResizing)
			{
				resizeDir.Y = -1;
			}
			if (BottomResizing)
			{
				resizeDir.Y = 1;
			}
			return resizeDir;
		}
	}
}
