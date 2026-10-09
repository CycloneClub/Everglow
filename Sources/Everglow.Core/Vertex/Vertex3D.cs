namespace Everglow.Commons.Vertex;

/// <summary>
/// 顶点数据结构：空间坐标,纹理坐标,法线向量
/// </summary>
public struct Vertex3D : IVertexType
{
	private static VertexDeclaration _vertexDeclaration = new(new VertexElement[3]
	{
			new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
			new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0),
			new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
	});
	public Vector3 position;
	public Vector3 texcoord;
	public Vector3 normal;

	public Vertex3D(Vector3 position, Vector3 texcoord, Vector3 normal)
	{
		this.position = position;
		this.texcoord = texcoord;
		this.normal = normal;
	}

	public override string ToString()
	{
		return $"[{position}, {texcoord}, {normal}]";
	}

	public VertexDeclaration VertexDeclaration => _vertexDeclaration;
}
