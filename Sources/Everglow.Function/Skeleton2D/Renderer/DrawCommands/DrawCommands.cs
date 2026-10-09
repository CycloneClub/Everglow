using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everglow.Commons.Skeleton2D.Renderer.DrawCommands;

public abstract class DrawCommand
{
	public abstract void Accept(IDrawCommandVisitor visitor);
}

public class DrawMesh<T> : DrawCommand
	where T : struct, IVertexType
{
	public PrimitiveType PrimitiveType { get; }

	public List<T> Vertices { get; }

	public int GeometryCount { get; }

	public int Offset { get; }

	public PipelineStateObject PipelineStateObject { get; }

	public DrawMesh(PipelineStateObject pipelineState, PrimitiveType primitiveType, List<T> vertices, int offset, int geometryCount)
	{
		this.PrimitiveType = primitiveType;
		this.Vertices = vertices;
		this.GeometryCount = geometryCount;
		this.Offset = offset;
		this.PipelineStateObject = pipelineState;
	}

	public override void Accept(IDrawCommandVisitor visitor)
	{
		visitor.Visit(this);
	}
}

public class DrawIndexedMesh<T> : DrawCommand
	where T : struct, IVertexType
{
	public PrimitiveType PrimitiveType { get; }

	public List<T> Vertices { get; }

	public List<int> Indices { get; }

	public int VertexStart { get; }

	public int VertexCount { get; }

	public int IndexStart { get; }

	public int GeometryCount { get; }

	public PipelineStateObject PipelineStateObject { get; }

	public DrawIndexedMesh(PipelineStateObject pipelineState, PrimitiveType primitiveType, List<T> vertices, List<int> indices, int geometryCount)
	{
		this.PrimitiveType = primitiveType;
		this.Vertices = vertices;
		this.GeometryCount = geometryCount;
		this.Indices = indices;
		this.VertexStart = 0;
		this.VertexCount = vertices.Count;
		this.IndexStart = 0;
		this.PipelineStateObject = pipelineState;
	}

	public override void Accept(IDrawCommandVisitor visitor)
	{
		visitor.Visit(this);
	}
}
