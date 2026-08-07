using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;
using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.Mechanics.Mission.UI.UIElements.MissionDetail;

public abstract class UIMissionDetailMaskContentBase<TMask> : UIBlock
	where TMask : UIMissionDetailMaskBase<TMask>, new()
{
	protected PlayerMissionBase _mission;

	public event Action<BaseElement> HideMask;

	public virtual Color? BackgroundColor => null;

	public override void OnInitialization()
	{
		Info.Width.SetFull();
		Info.Height.SetFull();
		Info.SetMargin(0);
		PanelColor = Color.Transparent;
		BorderWidth = 0;
	}

	protected void Hide(BaseElement _)
	{
		HideMask?.Invoke(this);
	}

	public virtual void SetMission(PlayerMissionBase mission)
	{
		_mission = mission;
	}

	public virtual void Reset()
	{
		_mission = null;
	}
}
