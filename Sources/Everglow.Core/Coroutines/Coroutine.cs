namespace Everglow.Commons.Coroutines;

/// <summary>
/// 表示一个标准的协程运行器
/// </summary>
public class Coroutine : ICoroutine
{
	private List<IEnumerator<ICoroutineInstruction>> m_enumerator;
	private ICoroutineInstruction m_lastInstruction;
	public Coroutine(IEnumerator<ICoroutineInstruction> enumerator)
	{
		m_enumerator = new List<IEnumerator<ICoroutineInstruction>> { enumerator };
		m_lastInstruction = null;
	}

	public bool MoveNext()
	{
		if (m_enumerator.Count == 0)
		{
			return false;
		}

		bool canRunNext = m_lastInstruction == null || !m_lastInstruction.ShouldWait();

		if (!canRunNext)
		{
			m_lastInstruction.Update();
			return true;
		}

		if (canRunNext)
		{
			var currentIE = m_enumerator[^1];
			if (!currentIE.MoveNext())
			{
				m_enumerator.RemoveAt(m_enumerator.Count - 1);
				if (m_enumerator.Count == 0)
				{
					return false;
				}
			}

			var instruction = currentIE.Current;
			if (instruction is AwaitForTask)
			{
				m_enumerator.Add((instruction as AwaitForTask).Task);
				m_lastInstruction = null;
				return true;
			}

			m_lastInstruction = instruction;
		}
		return true;
	}
}
