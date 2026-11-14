using Everglow.Myth.LanternMoon.Projectiles.Weapons;

namespace Everglow.Myth.LanternMoon.Buffs;

public class GoldenLotusStaff_Buff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = true;
		Main.buffNoTimeDisplay[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		if (player.ownedProjectileCounts[ModContent.ProjectileType<GoldenLotusStaff_Proj>()] + player.ownedProjectileCounts[ModContent.ProjectileType<GoldenLotusStaff_Subproj>()] <= 0)
		{
			if (player.buffTime[buffIndex] > 1)
			{
				player.buffTime[buffIndex] = 0;
			}
		}
		else
		{
			if (player.buffTime[buffIndex] < 5)
			{
				player.buffTime[buffIndex] = 5;
			}
		}
	}

	public override bool RightClick(int buffIndex)
	{
		if (!Main.dedServ)
		{
			foreach (var proj in Main.projectile)
			{
				if (proj is not null && proj.active && proj.owner == Main.myPlayer)
				{
					if (proj.type == ModContent.ProjectileType<GoldenLotusStaff_Proj>() || proj.type == ModContent.ProjectileType<GoldenLotusStaff_Subproj>())
					{
						proj.Kill();
					}
				}
			}
		}
		return base.RightClick(buffIndex);
	}
}
