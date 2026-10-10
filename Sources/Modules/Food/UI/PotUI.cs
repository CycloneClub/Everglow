using Everglow.Commons.DataStructures;
using Everglow.Commons.Utilities;
using Everglow.Food.Items.Cookers;
using Everglow.Food.Tiles;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.UI;
using static Everglow.Food.FoodIngredientItem;
using static Everglow.Food.FoodRecipes.FoodRecipes;
using static Everglow.Food.UI.StoveUIManager;

namespace Everglow.Food.UI;

public abstract class PotUI
{
	/// <summary>
	/// The top left anchor tile (stove).
	/// </summary>
	public Point AnchorTilePos;

	/// <summary>
	/// Ingredient Slots content.
	/// </summary>
	public int[] Ingredients = new int[10];

	/// <summary>
	/// Ingredient Slots position.
	/// </summary>
	public Vector2[] IngredientsSlotPos = new Vector2[10];

	/// <summary>
	/// Max counts of ingredient slots. Less than 100.
	/// </summary>
	public int MaxSlotCount;

	/// <summary>
	/// Whether the UI display.
	/// </summary>
	public bool Open = false;

	/// <summary>
	/// Decide the display method
	/// </summary>
	public bool Maximized = false;

	/// <summary>
	/// Mouse over which type of panel. 0~99:ingredient slots index ; 100:Close ; 101:Remove ; 102:Clear ; 103:Cook ; 104:Maximize
	/// </summary>
	public int MouseOverIndex = -1;
	public Vector2 ClosePanelPos;
	public Vector2 RemovePanelPos;
	public Vector2 ClearPanelPos;
	public Vector2 CookPanelPos;
	public Vector2 MaximizePanelPos;
	public bool ClosePanelEnable;
	public bool RemovePanelEnable;
	public bool ClearPanelEnable;
	public bool CookPanelEnable;

	/// <summary>
	/// Timer for cooking
	/// </summary>
	public int CookTimer = 0;

	/// <summary>
	/// Max time of an cuisine.
	/// </summary>
	public int CookTimerMax = 300;

	/// <summary>
	/// The cooking cuisine
	/// </summary>
	public int CuisineType = -1;

	/// <summary>
	/// The number of the cooking cuisine
	/// </summary>
	public int CuisineNum = -1;

	public static List<CookingUnit> PotMenu = new List<CookingUnit>();
	public static List<CookingUnitWithOrder> PotMenuWithOrder = new List<CookingUnitWithOrder>();

	public PotUI(Point anchorTilePos)
	{
		AnchorTilePos = anchorTilePos;
		SetDefault(6);
		Open = true;
	}

	public virtual void SetDefault(int maxSlotCount)
	{
		MaxSlotCount = maxSlotCount;
		Ingredients = new int[maxSlotCount];
		IngredientsSlotPos = new Vector2[maxSlotCount];
		CuisineType = -1;
		CookTimer = 0;
		CookTimerMax = 300;
		for (int i = 0; i < MaxSlotCount; i++)
		{
			IngredientsSlotPos[i] = new Vector2(i * 50, 0);
			Ingredients[i] = -1;
		}
	}

	public virtual Vector2 GetDrawPos()
	{
		// Adjust draw position between effectMatrix(UI) and transformationMatrix(World).
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Vector2 drawPos = AnchorTilePos.ToWorldCoordinates() - Main.screenPosition;
		drawPos = Vector2.Transform(drawPos, Main.GameViewMatrix.TransformationMatrix);
		drawPos = Vector2.Transform(drawPos, Matrix.Invert(sBS.TransformMatrix));

		if (Maximized)
		{
			drawPos += new Vector2(10 * Main.GameViewMatrix.Zoom.X, -40 * Main.GameViewMatrix.Zoom.Y - 60);
		}
		else
		{
			drawPos += new Vector2(10 * Main.GameViewMatrix.Zoom.X, -20 * Main.GameViewMatrix.Zoom.Y - 60);
		}
		return drawPos;
	}

	public void Draw()
	{
		if (!Open)
		{
			return;
		}

		// DrawCloseButtom();
		if (CookTimer == 0 || CookTimer > CookTimerMax - 3)
		{
			DrawButtom(100, ClosePanelPos, new Color(0.8f, 0, 0), new Color(1f, 0.3f, 0.2f), 0, 0);
			DrawMaximizeButtom();
			DrawButtom(101, RemovePanelPos, new Color(0.5f, 0.5f, 0.5f), new Color(0.6f, 0.6f, 0.6f), 0, 14);
			DrawButtom(102, ClearPanelPos, new Color(0.0f, 0.5f, 0.0f), new Color(0.1f, 0.6f, 0.3f), 0, 42);
			DrawButtom(103, CookPanelPos, new Color(0.7f, 0.2f, 0.0f), new Color(1f, 0.6f, 0.1f), 0, 28);
		}
		if (Maximized)
		{
			DrawMainPanel();
		}
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		var sBS2 = sBS;
		sBS2.SortMode = SpriteSortMode.Immediate;
		Main.spriteBatch.Begin(sBS2);
		if (CookTimer == 0)
		{
			DrawIngredientSlots();
		}
		else
		{
			DrawCookingAnimation();
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public virtual void DrawMainPanel()
	{
		Vector2 drawPos = GetDrawPos();
		Draw9Pieces(drawPos, 84, 64, new Color(0.8f, 0.6f, 0.5f), 0);
	}

	public virtual void DrawIngredientSlots()
	{
		if (Maximized)
		{
			Vector2 drawPos = GetDrawPos();
			for (int i = 0; i < MaxSlotCount; i++)
			{
				if (MouseOverIndex == i)
				{
					Draw9Pieces(drawPos + IngredientsSlotPos[i], 24, 24, new Color(86, 69, 58), 0);
				}
				else
				{
					Draw9Pieces(drawPos + IngredientsSlotPos[i], 24, 24, new Color(100, 90, 70), 0);
				}
				if (Ingredients[i] >= 0)
				{
					var item = new Item(Ingredients[i]);
					Color color = Color.White;
					Texture2D food = TextureAssets.Item[Ingredients[i]].Value;
					Rectangle frame = Main.itemAnimations[Ingredients[i]] == null ? food.Frame() : Main.itemAnimations[Ingredients[i]].GetFrame(food);
					float scale;
					ItemSlot.DrawItem_GetColorAndScale(item, 1, ref color, 20000, ref frame, out color, out scale);
					Main.spriteBatch.Draw(food, drawPos + IngredientsSlotPos[i], frame, color, 0f, frame.Size() * 0.5f, scale, SpriteEffects.None, 0f);
				}
			}
		}
		else
		{
			Vector2 drawPos = GetDrawPos();
			for (int i = 0; i < MaxSlotCount; i++)
			{
				if (MouseOverIndex == i)
				{
					Draw9Pieces(drawPos + IngredientsSlotPos[i], 16, 16, new Color(240, 60, 50, 200), 0);
				}
				else
				{
					Draw9Pieces(drawPos + IngredientsSlotPos[i], 16, 16, new Color(120, 60, 50, 90), 0);
				}
				Draw9Pieces(drawPos + IngredientsSlotPos[i], 16, 16, new Color(210, 210, 185), 0, ModAsset.StoveUIPanel_outline_thin.Value);
				if (Ingredients[i] >= 0)
				{
					var item = new Item(Ingredients[i]);
					Color color = Color.White;
					Texture2D food = TextureAssets.Item[Ingredients[i]].Value;
					Rectangle frame = Main.itemAnimations[Ingredients[i]] == null ? food.Frame() : Main.itemAnimations[Ingredients[i]].GetFrame(food);
					float scale;
					ItemSlot.DrawItem_GetColorAndScale(item, 1, ref color, 20000, ref frame, out color, out scale);
					Main.spriteBatch.Draw(food, drawPos + IngredientsSlotPos[i], frame, color, 0f, frame.Size() * 0.5f, scale, SpriteEffects.None, 0f);
				}
			}
		}
	}

	public virtual void DrawCookingAnimation()
	{
		Vector2 drawPos = GetDrawPos();
		var offsetPanel = new Vector2(0, 0);
		if (!Maximized)
		{
			offsetPanel = new Vector2(0, 70);
			Draw9Pieces(drawPos + offsetPanel, 16, 16, new Color(20, 20, 20, 160), 0);
		}
		else
		{
			Draw9Pieces(drawPos + offsetPanel, 24, 24, new Color(100, 90, 70), 0);
		}
		if (CuisineType == -1)
		{
			return;
		}
		var item = new Item(CuisineType);
		Color color = Color.White;
		Texture2D food = TextureAssets.Item[CuisineType].Value;
		Rectangle frame = Main.itemAnimations[CuisineType] == null ? food.Frame() : Main.itemAnimations[CuisineType].GetFrame(food);
		float scale;
		ItemSlot.DrawItem_GetColorAndScale(item, 1, ref color, 20000, ref frame, out color, out scale);
		if (!Maximized)
		{
			scale *= 0.6f;
		}
		Main.spriteBatch.Draw(food, drawPos + offsetPanel, frame, color, 0f, frame.Size() * 0.5f, scale, SpriteEffects.None, 0f);

		// Timer bar
		var offsetTimer = new Vector2(0, 50);
		if (!Maximized)
		{
			offsetTimer = new Vector2(0, 92);
			Draw9Pieces(drawPos + offsetTimer, 26, 5, new Color(0.3f, 0.3f, 0.6f), 0);
			Draw9Pieces(drawPos + offsetTimer, 24, 4, new Color(0f, 0f, 0f), 0);
		}
		else
		{
			Draw9Pieces(drawPos + offsetTimer, 46, 8, new Color(0.3f, 0.3f, 0.6f), 0);
			Draw9Pieces(drawPos + offsetTimer, 44, 6, new Color(0f, 0f, 0f), 0);
		}
		float timeValue = 1 - CookTimer / (float)CookTimerMax;

		Texture2D timer = ModAsset.StoveUIPanelTimer.Value;
		if (!Maximized)
		{
			Main.spriteBatch.Draw(timer, drawPos + offsetTimer, new Rectangle(0, 0, (int)(timer.Width * timeValue), timer.Height), Color.Lerp(new Color(0f, 1f, 0f), new Color(1f, 1f, 0f), timeValue) * 0.9f, 0, timer.Size() * 0.5f, new Vector2(0.7f, 1.6f) * 0.5f, SpriteEffects.None, 0);
		}
		else
		{
			Main.spriteBatch.Draw(timer, drawPos + offsetTimer, new Rectangle(0, 0, (int)(timer.Width * timeValue), timer.Height), Color.Lerp(new Color(0f, 1f, 0f), new Color(1f, 1f, 0f), timeValue) * 0.9f, 0, timer.Size() * 0.5f, new Vector2(0.7f, 1.6f), SpriteEffects.None, 0);
			Main.spriteBatch.Draw(timer, drawPos + new Vector2(96 * timeValue, 0) + offsetTimer, new Rectangle((int)(timer.Width * timeValue), 0, 2, timer.Height), Color.Lerp(new Color(0f, 1f, 0f), new Color(1f, 1f, 0f), timeValue) * 3, 0, timer.Size() * 0.5f, new Vector2(0.7f, 0.8f), SpriteEffects.None, 0);
		}
	}

	public void DrawButtom(int index, Vector2 addPos, Color color, Color colorHit, int frameX, int frameY)
	{
		Vector2 drawPos = GetDrawPos();
		Texture2D iconAtlas = ModAsset.StoveUIIcons.Value;
		Vector2 CloseButtonPos = drawPos + addPos + new Vector2(10, 16);
		int sizeX = 20;
		int sizeY = 16;
		float iconSize = 1.5f;
		var frame = new Rectangle(frameX, frameY, 14, 14);
		if (!Maximized)
		{
			sizeX = 12;
			sizeY = 12;
			iconSize = 1f;
			color *= 0.8f;
		}
		Color darkerColor = color * 0.1f;
		darkerColor.A = 255;
		if (MouseOverIndex == index)
		{
			Draw9Pieces(CloseButtonPos, sizeX * 1.2f, sizeY * 1.2f, colorHit, 0.2f);
			Draw9Pieces(CloseButtonPos, sizeX * 1f, sizeY * 1f, colorHit * 2, 0, ModAsset.StoveUIPanel_outline_thin.Value);
			Main.spriteBatch.Draw(iconAtlas, CloseButtonPos, frame, darkerColor, 0f, frame.Size() * 0.5f, iconSize * 1.33f, SpriteEffects.None, 0f);
		}
		else
		{
			Draw9Pieces(CloseButtonPos, sizeX, sizeY, color, 0.2f);
			Draw9Pieces(CloseButtonPos, sizeX * 1f, sizeY * 1f, color * 2, 0, ModAsset.StoveUIPanel_outline_thin.Value);
			Main.spriteBatch.Draw(iconAtlas, CloseButtonPos, frame, darkerColor, 0f, frame.Size() * 0.5f, iconSize, SpriteEffects.None, 0f);
		}
	}

	public virtual void DrawMaximizeButtom()
	{
		var color = new Color(0.3f, 0.4f, 0.9f);
		var colorHit = new Color(0.5f, 0.7f, 1f);
		Vector2 drawPos = GetDrawPos();
		Texture2D iconAtlas = ModAsset.StoveUIIcons.Value;
		Vector2 CloseButtonPos = drawPos + MaximizePanelPos + new Vector2(10, 16);
		int sizeX = 20;
		int sizeY = 16;
		float iconSize = 1.5f;
		var frame = new Rectangle(14, 28, 14, 14);
		if (!Maximized)
		{
			sizeX = 12;
			sizeY = 12;
			iconSize = 1f;
			color *= 0.8f;
			frame = new Rectangle(14, 0, 14, 14);
		}
		Color darkerColor = color * 0.1f;
		darkerColor.A = 255;
		if (MouseOverIndex == 104)
		{
			Draw9Pieces(CloseButtonPos, sizeX * 1.2f, sizeY * 1.2f, colorHit, 0.2f);
			Draw9Pieces(CloseButtonPos, sizeX * 1f, sizeY * 1f, colorHit * 2, 0, ModAsset.StoveUIPanel_outline_thin.Value);
			Main.spriteBatch.Draw(iconAtlas, CloseButtonPos, frame, darkerColor, 0f, frame.Size() * 0.5f, iconSize * 1.33f, SpriteEffects.None, 0f);
		}
		else
		{
			Draw9Pieces(CloseButtonPos, sizeX, sizeY, color, 0.2f);
			Draw9Pieces(CloseButtonPos, sizeX * 1f, sizeY * 1f, color * 2, 0, ModAsset.StoveUIPanel_outline_thin.Value);
			Main.spriteBatch.Draw(iconAtlas, CloseButtonPos, frame, darkerColor, 0f, frame.Size() * 0.5f, iconSize, SpriteEffects.None, 0f);
		}
	}

	public void ClearPot(bool noItem = false)
	{
		for (int index = 0; index < Ingredients.Length; index++)
		{
			if (Ingredients[index] != -1)
			{
				if (!noItem)
				{
					Item.NewItem(null, AnchorTilePos.ToWorldCoordinates(), Ingredients[index], 1);
				}
				Ingredients[index] = -1;
			}
		}
	}

	public virtual void Update()
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Vector2 drawPos = GetDrawPos();

		if (CookTimer > 0)
		{
			if (Main.gamePaused)
			{
				return;
			}
			CookTimer--;
			if (CookTimer == 0)
			{
				Item.NewItem(null, AnchorTilePos.ToWorldCoordinates(), CuisineType, CuisineNum);
				ClearPot(true);
				CuisineType = -1;
				CuisineNum = -1;
				Open = false;
			}
			return;
		}
		else
		{
			CookTimer = 0;
		}
		if (!Open)
		{
			return;
		}

		// Close incident
		if (HitPanel(Close, 100, drawPos + ClosePanelPos))
		{
			return;
		}

		// Remove incident
		if (HitPanel(Remove, 101, drawPos + RemovePanelPos))
		{
			return;
		}

		// Clear incident
		if (HitPanel(Clear, 102, drawPos + ClearPanelPos))
		{
			return;
		}

		// Cook incident
		if (HitPanel(Cook, 103, drawPos + CookPanelPos))
		{
			return;
		}

		// Maximize incident
		if (HitPanel(Maximize, 104, drawPos + MaximizePanelPos))
		{
			return;
		}

		Player player = Main.LocalPlayer;

		// Ingredient slot
		for (int i = 0; i < MaxSlotCount; i++)
		{
			Vector2 slotPos = drawPos + IngredientsSlotPos[i];
			int size = 32;
			if (Maximized)
			{
				size = 48;
			}
			var ingredientBox = new Rectangle((int)slotPos.X - size / 2, (int)slotPos.Y - size / 2, size, size);
			if (ingredientBox.Contains((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y))
			{
				if (MouseOverIndex != i)
				{
					SoundEngine.PlaySound(SoundID.MenuTick);
				}
				MouseOverIndex = i;

				// add
				if (Ingredients[i] == -1)
				{
					if (IsIngredient(player.HeldItem.type))
					{
						if (Main.mouseRight && Main.mouseRightRelease)
						{
							player.HeldItem.stack--;
							Ingredients[i] = player.HeldItem.type;
						}
						else if (Main.mouseLeft && Main.mouseLeftRelease
							&& Main.mouseItem != null && Main.mouseItem.type != 0)
						{
							Main.mouseItem.stack--;
							Ingredients[i] = Main.mouseItem.type;
						}
						if (Maximized)
						{
							Main.instance.MouseText("[i:" + player.HeldItem.type + "] → [i:" + ModContent.ItemType<Casserole_Item>() + "]", ItemRarityID.White);
						}
						else
						{
							Main.instance.MouseText("[i:" + player.HeldItem.type + "]");
						}
					}
				}

				// remove
				else
				{
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						Item.NewItem(null, AnchorTilePos.ToWorldCoordinates(), Ingredients[i], 1);
						Ingredients[i] = -1;
					}
					if (Maximized)
					{
						Main.instance.MouseText("[i:" + ModContent.ItemType<Casserole_Item>() + "] → [i:" + Ingredients[i] + "]", ItemRarityID.White);
					}
					else
					{
						Main.instance.MouseText("[i:" + Ingredients[i] + "]");
					}
				}
				return;
			}
		}
		MouseOverIndex = -1;
	}

	public bool HitPanel(Action hitEffect, int mouseIndex, Vector2 position)
	{
		int sizeX = 20;
		int sizeY = 16;
		if (!Maximized)
		{
			sizeX = 12;
			sizeY = 12;
		}
		var cancelBox = new Rectangle((int)position.X - sizeX / 2, (int)position.Y, 2 * sizeX, 2 * sizeY);
		if (cancelBox.Contains((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y))
		{
			if (MouseOverIndex != mouseIndex)
			{
				SoundEngine.PlaySound(SoundID.MenuTick);
			}
			MouseOverIndex = mouseIndex;
			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				SoundEngine.PlaySound(SoundID.MenuClose);
				hitEffect();
			}
			return true;
		}
		return false;
	}

	public void Close()
	{
		Open = false;
	}

	public virtual void Remove()
	{
		StoveEntity stoveEneity;
		Stove.TryGetStoveEntityAs(AnchorTilePos.X, AnchorTilePos.Y, out stoveEneity);
		switch (stoveEneity.PotState)
		{
			case 1:
				{
					Item.NewItem(null, AnchorTilePos.ToWorldCoordinates(), ModContent.ItemType<Casserole_Item>(), 1);
					break;
				}
			case 2:
				{
					Item.NewItem(null, AnchorTilePos.ToWorldCoordinates(), ModContent.ItemType<SteamBox_Item>(), 1);
					break;
				}
			case 3:
				{
					Item.NewItem(null, AnchorTilePos.ToWorldCoordinates(), ModContent.ItemType<SteamBox_Item>(), 2);
					break;
				}
			default:
				{
					break;
				}
		}
		if (stoveEneity != null)
		{
			stoveEneity.PotState = 0;
		}
		ClearPot();

		Open = false;
	}

	public virtual void Clear()
	{
		ClearPot();
	}

	public virtual void Cook()
	{
		if (CuisineType == -1)
		{
			FurnitureUtils.LightHitwire(AnchorTilePos.X, AnchorTilePos.Y, ModContent.TileType<Stove>(), 2, 3);
			return;
		}
		Tile tile = Main.tile[AnchorTilePos];
		if (tile.TileFrameX < 36)
		{
			FurnitureUtils.LightHitwire(AnchorTilePos.X, AnchorTilePos.Y, ModContent.TileType<Stove>(), 2, 3);
		}
		CookTimer = CookTimerMax;
	}

	public void Maximize()
	{
		Maximized = !Maximized;
	}
}
