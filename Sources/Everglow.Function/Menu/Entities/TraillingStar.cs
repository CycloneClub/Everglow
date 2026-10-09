using Everglow.Commons.Vertex;

namespace Everglow.Commons.Menu.Entities;

internal class TraillingStar : Star
{
	public Vector2[] oldPos = new Vector2[15];
	public Vector2 velocity;
	public override void Update()
	{
		position += velocity;
		base.Update();
	}
	public override void Draw()
	{
		base.Draw();

		for (int i = oldPos.Length - 1; i > 0; --i)
		{
			oldPos[i] = oldPos[i - 1];
		}
		oldPos[0] = position;

		List<Vertex2D> vertices = new();
		var c = new Color(200, 200, 255);
		for (int i = 0; i < oldPos.Length; i++)
		{
			if (oldPos[i] == Vector2.Zero)
			{
				continue;
			}
			var v = Vector2.Normalize(velocity.RotatedBy(1.57f));
			float w = 12 * velocity.Length() / 30f;
			float f = (float)i / oldPos.Length;

			float a = 0.7f;
			if (f > 0.5f)
			{
				a *= 1 - f;
			}

			vertices.Add(new(oldPos[i] + w * v, c * a, new Vector3(f, 0, 0)));
			vertices.Add(new(oldPos[i] - w * v, c * a, new Vector3(f, 1, 0)));
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

		Texture2D tex = ModContent.Request<Texture2D>("Terraria/Images/Extra_196").Value;
		Main.graphics.GraphicsDevice.Textures[0] = tex;
		if (vertices.Count > 2)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
	}
}
