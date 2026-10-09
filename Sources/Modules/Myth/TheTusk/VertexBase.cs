namespace Everglow.Myth.TheTusk;

public class VertexBase
{
	public struct CustomVertexInfo : IVertexType
	{
		private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[3]
		{
			new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
			new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
			new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0),
		});

		public Vector2 Position;
		public Color Color;
		public Vector3 TexCoord;

		public CustomVertexInfo(Vector2 position, Color color, Vector3 texCoord)
		{
			Position = position;
			Color = color;
			TexCoord = texCoord;
		}

		public VertexDeclaration VertexDeclaration
		{
			get
			{
				return _vertexDeclaration;
			}
		}
	}

	public struct CustomVertexInfoFor3D : IVertexType
	{
		private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[3]
		{
			new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
			new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0),
			new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
		});

		public Vector3 Position;
		public Vector3 Texcoord;
		public Vector3 Normal;

		public CustomVertexInfoFor3D(Vector3 position, Vector3 texcoord, Vector3 normal)
		{
			Position = position;
			Texcoord = texcoord;
			Normal = normal;
		}

		public VertexDeclaration VertexDeclaration
		{
			get
			{
				return _vertexDeclaration;
			}
		}
	}
}
