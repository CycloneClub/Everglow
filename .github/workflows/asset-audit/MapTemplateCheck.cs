using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace CiAssetAudit;

public static class MapTemplateCheck
{
	public static List<string> Check(Stream source, Func<string, char, bool> exists)
	{
		var failures = new List<string>();
		try
		{
			using var zip = new GZipStream(source, CompressionMode.Decompress, leaveOpen: true);
			using var reader = new BinaryReader(zip, Encoding.UTF8, leaveOpen: true);
			int count = reader.ReadInt32();
			if (count < 0)
			{
				throw new InvalidDataException("Negative content index count.");
			}
			for (int i = 0; i < count; i++)
			{
				string entry = reader.ReadString();
				ushort id = reader.ReadUInt16();
				int separator = entry.IndexOf('/');
				if (separator < 1 || separator >= entry.Length - 2 || entry[^1] is not ('t' or 'w' or 'e'))
				{
					failures.Add($"index {id}: invalid content entry '{entry}'");
					continue;
				}
				string name = entry[..^1];
				if (name.StartsWith("Everglow/", StringComparison.Ordinal) && !ContentNameCheck.IsCanonical(name[(separator + 1)..]))
				{
					failures.Add($"index {id}: noncanonical content name '{name}'");
				}
				if (!exists(name, entry[^1]))
				{
					failures.Add($"index {id}: unresolved content '{name}' (kind '{entry[^1]}')");
				}
			}
		}
		catch (Exception exception) when (exception is IOException or InvalidDataException or FormatException)
		{
			failures.Add($"invalid MapIO index: {exception.Message}");
		}
		return failures;
	}
}
