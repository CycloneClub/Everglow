using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Everglow.Commons.Skeleton2D.Renderer.DrawCommands;
using Everglow.Commons.Vertex;
using Spine;
using Terraria.GameContent;

namespace Everglow.Commons.Skeleton2D.Renderer;

public class GeometryBuffer
{
	private List<Vertex2D> vertices = new List<Vertex2D>();
	private Color color = Color.White;

	public GeometryBuffer()
	{
	}

	public void SetColor(Color color)
	{
		this.color = color;
	}

	public void Begin()
	{
		vertices.Clear();
		// device.RasterizerState = new RasterizerState();
		// device.BlendState = BlendState.AlphaBlend;
	}

	public void Line(float x1, float y1, float x2, float y2, float z = 0f)
	{
		vertices.Add(new Vertex2D(new Vector2(x1, y1) - Main.screenPosition, color, Vector3.Zero));
		vertices.Add(new Vertex2D(new Vector2(x2, y2) - Main.screenPosition, color, Vector3.Zero));
	}

	/** Calls {@link #circle(float, float, float, int)} by estimating the number of segments needed for a smooth circle. */
	public void Circle(float x, float y, float radius, float z = 0f)
	{
		Circle(x, y, radius, Math.Max(1, (int)(6 * (float)Math.Pow(radius, 1.0f / 3.0f))), z);
	}

	/** Draws a circle using {@link ShapeType#Line} or {@link ShapeType#Filled}. */
	public void Circle(float x, float y, float radius, int segments, float z = 0f)
	{
		if (segments <= 0)
		{
			throw new ArgumentException("segments must be > 0.");
		}
		float angle = 2 * MathUtils.PI / segments;
		float cos = MathUtils.Cos(angle);
		float sin = MathUtils.Sin(angle);
		float cx = radius, cy = 0;
		float temp = 0;

		for (int i = 0; i < segments; i++)
		{
			vertices.Add(new Vertex2D(new Vector2(x + cx, y + cy) - Main.screenPosition, color, Vector3.Zero));
			temp = cx;
			cx = cos * cx - sin * cy;
			cy = sin * temp + cos * cy;
			vertices.Add(new Vertex2D(new Vector2(x + cx, y + cy) - Main.screenPosition, color, Vector3.Zero));
		}
		vertices.Add(new Vertex2D(new Vector2(x + cx, y + cy) - Main.screenPosition, color, Vector3.Zero));

		temp = cx;
		cx = radius;
		cy = 0;
		vertices.Add(new Vertex2D(new Vector2(x + cx, y + cy) - Main.screenPosition, color, Vector3.Zero));
	}

	public void Triangle(float x1, float y1, float x2, float y2, float x3, float y3, float z = 0f)
	{
		vertices.Add(new Vertex2D(new Vector2(x1, y1) - Main.screenPosition, color, Vector3.Zero));
		vertices.Add(new Vertex2D(new Vector2(x2, y2) - Main.screenPosition, color, Vector3.Zero));

		vertices.Add(new Vertex2D(new Vector2(x2, y2) - Main.screenPosition, color, Vector3.Zero));
		vertices.Add(new Vertex2D(new Vector2(x3, y3) - Main.screenPosition, color, Vector3.Zero));

		vertices.Add(new Vertex2D(new Vector2(x3, y3) - Main.screenPosition, color, Vector3.Zero));
		vertices.Add(new Vertex2D(new Vector2(x1, y1) - Main.screenPosition, color, Vector3.Zero));
	}

	public void X(float x, float y, float len, float z = 0f)
	{
		Line(x + len, y + len, x - len, y - len, z);
		Line(x - len, y + len, x + len, y - len, z);
	}

	public void Polygon(float[] polygonVertices, int offset, int count, float z = 0f)
	{
		if (count < 3)
			throw new ArgumentException("Polygon must contain at least 3 vertices");

		offset <<= 1;

		var firstX = polygonVertices[offset];
		var firstY = polygonVertices[offset + 1];
		var last = offset + count;

		for (int i = offset, n = offset + count; i < n; i += 2)
		{
			var x1 = polygonVertices[i];
			var y1 = polygonVertices[i + 1];

			var x2 = 0f;
			var y2 = 0f;

			if (i + 2 >= last)
			{
				x2 = firstX;
				y2 = firstY;
			}
			else
			{
				x2 = polygonVertices[i + 2];
				y2 = polygonVertices[i + 3];
			}

			Line(x1, y1, x2, y2, z);
		}
	}

	public void Rect(float x, float y, float width, float height, float z = 0f)
	{
		Line(x, y, x + width, y, z);
		Line(x + width, y, x + width, y + height, z);
		Line(x + width, y + height, x, y + height, z);
		Line(x, y + height, x, y, z);
	}

	public DrawCommandList End()
	{
		DrawCommandList drawCommands = new DrawCommandList();
		if (vertices.Count == 0)
		{
			return drawCommands;
		}

		PipelineStateObject pipelineState = new PipelineStateObject()
		{
			Texture = TextureAssets.MagicPixel.Value,
		};
		drawCommands.EmitDrawLines(pipelineState, vertices);
		return drawCommands;
	}
}
