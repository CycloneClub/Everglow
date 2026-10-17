namespace Everglow.Myth.LanternMoon.NPCs.LanternGhostKing;

[Pipeline(typeof(WCSPipeline_PointWrap))]
public class LanternCrackingRay : Visual
{
	public NPC OwnerLanternKing;
	public float Timer;
	public float MaxTime;
	public Vector2 Position;

	public override CodeLayer DrawLayer => CodeLayer.PostDrawBG;

	public override void OnSpawn()
	{
		if (OwnerLanternKing == null)
		{
			foreach (NPC npc in Main.npc)
			{
				if (npc != null && npc.active)
				{
					if (npc.type == ModContent.NPCType<LanternGhostKing>())
					{
						OwnerLanternKing = npc;
					}
				}
			}
		}
	}

	public override void Update()
	{
		if (OwnerLanternKing == null || !OwnerLanternKing.active)
		{
			Active = false;
			return;
		}
		LanternGhostKing lanternGhostKing = OwnerLanternKing.ModNPC as LanternGhostKing;
		if (lanternGhostKing is null)
		{
			return;
		}
		Position = OwnerLanternKing.Center;
		if (lanternGhostKing.GoldenShieldBreakEffectTimer is > 0 and < 100)
		{
			Timer++;
		}
		if (Timer > MaxTime)
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		if (OwnerLanternKing == null)
		{
			return;
		}
		LanternGhostKing lanternGhostKing = OwnerLanternKing.ModNPC as LanternGhostKing;
		if (lanternGhostKing is null)
		{
			return;
		}
		float timeValue = (float)(Main.timeForVisualEffects * 0.001f);
		float fluctuation = 1f * (1 + MathF.Sin(timeValue * MathHelper.TwoPi * 18) * 0.45f);
		List<Vertex2D> bars = new List<Vertex2D>();
		float maxCount = 80;
		for (int i = 0; i <= maxCount; i++)
		{
			float mulColor = 1f;
			float targetValueY = Math.Min(lanternGhostKing.EffectValueZ, 1);
			targetValueY -= 0.9f;
			targetValueY *= 20f;
			targetValueY -= 1f;
			targetValueY *= 360;
			if (Timer <= 0)
			{
				targetValueY += 200;
			}
			float range = 300f;
			if (Timer > 0)
			{
				range += Timer * 30f;
				range = Math.Min(range, 600);
			}
			if (Timer > 65 && Timer <= 70)
			{
				range += (Timer - 65) * 200f;
				targetValueY -= 20000;
			}
			if (Timer > 70)
			{
				mulColor *= (90 - Timer) / 20f;
			}
			Vector2 pos = new Vector2(0, -range).RotatedBy(i / maxCount * MathHelper.TwoPi);
			pos.Y *= 0.6f;
			Vector2 innerPos = Position + pos * 0.1f;
			Vector2 outerPos = Position + pos;
			if (pos.Y < targetValueY)
			{
				mulColor *= 0;
			}
			bars.Add(innerPos, new Color(0.7f, 0.7f, 0.4f) * mulColor * fluctuation, new Vector3(i / maxCount, timeValue, 0));
			bars.Add(outerPos, new Color(0f, 0f, 0f, 0), new Vector3(i / maxCount, timeValue, 0));
		}
		Ins.Batch.Draw(Commons.ModAsset.Noise_Flame_3.Value, bars, PrimitiveType.TriangleStrip);
		if (Timer > 65 && Timer <= 70)
		{
			float count = Timer - 65;
			for (int k = 0; k < count; k++)
			{
				Ins.Batch.Draw(Commons.ModAsset.Noise_Flame_3.Value, bars, PrimitiveType.TriangleStrip);
			}
		}
	}
}
