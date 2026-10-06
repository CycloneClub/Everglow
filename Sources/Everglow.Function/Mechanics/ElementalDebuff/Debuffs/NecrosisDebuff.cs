using Everglow.Commons.VFX.CommonVFXDusts;
using ReLogic.Content;

namespace Everglow.Commons.Mechanics.ElementalDebuff.Debuffs;

public class NecrosisDebuff : ElementalDebuffHandler
{
	public static new string ID => nameof(NecrosisDebuff);

	public override string TypeID => ID;

	public override int Duration { get; protected set; } = 1200;

	public override Asset<Texture2D> Texture => ModAsset.Necrosis;

	public override Color Color => new Color(0.1f, 0.1f, 0.1f, 1f);

	public override void PostProc(NPC npc)
	{
		for (int i = 0; i < 20; i++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 8f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new NecrosisSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = npc.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(30, 45),
				Scale = Main.rand.NextFloat(70f, 120f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}
}
