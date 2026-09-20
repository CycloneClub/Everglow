using Everglow.Yggdrasil.KelpCurtain.VFXs;

namespace Everglow.Yggdrasil.KelpCurtain.Buffs;

public class RedAlgae_FriendlyDebuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
	}

	public override void Update(NPC npc, ref int buffIndex)
	{
		npc.color = Color.Lerp(npc.color, new Color(0.7f, 0.1f, 0.6f), 0.1f);
		if (Main.rand.NextBool(12))
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(2)).RotatedByRandom(MathHelper.TwoPi);
			var redAlgaeDust = new RedAlgae_Small_Dust();
			redAlgaeDust.Position = npc.position + new Vector2(Main.rand.NextFloat(npc.width), Main.rand.NextFloat(npc.height));
			redAlgaeDust.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
			redAlgaeDust.Velocity = vel;
			redAlgaeDust.ai = new float[] { 0.99f };
			redAlgaeDust.MaxTime = 30;
			redAlgaeDust.Scale = 1f;
			redAlgaeDust.Frame = Main.rand.Next(10);
			redAlgaeDust.Visible = true;
			redAlgaeDust.Active = true;
			Ins.VFXManager.Add(redAlgaeDust);
		}
		base.Update(npc, ref buffIndex);
	}
}
