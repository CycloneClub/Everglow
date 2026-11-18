using Everglow.Commons.Mechanics.ElementalDebuff.Debuffs;
using Everglow.Commons.Utilities;

namespace Everglow.Example.ElementalDebuff;

public class ElementalResistanceReductionDebuffTest : ModBuff
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Examples;

	public override string Texture => Commons.ModAsset.Point_Mod;

	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
	}

	public override void Update(NPC npc, ref int buffIndex)
	{
		npc.GetElementalResistance(BurnDebuff.ID).Base += 0.1f;
		npc.GetElementalResistance(BurnDebuff.ID) += 0.05f;
	}
}
