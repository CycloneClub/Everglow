namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class BaconBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("BaconBuff");
		// Description.SetDefault("加2生命回复，所受冷系伤害降低\n“开胃祛寒”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.resistCold = true; // 保暖
		player.lifeRegen += 2; // 加2生命回复
	}
}

