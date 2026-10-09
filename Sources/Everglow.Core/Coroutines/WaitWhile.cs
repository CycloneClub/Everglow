namespace Everglow.Commons.Coroutines;

/// <summary>
/// 协程将会等到条件不满足才继续执行
/// </summary>
public class WaitWhile : ICoroutineInstruction
{
	private Func<bool> m_predicate;

	public WaitWhile(Func<bool> predicate)
	{
		m_predicate = predicate;
	}

	public bool ShouldWait()
	{
		return m_predicate();
	}

	public void Update()
	{
	}
}
