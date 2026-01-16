using Everglow.Commons.Enums;

namespace Everglow.Commons.VFX.CommonVFXDusts.Fluid_Smoke;

[Pipeline(typeof(Fluid_field_pipeline), typeof(Fluid_smoke_Pipeline))]
public class BrushCanvas : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public float Timer;

	public Vector2 Position;

	public Vector2 OldVel;

	public override void Update()
	{
		Timer++;
		if (Main.mouseRight && Main.mouseRightRelease)
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		Texture2D whitePoint = ModAsset.PressSpot.Value;
		Texture2D blackPoint = ModAsset.PressSpot_black.Value;
		Vector2 vel = Main.MouseWorld - Position;
		float brushScale = 0.5f;
		int length = (int)vel.Length();
		if (length <= 1)
		{
			float velXToR = TransformVelocityToChannel(vel.X);
			float velYToG = TransformVelocityToChannel(vel.Y);
			Ins.Batch.Draw(blackPoint, Main.MouseWorld, null, Color.White, (float)Main.timeForVisualEffects * 0.15f, blackPoint.Size() * 0.5f, brushScale, SpriteEffects.None);
			Ins.Batch.Draw(whitePoint, Main.MouseWorld, null, new Color(velXToR + 1 / 256f, velYToG + 1 / 256f, 0f, 0f), (float)Main.timeForVisualEffects * 0.15f, whitePoint.Size() * 0.5f, brushScale, SpriteEffects.None);
		}
		else
		{
			for (int i = 0; i < length; i++)
			{
				float velXToR = TransformVelocityToChannel((vel.X * i + OldVel.X * (length - i)) / length);
				float velYToG = TransformVelocityToChannel((vel.Y * i + OldVel.Y * (length - i)) / length);
				Ins.Batch.Draw(blackPoint, (Main.MouseWorld * i + Position * (length - i)) / length, null, Color.White, (float)Main.timeForVisualEffects * 0.15f, blackPoint.Size() * 0.5f, brushScale, SpriteEffects.None);
				Ins.Batch.Draw(whitePoint, (Main.MouseWorld * i + Position * (length - i)) / length, null, new Color(velXToR + 1 / 256f, velYToG + 1 / 256f, 0f, 0f), (float)Main.timeForVisualEffects * 0.15f, whitePoint.Size() * 0.5f, brushScale, SpriteEffects.None);
			}
		}

		Position = Main.MouseWorld;
		OldVel = vel;
	}

	public float TransformVelocityToChannel(float value)
	{
		float size = 320;
		value = Math.Clamp(value, -size, size);
		value += size;
		return value / size * 0.5f;
	}
}