using Everglow.EternalResolve.VFXs;
using Everglow.Commons.Utilities;

namespace Everglow.EternalResolve.Buffs;

public class OnElectric : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = true;
		Main.buffNoTimeDisplay[Type] = true;
	}

	public override void Update(NPC npc, ref int buffIndex)
	{
		int buffDamage = 35;
		float currentElectrityTimeFactor = 1f;
		if (npc.wet)
		{
			buffDamage *= 3;
			currentElectrityTimeFactor *= 3;
		}
		npc.lifeRegen = -buffDamage;
		npc.SetLifeRegenExpectedLossPerSecond(7);
		if (npc.wet)
		{
			npc.SetLifeRegenExpectedLossPerSecond(7 * 3);
		}
		if (Main.rand.NextBool(3))
		{
			Dust d = Dust.NewDustDirect(npc.Center, 0, 0, ModContent.DustType<TriggerElectricCurrentDust>(), 0, 0);
			d.scale = Main.rand.NextFloat(0.85f, 1.15f) * 0.04f;
		}

		float mulVelocity = 0.5f * currentElectrityTimeFactor;
		float size = Main.rand.NextFloat(8f, Main.rand.NextFloat(4f, 10f));
		Vector2 afterVelocity = new Vector2(0, size * 1.3f).RotatedByRandom(MathHelper.TwoPi);
		var electric = new YoenLeZedElecticFlow
		{
			Velocity = afterVelocity * mulVelocity,
			Active = true,
			Visible = true,
			Position = npc.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - afterVelocity * mulVelocity * 6,
			MaxTime = size * size / 34f,
			Scale = size * MathF.Sqrt(currentElectrityTimeFactor),
			ai = new float[] { Main.rand.NextFloat(0.0f, 0.6f), 1f, Main.rand.NextFloat(0.2f, Main.rand.NextFloat(0.2f, 0.4f)) },
		};
		Ins.VFXManager.Add(electric);
	}
}
