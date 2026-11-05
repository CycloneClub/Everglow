using System.Reflection;
using System.Runtime.CompilerServices;
using Everglow.Food;
using Everglow.Food.FoodUtilities;
using Everglow.Food.Items;
using Everglow.Food.Buffs.VanillaDrinkBuffs;
using Everglow.Food.Buffs.VanillaFoodBuffs;
using Everglow.UnitTests.Function;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules;

[TestClass]
[DoNotParallelize]
public class FoodContentLoadingTests
{
	[TestInitialize]
	public void Initialize() => Program.SavePath = string.Empty;

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ConsumptionResolvesBuffRegisteredAfterGlobalConstruction(bool drink)
	{
		Type buffClass = drink ? typeof(AppleJuiceBuff) : typeof(AppleBuff);
		var instance = typeof(ContentInstance<>).MakeGenericType(buffClass).GetProperty("Instance")!;
		object? originalInstance = instance.GetValue(null);
		var originalFood = FoodGlobalItem.VanillaFoodInfos;
		var originalDrink = DrinkGlobalItem.VanillaDrinkInfos;
		try
		{
			// ModuleManager constructs the GlobalItem before registering the food Buffs.
			instance.SetValue(null, null);
			GlobalItem global = drink ? new DrinkGlobalItem() : new FoodGlobalItem();
			global.SetDefaults(new Item { type = drink ? ItemID.AppleJuice : ItemID.Apple });
			var buff = (ModBuff)Activator.CreateInstance(buffClass)!;
			// Use a valid native slot so Player.AddBuff needs no fake content arrays.
			typeof(ModBuff).GetProperty(nameof(ModBuff.Type))!.SetValue(buff, BuffID.Regeneration);
			instance.SetValue(null, buff);
			global.SetStaticDefaults();

			int itemType = drink ? ItemID.AppleJuice : ItemID.Apple;
			int mappedType = drink ? DrinkGlobalItem.VanillaDrinkInfos[itemType].BuffType : FoodGlobalItem.VanillaFoodInfos[itemType].BuffType;
			Assert.AreEqual(buff.Type, mappedType, "The mapping must use the registered Buff ID, not an early zero snapshot.");

			using var context = new CooldownTestContext();
			var foodPlayer = context.Attach<FoodModPlayer>();
			foodPlayer.Thirstystate = true;
			global.OnConsumeItem(new Item { type = itemType }, context.Player);
			Assert.IsTrue(context.Player.HasBuff(buff.Type), "Real consumption must grant the mapped Buff.");
			int expectedDuration = drink ? DrinkGlobalItem.VanillaDrinkInfos[itemType].BuffTime.TotalFrames : FoodGlobalItem.VanillaFoodInfos[itemType].BuffTime.TotalFrames;
			Assert.AreEqual(expectedDuration, context.Player.buffTime[context.Player.FindBuffIndex(buff.Type)]);
			if (drink)
			{
				Assert.IsFalse(foodPlayer.Thirstystate);
			}
			else
			{
				Assert.AreEqual(10, foodPlayer.CurrentSatiety);
			}
		}
		finally
		{
			instance.SetValue(null, originalInstance);
			FoodGlobalItem.VanillaFoodInfos = originalFood;
			DrinkGlobalItem.VanillaDrinkInfos = originalDrink;
		}
	}

	[TestMethod]
	[DataRow(false, false, false)]
	[DataRow(false, false, true)]
	[DataRow(false, true, false)]
	[DataRow(false, true, true)]
	[DataRow(true, false, false)]
	[DataRow(true, false, true)]
	[DataRow(true, true, false)]
	[DataRow(true, true, true)]
	public void TooltipsUseTheRegisteredBuffDescription(bool drink, bool customItem, bool replaceTooltip)
	{
		var cacheField = typeof(Lang).GetField("_buffDescriptionCache", BindingFlags.Static | BindingFlags.NonPublic)!;
		var descriptions = (LocalizedText[])cacheField.GetValue(null)!;
		var previousDescription = descriptions[BuffID.Regeneration];
		var previousFood = FoodGlobalItem.VanillaFoodInfos;
		var previousDrink = DrinkGlobalItem.VanillaDrinkInfos;
		try
		{
			const string expected = "+40 mana";
			descriptions[BuffID.Regeneration] = (LocalizedText)Activator.CreateInstance(
				typeof(LocalizedText), BindingFlags.Instance | BindingFlags.NonPublic,
				null, ["Mods.Everglow.Buffs.CookedFishBuff.Description", expected], null)!;
			var mod = (TooltipTestMod)RuntimeHelpers.GetUninitializedObject(typeof(TooltipTestMod));
			GlobalItem global = drink ? new DrinkGlobalItem() : new FoodGlobalItem();
			typeof(ModType).GetProperty(nameof(ModType.Mod))!.SetValue(global, mod);
			var item = new Item { type = drink ? ItemID.AppleJuice : ItemID.Apple };
			item.SetNameOverride("Test food");
			var food = new TooltipFood();
			var beverage = new TooltipDrink();
			FoodGlobalItem.VanillaFoodInfos = new Dictionary<int, FoodInfo>();
			DrinkGlobalItem.VanillaDrinkInfos = new Dictionary<int, DrinkInfo>();
			if (customItem)
			{
				typeof(Item).GetProperty(nameof(Item.ModItem))!.SetValue(item, drink ? beverage : food);
			}
			else if (drink)
			{
				DrinkGlobalItem.VanillaDrinkInfos[item.type] = beverage.DrinkInfo;
			}
			else
			{
				FoodGlobalItem.VanillaFoodInfos[item.type] = food.FoodInfo;
			}
			var title = new TooltipLine(mod, "ItemName", "Test food");
			var lines = new List<TooltipLine> { title };
			if (replaceTooltip)
			{
				lines.Add(new TooltipLine(mod, "Tooltip0", "Old tooltip"));
			}
			global.ModifyTooltips(item, lines);
			Assert.AreEqual(1, lines.Count(line => line.Text == expected), "Use the registered Buff description instead of reconstructing a legacy key from Name.");
			Assert.IsFalse(lines.Any(line => line.Text == "Old tooltip" || line.Text.Contains("BuffDescription.")));
			Assert.AreSame(title, lines[0]);
		}
		finally
		{
			descriptions[BuffID.Regeneration] = previousDescription;
			FoodGlobalItem.VanillaFoodInfos = previousFood;
			DrinkGlobalItem.VanillaDrinkInfos = previousDrink;
		}
	}

	private sealed class TooltipTestMod : Mod
	{
		public override string Name => "Everglow";
	}

	private sealed class TooltipFood : FoodBase
	{
		public override FoodInfo FoodInfo => new()
		{
			BuffType = BuffID.Regeneration,
			BuffTime = new FoodDuration(60),
			Satiety = 5,
			Name = "LegacyFoodName",
		};
	}

	private sealed class TooltipDrink : DrinkBase
	{
		public override DrinkInfo DrinkInfo => new()
		{
			BuffType = BuffID.Regeneration,
			BuffTime = new FoodDuration(60),
			Name = "LegacyDrinkName",
		};
	}
}
