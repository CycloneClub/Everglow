namespace Everglow.Commons.Coroutines;

/// <summary>
/// 指示协程机剩余的内容将在下一帧继续执行
/// </summary>
public class SkipThisFrame : ICoroutineInstruction
{
	public SkipThisFrame()
	{
	}

	public bool ShouldWait()
	{
		return false;
	}

	public void Update()
	{
	}
}
