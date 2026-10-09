namespace Everglow.Commons.Coroutines;

/// <summary>
/// 协程的剩余内容将在经过指定帧数以后继续执行
/// </summary>
public class WaitForFrames : ICoroutineInstruction
{
	private uint m_counter;
	private readonly uint m_totalFrames;

	public WaitForFrames(uint frames)
	{
		m_totalFrames = frames;
		m_counter = 0;
	}

	public bool ShouldWait()
	{
		return m_counter <= m_totalFrames;
	}

	public void Update()
	{
		++m_counter;
	}
}
