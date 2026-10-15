using Everglow.Commons.Interfaces;
using Everglow.Commons.Vertex;
using Terraria;

namespace Everglow.Commons.VFX;

public class VFXBatch : IDisposable
{
	#region Private Field

	//如果所有网格都是Strip形式
	private const int MAX_INDICES = MAX_VERTICES * 3;

	//numbers Copy from SpriteBatch
	private const int MAX_VERTICES = 1048576;

	private List<IBuffers> buffers = new();

	private GraphicsDevice _graphicsDevice;

	private IMainThreadContext _mainThread;

	private bool hasBegun = false;

	private List<bool> needFlush = new();

	#endregion Private Field

	public VFXBatch(GraphicsDevice graphics, IMainThreadContext mainThread)
	{
		_graphicsDevice = graphics;
		_mainThread = mainThread;
		mainThread.AddTask(() =>
		{
			RegisterVertex<VFX2D>(MAX_VERTICES, MAX_VERTICES * 6 / 4);//四个顶点两个三角形六个下标
			RegisterVertex<Vertex2D>();
		});
	}

	public GraphicsDevice GraphicsDevice => _graphicsDevice;

	#region Draw Method

	public void Draw(Vector2 position, Color color)
	{
		Debug.Assert(hasBegun, "Begin not called!");
		var tex = Buffer<VFX2D>.CurrentTexture;
		if (!Buffer<VFX2D>.CheckSize(4))
		{
			Flush<VFX2D>();
			Buffer<VFX2D>.Textures.Add(tex);
		}
		needFlush[0] = true;
		Buffer<VFX2D>.AddVertex(new VFX2D[]
		{
			new VFX2D(position, color, new Vector2(0,0)),
			new VFX2D(position + new Vector2(tex.Width, 0), color, new Vector2(1,0)),
			new VFX2D(position + new Vector2(0, tex.Height), color, new Vector2(0,1)),
			new VFX2D(position + new Vector2(tex.Width, tex.Height), color, new Vector2(1,1))
		}, PrimitiveType.TriangleStrip);
	}

	public void Draw(Vector2 position, Rectangle? sourceRectangle, Color color)
	{
		Debug.Assert(hasBegun, "Begin not called!");
		var tex = Buffer<VFX2D>.CurrentTexture;
		if (!Buffer<VFX2D>.CheckSize(4))
		{
			Flush<VFX2D>();
			Buffer<VFX2D>.Textures.Add(tex);
		}
		needFlush[0] = true;
		Rectangle sourceRect = sourceRectangle ?? new Rectangle(0, 0, tex.Width, tex.Height);

		float x = sourceRect.X / (float)tex.Width;
		float y = sourceRect.Y / (float)tex.Height;
		float width = sourceRect.Width / (float)tex.Width;
		float height = sourceRect.Height / (float)tex.Height;
		Buffer<VFX2D>.AddVertex(new VFX2D[]
		{
			new VFX2D(position, color, new Vector2(x, y)),
			new VFX2D(position + new Vector2(sourceRect.Width, 0), color, new Vector2(x + width, y)),
			new VFX2D(position + new Vector2(0, sourceRect.Height), color, new Vector2(x, y + height)),
			new VFX2D(position + new Vector2(sourceRect.Width, sourceRect.Height), color, new Vector2(x + width, y + height))
		}, PrimitiveType.TriangleStrip);
	}

	public void Draw(Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects)
	{
		Draw(position, sourceRectangle, color, rotation, origin, new Vector2(scale), effects);
	}

	public void Draw(Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects)
	{
		Debug.Assert(hasBegun, "Begin not called!");
		var tex = Buffer<VFX2D>.CurrentTexture;
		if (!Buffer<VFX2D>.CheckSize(4))
		{
			Flush<VFX2D>();
			Buffer<VFX2D>.Textures.Add(tex);
		}
		needFlush[0] = true;
		Rectangle sourceRect = sourceRectangle ?? new Rectangle(0, 0, tex.Width, tex.Height);
		Vector2 topLeftPosition = position - origin;
		Matrix matrix = Matrix.CreateTranslation(-position.X, -position.Y, 0)
			* Matrix.CreateScale(scale.X, scale.Y, 0)
			* Matrix.CreateRotationZ(rotation)
			* Matrix.CreateTranslation(position.X, position.Y, 0);
		float x = sourceRect.X / (float)tex.Width;
		float y = sourceRect.Y / (float)tex.Height;
		float width = sourceRect.Width / (float)tex.Width;
		float height = sourceRect.Height / (float)tex.Height;

		Vector2 topLeft = new(x, y);
		Vector2 topRight = new(x + width, y);
		Vector2 bottomLeft = new(x, y + height);
		Vector2 bottomRight = new(x + width, y + height);
		if (effects.HasFlag(SpriteEffects.FlipHorizontally))
		{
			(topLeft, topRight) = (topRight, topLeft);
			(bottomLeft, bottomRight) = (bottomRight, bottomLeft);
		}
		if (effects.HasFlag(SpriteEffects.FlipVertically))
		{
			(topLeft, bottomLeft) = (bottomLeft, topLeft);
			(topRight, bottomRight) = (bottomRight, topRight);
		}
		Buffer<VFX2D>.AddVertex(new VFX2D[]
		{
			new VFX2D(Vector2.Transform(topLeftPosition, matrix), color, topLeft),
			new VFX2D(Vector2.Transform(topLeftPosition + new Vector2(sourceRect.Width, 0), matrix), color, topRight),
			new VFX2D(Vector2.Transform(topLeftPosition + new Vector2(0, sourceRect.Height), matrix), color, bottomLeft),
			new VFX2D(Vector2.Transform(topLeftPosition + new Vector2(sourceRect.Width, sourceRect.Height), matrix), color, bottomRight)
		}, PrimitiveType.TriangleStrip);
	}

	public void Draw(Vector2 position, Rectangle? sourceRectangle, Color color, Matrix matrix)
	{
		Debug.Assert(hasBegun, "Begin not called!");
		var tex = Buffer<VFX2D>.CurrentTexture;
		if (!Buffer<VFX2D>.CheckSize(4))
		{
			Flush<VFX2D>();
			Buffer<VFX2D>.Textures.Add(tex);
		}
		needFlush[0] = true;
		Rectangle sourceRect = sourceRectangle ?? new Rectangle(0, 0, tex.Width, tex.Height);
		float x = sourceRect.X / (float)tex.Width;
		float y = sourceRect.Y / (float)tex.Height;
		float width = sourceRect.Width / (float)tex.Width;
		float height = sourceRect.Height / (float)tex.Height;

		Buffer<VFX2D>.AddVertex(new VFX2D[]
		{
			new VFX2D(Vector2.Transform(position, matrix), color, new Vector2(x, y)),
			new VFX2D(Vector2.Transform(position + new Vector2(sourceRect.Width, 0), matrix), color, new Vector2(x + width, y)),
			new VFX2D(Vector2.Transform(position + new Vector2(0, sourceRect.Height), matrix), color, new Vector2(x, y + height)),
			new VFX2D(Vector2.Transform(position + new Vector2(sourceRect.Width, sourceRect.Height), matrix), color, new Vector2(x + width, y + height))
		}, PrimitiveType.TriangleStrip);
	}

	public void Draw(Rectangle destinationRectangle, Color color)
	{
		Debug.Assert(hasBegun, "Begin not called!");
		if (!Buffer<VFX2D>.CheckSize(4))
		{
			var tex = Buffer<VFX2D>.CurrentTexture;
			Flush<VFX2D>();
			Buffer<VFX2D>.Textures.Add(tex);
		}
		needFlush[0] = true;
		Buffer<VFX2D>.AddVertex(new VFX2D[]
		{
			new VFX2D(new Vector2(destinationRectangle.X, destinationRectangle.Y), color, Vector2.Zero),
			new VFX2D(new Vector2(destinationRectangle.X + destinationRectangle.Width, destinationRectangle.Y), color, Vector2.UnitX),
			new VFX2D(new Vector2(destinationRectangle.X, destinationRectangle.Y + destinationRectangle.Height), color, Vector2.UnitY),
			new VFX2D(new Vector2(destinationRectangle.X + destinationRectangle.Width, destinationRectangle.Y + destinationRectangle.Height), color, Vector2.One)
		}, PrimitiveType.TriangleStrip);
	}

	public void Draw(Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
	{
		Debug.Assert(hasBegun, "Begin not called!");
		var tex = Buffer<VFX2D>.CurrentTexture;
		if (!Buffer<VFX2D>.CheckSize(4))
		{
			Flush<VFX2D>();
			Buffer<VFX2D>.Textures.Add(tex);
		}
		needFlush[0] = true;
		Rectangle sourceRect = sourceRectangle ?? new Rectangle(0, 0, tex.Width, tex.Height);
		float x = sourceRect.X / (float)tex.Width;
		float y = sourceRect.Y / (float)tex.Height;
		float width = sourceRect.Width / (float)tex.Width;
		float height = sourceRect.Height / (float)tex.Height;
		Buffer<VFX2D>.AddVertex(new VFX2D[]
		{
			new VFX2D(new Vector2(destinationRectangle.X, destinationRectangle.Y), color, new Vector2(x, y)),
			new VFX2D(new Vector2(destinationRectangle.X + destinationRectangle.Width, destinationRectangle.Y), color, new Vector2(x + width, y)),
			new VFX2D(new Vector2(destinationRectangle.X, destinationRectangle.Y + destinationRectangle.Height), color, new Vector2(x, y + height)),
			new VFX2D(new Vector2(destinationRectangle.X + destinationRectangle.Width, destinationRectangle.Y + destinationRectangle.Height), color, new Vector2(x + width, y + height))
		}, PrimitiveType.TriangleStrip);
	}

	public void Draw(Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects)
	{
		Debug.Assert(hasBegun, "Begin not called!");
		var tex = Buffer<VFX2D>.CurrentTexture;
		if (!Buffer<VFX2D>.CheckSize(4))
		{
			Flush<VFX2D>();
			Buffer<VFX2D>.Textures.Add(tex);
		}
		needFlush[0] = true;
		Rectangle sourceRect = sourceRectangle ?? new Rectangle(0, 0, tex.Width, tex.Height);
		float x = sourceRect.X / (float)tex.Width;
		float y = sourceRect.Y / (float)tex.Height;
		float width = sourceRect.Width / (float)tex.Width;
		float height = sourceRect.Height / (float)tex.Height;

		Vector2 position = new(destinationRectangle.X, destinationRectangle.Y);
		Vector2 topLeftPosition = position - origin;
		Matrix matrix = Matrix.CreateTranslation(-position.X, -position.Y, 0)
			* Matrix.CreateRotationZ(rotation)
			* Matrix.CreateTranslation(position.X, position.Y, 0);

		Vector2 topLeft = new(x, y);
		Vector2 topRight = new(x + width, y);
		Vector2 bottomLeft = new(x, y + height);
		Vector2 bottomRight = new(x + width, y + height);
		if (effects.HasFlag(SpriteEffects.FlipHorizontally))
		{
			(topLeft, topRight) = (topRight, topLeft);
			(bottomLeft, bottomRight) = (bottomRight, bottomLeft);
		}
		if (effects.HasFlag(SpriteEffects.FlipVertically))
		{
			(topLeft, bottomLeft) = (bottomLeft, topLeft);
			(topRight, bottomRight) = (bottomRight, topRight);
		}
		Buffer<VFX2D>.AddVertex(new VFX2D[]
		{
			new VFX2D(Vector2.Transform(topLeftPosition, matrix), color, topLeft),
			new VFX2D(Vector2.Transform(topLeftPosition + new Vector2(sourceRect.Width, 0), matrix), color, topRight),
			new VFX2D(Vector2.Transform(topLeftPosition + new Vector2(0, sourceRect.Height), matrix), color, bottomLeft),
			new VFX2D(Vector2.Transform(topLeftPosition + new Vector2(sourceRect.Width, sourceRect.Height), matrix), color, bottomRight)
		}, PrimitiveType.TriangleStrip);
	}

	public void Draw<T>(IEnumerable<T> vertices, PrimitiveType type) where T : struct, IVertexType
	{
		if (!vertices.Any())
		{
			return;
		}

		Debug.Assert(hasBegun, "Begin not called!");
		if (!Buffer<T>.CheckSize(vertices.Count()))
		{
			if (Buffer<T>.Textures.Count == 0)
			{
				Flush<T>();
			}
			else
			{
				var tex = Buffer<T>.CurrentTexture;
				Flush<T>();
				Buffer<T>.Textures.Add(tex);
			}
		}
		needFlush[GetBufferIndex<T>()] = true;
		Buffer<T>.AddVertex(vertices, type);
	}

	#endregion Draw Method

	#region State Change

	public void Begin()
	{
		Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointClamp, RasterizerState.CullNone);
	}

	public void Begin(BlendState blendState)
	{
		Begin(blendState, DepthStencilState.None, SamplerState.PointClamp, RasterizerState.CullNone);
	}

	public void Begin(BlendState blendState, DepthStencilState depthStencilState, SamplerState samplerState, RasterizerState rasterizerState)
	{
		Debug.Assert(!hasBegun);
		_graphicsDevice.RasterizerState = rasterizerState;
		_graphicsDevice.DepthStencilState = depthStencilState;
		_graphicsDevice.SamplerStates[0] = samplerState;
		_graphicsDevice.BlendState = blendState;
		hasBegun = true;
	}

	/// <summary>
	/// 绑定Texture2D用于接下来的Draw
	/// </summary>
	/// <param name="texture"></param>
	/// <returns>this</returns>
	public VFXBatch BindTexture(Texture2D texture)
	{
		return BindTexture<VFX2D>(texture);
	}

	/// <summary>
	/// 绑定Texture2D用于该顶点类型下一次的Draw
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="texture"></param>
	/// <returns>this</returns>
	public VFXBatch BindTexture<T>(Texture2D texture) where T : struct, IVertexType
	{
		if (Buffer<T>.Textures.Count == 0)
		{
			Buffer<T>.Textures.Add(texture);
			return this;
		}

		if (Buffer<T>.CurrentTexture == texture)
		{
			return this;
		}

		Buffer<T>.Textures.Add(texture);
		Buffer<T>.SameTexture.Enqueue((Buffer<T>.IndexPosition, Buffer<T>.VertexPosition));
		return this;
	}

	public void Dispose()
	{
		_mainThread.AddTask(() =>
		{
			foreach (var buffer in buffers)
			{
				buffer.Dispose();
			}
		});
		GC.SuppressFinalize(this);
	}

	public void End()
	{
		Flush();
		hasBegun = false;
	}

	public void Flush()
	{
		for (int i = 0; i < buffers.Count; i++)
		{
			if (needFlush[i])
			{
				buffers[i].DrawPrimitive();
				buffers[i].Clear();
				needFlush[i] = false;
			}
		}
	}

	public void Flush<T>() where T : struct, IVertexType
	{
		Buffer<T>.Instance.DrawPrimitive();
		Buffer<T>.Instance.Clear();
	}

	#endregion State Change

	#region Vertex

	public void RegisterVertex<T>(int maxVertices = MAX_VERTICES, int maxIndices = MAX_INDICES) where T : struct, IVertexType
	{
		buffers.Add(Buffer<T>.Create(GraphicsDevice, maxVertices, maxIndices));
		needFlush.Add(false);
	}

	private int GetBufferIndex<T>() where T : struct, IVertexType
	{
		return buffers.IndexOf(Buffer<T>.Instance);
	}

	private struct VFX2D : IVertexType
	{


		private static VertexDeclaration _vertexDeclaration = new(new VertexElement[3]
	 {
		new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
		new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
		new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0)
	 });
		public Vector2 position;
		public Color color;
		public Vector3 texCoord;

		public VFX2D(Vector2 position, Color color, Vector2 texCoord)
		{
			this.position = position;
			this.color = color;
			this.texCoord = new(texCoord, 0);
		}

		public override string ToString()
		{
			return $"[{position}, {color}, {texCoord}]";
		}

		public VertexDeclaration VertexDeclaration => _vertexDeclaration;
	}

	#endregion Vertex

	#region Buffer

	private interface IBuffers : IDisposable
	{
		Type VertexType
		{
			get;
		}

		void Clear();

		void DrawPrimitive();
	}

	private static class Buffer<T> where T : struct, IVertexType
	{
		private static Buffers instance;

		public static Texture2D CurrentTexture
		{
			get
			{
				Debug.Assert(instance.textures.Count != 0);
				return instance.textures[^1];
			}
		}

		public static DynamicIndexBuffer IndexBuffer => instance.indexBuffer;

		public static ref int IndexPosition => ref instance.indexPosition;

		public static int[] Indices => instance.indices;

		public static IBuffers Instance => instance;

		public static Queue<(int index, int vertex)> SameTexture => instance.sameTexture;

		public static List<Texture2D> Textures => instance.textures;

		public static DynamicVertexBuffer VertexBuffer => instance.vertexBuffer;

		public static ref int VertexPosition => ref instance.vertexPosition;

		public static T[] Vertices => instance.vertices;

		public static void AddVertex(IEnumerable<T> vertices, PrimitiveType type)
		{
			int pos = instance.vertexPosition;
			int count = 0;
			foreach (var vertex in vertices)
			{
				++count;
				instance.vertices[pos++] = vertex;
			}

			switch (type)
			{
				case PrimitiveType.TriangleList:
					for (int i = 0; i < count; i++)
					{
						instance.indices[instance.indexPosition++] = i + instance.vertexPosition;
					}
					break;

				case PrimitiveType.TriangleStrip:
					for (int i = 0; i < count - 2; i++)
					{
						instance.indices[instance.indexPosition++] = i + instance.vertexPosition;
						instance.indices[instance.indexPosition++] = i + 1 + instance.vertexPosition;
						instance.indices[instance.indexPosition++] = i + 2 + instance.vertexPosition;
					}
					break;

				default:
					throw new Exception("Unsupported PrimitiveType");
			}
			instance.vertexPosition = pos;
		}

		public static bool CheckSize(int vertexSize)
		{
			return instance.vertexPosition + vertexSize < instance.vertices.Length;
		}

		public static IBuffers Create(GraphicsDevice graphicsDevice, int maxVertices, int maxIndices)
		{
			Debug.Assert(instance == null, "Can't Create Twice");
			instance = new Buffers()
			{
				vertices = new T[maxVertices],
				indices = new int[maxIndices],
				vertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(T), maxVertices, BufferUsage.WriteOnly),
				indexBuffer = new DynamicIndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, maxIndices, BufferUsage.WriteOnly),
				textures = new List<Texture2D>(),
				sameTexture = new Queue<(int index, int vertex)>(),
				graphicsDevice = graphicsDevice
			};
			return instance;
		}

		private class Buffers : IBuffers
		{
			public GraphicsDevice graphicsDevice;
			public DynamicIndexBuffer indexBuffer;
			public int indexPosition;
			public int[] indices;
			public Queue<(int index, int vertex)> sameTexture;
			public List<Texture2D> textures;
			public DynamicVertexBuffer vertexBuffer;
			public int vertexPosition;
			public T[] vertices;
			public Type VertexType => typeof(T);

			public void Clear()
			{
				vertexPosition = indexPosition = 0;
				sameTexture.Clear();
				textures.Clear();
			}

			public void Dispose()
			{
				vertexBuffer.Dispose();
				indexBuffer.Dispose();
				GC.SuppressFinalize(this);
			}

			public void DrawPrimitive()
			{
				Debug.Assert(vertexPosition != 0 && indexPosition != 0, "Should not draw when no vertex");
				vertexBuffer.SetData(vertices, 0, vertexPosition, SetDataOptions.None);
				graphicsDevice.SetVertexBuffer(vertexBuffer);
				indexBuffer.SetData(indices, 0, indexPosition, SetDataOptions.None);
				graphicsDevice.Indices = indexBuffer;

				if (sameTexture.Count == 0)
				{
					if (textures.Count != 0)
					{
						graphicsDevice.Textures[0] = textures[0];
					}

					graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexPosition, 0, indexPosition / 3);
					return;
				}

				int currentIndex = 0, currentVertex = 0;
				int count = 0;
				sameTexture.Enqueue((indexPosition, vertexPosition));
				while (sameTexture.Count != 0)
				{
					var (nextIndex, nextVertex) = sameTexture.Dequeue();
					if (currentVertex == nextVertex || currentIndex == nextIndex)
					{
						continue;
					}

					graphicsDevice.Textures[0] = textures[count++];
					graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, nextVertex, currentIndex, (nextIndex - currentIndex) / 3);
					(currentIndex, currentVertex) = (nextIndex, nextVertex);
				}
			}
		}
	}

	#endregion Buffer
}
