namespace Everglow.Commons.Coroutines;

/// <summary>
/// 指示协程机等待一个新的任务完成以后再继续执行（相当于同步函数调用）
/// </summary>
public class AwaitForTask : ICoroutineInstruction
{
	public IEnumerator<ICoroutineInstruction> Task
	{
		get
		{
			return task;
		}
	}

	private IEnumerator<ICoroutineInstruction> task;

	public AwaitForTask(IEnumerator<ICoroutineInstruction> task)
	{
		this.task = task;
	}

	public bool ShouldWait()
	{
		return false;
	}

	public void Update()
	{
	}
}
