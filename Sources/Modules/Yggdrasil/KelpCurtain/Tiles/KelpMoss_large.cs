using Everglow.Commons.VFX.Scene;
using SubworldLibrary;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles;

public class KelpMoss_large_fore : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;
	public override void OnSpawn()
	{
		Texture = ModAsset.KelpMoss_large.Value;
	}

	public override void Update()
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>())
		{
			Active = false;
		}
		for (int i = 0; i < wigglerRotations.Count; i++)
		{
			if (i >= wigglerOmegas.Count)
			{
				wigglerOmegas.Add(MathF.Sin(Position.X + Position.Y + i * 0.4f + (float)Main.time * 0.05f) * 0.03f);
			}
			else
			{
				wigglerRotations[i] += wigglerOmegas[i];
				wigglerOmegas[i] *= 0.9f;
				wigglerOmegas[i] += MathF.Sin(Position.X + Position.Y + i * 0.4f + (float)Main.time * 0.05f) * 0.0005f - Main.windSpeedCurrent * 0.001f / wigglerRotations.Count;
				wigglerRotations[i] = wigglerRotations[i] * 0.9f + startRotation * MathF.Pow(0.7f, i) * 0.1f;
			}
		}
		base.Update();
	}

	public float startRotation;
	public float scale;
	public List<float> wigglerRotations = new List<float>();
	public List<float> wigglerOmegas = new List<float>();
	public Vector2 unit = new Vector2(0, 1);

	private static Color GetPosLight(Vector2 worldCoord)
	{
		return Lighting.GetColor((int)worldCoord.X / 16, (int)worldCoord.Y / 16);
	}

	public override void Draw()
	{
		Vector2 pos0 = Position + unit.RotatedBy(startRotation + MathHelper.PiOver2) * scale;
		Vector2 pos1 = Position - unit.RotatedBy(startRotation + MathHelper.PiOver2) * scale;
		Vector2 centerPos = Position;
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(pos0, GetPosLight(pos0), new Vector3(0, 0, 1)),
			new Vertex2D(pos1, GetPosLight(pos1), new Vector3(1, 0, 1)),
		};
		centerPos += unit.RotatedBy(startRotation) * 40;
		for (int i = 0; i < wigglerRotations.Count; i++)
		{
			Vector2 pos2 = centerPos + unit.RotatedBy(wigglerRotations[i] + MathHelper.PiOver2) * scale;
			Vector2 pos3 = centerPos - unit.RotatedBy(wigglerRotations[i] + MathHelper.PiOver2) * scale;
			float factor = (float)(i + 1) / wigglerRotations.Count;
			bars.Add(pos2, GetPosLight(pos2), new Vector3(0, factor, 1 - factor));
			bars.Add(pos3, GetPosLight(pos3), new Vector3(1, factor, 1 - factor));
			centerPos += unit.RotatedBy(wigglerRotations[i]) * 40;
		}
		Vector2 pos4 = centerPos + unit.RotatedBy(MathHelper.PiOver2) * scale;
		Vector2 pos5 = centerPos - unit.RotatedBy(MathHelper.PiOver2) * scale;
		bars.Add(pos4, GetPosLight(pos4), new Vector3(0, 1, 0));
		bars.Add(pos5, GetPosLight(pos5), new Vector3(1, 1, 0));
		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleStrip);
	}
}
