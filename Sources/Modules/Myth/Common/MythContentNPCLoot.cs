using Everglow.Myth.Misc.Items.Accessories;
using Everglow.Myth.Misc.Items.Weapons;
using Everglow.Myth.Misc.Items.Weapons.Clubs;
using Everglow.Myth.Misc.Items.Weapons.Slingshots;
using Everglow.Myth.TheTusk.Items.Weapons;
using Everglow.Myth.TheTusk.NPCs.BloodTusk;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Everglow.Myth.Common;

public class MythContentNPCLoot : GlobalNPC
{
	public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
	{
		if (npc.type == NPCID.Nutcracker || npc.type == NPCID.NutcrackerSpinning)
		{
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<WalnutClip>(), 180, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<WalnutClip>(), 270, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<WalnutClip>(), 360, 1, 1, 1));
		}

		if (npc.type == NPCID.Harpy)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FeatherMagic>(), 70));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<FeatherMagic>(), 60, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<FeatherMagic>(), 90, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<FeatherMagic>(), 120, 1, 1, 1));
		}

		// 待恢复：当前未找到这些旧物品的实现：BloodLightCyanFlame。
		// if (npc.type == NPCID.Retinazer || npc.type == NPCID.Spazmatism)
		// {
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<BloodLightCyanFlame>(), 16, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<BloodLightCyanFlame>(), 50, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<BloodLightCyanFlame>(), 200, 1, 1, 1));
		// }

		if (npc.type == NPCID.KingSlime)
		{
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<GelSlingshot>(), 8, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<GelSlingshot>(), 25, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<GelSlingshot>(), 100, 1, 1, 1));
		}

		// 待恢复：当前未找到这些旧物品的实现：ThornBomb。
		// if (npc.type == NPCID.Plantera)
		// {
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<ThornBomb>(), 8, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<ThornBomb>(), 25, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ThornBomb>(), 100, 1, 1, 1));
		// }

		// 待恢复：当前未找到这些旧物品的实现：TrapYoyo。
		// if (npc.type == NPCID.Golem)
		// {
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<TrapYoyo>(), 8, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<TrapYoyo>(), 25, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<TrapYoyo>(), 100, 1, 1, 1));
		// }

		// 待恢复：当前未找到这些旧物品的实现：MachineFit。
		// if (npc.type == NPCID.SkeletronPrime)
		// {
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<MachineFit>(), 8, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<MachineFit>(), 25, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<MachineFit>(), 100, 1, 1, 1));
		// }

		// 待恢复：当前未找到这些旧物品的实现：MachineSkeGun。
		// if (npc.type == NPCID.SkeletronPrime)
		// {
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<MachineSkeGun>(), 40, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<MachineSkeGun>(), 125, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<MachineSkeGun>(), 500, 1, 1, 1));
		// }

		// 待恢复：当前未找到这些旧物品的实现：BloodGoldBlade。
		// if (npc.type == NPCID.WallofFlesh)
		// {
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<BloodGoldBlade>(), 40, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<BloodGoldBlade>(), 125, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<BloodGoldBlade>(), 500, 1, 1, 1));
		// }

		// 待恢复：当前未找到这些旧物品的实现：LaserWhip。
		// if (npc.type == NPCID.TheDestroyer)
		// {
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<LaserWhip>(), 8, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<LaserWhip>(), 25, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<LaserWhip>(), 100, 1, 1, 1));
		// }

		// 待恢复：当前未找到这些旧物品的实现：ChaosCurrent。
		// if (npc.type == NPCID.BrainofCthulhu)
		// {
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<ChaosCurrent>(), 8, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<ChaosCurrent>(), 25, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ChaosCurrent>(), 100, 1, 1, 1));
		// }

		// 待恢复：当前未找到这些旧物品的实现：RainbowLight。
		// if (npc.type == NPCID.HallowBoss)
		// {
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<RainbowLight>(), 8, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<RainbowLight>(), 25, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<RainbowLight>(), 100, 1, 1, 1));
		// }

		if (npc.type == ModContent.NPCType<BloodTusk>())
		{
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<ToothSpear>(), 40, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<ToothSpear>(), 125, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ToothSpear>(), 500, 1, 1, 1));
		}

		if (npc.type == ModContent.NPCType<BloodTusk>())
		{
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<ToothBow>(), 8, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<ToothBow>(), 25, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ToothBow>(), 100, 1, 1, 1));
		}

		// 待恢复：当前未找到这些旧物品的实现：GelPowerGun。
		// if (npc.type == NPCID.QueenSlimeBoss)
		// {
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<GelPowerGun>(), 8, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<GelPowerGun>(), 25, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<GelPowerGun>(), 100, 1, 1, 1));
		// }

		// 待恢复：当前未找到这些旧物品的实现：AshBone。
		// if (npc.type == NPCID.BoneSerpentHead)
		// {
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<AshBone>(), 60, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<AshBone>(), 90, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<AshBone>(), 120, 1, 1, 1));
		// }

		// 待恢复：当前未找到这些旧物品的实现：SilenceMirror。
		// if (npc.type == NPCID.MoonLordCore)
		// {
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<SilenceMirror>(), 40, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<SilenceMirror>(), 125, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<SilenceMirror>(), 500, 1, 1, 1));
		// }

		// 待恢复：当前未找到这些旧物品的实现：BlueRain。
		// if (npc.type == NPCID.DukeFishron)
		// {
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<BlueRain>(), 8, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<BlueRain>(), 25, 1, 1, 1));
			// npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<BlueRain>(), 100, 1, 1, 1));
		// }

		if (npc.type == NPCID.PossessedArmor)
		{
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<AmbiguousNight>(), 100, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<AmbiguousNight>(), 150, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<AmbiguousNight>(), 200, 1, 1, 1));
		}

		if (npc.type == NPCID.WyvernHead)
		{
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<ThunderFlower>(), 50, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<ThunderFlower>(), 100, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ThunderFlower>(), 140, 1, 1, 1));
		}

		if (npc.type == NPCID.Wraith)
		{
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<ComingGhost>(), 100, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<ComingGhost>(), 150, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ComingGhost>(), 200, 1, 1, 1));
		}

		if (npc.type == NPCID.DukeFishron)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DukeTooth>(), 3));
		}

		// 待恢复：已迁移至 PlantAndFarm；此处未新增跨模块依赖。
		// if (npc.type == NPCID.Dandelion)
		// {
			// npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WindMoveSeed>(), 1, 3, 5));
		// }

		if (npc.type == NPCID.BlueSlime)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BlueGel>(), 800));
		}

		// 旧表的负数编号属于 netID，暂保留原有物品对应关系。
		if (npc.netID == NPCID.GreenSlime)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RedGel>(), 800));
		}

		// 旧表的负数编号属于 netID，暂保留原有物品对应关系。
		if (npc.netID == NPCID.Slimeling)
		{
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GreenGel>(), 800));
		}

		if (!npc.friendly && npc.lifeMax > 100)
		{
			npcLoot.Add(ItemDropRule.ByCondition(new EclipseMasterPostPlant(), ModContent.ItemType<GoldRoundYoyo>(), 40, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new EclipseExpertPostPlant(), ModContent.ItemType<GoldRoundYoyo>(), 60, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new EclipseNormalPostPlant(), ModContent.ItemType<GoldRoundYoyo>(), 80, 1, 1, 1));
		}

		if (!npc.friendly && npc.lifeMax > 100)
		{
			npcLoot.Add(ItemDropRule.ByCondition(new CorruptionMasterHardmode(), ModContent.ItemType<CurseClub_Item>(), 100, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new CorruptionExpertHardmode(), ModContent.ItemType<CurseClub_Item>(), 150, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new CorruptionNormalHardmode(), ModContent.ItemType<CurseClub_Item>(), 200, 1, 1, 1));
		}

		if (!npc.friendly && npc.lifeMax > 100)
		{
			npcLoot.Add(ItemDropRule.ByCondition(new CorruptionMasterHardmode(), ModContent.ItemType<CorruptEye>(), 160, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new CorruptionExpertHardmode(), ModContent.ItemType<CorruptEye>(), 275, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new CorruptionNormalHardmode(), ModContent.ItemType<CorruptEye>(), 450, 1, 1, 1));
		}

		if (!npc.friendly && npc.lifeMax > 100)
		{
			npcLoot.Add(ItemDropRule.ByCondition(new CrimsonExpertHardmode(), ModContent.ItemType<GoldLiquidPupil>(), 160, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new CrimsonExpertHardmode(), ModContent.ItemType<GoldLiquidPupil>(), 275, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new CrimsonNormalHardmode(), ModContent.ItemType<GoldLiquidPupil>(), 450, 1, 1, 1));
		}

		if (!npc.friendly && npc.lifeMax > 100)
		{
			npcLoot.Add(ItemDropRule.ByCondition(new CrimsonMasterHardmode(), ModContent.ItemType<IchorClub_Item>(), 100, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new CrimsonExpertHardmode(), ModContent.ItemType<IchorClub_Item>(), 150, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new CrimsonNormalHardmode(), ModContent.ItemType<IchorClub_Item>(), 200, 1, 1, 1));
		}

		if (!npc.friendly && npc.lifeMax > 100)
		{
			npcLoot.Add(ItemDropRule.ByCondition(new HallowMasterHardmode(), ModContent.ItemType<CrystalClub_Item>(), 100, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new HallowExpertHardmode(), ModContent.ItemType<CrystalClub_Item>(), 150, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new HallowNormalHardmode(), ModContent.ItemType<CrystalClub_Item>(), 200, 1, 1, 1));
		}

		if (npc.type == NPCID.Everscream)
		{
			// 待恢复：当前未找到 FrozenStormPine 的实现。
			// npcLoot.Add(ItemDropRule.ByCondition(new InFrostMoonFinal(), ModContent.ItemType<FrozenStormPine>(), 50, 1, 1, 1));
			npcLoot.Add(ItemDropRule.ByCondition(new InFrostMoonFinal(), ModContent.ItemType<XmasWhip>(), 50, 1, 1, 1));
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

internal class InFrostMoonFinal : IItemDropRuleCondition
{
	private bool CanD => Main.snowMoon && Main.invasionProgressWave >= 20;

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
		string desc = "Only Frost Moon during Chrismas";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "½öÔÚÊ¥µ®½ÚÆÚ¼äËªÔÂ";
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
