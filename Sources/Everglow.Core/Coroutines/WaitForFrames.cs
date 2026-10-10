namespace Everglow.Commons.Coroutines;

/// <summary>
/// 协程的剩余内容将在经过指定帧数以后继续执行
/// </summary>
public class WaitForFrames : ICoroutineInstruction
{
	private uint counter;
	private readonly uint totalFrames;

	public WaitForFrames(uint frames)
	{
		totalFrames = frames;
		counter = 0;
	}

	public bool ShouldWait()
	{
		return counter <= totalFrames;
	}

	public void Update()
	{
		++counter;
	}
}
