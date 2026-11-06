namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class FruitJuiceBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.statDefense += 20; // 加20防御
		player.GetCritChance(DamageClass.Generic) += 20; // 加20%暴击
		player.endurance += 0.25f; // 加25%减伤
		// +20% damage
		player.GetDamage(DamageClass.Generic) += 0.2f;
		player.GetAttackSpeed(DamageClass.Generic) += 0.2f; // 加20%攻速
		player.lifeRegen += 10; // +10 life regen
		// +20 mana regen
		player.manaRegenBonus += 20;
		// +100% movement speed
		player.moveSpeed += 1f;
		player.jumpSpeedBoost += 2;
	}
}
