using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Everglow.Commons.Vertex;

namespace Everglow.Commons.Skeleton2D.Renderer.DrawCommands;

public class DrawCommandList : List<DrawCommand>
{
	public DrawCommandList()
		: base()
	{
	}

	public DrawCommandList(IEnumerable<DrawCommand> commands)
		: base(commands)
	{
	}

	public void EmitDrawTriangleMesh<T>(PipelineStateObject pipelineState, List<T> vertices)
		where T : struct, IVertexType
	{
		this.Add(new DrawMesh<T>(pipelineState, PrimitiveType.TriangleList, vertices, 0, vertices.Count / 3));
	}

	public void EmitDrawLines<T>(PipelineStateObject pipelineState, List<T> vertices)
		where T : struct, IVertexType
	{
		this.Add(new DrawMesh<T>(pipelineState, PrimitiveType.LineList, vertices, 0, vertices.Count / 2));
	}

	public void EmitDrawIndexedTriangleMesh<T>(PipelineStateObject pipelineState, List<T> vertices, List<int> indices)
		where T : struct, IVertexType
	{
		this.Add(new DrawIndexedMesh<T>(pipelineState, PrimitiveType.TriangleList, vertices, indices, indices.Count / 3));
	}

	public void EmitCommand(DrawCommand drawCommand)
	{
		this.Add(drawCommand);
	}
}
