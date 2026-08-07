namespace Everglow.Commons.Utilities;

public static class TextUtils
{
	/// <summary>
	/// Helper method to check if a character is an English letter
	/// </summary>
	/// <param name="c"></param>
	/// <returns></returns>
	public static bool IsEnglishCharacter(char c)
	{
		return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
	}

	/// <summary>
	/// Check if a position can be split boundary
	/// </summary>
	/// <param name="text"></param>
	/// <param name="index"></param>
	/// <returns></returns>
	public static bool IsSplitBoundary(string text, int index)
	{
		if (index <= 0 || index >= text.Length)
		{
			return true;
		}

		char prevChar = text[index - 1];
		char currentChar = text[index];

		return char.IsWhiteSpace(currentChar)
			|| char.IsPunctuation(currentChar)
			|| char.IsDigit(currentChar)
			|| (IsEnglishCharacter(prevChar) && !IsEnglishCharacter(currentChar));
	}

	/// <summary>
	/// Find the previous split boundary
	/// </summary>
	/// <param name="text"></param>
	/// <param name="index"></param>
	/// <returns></returns>
	public static int FindPreviousSplitBoundary(string text, int index)
	{
		for (int i = index; i >= 0; i--)
		{
			if (IsSplitBoundary(text, i))
			{
				return i;
			}
		}
		return 0;
	}

	public static int FindNextSplitBoundnary(string text, int index)
	{
		for (int i = index + 1; i < text.Length; i++)
		{
			if (IsSplitBoundary(text, i))
			{
				return i;
			}
		}

		return text.Length - 1;
	}
}
