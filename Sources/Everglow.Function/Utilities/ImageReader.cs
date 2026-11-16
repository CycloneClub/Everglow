using System.Buffers.Binary;
using StbImageSharp;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Everglow.Commons.Utilities;

public record struct ImageKeyPoint(int Row, int Column);

public static class ImageReader
{
	private static string ConvertImagePath(string path)
	{
		if (Path.GetExtension(path) == string.Empty)
		{
			path = Path.ChangeExtension(path, ".bmp");
		}

		return path;
	}

	/// <summary>
	/// 读取图片中和targetColor匹配的坐标位置
	/// </summary>
	/// <param name="path"></param>
	/// <param name="targetColor"></param>
	/// <returns></returns>
	public static List<ImageKeyPoint> ReadImageKeyPoints(string path, Color targetColor)
	{
		List<ImageKeyPoint> keyPoints = new();
		var image = Read(path);
		image.ProcessPixelRows(accessor =>
			{
				for (int y = 0; y < accessor.Height; y++)
				{
					var pixelRow = accessor.GetRowSpan(y);
					for (int x = 0; x < pixelRow.Length; x++)
					{
						ref var pixel = ref pixelRow[x];
						if (pixel.R == targetColor.R && pixel.G == targetColor.G && pixel.B == targetColor.B)
						{
							keyPoints.Add(new ImageKeyPoint() { Row = y, Column = x });
						}
					}
				}
			});
		return keyPoints;
	}

	/// <summary>
	/// 从模组资源读取图片
	/// </summary>
	public static ImagePixelData Read(string path)
	{
		using var stream = new MemoryStream(ModContent.GetFileBytes(ConvertImagePath(path)));
		return Read(stream);
	}

	/// <summary>
	/// 读取 RGBA 像素
	/// </summary>
	public static ImagePixelData Read(Stream stream)
	{
		using var buffer = new MemoryStream();
		stream.CopyTo(buffer);
		var input = buffer.ToArray();
		ValidateBmpPixelOffset(input);
		// Stb 根据文件内容识别格式；统一输出 RGBA 像素。
		var decoded = ImageResult.FromMemory(input, ColorComponents.RedGreenBlueAlpha);
		var pixels = new Color[checked(decoded.Width * decoded.Height)];
		for (int i = 0; i < pixels.Length; i++)
		{
			int offset = i * 4;
			pixels[i] = new Color(decoded.Data[offset], decoded.Data[offset + 1], decoded.Data[offset + 2], decoded.Data[offset + 3]);
		}
		return new ImagePixelData(decoded.Width, decoded.Height, pixels);
	}

	/// <summary>
	/// 读取指定区域的颜色
	/// </summary>
	public static Color[,] Read(string path, Rectangle rect)
	{
		return Read(path).GetRegion(rect);
	}

	private static void ValidateBmpPixelOffset(byte[] input)
	{
		if (input.Length < 18 || input[0] != 'B' || input[1] != 'M')
		{
			return;
		}

		int pixelOffset = BinaryPrimitives.ReadInt32LittleEndian(input.AsSpan(10));
		uint headerSize = BinaryPrimitives.ReadUInt32LittleEndian(input.AsSpan(14));
		if (pixelOffset < 14L + headerSize || pixelOffset > input.Length)
		{
			throw new InvalidDataException("Invalid BMP pixel offset.");
		}
	}
}
