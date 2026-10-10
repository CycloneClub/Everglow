namespace Everglow.Commons.Modules;

public class DependencyGraph
{
	// 类型到Id的映射
	private Dictionary<Type, int> typeToIdMapping;
	// 类型列表，用于确定Id
	private List<Type> types;
	// 依赖图的邻接表
	private Dictionary<int, List<int>> dependencyGraph;
	// 依赖图中节点的入度表
	private Dictionary<int, int> dependencyFanin;

	public DependencyGraph()
	{
		typeToIdMapping = new Dictionary<Type, int>();
		types = new List<Type>();
		dependencyGraph = new Dictionary<int, List<int>>();
		dependencyFanin = new Dictionary<int, int>();
	}

	/// <summary>
	/// 添加一个没有依赖的<paramref name="type"/>
	/// </summary>
	/// <param name="type"></param>
	public void AddType(Type type)
	{
		int _ = GetInternalID(type);
	}

	/// <summary>
	/// 添加一个依赖链，表示<paramref name="depend"/>必须先于<paramref name="type"/>被加载
	/// </summary>
	/// <param name="depend"></param>
	/// <param name="type"></param>
	public void AddDependency(Type depend, Type type)
	{
		int u = GetInternalID(depend);
		int v = GetInternalID(type);

		if (dependencyGraph.ContainsKey(u))
		{
			dependencyGraph[u].Add(v);
		}
		else
		{
			dependencyGraph[u] = new List<int> { v };
		}

		AddFanin(v);
	}

	private void AddFanin(int v)
	{
		if (dependencyFanin.ContainsKey(v))
		{
			dependencyFanin[v]++;
		}
		else
		{
			dependencyFanin[v] = 1;
		}
	}

	/// <summary>
	/// 对依赖图进行拓扑排序，如果该图是个DAG，则会返回类型的正确顺序
	/// </summary>
	/// <returns></returns>
	/// <exception cref="ArgumentException"></exception>
	public List<Type> TopologicalSort()
	{
		List<Type> result = new();
		Queue<int> queue = new();

		for (int i = 0; i < types.Count; i++)
		{
			if (GetFanin(i) == 0)
			{
				queue.Enqueue(i);
			}
		}

		while (queue.Count > 0)
		{
			int u = queue.Dequeue();

			if (!dependencyGraph.ContainsKey(u))
			{
				result.Add(types[u]);
				continue;
			}
			result.Add(types[u]);
			foreach (var v in dependencyGraph[u])
			{
				dependencyFanin[v]--;
				if (dependencyFanin[v] == 0)
				{
					queue.Enqueue(v);
				}
			}
		}

		// 如果依赖图出现环就直接报错加载失败
		if (result.Count < types.Count)
		{
			throw new ArgumentException("Circular dependency detected, please remove the circle");
		}

		return result;
	}

	/// <summary>
	/// 获取某个类型的内部ID，如果该类型没有出现过就新分配一个ID
	/// </summary>
	/// <param name="t"></param>
	/// <returns></returns>
	private int GetInternalID(Type t)
	{
		if (!typeToIdMapping.ContainsKey(t))
		{
			int id = types.Count;
			typeToIdMapping.Add(t, id); // 是漏了吗？
			types.Add(t);
			return id;
		}
		return typeToIdMapping[t];
	}

	private int GetFanin(int v)
	{
		if (!dependencyFanin.ContainsKey(v))
		{
			return 0;
		}
		else
		{
			return dependencyFanin[v];
		}
	}
}
