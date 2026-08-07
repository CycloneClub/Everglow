using Everglow.Commons.Mechanics.Mission.Core;
using Everglow.Commons.Mechanics.Mission.PlayerSide;

namespace Everglow.Commons.Mechanics.Mission.Presentation;

public static class ColorDefinition
{
	public static readonly Color InitialLightColor = new Color(1f, 1f, 1f, 0f) * 0.8f;

	public static Color GetMissionStateColor(PlayerMissionState? poolType) => poolType switch
	{
		PlayerMissionState.Accepted => new Color(0f, 1f, 0f, 0f),
		PlayerMissionState.Available => new Color(0.9f, 0.88f, 0.06f, 0f),
		PlayerMissionState.Failed => new Color(1f, 0f, 0f, 0.3f),
		PlayerMissionState.Overdue => new Color(0.5f, 0f, 0.2f, 0.7f),
		PlayerMissionState.Completed => new Color(0.2f, 0.6f, 1f, 0.2f),
		null => InitialLightColor,
		_ => InitialLightColor,
	};

	public static Color GetMissionTypeColor(MissionType? missionType) => missionType switch
	{
		MissionType.None => new Color(0f, 0f, 0f, 1f),
		MissionType.MainStory => new Color(1f, 0.9f, 0.1f, 0f),
		MissionType.SideStory => new Color(0.4f, 0.1f, 1f, 0.4f),
		MissionType.Legendary => Color.Lerp(
			new Color(
			MathF.Sin((float)Main.timeForVisualEffects * 0.04f),
			MathF.Sin((float)Main.timeForVisualEffects * 0.04f + MathHelper.TwoPi / 3f),
			MathF.Sin((float)Main.timeForVisualEffects * 0.04f + MathHelper.TwoPi / 3f * 2),
			0f), new Color(1f, 1f, 1f, 0), 0.35f),
		MissionType.Achievement => new Color(0.15f, 0.7f, 0.3f, 0.3f),
		MissionType.Daily => new Color(0f, 0.2f, 1f, 0.4f),
		MissionType.Challenge => new Color(1f, 0f, 0f, 0.3f),
		null => InitialLightColor,
		_ => InitialLightColor,
	};

	public static Texture2D GetBackground(MissionType? missionType) => missionType switch
	{
		MissionType.None => ModAsset.MissionStackPanel_None.Value,
		MissionType.MainStory => ModAsset.MissionStackPanel_MainStory.Value,
		MissionType.SideStory => ModAsset.MissionStackPanel_SideStory.Value,
		MissionType.Achievement => ModAsset.MissionStackPanel_Achievement.Value,
		MissionType.Challenge => ModAsset.MissionStackPanel_Challenge.Value,
		MissionType.Daily => ModAsset.MissionStackPanel_Daily.Value,
		MissionType.Legendary => ModAsset.MissionStackPanel_Legendary.Value,
		_ => ModAsset.MissionStackPanel_None.Value,
	};

	public static Texture2D GetMissionStatus(PlayerMissionState? poolType) => poolType switch
	{
		PlayerMissionState.Accepted => ModAsset.MissionState_Accepted.Value,
		PlayerMissionState.Available => ModAsset.MissionState_Available.Value,
		PlayerMissionState.Completed => ModAsset.MissionState_Completed.Value,
		PlayerMissionState.Overdue => ModAsset.MissionState_Failed.Value,
		PlayerMissionState.Failed => ModAsset.MissionState_Failed.Value,
		_ => ModAsset.MissionState_Accepted.Value,
	};
}
