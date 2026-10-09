using Everglow.Commons.Interfaces;
using ReLogic.Content;

namespace Everglow.Commons.VFX;

public abstract class Pipeline : IPipeline
{
	public Asset<Effect> effect;

	/// <summary>
	/// 准备开始渲染
	/// </summary>
	public abstract void BeginRender();

	/// <summary>
	/// 结束渲染，刷新VFXBatch
	/// </summary>
	public abstract void EndRender();

	public virtual void Load()
	{
	}

	/// <summary>
	/// 渲染和绘制,经过调整后可以重写
	/// </summary>
	/// <param name="visuals"></param>
	public virtual void Render(IEnumerable<IVisual> visuals)
	{
		BeginRender();
		foreach (var visual in visuals)
		{
			visual.Draw();
		}
		EndRender();
	}

	public virtual void Unload()
	{
	}
}
