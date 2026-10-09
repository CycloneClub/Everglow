namespace Everglow.Commons.ObjectPool;

/// <summary>
/// 一个可独占资源的句柄，拥有者获得对在对象池中的对象的访问权，使用结束后需要调用Release来释放对对象池中对象的访问权
/// </summary>
/// <typeparam name="T"></typeparam>
public class ResourceLocker<T>
{
	/// <summary>
	/// 要访问资源请一定使用这个 Resource 属性，如果把之前的资源存下来可能会导致访问到错误的资源
	/// </summary>
	public T Resource
	{
		get
		{
			if (!m_released)
			{
				return m_resource;
			}
			else
			{
				throw new InvalidOperationException("Cannot access a released resource");
			}
		}
	}

	public bool IsReleased
	{
		get
		{
			return m_released;
		}
	}

	private T m_resource;
	private bool m_released;
	private Action m_releaseAction;

	/// <summary>
	/// 构造函数可以传入一个Action表示这个对象的释放流程
	/// </summary>
	/// <param name="resource"></param>
	/// <param name="releaseAction"></param>
	public ResourceLocker(T resource, Action releaseAction)
	{
		m_resource = resource;
		m_releaseAction = releaseAction;
		m_released = false;
	}

	public void Release()
	{
		if (!m_released)
		{
			m_released = true;
			m_releaseAction();
		}
		else
		{
			throw new InvalidOperationException("Resource already been released");
		}
	}
}
