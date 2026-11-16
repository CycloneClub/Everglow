using System.IO;
using System.IO.Compression;
using System.Reflection;
using Everglow.Commons.TileHelper;
using Terraria.ModLoader.IO;
using MapIO = Everglow.Commons.TileHelper.MapIO;

namespace Everglow.UnitTests.Function;

[TestClass]
public class MapIOContentNameTests
{
	[TestMethod]
	public void OldChestItemNameIsMigratedWithoutChangingItsCustomData()
	{
		var data = new TagCompound { ["name"] = "Tree_small_item", ["owner"] = "Everglow" };
		var item = new TagCompound
		{
			["mod"] = "Everglow",
			["name"] = "Tree_small_i",
			["data"] = data,
			["stack"] = 12,
		};
		using var stream = new MemoryStream();
		using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true))
		{
			TagIO.Write(item, writer);
		}
		stream.Position = 0;
		using var reader = new BinaryReader(stream);
		var restored = TagIO.Read(reader);
		NormalizeChestItemName(restored);
		Assert.AreEqual("Tree_Small_I", restored.GetString("name"));
		Assert.AreEqual("Everglow", restored.GetString("mod"));
		Assert.AreEqual(12, restored.GetInt("stack"));
		Assert.AreEqual("Tree_small_item", restored.GetCompound("data").GetString("name"));
		Assert.AreEqual("Everglow", restored.GetCompound("data").GetString("owner"));
	}

	[TestMethod]
	public void OtherModsAndVanillaChestItemsRetainTheirNamesAndData()
	{
		foreach (var mod in new[] { "Other_mod", "Terraria", string.Empty })
		{
			var item = new TagCompound { ["mod"] = mod, ["name"] = "Tree_small_item", ["id"] = 7 };
			NormalizeChestItemName(item);
			Assert.AreEqual("Tree_small_item", item.GetString("name"));
			Assert.AreEqual(mod, item.GetString("mod"));
			Assert.AreEqual(7, item.GetInt("id"));
		}
	}

	private static void NormalizeChestItemName(TagCompound item)
	{
		typeof(MapIO).GetMethod("NormalizeChestItemName", BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, [item]);
	}

	[TestMethod]
	public void ReExportPreservesPayloadAndIdsAndIsIdempotent()
	{
		byte[] payload = [0, 255, 4, 0, 0, 0, 37, 29, 18, 0];
		using var original = new MemoryStream();
		using (var zip = new GZipStream(original, CompressionLevel.Optimal, true))
		using (var writer = new BinaryWriter(zip))
		{
			writer.Write(3);
			writer.Write("Everglow/Tree_small_brancht");
			writer.Write((ushort)42);
			writer.Write("Everglow/Tree_Small_Branchw");
			writer.Write((ushort)17);
			writer.Write("Other_mod/Tree_small_branche");
			writer.Write((ushort)8);
			writer.Write(payload);
		}
		original.Position = 0;
		using var exported = new MemoryStream();
		MapIO.RewriteContentNames(original, exported);
		Assert.IsTrue(original.CanRead);
		Assert.IsTrue(exported.CanWrite);
		exported.Position = 0;
		using (var zip = new GZipStream(exported, CompressionMode.Decompress, true))
		using (var reader = new BinaryReader(zip))
		{
			Assert.AreEqual(3, reader.ReadInt32());
			Assert.AreEqual("Everglow/Tree_Small_Brancht", reader.ReadString());
			Assert.AreEqual((ushort)42, reader.ReadUInt16());
			Assert.AreEqual("Everglow/Tree_Small_Branchw", reader.ReadString());
			Assert.AreEqual((ushort)17, reader.ReadUInt16());
			Assert.AreEqual("Other_mod/Tree_small_branche", reader.ReadString());
			Assert.AreEqual((ushort)8, reader.ReadUInt16());
			CollectionAssert.AreEqual(payload, reader.ReadBytes(payload.Length));
			Assert.AreEqual(-1, zip.ReadByte());
		}
		exported.Position = 0;
		using var repeated = new MemoryStream();
		MapIO.RewriteContentNames(exported, repeated);
		CollectionAssert.AreEqual(exported.ToArray(), repeated.ToArray());
	}

	[TestMethod]
	public void WritingOldEntriesUsesCanonicalNamesAndPreservesIdentifiers()
	{
		var entries = new ModEntry();
		entries.entries.Add("Everglow/Tree_small_brancht", 42);
		entries.entries.Add("Everglow/Tree_Small_Branchw", 17);
		entries.entries.Add("Other_mod/Tree_small_branche", 8);
		using var stream = new MemoryStream();
		using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true))
		{
			entries.Write(writer);
		}
		stream.Position = 0;
		using var reader = new BinaryReader(stream);
		Assert.AreEqual(3, reader.ReadInt32());
		Assert.AreEqual("Everglow/Tree_Small_Brancht", reader.ReadString());
		Assert.AreEqual((ushort)42, reader.ReadUInt16());
		Assert.AreEqual("Everglow/Tree_Small_Branchw", reader.ReadString());
		Assert.AreEqual((ushort)17, reader.ReadUInt16());
		Assert.AreEqual("Other_mod/Tree_small_branche", reader.ReadString());
		Assert.AreEqual((ushort)8, reader.ReadUInt16());
	}
}
