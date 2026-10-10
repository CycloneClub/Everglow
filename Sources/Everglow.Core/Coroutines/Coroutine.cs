namespace Everglow.Commons.Coroutines;

/// <summary>
/// 表示一个标准的协程运行器
/// </summary>
public class Coroutine : ICoroutine
{
	private List<IEnumerator<ICoroutineInstruction>> enumerator;
	private ICoroutineInstruction lastInstruction;

	public Coroutine(IEnumerator<ICoroutineInstruction> enumerator)
	{
		this.enumerator = new List<IEnumerator<ICoroutineInstruction>> { enumerator };
		lastInstruction = null;
	}

	public bool MoveNext()
	{
		if (enumerator.Count == 0)
		{
			return false;
		}

		bool canRunNext = lastInstruction == null || !lastInstruction.ShouldWait();

		if (!canRunNext)
		{
			lastInstruction.Update();
			return true;
		}

		if (canRunNext)
		{
			var currentIE = enumerator[^1];
			if (!currentIE.MoveNext())
			{
				enumerator.RemoveAt(enumerator.Count - 1);
				if (enumerator.Count == 0)
				{
					return false;
				}
			}

			var instruction = currentIE.Current;
			if (instruction is AwaitForTask)
			{
				enumerator.Add((instruction as AwaitForTask).Task);
				lastInstruction = null;
				return true;
			}

			lastInstruction = instruction;
		}
		return true;
	}
}
