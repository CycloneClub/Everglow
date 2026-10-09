using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Everglow.Commons.Skeleton2D.Renderer.DrawCommands;

/// <summary>
/// 立即模式绘制，什么也不管直接向GPU发送绘制请求，同步执行
/// </summary>
public class NaiveExecuter : IDrawCommandExecuter, IDrawCommandVisitor
{
	private GraphicsDevice graphicsDevice;
	private PipelineStateObject pipelineState;

	public void Visit<T>(DrawMesh<T> command) where T : struct, IVertexType
	{
		Main.graphics.graphicsDevice.RasterizerState = command.PipelineStateObject.RasterizerState;
		graphicsDevice.Textures[0] = command.PipelineStateObject.Texture;
		this.graphicsDevice.DrawUserPrimitives<T>(command.PrimitiveType, command.Vertices.ToArray(), command.Offset, command.GeometryCount);
	}

	public void Visit<T>(DrawIndexedMesh<T> command) where T : struct, IVertexType
	{
		Main.graphics.graphicsDevice.RasterizerState = command.PipelineStateObject.RasterizerState;
		graphicsDevice.Textures[0] = command.PipelineStateObject.Texture;
		this.graphicsDevice.DrawUserIndexedPrimitives<T>(command.PrimitiveType, command.Vertices.ToArray(), command.VertexStart, command.VertexCount,
			command.Indices.ToArray(), command.IndexStart, command.GeometryCount);
	}

	private void SaveCurrentPipelineState()
	{
	}

	private void PushPipelineState()
	{
	}

	private void PopPipelineState()
	{
	}

	public void Execute(DrawCommandList commandList, GraphicsDevice graphicsDevice)
	{
		SaveCurrentPipelineState();
		this.graphicsDevice = graphicsDevice;
		foreach (DrawCommand command in commandList)
		{
			PushPipelineState();
			command.Accept(this);
			PopPipelineState();
		}
	}
}
