namespace Everglow.Yggdrasil.KelpCurtain.VFXs.VampireMat;

[Pipeline(typeof(WCSPipeline))]
public class VampireMatStickScreen : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Player Owner;
	public NPC ParentNPC;
	public float Timer;
	public float MaxTime;
	public float FinishTime = -1;
	public float Duration = 0;

	public override void Update()
	{
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
			return;
		}
		if (ParentNPC is null || !ParentNPC.active || ParentNPC.type != ModContent.NPCType<NPCs.VampireMat.VampireMat>())
		{
			Active = false;
			return;
		}
		var vMat = ParentNPC.ModNPC as NPCs.VampireMat.VampireMat;
		if (MaxTime - Timer > 30 && (vMat.TotalDamageIntakeWhenStickPlayer >= 100 || !vMat.StickPlayer))
		{
			Timer = MaxTime - 30;
		}
		if (Timer < 30)
		{
			Duration = (1 - MathF.Cos(Timer / 30f * MathHelper.Pi * 1.2f)) * 0.5f;
		}
		else if (MaxTime - Timer < 30)
		{
			Duration = (1 - MathF.Cos((MaxTime - Timer) / 30f * MathHelper.Pi * 1.2f)) * 0.5f;
		}
		else
		{
			if (vMat.HitTimer > 0)
			{
				Duration = 1 - vMat.HitTimer / 60f;
			}
			Duration = Duration * 0.9f + 0.1f;
		}
	}

	public override void Draw()
	{
		if (ParentNPC is null || !ParentNPC.active || ParentNPC.type != ModContent.NPCType<NPCs.VampireMat.VampireMat>())
		{
			Active = false;
			return;
		}
		var vMat = ParentNPC.ModNPC as NPCs.VampireMat.VampireMat;

		List<Vertex2D> bars = [];
		if (Owner is null || Main.LocalPlayer != Owner)
		{
			Ins.Batch.Draw(Main.LocalPlayer.Hitbox, Color.Transparent);
			return;
		}
		for (int k = 0; k < 60; k++)
		{
			AddATentacle(bars, k / 60f * MathHelper.TwoPi);
		}
		Ins.Batch.Draw(ModAsset.VampireMatStickScreen.Value, bars, PrimitiveType.TriangleStrip);

		List<Vertex2D> ring = [];
		SpriteBatchUtils.AddVerticesForCircleRing(ring, Owner.Center, 300, 30, new Color(0.5f, 0.5f, 0.5f, 0.5f) * Duration, 0.5f, 0.5f);
		Ins.Batch.Draw(Commons.ModAsset.TileBlock.Value, ring, PrimitiveType.TriangleStrip);

		List<Vertex2D> ring2 = [];
		SpriteBatchUtils.AddVerticesForCircleRing(ring2, Owner.Center, 324, 12, new Color(0.5f, 0.5f, 0.5f, 0.5f) * Duration, 0.5f, 0.5f);
		Ins.Batch.Draw(Commons.ModAsset.TileBlock.Value, ring2, PrimitiveType.TriangleStrip);

		List<Vertex2D> ring_duration = [];
		int maxValue = vMat.TotalDamageIntakeWhenStickPlayer;
		if (!vMat.StickPlayer)
		{
			maxValue = 100;
		}
		var durationColor = Color.White * 0.75f;
		var timeColor = Color.Lerp(Color.Yellow, Color.White, 0.75f) * 0.75f;
		if (MaxTime - Timer < 30)
		{
			if (vMat.FailToEscapeStickTimer > 0)
			{
				durationColor = Color.Red;
				timeColor = Color.Red;
			}
			else
			{
				if (FinishTime == -1)
				{
					FinishTime = Timer;
				}
			}
			maxValue = 100;
		}
		durationColor *= Duration;
		timeColor *= Duration;
		for (int k = 0; k <= maxValue; k++)
		{
			float rot = k / 100f * MathHelper.TwoPi;
			ring_duration.Add(Owner.Center + new Vector2(0, -315).RotatedBy(rot), durationColor, new Vector3(0, 0, 0));
			ring_duration.Add(Owner.Center + new Vector2(0, -285).RotatedBy(rot), durationColor, new Vector3(0, 0, 0));
		}
		Ins.Batch.Draw(Commons.ModAsset.TileBlock.Value, ring_duration, PrimitiveType.TriangleStrip);

		List<Vertex2D> ring2_duration = [];
		int ring2_time = (int)Timer;
		if (FinishTime != -1)
		{
			ring2_time = (int)FinishTime;
		}
		if (ring2_time > 120)
		{
			ring2_time = 120;
		}
		for (int k = 0; k <= ring2_time; k++)
		{
			float rot = k / 120f * MathHelper.TwoPi;
			ring2_duration.Add(Owner.Center + new Vector2(0, -330).RotatedBy(rot), timeColor, new Vector3(0, 0, 0));
			ring2_duration.Add(Owner.Center + new Vector2(0, -318).RotatedBy(rot), timeColor, new Vector3(0, 0, 0));
		}
		Ins.Batch.Draw(Commons.ModAsset.TileBlock.Value, ring2_duration, PrimitiveType.TriangleStrip);
	}

	public void AddATentacle(List<Vertex2D> bars, float direction)
	{
		if (Owner is null)
		{
			return;
		}
		Vector2 tantecle_dir = new Vector2(Main.screenWidth, Main.screenHeight).RotatedBy(direction) * 0.5f;
		Vector2 tantecle_pos = Owner.Center + tantecle_dir;
		Vector2 tantecle_unit = tantecle_dir.SafeNormalize(Vector2.Zero) * 18f;
		float randomOffset = TileUtils.GetFixedRandomNumber_SingleSeed((int)(direction * 610), 200) / 200f;
		tantecle_unit *= 1f + MathF.Sin(randomOffset * MathHelper.TwoPi) * 0.25f;
		Vector2 tantecle_normal = tantecle_unit.RotatedBy(MathHelper.PiOver2);
		int length = (int)(Duration * 90f);
		for (int k = 0; k < length; k++)
		{
			tantecle_pos -= tantecle_unit;
			Vector2 unitPos = tantecle_pos + tantecle_normal * MathF.Sin((float)Main.time * 0.03f + k * 0.14f + randomOffset * MathHelper.TwoPi);
			float width = 0.6f + (length - k) * 0.05f;
			float valueX = k + 60 - length;
			valueX /= 60f;
			bars.Add(unitPos + tantecle_normal * width, new Color(0, 0, 0, 1f), new Vector3(valueX, 0, 0));
			bars.Add(unitPos - tantecle_normal * width, new Color(0, 0, 0, 1f), new Vector3(valueX, 1, 0));
		}
	}
}
