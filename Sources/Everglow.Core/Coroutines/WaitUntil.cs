namespace Everglow.Commons.Coroutines;

/// <summary>
/// 协程的剩余内容将在条件满足以后继续执行
/// </summary>
public class WaitUntil : ICoroutineInstruction
{
	private Func<bool> m_predicate;

	public WaitUntil(Func<bool> predicate)
	{
		m_predicate = predicate;
	}

	public bool ShouldWait()
	{
		return !m_predicate();
	}

	public void Update()
	{
	}
}
