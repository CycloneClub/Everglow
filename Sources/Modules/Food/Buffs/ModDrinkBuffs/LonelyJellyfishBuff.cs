namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class LonelyJellyfishBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.invis = true;
		if (Main.timeForVisualEffects % 20f == 0)
		{
			Projectile.NewProjectile(player.GetSource_Buff(buffIndex), Main.MouseWorld, Vector2.Zero, ProjectileID.Electrosphere, Math.Clamp(player.HeldItem.damage, 25, 150), 10, player.whoAmI);
		}
	}
}
