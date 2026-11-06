namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class LonelyJellyfishBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.invis = true;
		if (Main.timeForVisualEffects % 20f == 0)
		{
			Projectile.NewProjectile(player.GetSource_Buff(buffIndex), Main.MouseWorld, Vector2.Zero, ProjectileID.Electrosphere, Math.Clamp(player.HeldItem.damage, 25, 150), 10, player.whoAmI);
		}
	}
}
