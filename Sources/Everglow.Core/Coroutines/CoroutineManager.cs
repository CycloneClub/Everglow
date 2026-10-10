namespace Everglow.Commons.Coroutines;

/// <summary>
/// 管理所有协程运行器的类，支持异步地发起新的协程
/// </summary>
public class CoroutineManager
{
	private LinkedList<ICoroutine> coroutines;

	public CoroutineManager()
	{
		coroutines = new LinkedList<ICoroutine>();
	}

	public void StartCoroutine(ICoroutine coroutine)
	{
		coroutines.AddLast(coroutine);
	}

	public void Update()
	{
		var node = coroutines.First;

		while (node != null)
		{
			var nextNode = node.Next;

			var current = node.Value;
			bool finished = false;
			if (current != null)
			{
				finished = !current.MoveNext();
			}

			if (finished)
			{
				coroutines.Remove(node);
			}

			node = nextNode;
		}
	}
}
