using Everglow.Commons.Enums;
using Everglow.Commons.Interfaces;

namespace Everglow.Commons.ObjectPool;

/// <summary>
/// 管理屏幕大小的 RenderTarget 的对象池
/// </summary>
public class RenderTargetPool
{
	private List<RenderTarget2D> m_renderTargetsPool;
	private LinkedList<int> m_renderTargetsFreeList;
	private GraphicsDevice m_graphicsDevice;

	public RenderTargetPool(IHookManager hookManager)
	{
		m_renderTargetsPool = new List<RenderTarget2D>();
		m_renderTargetsFreeList = new LinkedList<int>();
		m_graphicsDevice = Ins.Device;

		hookManager.AddHook(CodeLayer.ResolutionChanged, Main_OnResolutionChanged);
	}

	/// <summary>
	/// 屏幕大小变化时刷新 RT 的尺寸
	/// </summary>
	/// <param name="size"></param>
	private void Main_OnResolutionChanged(Vector2 size)
	{
		int poolSize = m_renderTargetsPool.Count;
		for (int i = 0; i < poolSize; i++)
		{
			if (m_renderTargetsPool[i] != null)
			{
				m_renderTargetsPool[i].Dispose();
				m_renderTargetsPool[i] = null;
			}
			m_renderTargetsPool[i] = new RenderTarget2D(
				m_graphicsDevice,
				(int)size.X,
				(int)size.Y);
		}
	}

	/// <summary>
	/// 从对象池中获取1个 RT，并且返回句柄
	/// </summary>
	/// <returns></returns>
	public ResourceLocker<RenderTarget2D> GetRenderTarget2D()
	{
		int index = GetNextFreeIndexAndOccupy();
		return new ResourceLocker<RenderTarget2D>(m_renderTargetsPool[index], () =>
	   {
		   ReleaseResourceAt(index);
	   });
	}

	/// <summary>
	/// 从对象池中获取 size 那么多个 RT 对象，并且返回一个句柄
	/// </summary>
	/// <param name="size"></param>
	/// <returns></returns>
	public ResourceLocker<RenderTarget2D[]> GetRenderTarget2DArray(int size)
	{
		List<int> indices = new();
		List<RenderTarget2D> renderTargets = new();
		for (int i = 0; i < size; i++)
		{
			int index = GetNextFreeIndexAndOccupy();
			indices.Add(index);
			renderTargets.Add(m_renderTargetsPool[index]);
		}

		return new ResourceLocker<RenderTarget2D[]>(renderTargets.ToArray(), () =>
		{
			foreach (var i in indices)
			{
				ReleaseResourceAt(i);
			}
		});
	}

	/// <summary>
	/// 获取下一个空位的ID，并且占有它
	/// </summary>
	/// <returns></returns>
	private int GetNextFreeIndexAndOccupy()
	{
		lock (this)
		{
			// 如果 freelist 没有空位就扩充我们的 rendertarget 池子
			if (m_renderTargetsFreeList.Count == 0)
			{
				int index = m_renderTargetsPool.Count;
				m_renderTargetsPool.Add(new RenderTarget2D(
					m_graphicsDevice,
					m_graphicsDevice.Viewport.Width,
					m_graphicsDevice.Viewport.Height));
				return index;
			}
			else
			{
				// 否则我们就把之前的空位复用
				int index = m_renderTargetsFreeList.First.Value;
				m_renderTargetsFreeList.RemoveFirst();
				return index;
			}
		}
	}

	private void ReleaseResourceAt(int index)
	{
		lock (this)
		{
			m_renderTargetsFreeList.AddFirst(index);
		}
	}
}
