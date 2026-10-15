using Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

namespace Everglow.Yggdrasil.KelpCurtain.Buffs;

public class RestrictionDroneRE01Buff : ModBuff
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Buffs;

	public override string Texture => $"Terraria/Images/Buff_{BuffID.BabySlime}";

	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = true;
		Main.buffNoTimeDisplay[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		if (!player.dead && player.ownedProjectileCounts[ModContent.ProjectileType<RestrictionDroneRE01>()] > 0)
		{
			player.buffTime[buffIndex] = 18000;
		}
		else
		{
			player.DelBuff(buffIndex);
			buffIndex--;
		}
	}
}
