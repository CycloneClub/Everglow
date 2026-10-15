using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.IsleOfBloom;

[Pipeline(typeof(WCSPipeline_PointWrap))]
public class IsleOfBloom_CaveRay_VFX : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;

	public override void OnSpawn()
	{
		Texture = Commons.ModAsset.Noise_flame_2.Value;
		MaxDiatanceOutOfScreen = 3000;
	}

	public override void Update()
	{
		base.Update();
	}

	public override void Draw()
	{
		Vector2 center = OriginTilePos.ToWorldCoordinates(8, 8) + new Vector2(0, 320);
		Color color = new Color(0.4f, 0.4f, 0.6f, 0) * 0.1f;
		float timeValue = Main.GlobalTimeWrappedHourly * 0.05f;
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int dy = -30; dy <= 30; dy++)
		{
			float fade = MathF.Cos(dy / 30f * MathF.PI) * 0.5f + 0.5f;
			Vector2 drawPos = center + new Vector2(0, dy * 16);
			Lighting.AddLight(drawPos, new Vector3(0.1f, 0.1f, 0.15f));
			bars.Add(drawPos + new Vector2(-32, 0), color * fade, new Vector3(0.3f, timeValue, 0));
			bars.Add(drawPos + new Vector2(32, 0), color * fade, new Vector3(0.7f, timeValue, 0));
		}
		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleStrip);
		bars = new List<Vertex2D>();
		for (int dy = -30; dy <= 30; dy++)
		{
			float fade = MathF.Cos(dy / 30f * MathF.PI) * 0.5f + 0.5f;
			Vector2 drawPos = center + new Vector2(0, dy * 16);
			bars.Add(drawPos + new Vector2(-64, 0), color * 0, new Vector3(-0.1f, timeValue, 0));
			bars.Add(drawPos + new Vector2(-32, 0), color * fade, new Vector3(0.3f, timeValue, 0));
		}
		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleStrip);
		bars = new List<Vertex2D>();
		for (int dy = -30; dy <= 30; dy++)
		{
			float fade = MathF.Cos(dy / 30f * MathF.PI) * 0.5f + 0.5f;
			Vector2 drawPos = center + new Vector2(0, dy * 16);
			bars.Add(drawPos + new Vector2(64, 0), color * 0, new Vector3(1.1f, timeValue, 0));
			bars.Add(drawPos + new Vector2(32, 0), color * fade, new Vector3(0.7f, timeValue, 0));
		}
		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleStrip);
	}
}
