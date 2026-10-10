using Everglow.Commons.Interfaces;

namespace Everglow.Commons;

public class Future<T>
{
	private bool isLoaded;

	private T value;

	public T Value
	{
		get
		{
			if (!IsLoaded)
			{
				throw new InvalidOperationException("Hasn't been Loaded");
			}

			return value;
		}

		set
		{
			if (IsLoaded)
			{
				throw new InvalidOperationException("Has been Loaded");
			}

			isLoaded = true;
			this.value = value;
		}
	}

	public bool IsLoaded => isLoaded;
}

public class MainThreadContext : IMainThreadContext
{
	public void AddTask(Action task)
	{
		if (ThreadCheck.IsMainThread)
		{
			task.Invoke();
		}
		else
		{
			Main.QueueMainThreadAction(task);
		}
	}

	public Future<Color[]> DelayGetColors(Texture2D texture)
	{
		Future<Color[]> future = new();
		AddTask(() =>
		{
			var temp = new Color[texture.Width * texture.Height];
			texture.GetData(temp);
			future.Value = temp;
		});
		return future;
	}

	public Future<T> DelayGetData<T>(Func<T> func)
	{
		Future<T> future = new();
		AddTask(() =>
		{
			future.Value = func();
		});
		return future;
	}
}
