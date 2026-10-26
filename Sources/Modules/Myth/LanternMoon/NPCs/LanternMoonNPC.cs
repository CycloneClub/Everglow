using Everglow.Myth.LanternMoon.LanternCommon;
using Everglow.Myth.LanternMoon.Items;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Myth.LanternMoon.NPCs;

public abstract class LanternMoonNPC : ModNPC
{
	public LanternMoonInvasionEvent LanternMoon = ModContent.GetInstance<LanternMoonInvasionEvent>();

	public float LanternMoonScore = 1f;

	public float SpawnFrequencyInLanternMoon = 1f;

	public override bool SpecialOnKill()
	{
		LanternMoon.AddPoint(LanternMoonScore);
		return base.SpecialOnKill();
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		// 灯笼月普通敌怪独立掉落，暂按事件武器与消耗品的稀有度拟定。
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LanternBombRemoteControl>(), 200));
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FireCrackerLauncher>(), 100));
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FireCracker>(), 10, 1, 3));

		LeadingConditionRule LanternMoonHeartRule = new LeadingConditionRule(new LanternMoonNPCDropHeartCondition());
		int itemType = ItemID.Heart;
		var parameters = new DropOneByOne.Parameters()
		{
			ChanceNumerator = 1,
			ChanceDenominator = 1,
			MinimumStackPerChunkBase = 1,
			MaximumStackPerChunkBase = 1,
			MinimumItemDropsCount = 12,
			MaximumItemDropsCount = 15,
		};

		LanternMoonHeartRule.OnSuccess(new DropOneByOne(itemType, parameters));
		npcLoot.Add(LanternMoonHeartRule);
	}
}
