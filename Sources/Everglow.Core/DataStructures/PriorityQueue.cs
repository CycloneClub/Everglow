namespace Everglow.Commons.DataStructures;

/// <summary>
/// 优先队列数据结构，使用小根堆实现。Pop，Push复杂度保证O(log n)
/// </summary>
/// <typeparam name="T"></typeparam>
public class PriorityQueue<T>
	where T : IComparable<T>, new()
{
	private readonly List<T> heap;
	private int top;

	public PriorityQueue()
	{
		top = 0;
		heap = new List<T>
		{
			new T(),
		};
	}

	/// <summary>
	/// 判断堆内是否有元素
	/// </summary>
	public bool Empty => top == 0;

	/// <summary>
	/// 获取堆顶值
	/// </summary>
	public T Top
	{
		get
		{
			if (top < 1)
			{
				throw new IndexOutOfRangeException();
			}

			return heap[1];
		}
	}

	/// <summary>
	/// 将元素放入小根堆
	/// </summary>
	/// <param name="val"></param>
	public void Push(T val)
	{
		heap.Add(val);
		++top;
		Swim();
	}

	/// <summary>
	/// 获取并弹出堆顶上的最小值
	/// </summary>
	/// <returns></returns>
	public T Pop()
	{
		T ret = Top;
		Swap(1, top--);
		Sink();
		return ret;
	}

	private void Swap(int i, int j)
	{
		(heap[j], heap[i]) = (heap[i], heap[j]);
	}

	private void Swim()
	{
		int k = top;
		while (k > 1 && heap[k >> 1].CompareTo(heap[k]) > 0)
		{
			Swap(k >> 1, k);
			k >>= 1;
		}
	}

	private void Sink()
	{
		int k = 1;
		while (k << 1 <= top)
		{
			int j = k << 1;
			if (j + 1 <= top && heap[j].CompareTo(heap[j + 1]) > 0)
			{
				j++;
			}

			if (heap[k].CompareTo(heap[j]) <= 0)
			{
				break;
			}

			Swap(k, j);
			k = j;
		}
	}
}
