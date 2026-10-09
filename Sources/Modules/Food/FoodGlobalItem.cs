using Terraria.Localization;
using Terraria.ModLoader;
using Everglow.Food.Buffs.VanillaFoodBuffs;
using Everglow.Food.Items;
using Everglow.Food.FoodUtilities;

namespace Everglow.Food;

public class FoodGlobalItem : GlobalItem
{
	// 对于原版的食物进行类型Id到 FoodInfo 的映射，直接获取FoodInfo实例
	public static Dictionary<int, FoodInfo> m_vanillaFoodInfos;
	public override void Unload()
	{
		m_vanillaFoodInfos = null;
	}
	public FoodGlobalItem()
	{
		m_vanillaFoodInfos = new Dictionary<int, FoodInfo>
		{
                // 苹果
                {
				ItemID.Apple,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<AppleBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "AppleBuff",
				}
			},
                // 苹果派
                {
				ItemID.ApplePie,
				new FoodInfo() {
					Satiety = 15,
					BuffType = ModContent.BuffType<ApplePieBuff>(),
					BuffTime = new FoodDuration(6, 0, 0),
					Name = "ApplePieBuff",
				}
			},
                // 杏
                {
				ItemID.Apricot,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<ApricotBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "ApricotBuff",
				}
			},
                // 培根
                {
				ItemID.Bacon,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<BaconBuff>(),
					BuffTime = new FoodDuration(8, 0, 0),
					Name = "BaconBuff",
				}
			},
                // 香蕉
                {
				ItemID.Banana,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<BananaBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "BaconBuff",
				}
			},
                // 香蕉船
                {
				ItemID.BananaSplit,
				new FoodInfo() {
					Satiety = 15,
					BuffType = ModContent.BuffType<BananaSplitBuff>(),
					BuffTime = new FoodDuration(6, 0, 0),
					Name = "BananaSplitBuff",
				}
			},
                // 烧烤肋排
                {
				ItemID.BBQRibs,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<BBQRibsBuff>(),
					BuffTime = new FoodDuration(8, 0, 0),
					Name = "BBQRibsBuff",
				}
			},
                // 黑醋栗
                {
				ItemID.BlackCurrant,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<BlackCurrantBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "BlackCurrantBuff",
				}
			},
                // 血橙
                {
				ItemID.BloodOrange,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<BloodOrangeBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "BloodOrangeBuff",
				}
			},
                // 鱼菇汤
                {
				ItemID.BowlofSoup,
				new FoodInfo() {
					Satiety = 15,
					BuffType = ModContent.BuffType<BowlofSoupBuff>(),
					BuffTime = new FoodDuration(6, 0, 0),
					Name = "BowlofSoupBuff",
				}
			},
                // 炖兔兔
                {
				ItemID.BunnyStew,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<BunnyStewBuff>(),
					BuffTime = new FoodDuration(8, 0, 0),
					Name = "BunnyStewBuff",
				}
			},
                // 汉堡
                {
				ItemID.Burger,
				new FoodInfo() {
					Satiety = 15,
					BuffType = ModContent.BuffType<BurgerBuff>(),
					BuffTime = new FoodDuration(6, 0, 0),
					Name = "BurgerBuff",
				}
			},
                // 樱桃
                {
				ItemID.Cherry,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<CherryBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "CherryBuff",
				}
			},
                // 鸡块
                {
				ItemID.ChickenNugget,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<ChickenNuggetBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "ChickenNuggetBuff",
				}
			},
                // 巧克力曲奇饼干
                {
				ItemID.ChocolateChipCookie,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<ChocolateChipCookieBuff>(),
					BuffTime = new FoodDuration(4, 0, 0 ),
					Name = "ChocolateChipCookieBuff",
				}
			},
                // 圣诞布丁
                {
				ItemID.ChristmasPudding,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<ChristmasPuddingBuff>(),
					BuffTime = new FoodDuration(6, 0, 0),
					Name = "ChristmasPuddingBuff",
				}
			},
                // 椰子
                {
				ItemID.Coconut,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<CoconutBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "CoconutBuff",
				}
			},
                // 熟鱼
                {
				ItemID.CookedFish,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<CookedFishBuff>(),
					BuffTime = new FoodDuration(8, 0, 0),
					Name = "CookedFishBuff",
				}
			},
                // 熟棉花糖
                {
				ItemID.CookedMarshmallow,
				new FoodInfo() {
					Satiety = 5,
					BuffType = ModContent.BuffType<CookedMarshmallowBuff>(),
					BuffTime = new FoodDuration(5, 0, 0),
					Name = "CookedMarshmallowBuff",
				}
			},
                // 熟虾
                {
				ItemID.CookedShrimp,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<CookedShrimpBuff>(),
					BuffTime = new FoodDuration(8, 0, 0),
					Name = "CookedShrimpBuff",
				}
			},
                // 火龙果
                {
				ItemID.Dragonfruit,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<DragonfruitBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "DragonfruitBuff",
				}
			},
                // 接骨木果
                {
				ItemID.Elderberry,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<ElderberryBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "ElderberryBuff",
				}
			},
                // 食用蜗牛
                {
				ItemID.Escargot,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<EscargotBuff>(),
					BuffTime = new FoodDuration(8, 0, 0),
					Name = "EscargotBuff",
				}
			},
                // 煎蛋
                {
				ItemID.FriedEgg,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<FriedEggBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "FriedEggBuff",
				}
			},
                // 薯条
                {
				ItemID.Fries,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<FriesBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "FriesBuff",
				}
			},
                // 蛙腿三明治
                {
				ItemID.FroggleBunwich,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<FroggleBunwichBuff>(),
					BuffTime = new FoodDuration(8, 0, 0),
					Name = "FroggleBunwichBuff",
				}
			},
                // 水果色拉
                {
				ItemID.FruitSalad,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<FruitSaladBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "FruitSaladBuff",
				}
			},
                // 姜饼
                {
				ItemID.GingerbreadCookie,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<GingerbreadCookieBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "GingerbreadCookieBuff",
				}
			},
                // 金美味
                {
				ItemID.GoldenDelight,
				new FoodInfo() {
					Satiety = 30,
					BuffType = ModContent.BuffType<GoldenDelightBuff>(),
					BuffTime = new FoodDuration(10, 0, 0),
					Name = "GoldenDelightBuff",
				}
			},
                // 葡萄柚
                {
				ItemID.Grapefruit,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<GrapefruitBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "GrapefruitBuff",
				}
			},
                // 葡萄
                {
				ItemID.Grapes,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<GrapesBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "GrapesBuff",
				}
			},
                // 烤松鼠
                {
				ItemID.GrilledSquirrel,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<GrilledSquirrelBuff>(),
					BuffTime = new FoodDuration(8, 0, 0),
					Name = "GrilledSquirrelBuff",
				}
			},
                // 蛆虫汤
                {
				ItemID.GrubSoup,
				new FoodInfo() {
					Satiety = 15,
					BuffType = ModContent.BuffType<GrubSoupBuff>(),
					BuffTime = new FoodDuration(6, 0, 0),
					Name = "GrubSoupBuff",
				}
			},
                // 热狗
                {
				ItemID.Hotdog,
				new FoodInfo() {
					Satiety = 15,
					BuffType = ModContent.BuffType<HotdogBuff>(),
					BuffTime = new FoodDuration(6, 0, 0),
					Name = "HotdogBuff",
				}
			},
                // 冰淇淋
                {
				ItemID.IceCream,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<IceCreamBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "IceCreamBuff",
				}
			},
                // 柠檬
                {
				ItemID.Lemon,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<LemonBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "LemonBuff",
				}
			},
                // 龙虾尾
                {
				ItemID.LobsterTail,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<LobsterTailBuff>(),
					BuffTime = new FoodDuration(8, 0, 0),
					Name = "LobsterTailBuff",
				}
			},
                // 芒果
                {
				ItemID.Mango,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<MangoBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "MangoBuff",
				}
			},
                // 棉花糖
                {
				ItemID.Marshmallow,
				new FoodInfo() {
					Satiety = 5,
					BuffType = ModContent.BuffType<MarshmallowBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "MarshmallowBuff",
				}
			},
                // 怪物三明治
                {
				ItemID.MonsterLasagna,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<MonsterLasagnaBuff>(),
					BuffTime = new FoodDuration(8, 0, 0),
					Name = "MonsterLasagnaBuff",
				}
			},
                // 玉米片
                {
				ItemID.Nachos,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<NachosBuff>(),
					BuffTime = new FoodDuration(8, 0, 0),
					Name = "NachosBuff",
				}
			},
                // 泰式炒面
                {
				ItemID.PadThai,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<PadThaiBuff>(),
					BuffTime = new FoodDuration(8, 0, 0),
					Name = "PadThaiBuff",
				}
			},
                // 桃子
                {
				ItemID.Peach,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<PeachBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "PeachBuff",
				}
			},
                // 越南河粉
                {
				ItemID.Pho,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<PhoBuff>(),
					BuffTime = new FoodDuration(8, 0, 0),
					Name = "PhoBuff",
				}
			},
                // 菠萝
                {
				ItemID.Pineapple,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<PineappleBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "PineappleBuff",
				}
			},
                // 披萨
                {
				ItemID.Pizza,
				new FoodInfo() {
					Satiety = 15,
					BuffType = ModContent.BuffType<PizzaBuff>(),
					BuffTime = new FoodDuration(6, 0, 0),
					Name = "PizzaBuff",
				}
			},
                // 李子
                {
				ItemID.Plum,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<PlumBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "PlumBuff",
				}
			},
                // 薯片
                {
				ItemID.PotatoChips,
				new FoodInfo() {
					Satiety = 15,
					BuffType = ModContent.BuffType<PotatoChipsBuff>(),
					BuffTime = new FoodDuration(6, 0, 0),
					Name = "PotatoChipsBuff",
				}
			},
                // 南瓜派
                {
				ItemID.PumpkinPie,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<PumpkinPieBuff>(),
					BuffTime = new FoodDuration(8, 0, 0),
					Name = "PumpkinPieBuff",
				}
			},
                // 红毛丹
                {
				ItemID.Rambutan,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<RambutanBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "RambutanBuff",
				}
			},
                // 烤鸟
                {
				ItemID.RoastedBird,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<RoastedBirdBuff>(),
					BuffTime = new FoodDuration(8, 0, 0),
					Name = "RoastedBirdBuff",
				}
			},
                // 烤鸭
                {
				ItemID.RoastedDuck,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<RoastedDuckBuff>(),
					BuffTime = new FoodDuration(8, 0, 0),
					Name = "RoastedDuckBuff",
				}
			},
                // 生鱼片
                {
				ItemID.Sashimi,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<SashimiBuff>(),
					BuffTime = new FoodDuration(8, 0, 0),
					Name = "SashimiBuff",
				}
			},
                // 炒蛙腿
                {
				ItemID.SauteedFrogLegs,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<SauteedFrogLegsBuff>(),
					BuffTime = new FoodDuration(8, 0, 0),
					Name = "SauteedFrogLegsBuff",
				}
			},
                // 海鲜大餐
                {
				ItemID.SeafoodDinner,
				new FoodInfo() {
					Satiety = 30,
					BuffType = ModContent.BuffType<SeafoodDinnerBuff>(),
					BuffTime = new FoodDuration(10, 0, 0),
					Name = "SeafoodDinnerBuff",
				}
			},
                // 鲜虾三明治
                {
				ItemID.ShrimpPoBoy,
				new FoodInfo() {
					Satiety = 15,
					BuffType = ModContent.BuffType<ShrimpPoBoyBuff>(),
					BuffTime = new FoodDuration(6, 0, 0),
					Name = "ShrimpPoBoyBuff",
				}
			},
                // 去壳牡蛎
                {
				ItemID.ShuckedOyster,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<ShuckedOysterBuff>(),
					BuffTime = new FoodDuration(6, 0, 0),
					Name = "ShuckedOysterBuff",
				}
			},
                // 意大利面
                {
				ItemID.Spaghetti,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<SpaghettiBuff>(),
					BuffTime = new FoodDuration(8, 0, 0),
					Name = "SpaghettiBuff",
				}
			},
                // 杨桃
                {
				ItemID.Starfruit,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<StarfruitBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "StarfruitBuff",
				}
			},
                // 牛排
                {
				ItemID.Steak,
				new FoodInfo() {
					Satiety = 20,
					BuffType = ModContent.BuffType<SteakBuff>(),
					BuffTime = new FoodDuration(8, 0, 0),
					Name = "SteakBuff",
				}
			},
                // 蜜糖饼干
                {
				ItemID.SugarCookie,
				new FoodInfo() {
					Satiety = 10,
					BuffType = ModContent.BuffType<SugarCookieBuff>(),
					BuffTime = new FoodDuration(4, 0, 0),
					Name = "SugarCookieBuff",
				}
			},
		};
	}
	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
	{
		if (m_vanillaFoodInfos.ContainsKey(item.type) /*|| (item.ModItem is FoodBase)*/)
		{
			int firstIndex = -1;
			firstIndex = tooltips.FindIndex((tpline) =>
			{
				return tpline.Name.Contains("Tooltip");
			});
			// 如果有tooltip，就删掉所有Tooltip的line然后插入到第一个所在位置
			var FoodInfo = m_vanillaFoodInfos[item.type];
			if (firstIndex >= 0)
			{
				tooltips.RemoveAll((tp) => tp.Name.Contains("Tooltip"));
				tooltips.Insert(firstIndex, new TooltipLine(Mod, item.Name, Language.GetTextValue("Mods.Everglow.BuffDescription." + FoodInfo.Name)));
				tooltips.Insert(firstIndex, new TooltipLine(Mod, item.Name, FoodInfo.Satiety + Language.GetTextValue("Mods.Everglow.Common.FoodSystem.Satiety")));
			}
			else
			{
				// 否则加到最后面
				tooltips.Add(new TooltipLine(Mod, item.Name, Language.GetTextValue("Mods.Everglow.BuffDescription." + FoodInfo.Name)));
				tooltips.Add(new TooltipLine(Mod, item.Name, FoodInfo.Satiety + Language.GetTextValue("Mods.Everglow.Common.FoodSystem.Satiety")));
			}

			int buffTimeIndex = tooltips.FindIndex((tp) => tp.Name.Contains("BuffTime"));
			if (buffTimeIndex != -1)
			{
				tooltips[buffTimeIndex].Text = FoodInfo.BuffTime.ToBuffTimeString();
			}
		}
		if (item.ModItem is FoodBase)
		{
			int firstIndex = -1;
			firstIndex = tooltips.FindIndex((tpline) =>
			{
				return tpline.Name.Contains("Tooltip");
			});
			// 如果有tooltip，就删掉所有Tooltip的line然后插入到第一个所在位置
			var foodItem = item.ModItem as FoodBase;
			var FoodInfo = foodItem.FoodInfo;
			if (firstIndex >= 0)
			{
				tooltips.RemoveAll((tp) => tp.Name.Contains("Tooltip"));
				tooltips.Insert(firstIndex, new TooltipLine(Mod, item.Name, Language.GetTextValue("Mods.Everglow.BuffDescription." + FoodInfo.Name)));
				tooltips.Insert(firstIndex, new TooltipLine(Mod, item.Name, FoodInfo.Satiety + Language.GetTextValue("Mods.Everglow.Common.FoodSystem.Satiety")));
			}
			else
			{
				// 否则加到最后面
				tooltips.Add(new TooltipLine(Mod, item.Name, Language.GetTextValue("Mods.Everglow.BuffDescription." + FoodInfo.Name)));
				tooltips.Add(new TooltipLine(Mod, item.Name, FoodInfo.Satiety + Language.GetTextValue("Mods.Everglow.Common.FoodSystem.Satiety")));
			}

			int buffTimeIndex = tooltips.FindIndex((tp) => tp.Name.Contains("BuffTime"));
			if (buffTimeIndex != -1)
			{
				tooltips[buffTimeIndex].Text = FoodInfo.BuffTime.ToBuffTimeString();
			}
		}
	}
	public override void SetStaticDefaults()
	{
	}

	public override void SetDefaults(Item item)
	{
		// 如果是原版的食物，那么就手动处理
		if (m_vanillaFoodInfos.ContainsKey(item.type))
		{
			var FoodInfo = m_vanillaFoodInfos[item.type];
		}
		base.SetDefaults(item);
	}

	public override void OnConsumeItem(Item item, Player player)
	{
		// 如果是原版的食物，那么就手动处理，因为已经使用了物品，说明玩家满足饱食度要求
		if (m_vanillaFoodInfos.ContainsKey(item.type))
		{
			var FoodInfo = m_vanillaFoodInfos[item.type];
			var FoodPlayer = player.GetModPlayer<FoodModPlayer>();

			// 增加饱食度
			FoodPlayer.CurrentSatiety += FoodInfo.Satiety;
			// 加上Buff
			player.AddBuff(FoodInfo.BuffType, FoodInfo.BuffTime.TotalFrames);
		}
		else if (item.ModItem is FoodBase)
		{
			var foodItem = item.ModItem as FoodBase;
			var FoodInfo = foodItem.FoodInfo;
			var FoodPlayer = player.GetModPlayer<FoodModPlayer>();
			// 增加饱食度
			FoodPlayer.CurrentSatiety += FoodInfo.Satiety;
			// 加上Buff
			player.AddBuff(FoodInfo.BuffType, FoodInfo.BuffTime.TotalFrames);
		}
	}



	public override bool CanUseItem(Item item, Player player)
	{
		var foodPlayer = player.GetModPlayer<FoodModPlayer>();
		// 判断能否吃下物品
		if (m_vanillaFoodInfos.ContainsKey(item.type))
		{
			var FoodInfo = m_vanillaFoodInfos[item.type];
			if (!foodPlayer.CanEat(FoodInfo) && foodPlayer.CanText())
			{
				CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height),
				new Color(255, 0, 0),
				Language.GetTextValue("Mods.Everglow.Common.FoodSystem.CannotEat"),
				true, false);

				foodPlayer.TextTimer = FoodUtils.GetFrames(0, 0, 2, 30);
				return false;
			}
		}
		else if (item.ModItem is FoodBase)
		{
			var foodItem = item.ModItem as FoodBase;
			var FoodInfo = foodItem.FoodInfo;
			if (!foodPlayer.CanEat(FoodInfo) && foodPlayer.CanText())
			{
				CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height),
				new Color(255, 0, 0),
				Language.GetTextValue("Mods.Everglow.Common.FoodSystem.CannotEat"),
				true, false);

				foodPlayer.TextTimer = FoodUtils.GetFrames(0, 0, 2, 30);
				return false;
			}
		}
		return base.CanUseItem(item, player);
	}

	public override bool ConsumeItem(Item item, Player player)
	{
		var foodPlayer = player.GetModPlayer<FoodModPlayer>();
		// 判断能否吃下物品
		if (m_vanillaFoodInfos.ContainsKey(item.type))
		{
			var foodInfo = m_vanillaFoodInfos[item.type];
			if (!foodPlayer.CanEat(foodInfo))
			{
				// Main.NewText($"Cannot eat this!");
				return false;
			}
		}
		else if (item.ModItem is FoodBase)
		{
			var foodItem = item.ModItem as FoodBase;
			var foodInfo = foodItem.FoodInfo;
			if (!foodPlayer.CanEat(foodInfo))
			{
				// Main.NewText($"Cannot eat this!");
				return false;
			}
		}
		return true;
	}
}
