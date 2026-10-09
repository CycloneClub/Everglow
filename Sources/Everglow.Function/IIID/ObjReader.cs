namespace Everglow.Commons.IIID
{
	public class ObjReader
	{
		public struct Face
		{
			public int TextureType = default(int);
			public List<int> Positions = new List<int>();
			public List<int> TextureCoords = new List<int>();
			public List<int> Normals = new List<int>();

			public Face()
			{
			}
		}

		public class Model
		{
			public List<Vector3> positions = new List<Vector3>(); // V:代表顶点。格式为V X Y Z&#xff0c;V后面的X Y Z表示三个顶点坐标。浮点型
			public List<Vector2> texCoords = new List<Vector2>(); // 表示纹理坐标。格式为VT TU TV。浮点型
			public List<Vector3> normals = new List<Vector3>(); // VN:法向量。每个三角形的三个顶点都要指定一个法向量。格式为VN NX NY NZ。浮点型
			public List<Face> faces = new List<Face>(); // F:面。面后面跟着的整型值分别是属于这个面的顶点、纹理坐标、法向量的索引。

			// 面的格式为:f Vertex1/Texture1/Normal1 Vertex2/Texture2/Normal2 Vertex3/Texture3/Normal3
		}

		public static Model LoadFile(string fileName)
		{
			Model mesh = new Model();
			var bytes = ModContent.GetFileBytes(fileName);
			using (StreamReader objReader = new StreamReader(new MemoryStream(bytes)))
			{
				mesh.positions.Add(Vector3.Zero);
				mesh.texCoords.Add(Vector2.Zero);
				mesh.normals.Add(Vector3.Zero);
				int s = 0;
				int Texturetype = 0;
				bool newModel = false;
				while (objReader.Peek() != -1)
				{
					s++;
					string text = objReader.ReadLine();
					if (text.Length < 2)
					{
						continue;
					}
					string[] tempArray;
					if (text.IndexOf("usemtl") == 0)
					{
						Texturetype++;
						continue;
					}
					else if (text.IndexOf("v") == 0)
					{
						if (!newModel)
						{
							newModel = true;
						}
						if (text.IndexOf("t") == 1)// vt 0.581151 0.979929 纹理
						{
							tempArray = text.Split(' ');
							mesh.texCoords.Add(new Vector2(float.Parse(tempArray[1]), float.Parse(tempArray[2])));
						}
						else if (text.IndexOf("n") == 1)// vn 0.637005 -0.0421857 0.769705 法向量
						{
							tempArray = text.Split(new char[] { '/', ' ' }, StringSplitOptions.RemoveEmptyEntries);
							if (tempArray[3] != "\\")
							{
								mesh.normals.Add(new Vector3(float.Parse(tempArray[1]), float.Parse(tempArray[2]), float.Parse(tempArray[3])));
							}
							else
							{
								text = objReader.ReadLine();
								mesh.normals.Add(new Vector3(float.Parse(tempArray[1]), float.Parse(tempArray[2]), float.Parse(text)));
							}
						}
						else
						{// v -53.0413 158.84 -135.806 点
							tempArray = text.Split(' ');
							mesh.positions.Add(new Vector3(float.Parse(tempArray[1]), float.Parse(tempArray[2]), float.Parse(tempArray[3])));
						}
					}
					else if (text.IndexOf("f") == 0)
					{
						newModel = false;

						// f 2443//2656 2442//2656 2444//2656 面
						var componentArray = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

						// componentArray[0] 是 f
						// componentArray[1 ~ n] 是组成顶点的信息列表
						Face face = new Face();

						// 只允许读取三角网格，多边形网格请先三角化
						Debug.Assert(componentArray.Length == 4, "We only support triangle meshes");
						for (int i = 1; i < componentArray.Length; i++)
						{
							var components = componentArray[i].Split('/');

							if (int.TryParse(components[0], out int idPos))
							{
								face.Positions.Add(idPos);
							}

							if (int.TryParse(components[1], out int idTex))
							{
								face.TextureCoords.Add(idTex);
							}

							if (int.TryParse(components[2], out int idNormal))
							{
								face.Normals.Add(idNormal);
							}
						}
						face.TextureType = Texturetype;

						// Debug.Assert(face.Positions.Count == 3 && face.Normals.Count == 3, "We only support triangle meshes");

						// int k = 1;
						// for (int i = 0; i < 3; i++)
						// {
						//    if (hasPos)
						//    {
						//        face.V[i] = int.Parse(tempArray[k]) - 1;
						//        k++;
						//    }
						//    if (hasTex)
						//    {
						//        face.T[i] = int.Parse(tempArray[k]) - 1;
						//        k++;
						//    }
						//    if (hasNormal)
						//    {
						//        face.N[i] = int.Parse(tempArray[k]) - 1;
						//        k++;
						//    }
						// }
						mesh.faces.Add(face);
					}
				}
			}
			return mesh;
		}
	}
}
