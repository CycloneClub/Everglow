namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class PotatoChipsBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.statDefense += 4; // 加4防御
		// +4% melee damage
		player.GetDamage(DamageClass.Melee) += 0.04f;
	}
}
