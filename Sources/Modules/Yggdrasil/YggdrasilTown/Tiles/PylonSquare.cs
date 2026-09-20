using Everglow.Commons.Utilities.BackgroundHelper;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class PylonSquare : BackgroundSlideBase
{
	public Point TileAnchor;

	public List<Point> BgTiles = [];

	public List<Rectangle> GlowFrames = [];

	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.PylonSquare.Value;
		Distance = 1;
		UseColorStyle = 1;
		LayerPriority = 2;
		Shader = Effects.XWrap_YWrap_Shader;
	}

	public override void Update()
	{
		base.Update();
	}

	public override void Draw()
	{
		Texture2D chain = ModAsset.PylonSquare_Chain.Value;
		Vector2 center = WorldAnchor + new Vector2(1616, 176);
		Vector2 drawPos0 = center + new Vector2(454, 300);
		Vector2 drawPos1 = center + new Vector2(-454, 300);
		//Main.spriteBatch.Draw(chain, drawPos0 - Main.screenPosition, null, Main.ColorOfTheSkies, 0, new Vector2(chain.Width * 0.5f, chain.Height), 1f, SpriteEffects.None, 0);
		//Main.spriteBatch.Draw(chain, drawPos1 - Main.screenPosition, null, Main.ColorOfTheSkies, 0, new Vector2(chain.Width * 0.5f, chain.Height), 1f, SpriteEffects.FlipHorizontally, 0);

		var bars = new List<Vertex2D>();
		BackgroundHigherPerformanceHelper.Add_TileBgVertice(this, BgTiles, bars, 1);
		DrawVertexBackground(this, PrimitiveType.TriangleStrip, bars);

		Texture2D torch = ModAsset.PylonSquare_Torch.Value;

		drawPos0 = center - new Vector2(664, 0);
		drawPos1 = center + new Vector2(664, 0);
		int tick = (int)(Main.time / 6) % 10;
		Rectangle frame = new Rectangle(0, 350 * tick, 200, 350);

		Lighting.AddLight(drawPos0 + new Vector2(0, -150), new Vector3(3f, 2f, 6f));
		Main.spriteBatch.Draw(torch, drawPos0 - Main.screenPosition, frame, Color.White, 0, new Vector2(100, 350), 1f, SpriteEffects.None, 0);

		Lighting.AddLight(drawPos1 + new Vector2(0, -150), new Vector3(3f, 2f, 6f));
		Main.spriteBatch.Draw(torch, drawPos1 - Main.screenPosition, frame, Color.White, 0, new Vector2(100, 350), 1f, SpriteEffects.FlipHorizontally, 0);
	}

	public override bool CanActive()
	{
		return TileUtils.SafeGetTile(TileAnchor).TileType == ModContent.TileType<YggdrasilCommandBlock>();
	}
}
