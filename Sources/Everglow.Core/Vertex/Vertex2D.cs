namespace Everglow.Commons.Vertex;

public struct Vertex2D : IVertexType
{
	private static VertexDeclaration _vertexDeclaration = new(new VertexElement[3]
	{
		new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
		new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
		new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0),
	});
	public Vector2 position;
	public Color color;
	public Vector3 texCoord;

	public Vertex2D(Vector2 position, Color color, Vector3 texCoord)
	{
		this.position = position;
		this.color = color;
		this.texCoord = texCoord;
	}

	public override string ToString()
	{
		return $"[{position}, {color}, {texCoord}]";
	}

	public VertexDeclaration VertexDeclaration => _vertexDeclaration;
}
