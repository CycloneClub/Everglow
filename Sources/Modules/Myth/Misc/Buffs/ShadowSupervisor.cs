using Everglow.Myth.Misc.Projectiles.Weapon.Ranged.Slingshots;

namespace Everglow.Myth.Misc.Buffs;

public class ShadowSupervisor : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
		Main.buffNoSave[Type] = true;
	}

	public override void Update(NPC npc, ref int buffIndex)
	{
		Dust index = Dust.NewDustDirect(npc.position - new Vector2(4), npc.width, npc.height, DustID.WaterCandle, 0f, 0f, 0, default, Main.rand.NextFloat(0.6f, 2.3f));
		index.velocity = new Vector2(0, Main.rand.NextFloat(0.5f, 2f)).RotatedByRandom(6.283) + new Vector2(0, -Main.rand.NextFloat(1.1f, 2f));
		index.noGravity = true;
		int LuckyTarget = Main.rand.Next(200);
		if (LuckyTarget == npc.whoAmI)
		{
			return;
		}

		NPC target = Main.npc[LuckyTarget];
		if (target.active)
		{
			int i = target.FindBuffIndex(ModContent.BuffType<ShadowSupervisor>());
			if (i != -1)
			{
				if (!target.dontTakeDamage)
				{
					if (!target.friendly)
					{
						if ((target.Center - npc.position).Length() < 600)
						{
							int x = (int)Main.timeForVisualEffects;
							Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<AmbiguousLine>(), 40, 0, Main.myPlayer, x, 0);
							Projectile.NewProjectile(npc.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<AmbiguousLine>(), 40, 0, Main.myPlayer, x, 1/*ai1 = 1才绘制*/);
							ScreenShaker Gsplayer = Main.player[Main.myPlayer].GetModPlayer<ScreenShaker>();
							Gsplayer.FlyCamPosition = new Vector2(0, 2).RotatedByRandom(6.283);
							npc.buffTime[buffIndex] -= 120;
						}
					}
				}
			}
		}
	}
}
