using Terraria.DataStructures;

namespace Everglow.Commons.Mechanics.Mission.Core;

public abstract class MissionSourceBase
{
	public sealed class SystemMissionSource : MissionSourceBase
	{
		public override Texture2D Texture => ModAsset.Point.Value;

		public override string Name => "Everglow System";
	}

	public static readonly MissionSourceBase Default = new SystemMissionSource();

	public abstract Texture2D Texture { get; }

	public abstract string Name { get; }

	public virtual DrawAnimation Animation => null;

	public override bool Equals(object obj) => obj.GetType() == GetType();

	public override int GetHashCode() => GetType().GetHashCode();
}
