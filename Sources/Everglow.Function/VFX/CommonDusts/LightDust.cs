using Everglow.Commons.Enums;
using Everglow.Commons.Graphics;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX.Pipelines;
using SteelSeries.GameSense;

namespace Everglow.Commons.VFX.CommonDusts;

[Pipeline(typeof(WCSPipeline))]
public class LightDust : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 position;
	public Vector2 velocity;
	public float timeleft;
	public float maxTimeleft;
	public float scale;
	public GradientColor color;
	public Entity Owner;
	public AIStyle aiStyle = AIStyle.Normal;
	public float alpha;

	public enum AIStyle
	{
		Normal,
		Rotation,
		Rotation2,
	}

	public override void OnSpawn()
	{
	}

	public override void Update()
	{
		position += velocity;
		timeleft--;
		if (timeleft < 0)
		{
			Kill();
		}

		if (alpha < 1)
		{
			alpha += 0.1f;
		}
		if (aiStyle == AIStyle.Normal)
		{
		}
		else if (aiStyle == AIStyle.Rotation)
		{
			velocity = velocity.RotatedBy(0.04f);
		}
		else
		{
			velocity = velocity.RotatedBy(-0.04f);
		}
		if (timeleft < maxTimeleft * 0.6f)
		{
			scale *= 0.93f;
			velocity *= 0.95f;
		}
	}

	public override void Draw()
	{
		if (timeleft > 0)
		{
			Color c = color.GetColor(1 - timeleft / maxTimeleft) * alpha;
			// c.A = (byte)((1 - timeleft / maxTimeleft) * 255);
			Vector2 drawPos = position;
			if (Owner != null)
			{
				drawPos += Owner.Center;
			}

			Texture2D tex = ModAsset.LightPoint2.Value;
			Ins.Batch.Draw(tex, drawPos, null, c, 0, tex.Size() / 2, scale, 0);
			Ins.Batch.Draw(tex, drawPos, null, c, 0, tex.Size() / 2, scale, 0);
		}
	}
}
