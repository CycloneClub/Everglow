namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class ChickenNuggetBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.lifeRegen += 1; // 加1生命回复
		player.GetAttackSpeed(DamageClass.Generic) += 0.04f; // 加4%攻速
	}
}
