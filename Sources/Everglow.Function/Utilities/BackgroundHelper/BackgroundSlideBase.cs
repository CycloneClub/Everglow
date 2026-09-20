using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;

namespace Everglow.Commons.Utilities.BackgroundHelper;

public abstract class BackgroundSlideBase
{
	public float Distance;

	public Vector2 WorldAnchor;

	public Texture2D Texture;

	public string UniqueName => GetType().FullName;

	public Effect Shader;

	public float Alpha = 0f;

	public float Scale = 1f;

	public bool Active = true;

	/// <summary>
	/// 0 : Sky Color<br/>
	/// 1 : TileMap Colop<br/>
	/// 2 : White
	/// </summary>
	public int UseColorStyle = 0;

	/// <summary>
	/// It has higher priority in ordering the layer of backgrounds then <see cref="Distance"/>.<br/>
	/// Larger overlap the smaller.<br/>
	/// In most cases it should be 0.
	/// </summary>
	public int LayerPriority = 0;

	public virtual bool AllowMultiple => false;

	/// <summary>
	/// Only available when <see cref="AllowMultiple"/> is true. Defaults to 30. -1 means no max value.
	/// </summary>
	public virtual int MaxInstanceNumber => BackgroundSystem.MaxMultipleInstanceNumber;

	public virtual void SetDefaults()
	{
		Distance = float.PositiveInfinity;
		Shader = Effects.XWrap_YClamp_Shader;
	}

	public virtual void Update()
	{
		if (!CanActive())
		{
			FadeOut();
		}
		else
		{
			FadeIn();
		}
	}

	public virtual void FadeIn()
	{
		if (Alpha < 1)
		{
			Alpha += 0.01f;
		}
		else
		{
			Alpha = 1f;
		}
	}

	public virtual void FadeOut()
	{
		if (Alpha > 0)
		{
			Alpha -= 0.01f;
		}
		else
		{
			Alpha = 0f;
			Active = false;
		}
	}

	/// <summary>
	/// Active state condition.
	/// The system updates <see cref="Active"/> according to this.
	/// </summary>
	/// <returns></returns>
	public virtual bool CanActive()
	{
		return false;
	}

	public virtual void Draw()
	{
		DrawPreset_Normal(this);
	}

	private static Color GetColor(BackgroundSlideBase bg, Vector2 worldPos)
	{
		Color c = Color.White;
		switch (bg.UseColorStyle)
		{
			case 0:
				c = Main.ColorOfTheSkies;
				break;
			case 1:
				c = Lighting.GetColor((worldPos + new Vector2(-8)).ToTileCoordinates());
				break;
		}
		return c * bg.Alpha;
	}

	public static void DrawPreset_Piece(BackgroundSlideBase bg)
	{
		Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
		Vector2 move = (screenCenter - bg.WorldAnchor) / bg.Distance;
		Main.spriteBatch.Draw(bg.Texture, bg.WorldAnchor - move - Main.screenPosition, null, GetColor(bg, bg.WorldAnchor), 0, bg.Texture.Size() * 0.5f, bg.Scale / bg.Distance, SpriteEffects.None, 0);
	}

	public static void DrawPreset_Normal(BackgroundSlideBase bg)
	{
		Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
		Vector2 move = (screenCenter - bg.WorldAnchor) / bg.Distance;
		Vector2 texcoord_Move = move / bg.Texture.Size();

		int[] dirs = new int[] { 0, 1, -1, 2 };
		var bars = new List<Vertex2D>();
		for (int k = 0; k < 4; k++)
		{
			Vector2 dir = new Vector2(1, 1).RotatedBy(MathHelper.PiOver2 * dirs[k]);
			Vector2 screenPoint = new Vector2(-Main.screenWidth, -Main.screenHeight) * dir * 0.5f;
			Vector2 screen_move = screenPoint / bg.Texture.Size() + texcoord_Move;
			Color drawColor = GetColor(bg, screenCenter + screenPoint);
			bars.Add(screenCenter + screenPoint - Main.screenPosition, drawColor, new Vector3(screen_move + new Vector2(0.5f), 0));
		}
		DrawVertexBackground(bg, PrimitiveType.TriangleStrip, bars);
	}

	public static void Add_WorldPosVertex(BackgroundSlideBase bg, Vector2 position, List<Vertex2D> bars)
	{
		Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
		Vector2 move = (screenCenter - bg.WorldAnchor) / bg.Distance;
		Vector2 texcoord_Move = move / bg.Texture.Size();
		Vector2 screenPoint = position - screenCenter;
		Vector2 screen_move = screenPoint / bg.Texture.Size() + texcoord_Move;

		Color drawColor = GetColor(bg, position);
		bars.Add(position - Main.screenPosition, drawColor, new Vector3(screen_move + new Vector2(0.5f), 0));
	}

	public static void Add_WorldTriangle(BackgroundSlideBase bg, Vector2 v0, Vector2 v1, Vector2 v2, List<Vertex2D> bars)
	{
		Add_WorldPosVertex(bg, v0, bars);
		Add_WorldPosVertex(bg, v1, bars);
		Add_WorldPosVertex(bg, v2, bars);
	}

	/// <summary>
	/// Legacy code, use <see cref="BackgroundHigherPerformanceHelper.Add_TileBgVertice"> Instead.
	/// </summary>
	/// <param name="bg"></param>
	/// <param name="tiles"></param>
	/// <param name="bars"></param>
	[Obsolete]
	public static void Add_TileBgVertice(BackgroundSlideBase bg, List<Point> tiles, List<Vertex2D> bars)
	{
		foreach (var pos in tiles)
		{
			Vector2 worldTilePos = pos.ToWorldCoordinates();
			if (VFXManager.InScreen(worldTilePos, 64))
			{
				int[] dirs = new int[] { 0, 1, -1, -1, 1, 2 };
				for (int k = 0; k < dirs.Length; k++)
				{
					Add_WorldPosVertex(bg, worldTilePos + new Vector2(-24).RotatedBy(dirs[k] * MathHelper.PiOver2) + new Vector2(24), bars);
				}
			}
		}
	}

	[Obsolete]
	private static readonly Vector2[] _rotOffsets = new[]
	{
		new Vector2(0, -24),
		new Vector2(0, 24),
	};

	/// <summary>
	/// Legacy code, use <see cref="BackgroundHigherPerformanceHelper.Add_TileBgVertice"> Instead.
	/// </summary>
	/// <param name="bg"></param>
	/// <param name="tiles"></param>
	/// <param name="bars"></param>
	[Obsolete]
	public static void Add_TileBgVertice_Strip(BackgroundSlideBase bg, List<Point> tiles, List<Vertex2D> bars)
	{
		var visibleTiles = tiles
			.Select(p => new { Pos = p, World = p.ToWorldCoordinates() })
			.Where(t => VFXManager.InScreen(t.World, 64))
			.OrderBy(t => t.Pos.Y)
			.ThenBy(t => t.Pos.X)
			.ToList();

		if (visibleTiles.Count == 0)
		{
			return;
		}

		int lastY = visibleTiles[0].Pos.Y;

		foreach (var tile in visibleTiles)
		{
			Vector2 basePos = tile.World;
			if (tile.Pos.Y != lastY)
			{
				Vector2 lastPos = bars[^1].position;
				bars.Add(lastPos + new Vector2(0, -48), Color.Transparent, new Vector3(0));
				bars.Add(lastPos + new Vector2(0, 0), Color.Transparent, new Vector3(0));
				Vector2 finalPos = basePos + new Vector2(24, 24) - Main.screenPosition;
				bars.Add(finalPos + new Vector2(0, -24), Color.Transparent, new Vector3(0));
				bars.Add(finalPos + new Vector2(0, 24), Color.Transparent, new Vector3(0));
				lastY = tile.Pos.Y;
			}
			foreach (var offset in _rotOffsets)
			{
				Vector2 finalPos = basePos + offset + new Vector2(24, 24);
				Add_WorldPosVertex(bg, finalPos, bars);
			}
		}
	}

	public static void DrawVertexBackground(BackgroundSlideBase bg, PrimitiveType primitiveType, List<Vertex2D> bars)
	{
		if (bars.Count > 2)
		{
			Main.graphics.graphicsDevice.Textures[0] = bg.Texture;
			if (primitiveType == PrimitiveType.TriangleList)
			{
				Main.graphics.graphicsDevice.DrawUserPrimitives(primitiveType, bars.ToArray(), 0, bars.Count / 3);
			}
			if (primitiveType == PrimitiveType.TriangleStrip)
			{
				Main.graphics.graphicsDevice.DrawUserPrimitives(primitiveType, bars.ToArray(), 0, bars.Count - 2);
			}
		}
	}

	public static void DrawVertexBackground(Texture2D texture, PrimitiveType primitiveType, List<Vertex2D> bars)
	{
		if (bars.Count > 2)
		{
			Main.graphics.graphicsDevice.Textures[0] = texture;
			if (primitiveType == PrimitiveType.TriangleList)
			{
				Main.graphics.graphicsDevice.DrawUserPrimitives(primitiveType, bars.ToArray(), 0, bars.Count / 3);
			}
			if (primitiveType == PrimitiveType.TriangleStrip)
			{
				Main.graphics.graphicsDevice.DrawUserPrimitives(primitiveType, bars.ToArray(), 0, bars.Count - 2);
			}
		}
	}

	public class Effects
	{
		public static readonly Effect XClamp_YClamp_Shader = ModAsset.BgShader_X_Clamp_Y_Clamp.Value;

		public static readonly Effect XWrap_YClamp_Shader = ModAsset.BgShader_X_Wrap_Y_Clamp.Value;

		public static readonly Effect XClamp_YWrap_Shader = ModAsset.BgShader_X_Clamp_Y_Wrap.Value;

		public static readonly Effect XWrap_YWrap_Shader = ModAsset.BgShader_X_Wrap_Y_Wrap.Value;
	}
}
