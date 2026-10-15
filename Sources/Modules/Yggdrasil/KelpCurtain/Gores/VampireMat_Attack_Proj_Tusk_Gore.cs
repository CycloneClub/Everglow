namespace Everglow.Yggdrasil.KelpCurtain.Gores;

[Pipeline(typeof(WCSPipeline))]
public class VampireMat_Attack_Proj_Tusk_Gore : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public Vector2 Position;

	public Vector2 Velocity;

	public float Rotation;

	public float RotateSpeed;

	public float Timer = 0;

	public float MaxTime = 120;

	public float Fade = 1;

	public bool Stop = false;

	public Rectangle Frame;

	public override void Update()
	{
		Timer++;
		if (MaxTime - Timer < 30)
		{
			Fade = (MaxTime - Timer) / 30f;
		}
		else
		{
			Fade = 1;
		}
		if (Timer >= MaxTime)
		{
			Active = false;
			return;
		}
		if (!Stop)
		{
			Position += Velocity;
			Rotation += RotateSpeed;
			float size = Frame.Width * Frame.Height / 300f;
			Velocity.Y += 0.25f * size;
			Velocity *= 0.98f;
			RotateSpeed *= 0.98f;
			if (Collision.IsWorldPointSolid(Position + new Vector2(Velocity.X, 0)))
			{
				Velocity.X *= -Main.rand.NextFloat(0.25f, 0.96f);
				RotateSpeed *= Main.rand.NextFloat(0.25f, 0.96f);
			}
			if (Collision.IsWorldPointSolid(Position + new Vector2(0, Velocity.Y)))
			{
				Velocity.Y *= -Main.rand.NextFloat(0.25f, 0.96f);
				RotateSpeed *= Main.rand.NextFloat(0.25f, 0.96f);
			}
			if (Velocity.Length() < 1f)
			{
				Stop = true;
			}
		}
	}

	public override void Draw()
	{
		if (Timer < 20)
		{
			Rectangle whiteFrame = Frame;
			whiteFrame.X += 36;
			Color drawColor = Color.White;
			if (Timer >= 10)
			{
				drawColor *= (20 - Timer) / 10f;
			}
			Ins.Batch.Draw(ModAsset.VampireMat_Attack_Proj_Tusk_Gore.Value, Position, whiteFrame, drawColor, Rotation, whiteFrame.Size() * 0.5f, 1, SpriteEffects.None);
		}
		Ins.Batch.Draw(ModAsset.VampireMat_Attack_Proj_Tusk_Gore.Value, Position, Frame, Lighting.GetColor(Position.ToTileCoordinates()) * Fade, Rotation, Frame.Size() * 0.5f, 1, SpriteEffects.None);
	}
}
