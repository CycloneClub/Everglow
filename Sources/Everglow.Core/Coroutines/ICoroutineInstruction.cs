namespace Everglow.Commons.Coroutines;

/// <summary>
/// 协程的控制指令
/// </summary>
public interface ICoroutineInstruction
{
	void Update();

	bool ShouldWait();
}
