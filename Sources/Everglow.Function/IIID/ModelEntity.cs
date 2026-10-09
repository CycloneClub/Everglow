using Everglow.Commons.Vertex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everglow.Commons.IIID
{
	public struct VertexRP : IVertexType
	{
		private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[4]
		{
			new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
			new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0),
			new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
			new VertexElement(36, VertexElementFormat.Vector3, VertexElementUsage.Normal, 1),
		});
		public Vector3 position;
		public Vector3 texcoord;
		public Vector3 normal;
		public Vector3 tangent;

		public VertexRP(Vector3 position, Vector3 texcoord, Vector3 normal, Vector3 tangent)
		{
			this.position = position;
			this.texcoord = texcoord;
			this.normal = normal;
			this.tangent = tangent;
		}

		public VertexDeclaration VertexDeclaration
		{
			get
			{
				return _vertexDeclaration;
			}
		}
	}
	public class ModelEntity
	{
		/// <summary>
		/// 模型的主要顶点列表
		/// </summary>
		public List<VertexRP> Vertices
		{
			get; set;
		}

		/// <summary>
		/// 模型的主要贴图
		/// </summary>
		public Texture2D Texture
		{
			get; set;
		}

		/// <summary>
		/// 模型的法线贴图
		/// </summary>
		public Texture2D NormalTexture
		{
			get; set;
		}

		/// <summary>
		/// 模型的材质参数贴图（粗糙度，金属度等
		/// </summary>
		public Texture2D MaterialTexture
		{
			get; set;
		}

		/// <summary>
		/// 模型的发光贴图参数（HDR，或者用 （r，g，b）* a * 256的方式
		/// </summary>
		public Texture2D EmissionTexture
		{
			get; set;
		}

		public Matrix ModelTransform
		{
			get; set;
		}

		public static Vector3 SafeNormalize(Vector3 v, Vector3 defaultValue)
		{
			if (v == Vector3.Zero)
			{
				return defaultValue;
			}

			return Vector3.Normalize(v);
		}

		public static Vector3 CalculateTangentVector(Vector3 A, Vector3 B, Vector3 C, Vector2 uv1, Vector2 uv2, Vector2 uv3)
		{
			Vector3 E1 = C - A;
			Vector3 E2 = B - A;
			Vector2 DeltaUV1 = uv2 - uv1;
			Vector2 DeltaUV2 = uv3 - uv1;
			float det = DeltaUV1.X * DeltaUV2.Y - DeltaUV2.X * DeltaUV1.Y;
			if (MathF.Abs(det) < 1e-5)
			{
				return SafeNormalize(E1, new Vector3(1, 0, 0));
			}
			return SafeNormalize((DeltaUV1.Y * E1 - DeltaUV2.Y * E2) / det, new Vector3(1, 0, 0));
		}
	}
}
