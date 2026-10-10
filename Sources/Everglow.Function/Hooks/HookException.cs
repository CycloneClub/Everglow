using System.Runtime.CompilerServices;
using System.Text;

namespace Everglow.Commons.Hooks;

public class HookException : Exception
{
	public HookException(string message = default, Exception innerException = null, [CallerFilePath] string file = default, [CallerLineNumber] int line = 0)
		: base(GetMessage(message ?? string.Empty, file, line), innerException)
	{
	}

	private static readonly string[] safeMods = new string[]
	{
		"ModLoader",
		"Hero",
		"CheatSheet",
		"Everglow",
	};

	private static string GetMessage(string message, string file, int line)
	{
		// TODO Translation
		var sb = new StringBuilder();
		sb.AppendLine($"钩子炸了 {message}");
		sb.AppendLine($"File: {file}, Line: {line}");
		sb.AppendLine("可能由于版本更新产生位置错误，或者可能与以下Mod冲突：");
		foreach (var mod in ModLoader.Mods.Where(mod => !safeMods.Contains(mod.Name)))
		{
			sb.AppendLine($"{mod.DisplayName} ({mod.Name})");
		}
		return sb.ToString();
	}
}
