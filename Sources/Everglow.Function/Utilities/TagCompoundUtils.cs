using Terraria.ModLoader.IO;

namespace Everglow.Commons.Utilities;

public class TagCompoundUtils
{
	public class KeyValuePairSerializer_Int_Int : TagSerializer<KeyValuePair<int, int>, TagCompound>
	{
		private const string Key = nameof(Key);
		private const string Value = nameof(Value);

		public override KeyValuePair<int, int> Deserialize(TagCompound tag) => new(tag.GetInt(Key), tag.GetInt(Value));

		public override TagCompound Serialize(KeyValuePair<int, int> value) => new TagCompound()
		{
			[Key] = value.Key,
			[Value] = value.Value,
		};
	}
}
