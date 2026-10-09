using Terraria.Localization;
using Terraria.ModLoader;
using Everglow.Food.Buffs.VanillaDrinkBuffs;
using Everglow.Food.Items;
using Everglow.Food.FoodUtilities;

namespace Everglow.Food;

public class DrinkGlobalItem : GlobalItem
{
	// 对于原版的饮料进行类型Id到 DrinkInfo 的映射，直接获取DrinkInfo实例
	public static Dictionary<int, DrinkInfo> m_vanillaDrinkInfos;

	public override void Unload()
	{
		m_vanillaDrinkInfos = null;
	}

	public DrinkGlobalItem()
	{
		m_vanillaDrinkInfos = new Dictionary<int, DrinkInfo>
		{
			// 麦芽酒
			{
				ItemID.Ale,
				new DrinkInfo()
				{
					Thirsty = false,
					BuffType = ModContent.BuffType<AleBuff> (),
					BuffTime = new FoodDuration(0, 10, 0),
					Name =  "SakeBuff",
				}
			},
			// 苹果汁
			{
				ItemID.AppleJuice,
				new DrinkInfo()
				{
					Thirsty = false,
					BuffType = ModContent.BuffType<AppleJuiceBuff>(),
					BuffTime = new FoodDuration(0, 10, 0),
					Name =  "AppleJuiceBuff",
				}
			},
			// 冰冻香蕉代基里
			{
				ItemID.BananaDaiquiri,
				new DrinkInfo()
				{
					Thirsty = false,
					BuffType = ModContent.BuffType<BananaDaiquiriBuff>(),
					BuffTime = new FoodDuration(0, 10, 0),
					Name = "BananaDaiquiriBuff",
				}
			},
			// 血腥麝香葡萄
			{
				ItemID.BloodyMoscato,
				new DrinkInfo()
				{
					Thirsty = false,
					BuffType = ModContent.BuffType<BloodyMoscatoBuff>(),
					BuffTime = new FoodDuration(0, 10, 0),
					Name = "BloodyMoscatoBuff",
				}
			},
			// 奶油苏打水
			{
				ItemID.CreamSoda,
				new DrinkInfo()
				{
					Thirsty = false,
					BuffType = ModContent.BuffType<CreamSodaBuff>(),
					BuffTime = new FoodDuration(0, 10, 0),
					Name = "CreamSodaBuff",
				}
			},
			// 咖啡
			{
				ItemID.CoffeeCup,
				new DrinkInfo()
				{
					Thirsty = false,
					BuffType = ModContent.BuffType<CoffeeCupBuff>(),
					BuffTime = new FoodDuration(0, 30, 0),
					Name = "CoffeeCupBuff",
				}
			},
			// 果汁
			{
				ItemID.FruitJuice,
				new DrinkInfo()
				{
					Thirsty = false,
					BuffType = ModContent.BuffType<FruitJuiceBuff>(),
					BuffTime = new FoodDuration(0, 10, 0),
					Name = "FruitJuiceBuff",
				}
			},
			// 葡萄汁
			{
				ItemID.GrapeJuice,
				new DrinkInfo()
				{
					Thirsty = false,
					BuffType = ModContent.BuffType<GrapeJuiceBuff>(),
					BuffTime = new FoodDuration(0, 10, 0),
					Name = "GrapeJuiceBuff",
				}
			},
			// 柠檬水
			{
				ItemID.Lemonade,
				new DrinkInfo()
				{
					Thirsty = false,
					BuffType = ModContent.BuffType<LemonadeBuff>(),
					BuffTime = new FoodDuration(0, 10, 0),
					Name = "LemonadeBuff",
				}
			},
			// 盒装牛奶
			{
				ItemID.MilkCarton,
				new DrinkInfo()
				{
					Thirsty = false,
					BuffType = ModContent.BuffType<MilkCartonBuff>(),
					BuffTime = new FoodDuration(0, 10, 0),
					Name = "MilkCartonBuff",
				}
			},
			// 奶昔
			{
				ItemID.Milkshake,
				new DrinkInfo()
				{
					Thirsty = false,
					BuffType = ModContent.BuffType<MilkshakeBuff>(),
					BuffTime = new FoodDuration(0, 10, 0),
					Name = "MilkshakeBuff",
				}
			},
			// 桃子果酒
			{
				ItemID.PeachSangria,
				new DrinkInfo()
				{
					Thirsty = false,
					BuffType = ModContent.BuffType<PeachSangriaBuff>(),
					BuffTime = new FoodDuration(0, 10, 0),
					Name = "PeachSangriaBuff",
				}
			},
			// 椰林飘香
			{
				ItemID.PinaColada,
				new DrinkInfo()
				{
					Thirsty = false,
					BuffType = ModContent.BuffType<PinaColadaBuff>(),
					BuffTime = new FoodDuration(0, 10, 0),
					Name = "PinaColadaBuff",
				}
			},
			// 七彩潘趣酒
			{
				ItemID.PrismaticPunch,
				new DrinkInfo()
				{
					Thirsty = false,
					BuffType = ModContent.BuffType<PrismaticPunchBuff>(),
					BuffTime = new FoodDuration(0, 10, 0),
					Name = "PrismaticPunchBuff",
				}
			},
			// 清酒
			{
				ItemID.Sake,
				new DrinkInfo()
				{
					Thirsty = false,
					BuffType = ModContent.BuffType<SakeBuff>(),
					BuffTime = new FoodDuration(0, 10, 0),
					Name = "SakeBuff",
				}
			},
			// 暗黑奶昔
			{
				ItemID.SmoothieofDarkness,
				new DrinkInfo()
				{
					Thirsty = false,
					BuffType = ModContent.
					BuffType<SmoothieofDarknessBuff>(),
					BuffTime = new FoodDuration(0, 10, 0),
					Name = "SmoothieofDarknessBuff",
				}
			},
			// 一杯茶
			{
				ItemID.Teacup,
				new DrinkInfo()
				{
					Thirsty = false,
					BuffType = ModContent.BuffType<TeacupBuff>(),
					BuffTime = new FoodDuration(0, 10, 0),
					Name = "TeacupBuff",
				}
			},
			// 热带奶昔
			{
				ItemID.TropicalSmoothie,
				new DrinkInfo()
				{
					Thirsty = false,
					BuffType = ModContent.BuffType<TropicalSmoothieBuff>(),
					BuffTime = new FoodDuration(0, 10, 0),
					Name = "TropicalSmoothieBuff",
				}
			},
		};
	}

	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
	{
		if (m_vanillaDrinkInfos.ContainsKey(item.type) /*|| (item.ModItem is DrinkBase)*/)
		{
			int firstIndex = -1;
			firstIndex = tooltips.FindIndex((tpline) =>
			{
				return tpline.Name.Contains("Tooltip");
			});
			// 如果有tooltip，就删掉所有Tooltip的line然后插入到第一个所在位置
			var DrinkInfo = m_vanillaDrinkInfos[item.type];
			if (firstIndex >= 0)
			{
				tooltips.RemoveAll((tp) => tp.Name.Contains("Tooltip"));
				tooltips.Insert(firstIndex, new TooltipLine(Mod, item.Name, Language.GetTextValue("Mods.Everglow.BuffDescription." + DrinkInfo.Name)));
			}
			else
			{
				// 否则加到最后面
				tooltips.Add(new TooltipLine(Mod, item.Name, Language.GetTextValue("Mods.Everglow.BuffDescription." + DrinkInfo.Name)));
			}

			int buffTimeIndex = tooltips.FindIndex((tp) => tp.Name.Contains("BuffTime"));
			if (buffTimeIndex != -1)
			{
				tooltips[buffTimeIndex].Text = DrinkInfo.BuffTime.ToBuffTimeString();
			}
		}
		if (item.ModItem is DrinkBase)
		{
			int firstIndex = -1;
			firstIndex = tooltips.FindIndex((tpline) =>
			{
				return tpline.Name.Contains("Tooltip");
			});
			// 如果有tooltip，就删掉所有Tooltip的line然后插入到第一个所在位置
			var drinkItem = item.ModItem as DrinkBase;
			var drinkInfo = drinkItem.DrinkInfo;
			if (firstIndex >= 0)
			{
				tooltips.RemoveAll((tp) => tp.Name.Contains("Tooltip"));
				tooltips.Insert(firstIndex, new TooltipLine(Mod, item.Name, Language.GetTextValue("Mods.Everglow.BuffDescription." + drinkInfo.Name)));
			}
			else
			{
				// 否则加到最后面
				tooltips.Add(new TooltipLine(Mod, item.Name, Language.GetTextValue("Mods.Everglow.BuffDescription." + drinkInfo.Name)));
			}

			int buffTimeIndex = tooltips.FindIndex((tp) => tp.Name.Contains("BuffTime"));
			if (buffTimeIndex != -1)
			{
				tooltips[buffTimeIndex].Text = drinkInfo.BuffTime.ToBuffTimeString();
			}
		}
	}

	public override void SetStaticDefaults()
	{
	}

	public override void SetDefaults(Item item)
	{
		// 如果是原版的饮料，那么就手动处理
		if (m_vanillaDrinkInfos.ContainsKey(item.type))
		{
			var drinkInfo = m_vanillaDrinkInfos[item.type];
		}
		base.SetDefaults(item);
	}

	public override void OnConsumeItem(Item item, Player player)
	{
		// 如果是原版的饮料，那么就手动处理，因为已经使用了物品，说明玩家满足饱食度要求
		if (m_vanillaDrinkInfos.ContainsKey(item.type))
		{
			var drinkInfo = m_vanillaDrinkInfos[item.type];
			var FoodPlayer = player.GetModPlayer<FoodModPlayer>();

			// 变得不渴
			FoodPlayer.Thirstystate = drinkInfo.Thirsty;
			// 加上Buff
			player.AddBuff(drinkInfo.BuffType, drinkInfo.BuffTime.TotalFrames);
		}
		else if (item.ModItem is DrinkBase)
		{
			var drinkItem = item.ModItem as DrinkBase;
			var drinkInfo = drinkItem.DrinkInfo;
			var FoodPlayer = player.GetModPlayer<FoodModPlayer>();
			// 变得不渴
			FoodPlayer.Thirstystate = drinkInfo.Thirsty;
			// 加上Buff
			player.AddBuff(drinkInfo.BuffType, drinkInfo.BuffTime.TotalFrames);
		}
	}

	public override bool CanUseItem(Item item, Player player)
	{
		bool CanText = true;
		var foodPlayer = player.GetModPlayer<FoodModPlayer>();
		// 判断能否喝下物品
		if (m_vanillaDrinkInfos.ContainsKey(item.type) && CanText == true)
		{
			var drinkInfo = m_vanillaDrinkInfos[item.type];
			if (!foodPlayer.CanDrink(drinkInfo) && foodPlayer.CanText())
			{
				CombatText.NewText(
					new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height),
					new Color(255, 0, 0),
					Language.GetTextValue("Mods.Everglow.Common.FoodSystem.CannotDrink"),
					true, false);

				foodPlayer.TextTimer = FoodUtils.GetFrames(0, 0, 2, 30);
				return false;
			}
		}
		else if (item.ModItem is DrinkBase && CanText == true)
		{
			var foodItem = item.ModItem as DrinkBase;
			var drinkInfo = foodItem.DrinkInfo;
			if (!foodPlayer.CanDrink(drinkInfo) && foodPlayer.CanText())
			{
				CombatText.NewText(
					new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height),
					new Color(255, 0, 0),
					Language.GetTextValue("Mods.Everglow.Common.FoodSystem.CannotDrink"),
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
		// 判断能否喝下物品
		if (m_vanillaDrinkInfos.ContainsKey(item.type))
		{
			var drinkInfo = m_vanillaDrinkInfos[item.type];
			if (!foodPlayer.CanDrink(drinkInfo))
			{
				// Main.NewText(Language.GetTextValue("Mods.Everglow.Common.FoodSystem.CannotDrink"));

				return false;
			}
		}
		else if (item.ModItem is DrinkBase)
		{
			var foodItem = item.ModItem as DrinkBase;
			var drinkInfo = foodItem.DrinkInfo;
			if (!foodPlayer.CanDrink(drinkInfo))
			{
				// Main.NewText(Language.GetTextValue("Mods.Everglow.Common.FoodSystem.CannotDrink"));
				return false;
			}
		}
		return base.ConsumeItem(item, player);
	}
}
