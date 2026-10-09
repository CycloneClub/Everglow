using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
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
	/// 读取图片中和targetColor匹配的坐标位置们
	/// </summary>
	/// <param name="path"></param>
	/// <param name="targetColor"></param>
	/// <returns></returns>
	public static List<ImageKeyPoint> ReadImageKeyPoints(string path, Color targetColor)
	{
		List<ImageKeyPoint> keyPoints = new();
		using var image = Read<Rgb24>(path);
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
	/// 直接读取图片并且强转至TPixel格式
	/// </summary>
	/// <param name="path">路径，可以不带后缀名</param>
	/// <returns></returns>
	public static Image<TPixel> Read<TPixel>(string path)
		where TPixel : unmanaged, IPixel<TPixel>
	{
		path = ConvertImagePath(path);
		using var memoryStream = new MemoryStream(ModContent.GetFileBytes(path));
		using var image = Image.Load(memoryStream);
		return image.CloneAs<TPixel>();
	}

	/// <summary>
	/// RGB通道默认值0，Alpha默认值255，默认路径名为.bmp
	/// </summary>
	/// <param name="path">路径，可以不带后缀名</param>
	/// <param name="rect">读取范围，[)</param>
	/// <returns></returns>
	public static Color[,] Read(string path, Rectangle rect)
	{
		path = ConvertImagePath(path);
		using var memoryStream = new MemoryStream(ModContent.GetFileBytes(path));
		using var image = Image.Load(memoryStream);
		CorpImage(image, rect);
		int maxX = rect.X + rect.Width, maxY = rect.Y + rect.Height;
		var colors = new Color[rect.Width, rect.Height];
		Debug.Assert(maxX <= image.Width && maxY <= image.Height);
		if (image is Image<Rgba32> png)
		{
			png.ProcessPixelRows(accessor =>
			{
				for (int j = rect.Y; j < maxY; j++)
				{
					var span = accessor.GetRowSpan(j);
					for (int i = rect.X; i < maxX; i++)
					{
						var t = span[i];
						colors[i - rect.X, j - rect.Y] = new Color(t.R, t.G, t.B, t.A);
					}
				}
			});
		}
		else if (image is Image<Rgb24> jpg)
		{
			jpg.ProcessPixelRows(accessor =>
			{
				for (int j = rect.Y; j < maxY; j++)
				{
					var span = accessor.GetRowSpan(j);
					for (int i = rect.X; i < maxX; i++)
					{
						var t = span[i];
						colors[i, j] = new Color(t.R, t.G, t.B, 255);
					}
				}
			});
		}
		else
		{
			Debug.Fail("剩下格式没写");
		}
		return colors;
	}

	/// <summary>
	/// 根据传入矩形剪裁图片的一部分
	/// </summary>
	/// <param name="input"></param>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	private static void CorpImage(Image input, Rectangle rect)
	{
		input.Mutate(img => img.Crop(SixLabors.ImageSharp.Rectangle.FromLTRB(rect.X, rect.Y, rect.Width + rect.X, rect.Height + rect.Y)));
	}
}
