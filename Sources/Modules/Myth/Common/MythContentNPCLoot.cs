using Everglow.Myth.TheFirefly.Items.SummonItems;
using Everglow.Myth.TheFirefly.WorldGeneration;
using SubworldLibrary;
using Everglow.Myth.Misc.Items.Accessories;
using Everglow.Myth.Misc.Items.Weapons;
using Everglow.Myth.Misc.Items.Weapons.Slingshots;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Everglow.Myth.Common;

public class MythContentNPCLoot : GlobalNPC
{
	// SimpleItemDropRuleCondition 的谓词无法接收本次掉落的玩家。
	private sealed class JungleDropCondition : IItemDropRuleCondition
	{
		public bool CanDrop(DropAttemptInfo info) => info.player.ZoneJungle;

		public bool CanShowItemDropInUI() => true;

		public string GetConditionDescription() => Condition.InJungle.Description.Value;
	}

	public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
	{
		if (npc.type == NPCID.Nutcracker || npc.type == NPCID.NutcrackerSpinning)
		{
			npcLoot.Add(new DropBasedOnMasterMode(
				ItemDropRule.NormalvsExpert(ModContent.ItemType<WalnutClip>(), 360, 270),
				ItemDropRule.Common(ModContent.ItemType<WalnutClip>(), 180)));
		}

		if (npc.type == NPCID.Harpy)
		{
			npcLoot.Add(new DropBasedOnMasterMode(
				ItemDropRule.NormalvsExpert(ModContent.ItemType<FeatherMagic>(), 120, 90),
				ItemDropRule.Common(ModContent.ItemType<FeatherMagic>(), 60)));
		}

		if (npc.type == NPCID.KingSlime)
		{
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<GelSlingshot>(), 4));
		}


		if (npc.type == NPCID.PossessedArmor)
		{
			npcLoot.Add(new DropBasedOnMasterMode(
				ItemDropRule.NormalvsExpert(ModContent.ItemType<AmbiguousNight>(), 200, 150),
				ItemDropRule.Common(ModContent.ItemType<AmbiguousNight>(), 100)));
		}

		if (npc.type == NPCID.WyvernHead)
		{
			npcLoot.Add(new DropBasedOnMasterMode(
				ItemDropRule.NormalvsExpert(ModContent.ItemType<ThunderFlower>(), 140, 100),
				ItemDropRule.Common(ModContent.ItemType<ThunderFlower>(), 50)));
		}

		if (npc.type == NPCID.Wraith)
		{
			npcLoot.Add(new DropBasedOnMasterMode(
				ItemDropRule.NormalvsExpert(ModContent.ItemType<ComingGhost>(), 200, 150),
				ItemDropRule.Common(ModContent.ItemType<ComingGhost>(), 100)));
		}

		if (npc.type == NPCID.DukeFishron)
		{
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<DukeTooth>(), 3));
		}

		// WindMoveSeed 已迁移至 PlantAndFarm，获取规则由该模块另行处理。
		// if (npc.type == NPCID.Dandelion)
		// {
			// npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WindMoveSeed>(), 1, 3, 5));
		// }

		if (npc.netID == NPCID.BlueSlime)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BlueGel>(), 800));
		}

		if (npc.netID == NPCID.RedSlime)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RedGel>(), 800));
		}

		if (npc.netID == NPCID.GreenSlime)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GreenGel>(), 800));
		}

		if (npc.type == NPCID.Everscream)
		{
			foreach (var rule in npcLoot.Get().OfType<LeadingConditionRule>())
			{
				if (rule.condition is not Conditions.FrostMoonDropGatingChance)
				{
					continue;
				}
				foreach (var pool in rule.ChainedRules.SelectMany(chain => chain.RuleToChain.ChainedRules)
					.Select(chain => chain.RuleToChain).OfType<OneFromOptionsDropRule>())
				{
					if (pool.dropIds.Contains(ItemID.ChristmasTreeSword))
					{
						pool.dropIds = [.. pool.dropIds, ModContent.ItemType<XmasWhip>()];
					}
				}
			}
		}

		if (!npc.boss)
		{
			if (!npc.friendly && npc.lifeMax > 5 && !npc.CountsAsACritter)
			{
				npcLoot.Add(ItemDropRule.ByCondition(new JungleDropCondition(), ModContent.ItemType<JungleVitamin>(), 100));
				var mothWorld = new SimpleItemDropRuleCondition(
					Language.GetText("Mods.Everglow.Biomes.FireflyBiome.DisplayName"),
					() => SubworldSystem.IsActive<MothWorld>(), ShowItemDropInUI.Always);
				npcLoot.Add(ItemDropRule.ByCondition(mothWorld, ModContent.ItemType<EvilCocoon>(), 2500));
			}

			if (!npc.friendly && npc.lifeMax > 100)
			{
				npcLoot.Add(ItemDropRule.ByCondition(new EclipseMasterPostPlant(), ModContent.ItemType<GoldRoundYoyo>(), 40, 1, 1, 1));
				npcLoot.Add(ItemDropRule.ByCondition(new EclipseExpertPostPlant(), ModContent.ItemType<GoldRoundYoyo>(), 60, 1, 1, 1));
				npcLoot.Add(ItemDropRule.ByCondition(new EclipseNormalPostPlant(), ModContent.ItemType<GoldRoundYoyo>(), 80, 1, 1, 1));

				npcLoot.Add(ItemDropRule.ByCondition(new CorruptionMasterHardmode(), ModContent.ItemType<CorruptEye>(), 160, 1, 1, 1));
				npcLoot.Add(ItemDropRule.ByCondition(new CorruptionExpertHardmode(), ModContent.ItemType<CorruptEye>(), 275, 1, 1, 1));
				npcLoot.Add(ItemDropRule.ByCondition(new CorruptionNormalHardmode(), ModContent.ItemType<CorruptEye>(), 450, 1, 1, 1));

				npcLoot.Add(ItemDropRule.ByCondition(new CrimsonMasterHardmode(), ModContent.ItemType<GoldLiquidPupil>(), 160, 1, 1, 1));
				npcLoot.Add(ItemDropRule.ByCondition(new CrimsonExpertHardmode(), ModContent.ItemType<GoldLiquidPupil>(), 275, 1, 1, 1));
				npcLoot.Add(ItemDropRule.ByCondition(new CrimsonNormalHardmode(), ModContent.ItemType<GoldLiquidPupil>(), 450, 1, 1, 1));
			}
		}
	}
}

internal class CrimsonExpertHardmode : IItemDropRuleCondition
{
	// TODO:ÏÂÁÐµôÂäÌõ¼þÐèÒª·­Òë
	private bool CanD => Main.expertMode && !Main.masterMode && Main.hardMode && Main.LocalPlayer.ZoneCrimson;

	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}

	public bool CanShowItemDropInUI()
	{
		return CanD;
	}

	public string GetConditionDescription()
	{
		string desc = "Only hardmode and in the Crimson";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÀ§ÄÑÄ£Ê½µÄÐÉºìÖ®µØ";
		}

		return desc;
	}
}

internal class CrimsonMasterHardmode : IItemDropRuleCondition
{
	private bool CanD => Main.masterMode && Main.hardMode && Main.LocalPlayer.ZoneCrimson;

	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}

	public bool CanShowItemDropInUI()
	{
		return CanD;
	}

	public string GetConditionDescription()
	{
		string desc = "Only hardmode and in the  Crimson";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÀ§ÄÑÄ£Ê½µÄÐÉºìÖ®µØ";
		}

		return desc;
	}
}

internal class CrimsonNormalHardmode : IItemDropRuleCondition
{
	private bool CanD => !Main.expertMode && Main.hardMode && Main.LocalPlayer.ZoneCrimson;

	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}

	public bool CanShowItemDropInUI()
	{
		return CanD;
	}

	public string GetConditionDescription()
	{
		string desc = "Only hardmode and in the  Crimson";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÀ§ÄÑÄ£Ê½µÄÐÉºìÖ®µØ";
		}

		return desc;
	}
}

internal class CorruptionExpertHardmode : IItemDropRuleCondition
{
	private bool CanD => Main.expertMode && !Main.masterMode && Main.hardMode && Main.LocalPlayer.ZoneCorrupt;

	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}

	public bool CanShowItemDropInUI()
	{
		return CanD;
	}

	public string GetConditionDescription()
	{
		string desc = "Only hardmode and in the Corrupt";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÀ§ÄÑÄ£Ê½µÄ¸¯»¯Ö®µØ";
		}

		return desc;
	}
}

internal class CorruptionMasterHardmode : IItemDropRuleCondition
{
	private bool CanD => Main.masterMode && Main.hardMode && Main.LocalPlayer.ZoneCorrupt;

	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}

	public bool CanShowItemDropInUI()
	{
		return CanD;
	}

	public string GetConditionDescription()
	{
		string desc = "Only hardmode and in the Corrupt";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÀ§ÄÑÄ£Ê½µÄ¸¯»¯Ö®µØ";
		}

		return desc;
	}
}

internal class CorruptionNormalHardmode : IItemDropRuleCondition
{
	private bool CanD => !Main.expertMode && Main.hardMode && Main.LocalPlayer.ZoneCorrupt;

	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}

	public bool CanShowItemDropInUI()
	{
		return CanD;
	}

	public string GetConditionDescription()
	{
		string desc = "Only hardmode and in the Corrupt";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÀ§ÄÑÄ£Ê½µÄ¸¯»¯Ö®µØ";
		}

		return desc;
	}
}

internal class HallowExpertHardmode : IItemDropRuleCondition
{
	private bool CanD => Main.expertMode && !Main.masterMode && Main.hardMode && Main.LocalPlayer.ZoneHallow;

	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}

	public bool CanShowItemDropInUI()
	{
		return CanD;
	}

	public string GetConditionDescription()
	{
		string desc = "Only hardmode and in the Hallow";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÀ§ÄÑÄ£Ê½µÄÉñÊ¥Ö®µØ";
		}

		return desc;
	}
}

internal class HallowMasterHardmode : IItemDropRuleCondition
{
	private bool CanD => Main.masterMode && Main.hardMode && Main.LocalPlayer.ZoneHallow;

	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}

	public bool CanShowItemDropInUI()
	{
		return CanD;
	}

	public string GetConditionDescription()
	{
		string desc = "Only hardmode and in the Hallow";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÀ§ÄÑÄ£Ê½µÄÉñÊ¥Ö®µØ";
		}

		return desc;
	}
}

internal class HallowNormalHardmode : IItemDropRuleCondition
{
	private bool CanD => !Main.expertMode && Main.hardMode && Main.LocalPlayer.ZoneHallow;

	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}

	public bool CanShowItemDropInUI()
	{
		return CanD;
	}

	public string GetConditionDescription()
	{
		string desc = "Only hardmode and in the Hallow";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÀ§ÄÑÄ£Ê½µÄÉñÊ¥Ö®µØ";
		}

		return desc;
	}
}

internal class EclipseExpertPostPlant : IItemDropRuleCondition
{
	private bool CanD => Main.expertMode && !Main.masterMode && Main.eclipse && NPC.downedPlantBoss;

	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}

	public bool CanShowItemDropInUI()
	{
		return CanD;
	}

	public string GetConditionDescription()
	{
		string desc = "Only defeated Plantera and in the Eclipse";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÊÀ¼ÍÖ®»¨ºóÈÕÊ³";
		}

		return desc;
	}
}

internal class EclipseMasterPostPlant : IItemDropRuleCondition
{
	private bool CanD => Main.masterMode && Main.eclipse && NPC.downedPlantBoss;

	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}

	public bool CanShowItemDropInUI()
	{
		return CanD;
	}

	public string GetConditionDescription()
	{
		string desc = "Only defeated Plantera and in the Eclipse";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÊÀ¼ÍÖ®»¨ºóÈÕÊ³";
		}

		return desc;
	}
}

internal class EclipseNormalPostPlant : IItemDropRuleCondition
{
	private bool CanD => !Main.expertMode && Main.eclipse && NPC.downedPlantBoss;

	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}

	public bool CanShowItemDropInUI()
	{
		return CanD;
	}

	public string GetConditionDescription()
	{
		string desc = "Only defeated Plantera and in the Eclipse";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÊÀ¼ÍÖ®»¨ºóÈÕÊ³";
		}

		return desc;
	}
}

internal class InPumpkMoonFinal : IItemDropRuleCondition
{
	private bool CanD => Main.pumpkinMoon && Main.invasionProgressWave >= 20;

	public bool CanDrop(DropAttemptInfo info)
	{
		return CanD;
	}

	public bool CanShowItemDropInUI()
	{
		return CanD;
	}

	public string GetConditionDescription()
	{
		string desc = "Only Pumpkin Moon during Chrismas";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÊ¥µ®½ÚÆÚ¼äËªÔÂ";
		}

		return desc;
	}
}
