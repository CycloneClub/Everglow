using Everglow.Food.FoodRecipes;
using static Everglow.Food.FoodRecipes.FoodRecipes;

namespace Everglow.Food.UI;

public class CasseroleUI : PotUI
{
	public Vector2 PanelPos0 = new Vector2(100, -100);
	public Vector2 PanelPos1 = new Vector2(80, -92);
	public Vector2 PanelPos2 = new Vector2(104, -88);
	public Vector2 PanelPos3 = new Vector2(140, -50);
	public Vector2 PanelPos4 = new Vector2(60, -100);

	public static CookingUnit MapoTofu;
	public static CookingUnit YuxiangEggplant;
	public static CookingUnit BoiledBullfrog;

	public CasseroleUI(Point anchorTilePos)
		: base(anchorTilePos)
	{
	}

	public override void SetDefault(int count)
	{
		int maxSlotCount = 6;
		Ingredients = new int[maxSlotCount];
		IngredientsSlotPos = new Vector2[maxSlotCount];
		for (int y = 0; y < 2; y++)
		{
			for (int x = 0; x < 3; x++)
			{
				int index = 3 * y + x;
				IngredientsSlotPos[index] = new Vector2(x - 1, y - 0.3f) * 50;
				Ingredients[index] = -1;
			}
		}
		MaxSlotCount = maxSlotCount;
		if (PotMenu.Count == 0)
		{
			SetMenu();
		}
	}

	public static void SetMenu()
	{
		CasseroleRecipe CasseroleRecipe = new CasseroleRecipe();
		PotMenuWithOrder = CasseroleRecipe.CookingUnitWithOrderMenu;
		PotMenu = CasseroleRecipe.CookingUnitMenu;
	}

	public override void Update()
	{
		if (Maximized)
		{
			PanelPos0 = new Vector2(140, -90);
			PanelPos1 = new Vector2(-60, 82);
			PanelPos2 = new Vector2(40, 82);
			PanelPos3 = new Vector2(-10, 82);
			PanelPos4 = new Vector2(100, -90);
			for (int y = 0; y < 2; y++)
			{
				for (int x = 0; x < 3; x++)
				{
					int index = 3 * y + x;
					IngredientsSlotPos[index] = new Vector2(x - 1, y - 0.3f) * 50;
				}
			}
		}
		else
		{
			PanelPos0 = new Vector2(44, -14);
			PanelPos1 = new Vector2(-34, 70);
			PanelPos2 = new Vector2(14, 70);
			PanelPos3 = new Vector2(-10, 70);
			PanelPos4 = new Vector2(20, -14);
			for (int y = 0; y < 2; y++)
			{
				for (int x = 0; x < 3; x++)
				{
					int index = 3 * y + x;
					IngredientsSlotPos[index] = new Vector2(x - 1, y + 1) * 30;
				}
			}
		}
		if (CookTimer > 0)
		{
			ClosePanelPos = Vector2.Lerp(ClosePanelPos, Vector2.zeroVector, 0.3f);
			RemovePanelPos = Vector2.Lerp(RemovePanelPos, Vector2.zeroVector, 0.3f);
			ClearPanelPos = Vector2.Lerp(ClearPanelPos, Vector2.zeroVector, 0.3f);
			CookPanelPos = Vector2.Lerp(CookPanelPos, Vector2.zeroVector, 0.3f);
			MaximizePanelPos = Vector2.Lerp(MaximizePanelPos, Vector2.zeroVector, 0.3f);
		}
		else
		{
			ClosePanelPos = Vector2.Lerp(ClosePanelPos, PanelPos0, 0.3f);
			RemovePanelPos = Vector2.Lerp(RemovePanelPos, PanelPos1, 0.3f);
			ClearPanelPos = Vector2.Lerp(ClearPanelPos, PanelPos2, 0.3f);
			CookPanelPos = Vector2.Lerp(CookPanelPos, PanelPos3, 0.3f);
			MaximizePanelPos = Vector2.Lerp(MaximizePanelPos, PanelPos4, 0.3f);
		}

		base.Update();
	}

	public override void Cook()
	{
		CuisineType = CheckCuisineType().Item1;
		CuisineNum = CheckCuisineType().Item2;
		base.Cook();
	}

	public override void DrawMainPanel()
	{
		Vector2 drawPos = GetDrawPos();
		Texture2D casserole = ModAsset.CasseroleUIPanel.Value;
		Main.spriteBatch.Draw(casserole, drawPos, null, Color.White, 0, casserole.Size() * 0.5f, 2, SpriteEffects.None, 0);
	}

	public bool CanCookWithOrder(CookingUnitWithOrder cookingUnitwithorder)
	{
		for (int index = 0; index < cookingUnitwithorder.Ingredients.Length; index++)
		{
			if (!cookingUnitwithorder.Ingredients[index].Contains(Ingredients[index]))
			{
				return false;
			}
		}
		return true;
	}

	public bool CanCook(CookingUnit cookingUnit)
	{
		List<int[]> ingredientsCopy = [.. cookingUnit.Ingredients];
		List<int> CopyInUI = Ingredients.ToList();
		// for (int i = 0; i < ingredientsCopy.Count(); i++)
		// {
		// Main.NewText(ingredientsCopy[i][0]);
		// }

		foreach (int type in Ingredients)
		{
			if (type == -1) // 如果槽内为空则不管
			{
				continue;
			}
			else // 如果槽内有东西则检测是否在配方组中
			{
				foreach (int[] group in cookingUnit.Ingredients)
				{
					if (group.Contains(type))
					{
						if (!ingredientsCopy.Remove(group)) // 如果配方组里面没有则return false
						{
							return false;
						}
						CopyInUI.Remove(type);
						break;
					}
				}
			}
		}
		CopyInUI.RemoveAll(n => n == -1);

		if (ingredientsCopy.Count == 0 && CopyInUI.Count == 0)
		{
			return true;
		}
		return false;
	}

	public Tuple<int, int> CheckCuisineType()
	{
		foreach (var cookingUnitwithorder in PotMenuWithOrder)
		{
			if (CanCookWithOrder(cookingUnitwithorder))
			{
				return new Tuple<int, int>(cookingUnitwithorder.Type, cookingUnitwithorder.Num);
			}
		}
		foreach (var cookingUnit in PotMenu)
		{
			if (CanCook(cookingUnit))
			{
				return new Tuple<int, int>(cookingUnit.Type, cookingUnit.Num);
			}
		}
		return new Tuple<int, int>(-1, -1);
	}
}
