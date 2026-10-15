namespace Everglow.Commons.DeveloperContent.VFXs;

public static class TileToolBoxSystemListExtension
{
	public static void AddRangeDistinct<T>(this List<T> list, IEnumerable<T> source)
	{
		HashSet<T> temp = new HashSet<T>(list);
		foreach (var item in source)
		{
			temp.Add(item);
		}

		list.Clear();
		list.AddRange(temp);
	}
}
