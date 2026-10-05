using Everglow.Yggdrasil.CorruptWormHive.VFXs;

namespace Everglow.Yggdrasil.CorruptWormHive.Buffs;

public class DeathFlame : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
		Main.buffNoSave[Type] = true;
	}

	public override void Update(NPC npc, ref int buffIndex)
	{
		int freq = (int)(npc.velocity.Length() / 2f + 1);
		for (int d = 0; d < freq; d++)
		{
			if (Main.rand.NextBool(2))
			{
				var df = new DevilFlame3DSickle_worldCoordDust
				{
					Velocity3D = new Vector3(new Vector2(0, Main.rand.NextFloat(1, 2f)).RotatedByRandom(MathHelper.TwoPi) + npc.velocity * 0.2f, 0) + new Vector3(0, -1, 0),
					Active = true,
					Visible = true,
					Position3D = new Vector3(new Vector2(Main.rand.NextFloat(npc.width), Main.rand.NextFloat(npc.height)) + npc.position, 0),
					rotateAxis = new Vector3(0, 0, 1),
					Scale = Main.rand.NextFloat(2, 6),
					MaxTime = Main.rand.Next(36, 40),
					OwnerWhoAmI = Main.LocalPlayer.whoAmI,
					ai = new float[] { Main.rand.NextFloat(0, 1f), Main.rand.NextFloat(-0.1f, 0.1f), 0f },
				};
				Ins.VFXManager.Add(df);
			}
		}
		int BuffDamage = (int)((1 - npc.life / (float)npc.lifeMax) * 400 + npc.defense + 40);
		npc.SetLifeRegenExpectedLossPerSecond(BuffDamage / 24);
		npc.lifeRegen = -BuffDamage;
		if (npc.life + npc.lifeRegen / 40 <= 0 && npc.active && !npc.dontTakeDamage)
		{
			for (int d = 0; d < 40; d++)
			{
				var df = new DevilFlame3DSickle_worldCoordDust
				{
					Velocity3D = new Vector3(new Vector2(0, Main.rand.NextFloat(6, 9f)).RotatedByRandom(MathHelper.TwoPi), 0),
					Active = true,
					Visible = true,
					Position3D = new Vector3(npc.Center, 0),
					rotateAxis = new Vector3(0, 0, 1),
					Scale = Main.rand.NextFloat(6, 12),
					MaxTime = Main.rand.Next(36, 40),
					OwnerWhoAmI = Main.LocalPlayer.whoAmI,
					ai = new float[] { Main.rand.NextFloat(0, 1f), Main.rand.NextFloat(-0.1f, 0.1f), 0f },
				};
				Ins.VFXManager.Add(df);
			}
		}
	}
}
