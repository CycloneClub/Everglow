namespace Everglow.Commons.Coroutines;

/// <summary>
/// 协程将会等到条件不满足才继续执行
/// </summary>
public class WaitWhile : ICoroutineInstruction
{
	private Func<bool> predicate;

	public WaitWhile(Func<bool> predicate)
	{
		this.predicate = predicate;
	}

	public bool ShouldWait()
	{
		return predicate();
	}

	public void Update()
	{
	}
}
