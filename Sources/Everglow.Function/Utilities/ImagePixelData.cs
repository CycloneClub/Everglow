using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Everglow.Commons.Utilities;

/// <summary>
/// CPU 内存中的图片像素，按从上到下、从左到右的顺序存储。
/// </summary>
public sealed class ImagePixelData
{
	private readonly Color[] pixels;

	internal ImagePixelData(int width, int height, Color[] pixels)
	{
		Width = width;
		Height = height;
		this.pixels = pixels;
	}

	public int Width { get; }

	public int Height { get; }

	/// <summary>
	/// 在回调中逐行访问或修改 RGBA 像素。
	/// </summary>
	public void ProcessPixelRows(Action<ImagePixelData> processPixels)
	{
		ArgumentNullException.ThrowIfNull(processPixels);
		processPixels(this);
	}

	public Span<Color> GetRowSpan(int row)
	{
		if ((uint)row >= (uint)Height)
		{
			throw new ArgumentOutOfRangeException(nameof(row));
		}
		return pixels.AsSpan(row * Width, Width);
	}

	public Color[,] GetRegion(Rectangle rect)
	{
		if (rect.X < 0 || rect.Y < 0 || rect.Width <= 0 || rect.Height <= 0
			|| rect.X > Width || rect.Y > Height || rect.Width > Width - rect.X || rect.Height > Height - rect.Y)
		{
			throw new ArgumentOutOfRangeException(nameof(rect));
		}

		var colors = new Color[rect.Width, rect.Height];
		for (int y = 0; y < rect.Height; y++)
		{
			var row = GetRowSpan(rect.Y + y).Slice(rect.X, rect.Width);
			for (int x = 0; x < rect.Width; x++)
			{
				colors[x, y] = row[x];
			}
		}
		return colors;
	}
}
