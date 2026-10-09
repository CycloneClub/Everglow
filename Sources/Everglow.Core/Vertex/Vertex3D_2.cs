namespace Everglow.Commons.Vertex;

/// <summary>
/// 顶点数据结构：空间坐标,颜色,纹理坐标
/// </summary>
public struct Vertex3D_2 : IVertexType
{
	private static VertexDeclaration _vertexDeclaration = new(new VertexElement[3]
	{
		new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
		new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
		new VertexElement(16, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0),
	});

	public Vector3 Position;
	public Color Color;
	public Vector3 TexCoord;

	public Vertex3D_2(Vector3 position, Vector3 texCoord, Color color)
	{
		Position = position;
		TexCoord = texCoord;
		Color = color;
	}

	public override string ToString()
	{
		return $"[{Position}, {Color}, {TexCoord}]";
	}

	public VertexDeclaration VertexDeclaration => _vertexDeclaration;
}
