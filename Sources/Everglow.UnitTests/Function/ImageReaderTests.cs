using System.Buffers.Binary;
using Everglow.Commons.Utilities;
using Microsoft.Xna.Framework;

namespace Everglow.UnitTests.Function;

[TestClass]
public class ImageReaderTests
{
	[TestMethod]
	[DataRow(0, false)]
	[DataRow(12, false)]
	[DataRow(0, true)]
	[DataRow(12, true)]
	public void Read_V5Bmp_PreservesPixelCoordinatesAndTransparentRgb(int gap, bool topDown)
	{
		using var stream = new MemoryStream(CreateV5Bmp(gap, topDown));
		var image = ImageReader.Read(stream);

		Assert.AreEqual(2, image.Width);
		Assert.AreEqual(2, image.Height);
		CollectionAssert.AreEqual(new[] { new Color(255, 0, 0, 0), new Color(0, 255, 0, 128) }, image.GetRowSpan(0).ToArray());
		CollectionAssert.AreEqual(new[] { Color.Blue, Color.White }, image.GetRowSpan(1).ToArray());
		Assert.IsTrue(stream.CanRead);
	}

	[TestMethod]
	public void GetRegion_NonzeroOrigin_UsesLocalCoordinates()
	{
		using var stream = new MemoryStream(CreateV5Bmp(12, false));
		var region = ImageReader.Read(stream).GetRegion(new Rectangle(1, 0, 1, 2));

		Assert.AreEqual(1, region.GetLength(0));
		Assert.AreEqual(2, region.GetLength(1));
		Assert.AreEqual(new Color(0, 255, 0, 128), region[0, 0]);
		Assert.AreEqual(Color.White, region[0, 1]);
	}

	[TestMethod]
	[DataRow(-1, 0, 1, 1)]
	[DataRow(0, -1, 1, 1)]
	[DataRow(1, 0, 2, 1)]
	[DataRow(0, 1, 1, 2)]
	[DataRow(0, 0, 0, 1)]
	[DataRow(int.MaxValue, 0, 1, 1)]
	public void GetRegion_OutsideImage_Throws(int x, int y, int width, int height)
	{
		using var stream = new MemoryStream(CreateV5Bmp(0, false));
		var image = ImageReader.Read(stream);

		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => image.GetRegion(new Rectangle(x, y, width, height)));
	}

	[TestMethod]
	[DataRow(-1)]
	[DataRow(2)]
	public void GetRowSpan_OutsideImage_Throws(int row)
	{
		using var stream = new MemoryStream(CreateV5Bmp(0, false));
		var image = ImageReader.Read(stream);

		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => image.GetRowSpan(row));
	}

	[TestMethod]
	[DataRow(137)]
	[DataRow(int.MaxValue)]
	public void Read_V5BmpWithInvalidPixelOffset_Throws(int offset)
	{
		var bytes = CreateV5Bmp(0, false);
		BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(10), offset);
		using var stream = new MemoryStream(bytes);

		Assert.ThrowsExactly<InvalidDataException>(() => ImageReader.Read(stream));
	}

	[TestMethod]
	public void Read_FromCurrentStreamPosition_LeavesStreamOpen()
	{
		using var stream = new MemoryStream();
		stream.Write(new byte[7]);
		stream.Write(CreateV5Bmp(12, false));
		stream.Position = 7;

		var image = ImageReader.Read(stream);

		Assert.AreEqual(Color.White, image.GetRowSpan(1)[1]);
		Assert.IsTrue(stream.CanRead);
	}

	[TestMethod]
	public void Read_Gif_UsesSharedDecoder()
	{
		using var stream = new MemoryStream(Convert.FromBase64String("R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7"));
		var image = ImageReader.Read(stream);

		Assert.AreEqual(1, image.Width);
		Assert.AreEqual(1, image.Height);
	}

	[TestMethod]
	public void ProcessPixelRows_PreservesRowOrderAndSupportsPixelEdits()
	{
		using var stream = new MemoryStream(CreateV5Bmp(12, false));
		var image = ImageReader.Read(stream);
		var visited = new List<Color>();
		image.ProcessPixelRows(accessor =>
		{
			Assert.AreEqual(2, accessor.Width);
			Assert.AreEqual(2, accessor.Height);
			for (int y = 0; y < accessor.Height; y++)
			{
				var pixelRow = accessor.GetRowSpan(y);
				for (int x = 0; x < pixelRow.Length; x++)
				{
					ref var pixel = ref pixelRow[x];
					visited.Add(pixel);
					pixel.A = 0;
				}
			}
		});

		CollectionAssert.AreEqual(new[] { new Color(255, 0, 0, 0), new Color(0, 255, 0, 128), Color.Blue, Color.White }, visited);
		Assert.AreEqual(new Color(0, 255, 0, 0), image.GetRowSpan(0)[1]);
		Assert.AreEqual(new Color(255, 255, 255, 0), image.GetRegion(new Rectangle(1, 1, 1, 1))[0, 0]);
	}

	[TestMethod]
	public void ProcessPixelRows_NullCallback_Throws()
	{
		using var stream = new MemoryStream(CreateV5Bmp(0, false));
		var image = ImageReader.Read(stream);

		Assert.ThrowsExactly<ArgumentNullException>(() => image.ProcessPixelRows(null!));
	}

	// A 2x2 V5 BMP, stored as BGRA. The top-left transparent red is meaningful map data.
	private static byte[] CreateV5Bmp(int gap, bool topDown)
	{
		int offset = 138 + gap;
		var bytes = new byte[offset + 16];
		bytes[0] = (byte)'B';
		bytes[1] = (byte)'M';
		BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(2), bytes.Length);
		BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(10), offset);
		BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(14), 124);
		BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(18), 2);
		BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(22), topDown ? -2 : 2);
		BinaryPrimitives.WriteUInt16LittleEndian(bytes.AsSpan(26), 1);
		BinaryPrimitives.WriteUInt16LittleEndian(bytes.AsSpan(28), 32);
		BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(30), 3);
		BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(34), 16);
		BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(54), 0x00FF0000);
		BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(58), 0x0000FF00);
		BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(62), 0x000000FF);
		BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(66), 0xFF000000);
		byte[] top = [0, 0, 255, 0, 0, 255, 0, 128];
		byte[] bottom = [255, 0, 0, 255, 255, 255, 255, 255];
		(topDown ? top : bottom).CopyTo(bytes, offset);
		(topDown ? bottom : top).CopyTo(bytes, offset + 8);
		return bytes;
	}
}
