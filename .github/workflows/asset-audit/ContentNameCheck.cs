namespace CiAssetAudit;

public static class ContentNameCheck
{
	public static bool IsCanonical(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}
		for (int i = 1; i < name.Length; i++)
		{
			if (name[i - 1] == '_' && name[i] is >= 'a' and <= 'z')
			{
				return false;
			}
		}
		return true;
	}
}
