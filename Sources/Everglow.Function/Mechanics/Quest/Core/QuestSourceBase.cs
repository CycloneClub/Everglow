using Terraria.DataStructures;

namespace Everglow.Commons.Mechanics.Quest.Core;

public abstract class QuestSourceBase
{
	public sealed class SystemQuestSource : QuestSourceBase
	{
		public override Texture2D Texture => ModAsset.Point.Value;

		public override string Name => "Everglow System";
	}

	public static readonly QuestSourceBase Default = new SystemQuestSource();

	public abstract Texture2D Texture { get; }

	public abstract string Name { get; }

	public virtual DrawAnimation Animation => null;

	public override bool Equals(object obj) => obj.GetType() == GetType();

	public override int GetHashCode() => GetType().GetHashCode();
}
