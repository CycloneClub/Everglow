namespace Everglow.Commons.DataStructures;

/// <summary>
/// 优先队列数据结构，使用小根堆实现。Pop，Push复杂度保证O(log n)
/// </summary>
/// <typeparam name="T"></typeparam>
public class PriorityQueue<T> where T : IComparable<T>, new()
{
	private readonly List<T> m_heap;
	private int m_top;
	public PriorityQueue()
	{
		m_top = 0;
		m_heap = new List<T>
		{
			new T(),
		};
	}

	/// <summary>
	/// 判断堆内是否有元素
	/// </summary>
	public bool Empty => m_top == 0;

	/// <summary>
	/// 获取堆顶值
	/// </summary>
	public T Top
	{
		get
		{
			if (m_top < 1)
				throw new IndexOutOfRangeException();
			return m_heap[1];
		}
	}

	/// <summary>
	/// 将元素放入小根堆
	/// </summary>
	/// <param name="val"></param>
	public void Push(T val)
	{
		m_heap.Add(val);
		++m_top;
		Swim();
	}

	/// <summary>
	/// 获取并弹出堆顶上的最小值
	/// </summary>
	/// <returns></returns>
	public T Pop()
	{
		T ret = Top;
		Swap(1, m_top--);
		Sink();
		return ret;
	}
	private void Swap(int i, int j)
	{
		(m_heap[j], m_heap[i]) = (m_heap[i], m_heap[j]);
	}
	private void Swim()
	{
		int k = m_top;
		while (k > 1 && m_heap[k >> 1].CompareTo(m_heap[k]) > 0)
		{
			Swap(k >> 1, k);
			k >>= 1;
		}
	}
	private void Sink()
	{
		int k = 1;
		while (k << 1 <= m_top)
		{
			int j = k << 1;
			if (j + 1 <= m_top && m_heap[j].CompareTo(m_heap[j + 1]) > 0)
				j++;
			if (m_heap[k].CompareTo(m_heap[j]) <= 0)
				break;
			Swap(k, j);
			k = j;
		}
	}
}
