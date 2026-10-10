using Everglow.Myth.LanternMoon.NPCs.LanternGhostKing;

namespace Everglow.Myth.LanternMoon.VFX;

[Pipeline(typeof(LanternGhostKing_Matrix_DissolvePipeline), typeof(BloomPipeline))]
public class LanternGhostKing_Matrix : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public NPC OwnerLanternGhostKing;
	public Vector2 Position;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;
	public float lightDisparity = 0;
	public int ExtraUpdate;

	public float Fade => 1 - Timer / MaxTime;

	public override void Update()
	{
		if (OwnerLanternGhostKing is not null)
		{
			Position = OwnerLanternGhostKing.Center;
			if (OwnerLanternGhostKing.type == ModContent.NPCType<LanternGhostKing>())
			{
				LanternGhostKing lGK = OwnerLanternGhostKing.ModNPC as LanternGhostKing;
				Rotation = lGK.RushDirectionPridiction - MathHelper.PiOver2;
			}
		}
		else
		{
			Rotation = (Position - Main.LocalPlayer.Center).ToRotation() + MathHelper.PiOver2;
		}
		Timer += ExtraUpdate;
		if (Timer > MaxTime)
		{
			Active = false;
		}
		float growThrethod = 60;
		if (Timer < growThrethod)
		{
			Scale += MathF.Cos(Timer / 90f * MathHelper.Pi) * 0.05f * ExtraUpdate / 6f;
		}
	}

	public override void Draw()
	{
		Texture2D tex = ModAsset.LanternGhostKing_Matrix.Value;
		float fade = 1f;
		float timeValue = MaxTime - Timer;
		float fadeThrethod = 60;
		if (timeValue < fadeThrethod)
		{
			fade *= timeValue / fadeThrethod;
		}
		if (Timer < fadeThrethod)
		{
			fade *= Timer / fadeThrethod;
		}
		float lightDisThrethod = 180;
		if (timeValue < lightDisThrethod)
		{
			lightDisparity = 1f - timeValue / lightDisThrethod;
			lightDisparity *= 1.5f;
		}

		Color powerColor = new Color(1f, 0.7f, 0.5f, 0f);
		Color ringColor = new Color(1f, 0.5f, 0.3f, 0f);
		int count = 200;
		float matrixRot = 0.568f;
		var bars = new List<Vertex2D>();
		for (int i = 0; i <= count; i++)
		{
			float rot = i / (float)count * MathHelper.TwoPi + matrixRot;
			float mulColor = 1 - Vector2.Dot(new Vector2(0, 1).RotatedBy(rot), new Vector2(0, 1).RotatedBy(Rotation)) * 0.5f + 0.5f;
			mulColor *= lightDisparity;
			mulColor = MathF.Pow(mulColor, 4);
			mulColor = 1 - mulColor;
			Vector2 drawPos0 = Position + new Vector2(0, 300).RotatedBy(rot) * Scale;
			Vector2 drawPos1 = Position + new Vector2(0, 100).RotatedBy(rot) * Scale;
			Vector2 drawPos2 = Position + new Vector2(0, 230).RotatedBy(rot) * Scale;

			float zCoord = 1 - mulColor * fade;
			bars.Add(drawPos0, powerColor * fade * mulColor, new Vector3(i * 4f / count, 0, zCoord));
			bars.Add(drawPos1, ringColor * fade * mulColor, new Vector3(i * 4f / count, 1, zCoord));
			Lighting.AddLight(drawPos2, new Vector3(1f, 0.8f, 0.5f) * fade);
		}
		Ins.Batch.Draw(tex, bars, PrimitiveType.TriangleStrip);

		Vector2 arrowRot = Position + new Vector2(0, 230).RotatedBy(Rotation) * Scale;
		Texture2D star = Commons.ModAsset.StarSlashGray.Value;
		float arrowScale = 0.5f;
		arrowScale += Math.Min(1.5f, Timer / 70f);
		Ins.Batch.Draw(star, arrowRot, null, powerColor * fade, Rotation, star.Size() * 0.5f, new Vector2(1f, arrowScale) * Scale, SpriteEffects.None);

		// if (!VFXManager.InScreen(arrowRot, -20))
		// {
		// for (int i = 0; i < 100; i++)
		// {
		// arrowRot += new Vector2(0, 10).RotatedBy(Rotation);
		// if (VFXManager.InScreen(arrowRot, -20))
		// {
		// arrowRot += new Vector2(0, 300).RotatedBy(Rotation);
		// Ins.Batch.Draw(star, arrowRot, null, powerColor * fade, Rotation, star.Size() * 0.5f, new Vector2(1f, arrowScale) * Scale, SpriteEffects.None);
		// break;
		// }
		// }
		// }

		float rotOffset = 1.5f;
		rotOffset -= Math.Min(1.5f, Timer / 100f);
		Vector2 arrowRotP = Position + new Vector2(0, 230).RotatedBy(Rotation + rotOffset) * Scale;
		Vector2 arrowRotN = Position + new Vector2(0, 230).RotatedBy(Rotation - rotOffset) * Scale;
		Ins.Batch.Draw(star, arrowRotP, null, ringColor * fade, Rotation + rotOffset, star.Size() * 0.5f, new Vector2(1f, arrowScale) * Scale * 0.75f, SpriteEffects.None);
		Ins.Batch.Draw(star, arrowRotN, null, ringColor * fade, Rotation - rotOffset, star.Size() * 0.5f, new Vector2(1f, arrowScale) * Scale * 0.75f, SpriteEffects.None);
	}
}
