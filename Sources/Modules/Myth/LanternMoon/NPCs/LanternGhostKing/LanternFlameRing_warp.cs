namespace Everglow.Myth.LanternMoon.NPCs.LanternGhostKing;

[Pipeline(typeof(WarpPipeline))]
internal class LanternFlameRing_warpDust : Visual
{
	public NPC OwnerLanternKing;
	public float Fade;
	public float MaxFade;
	public float Radius;

	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

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
			Fade++;
		}
		else
		{
			LanternGhostKing lanternGhostKing = OwnerLanternKing.ModNPC as LanternGhostKing;
			if (lanternGhostKing != null)
			{
				Radius = lanternGhostKing.RingRadius;
				Fade = lanternGhostKing.RingFade;
			}
		}
		if (Fade > MaxFade)
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
		Vector2 center = OwnerLanternKing.Center;
		LanternGhostKing lanternGhostKing = OwnerLanternKing.ModNPC as LanternGhostKing;
		if (lanternGhostKing != null)
		{
			center = lanternGhostKing.RingCenter;
		}
		float width = 150;
		float mulColor = 1f;
		if (Radius < 1000)
		{
			width *= MathF.Max(0, (Radius - 500) / 500f);
		}
		if (Fade > 120)
		{
			mulColor *= (240 - Fade) / 120f;
		}

		float timeValue = (float)(-Main.timeForVisualEffects * 0.001f);
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int i = 0; i <= 500; i++)
		{
			Vector2 pos = new Vector2(Radius, 0).RotatedBy(i / 500f * MathHelper.TwoPi);
			Vector2 outerPos = new Vector2(Radius + width, 0).RotatedBy(i / 500f * MathHelper.TwoPi);

			var color0 = new Color(0.5f + pos.X / Radius, 0.5f + pos.Y / Radius, 0.19f * mulColor, 1);
			var color1 = new Color(0.5f + pos.X / Radius, 0.5f + pos.Y / Radius, 0f, 1);

			bars.Add(center + pos, color0, new Vector3(i / 100f, 0 + timeValue, 0));
			bars.Add(center + outerPos, color1, new Vector3(i / 100f, 0.05f + timeValue, 0));
			if (i % 5 == 0)
			{
				Lighting.AddLight(pos, new Vector3(1f, 0.5f, 0) * width / 200f * mulColor);
			}
		}
		for (int i = 0; i <= 500; i++)
		{
			Vector2 pos = new Vector2(Radius, 0).RotatedBy(i / 500f * MathHelper.TwoPi);
			Vector2 pos2 = new Vector2(Radius, 0).RotatedBy((i + 6) / 500f * MathHelper.TwoPi);
			Vector2 innerrPos = new Vector2(Radius - width * 0.3f, 0).RotatedBy(i / 500f * MathHelper.TwoPi);

			var color0 = new Color(0.5f + pos2.X / Radius, 0.5f + pos2.Y / Radius, 0.16f * mulColor, 1);
			var color1 = new Color(0.5f + pos2.X / Radius, 0.5f + pos2.Y / Radius, 0f, 1);

			bars.Add(center + pos, color0, new Vector3(i / 100f, 0 + timeValue, 0));
			bars.Add(center + innerrPos, color1, new Vector3(i / 100f, 0.02f + timeValue, 0));
		}
		Ins.Batch.Draw(Commons.ModAsset.Noise_Melting.Value, bars, PrimitiveType.TriangleStrip);
	}
}
