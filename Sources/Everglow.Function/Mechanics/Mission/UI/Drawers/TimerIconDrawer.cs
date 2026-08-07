using Everglow.Commons.Mechanics.Mission.PlayerSide;
using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;
using Everglow.Commons.UI.StringDrawerSystem;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems;
using Terraria.GameContent;

namespace Everglow.Commons.Mechanics.Mission.UI.Drawers;

internal class TimerIconDrawer : DrawerItem
{
	public string MissionName;
	public int Size;
	public Color Color;
	public int Thickness;
	private PlayerMissionBase _mission;

	public override string ToString()
	{
		return $"{Line} Mission:{MissionName}";
	}

	public override void Draw(SpriteBatch sb)
	{
		if (_mission == null)
			_mission = PlayerMissionManager.GetMission(MissionName);
		if (_mission == null || _mission.TimeLimit < 0)
			return;
		var scissorRectangle = sb.GraphicsDevice.ScissorRectangle;
		var overflowHiddenRasterizerState = new RasterizerState
		{
			CullMode = sb.GraphicsDevice.RasterizerState.CullMode,
			ScissorTestEnable = sb.GraphicsDevice.RasterizerState.ScissorTestEnable,
		};

		sb.End();
		Effect effect = ModAsset.MissionProgressBar.Value;
		sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
			DepthStencilState.None, overflowHiddenRasterizerState, effect, Main.UIScaleMatrix);

		effect.Parameters["uScaleFactor"].SetValue(Vector2.One / Size);
		effect.Parameters["uRadius"].SetValue(Size / 2f);
		effect.Parameters["uThickness"].SetValue(1f);
		effect.Parameters["uProgress"].SetValue(_mission.TimeLimit == 0 ? 1f : (float)(_mission.Time / (double)_mission.TimeLimit));
		effect.Parameters["uColor"].SetValue(new Vector4(Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f));
		effect.Parameters["uOpposite"].SetValue(true);
		effect.Parameters["uInterval"].SetValue(new Vector2(0, 0));
		effect.CurrentTechnique.Passes[0].Apply();

		sb.Draw(
			TextureAssets.MagicPixel.Value,
			new Rectangle(
				(int)Position.X,
				(int)Position.Y,
				Size, Size), Color.White);

		sb.End();
		sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
			DepthStencilState.None, overflowHiddenRasterizerState, null, Main.UIScaleMatrix);
	}

	public override DrawerItem GetInstance(StringDrawer stringDrawer, string originalText, string name, StringParameters stringParameters)
	{
		var drawerItem = (TimerIconDrawer)Activator.CreateInstance(GetType());
		drawerItem.Init(stringDrawer, originalText, name, stringParameters);
		return drawerItem;
	}

	public override Vector2 GetSize()
	{
		return new Vector2(Size);
	}

	public override void Init(StringDrawer stringDrawer, string originalText, string name, StringParameters stringParameters)
	{
		base.Init(stringDrawer, originalText, name, stringParameters);
		Color = stringParameters.GetColor("Color",
			stringDrawer.DefaultParameters.GetColor(
				"MITColor",
				Color.White));
		Size = stringParameters.GetInt("Size",
			stringDrawer.DefaultParameters.GetInt("MITSize", 18));
		Thickness = stringParameters.GetInt("Thickness",
			stringDrawer.DefaultParameters.GetInt("MITThickness", 1));
		MissionName = stringParameters.GetString("MissionName",
			stringDrawer.DefaultParameters.GetString("MITMissionName", string.Empty));

		_mission = PlayerMissionManager.GetMission(MissionName);
		Size += Size % 2;
	}

	public override float WordWrap(ref int index, List<DrawerItem> drawerItems, ref int line, float width, float originWidth, int? maxLine = null)
	{
		Line++;
		line++;
		return originWidth - GetSize().X;
	}
}
