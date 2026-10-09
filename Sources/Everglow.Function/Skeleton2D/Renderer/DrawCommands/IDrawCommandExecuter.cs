using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everglow.Commons.Skeleton2D.Renderer.DrawCommands;

public interface IDrawCommandVisitor
{
	void Visit<T>(DrawMesh<T> command) where T : struct, IVertexType;

	void Visit<T>(DrawIndexedMesh<T> command) where T : struct, IVertexType;
}

internal interface IDrawCommandExecuter
{
	void Execute(DrawCommandList commandList, GraphicsDevice graphicsDevice);
}
