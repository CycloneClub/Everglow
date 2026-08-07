using Everglow.Commons.UI.UIElements;
using Everglow.Commons.Vertex;
using Terraria.GameContent;
using static Everglow.Commons.Mechanics.Mission.UI.MissionContainer;

namespace Everglow.Commons.Mechanics.Mission.UI.UIElements.MissionDetail;

public class UIMissionObjectiveTimer : UIMissionDetailMaskContentBase<UIMissionDetailSubContent>
{
	private const int ProgressVerticesCount = 180;
	private const int ClockRadius = 64;
	private const int ClockPinRadius = 54;
	private const float ClockCenterOffset = 16.5f;

	private UIBlock _back;
	private UIImage _backIcon;

	/// <summary>
	/// Mission timer progress
	/// </summary>
	private float Progress => _mission is not null && _mission.EnableTime // TODO: Replace mission timer with objective timer
		? _mission.Time / (float)_mission.TimeLimit : 0;

	public override void OnInitialization()
	{
		base.OnInitialization();

		// Tree
		_back = new UIBlock();
		_back.Info.SetMargin(0);
		_back.PanelColor = Color.Transparent;
		_back.BorderWidth = 0;
		_back.Info.IsSensitive = true;
		_back.Events.OnMouseHover += e => Instance.MouseText = "Mission Detail";
		_back.Events.OnLeftClick += Hide;
		Register(_back);

		_backIcon = new UIImage(ModAsset.ToPanelSurface.Value, Color.White);
		_backIcon.Events.OnMouseHover += e => _backIcon.Color = new Color(1f, 1f, 1f, 0f);
		_backIcon.Events.OnMouseOut += e => _backIcon.Color = Color.White;
		_back.Register(_backIcon);
	}

	public override void Calculation()
	{
		base.Calculation();
		_back.Info.Width.SetValue(56 * Scale);
		_back.Info.Height.SetValue(83 * Scale);
		_back.Info.Left.SetValue(600 * Scale);
		_back.Info.Top.SetValue(100 * Scale);

		_backIcon.Info.Width = _back.Info.Width;
		_backIcon.Info.Height = _back.Info.Height;
	}

	public override void Draw(SpriteBatch sb)
	{
		base.Draw(sb);

		var scale = Scale * 1.6f;

		// Clock frame
		var clockTexture = ModAsset.AcanthusClock.Value;
		var clockCenter = HitBox.Center.ToVector2();
		sb.Draw(clockTexture, clockCenter, null, Color.White, rotation: 0, clockTexture.Size() / 2, scale, SpriteEffects.None, 0);

		// Clock progress
		var progressRadius = ClockRadius * scale;
		var clockCenterOffset = new Vector2(0, ClockCenterOffset) * scale;
		var progressCenter = HitBox.Center.ToVector2() + clockCenterOffset;
		DrawTimerProgress(Progress, progressCenter, progressRadius);

		// Clock pin
		var clockPinTexture = ModAsset.AcanthusClock_Pin.Value;
		var clockPinCenter = progressCenter;
		var clockPinOrigin = new Vector2(-ClockPinRadius, clockPinTexture.Height / 2f);
		var clockRotation = Progress * MathHelper.TwoPi - MathHelper.PiOver2;
		sb.Draw(clockPinTexture, clockPinCenter, null, new Color(1f, 1f, 1f, 0f), clockRotation, clockPinOrigin, scale, SpriteEffects.None, 0);
	}

	private static void DrawTimerProgress(float progress, Vector2 center, float radius)
	{
		var vertices = new List<Vertex2D>();
		for (int i = 0; i < ProgressVerticesCount * progress; i++)
		{
			var rotation = MathHelper.TwoPi * i / ProgressVerticesCount;
			vertices.Add(center + new Vector2(0, 0), Color.White, new(0, 1, 1));
			vertices.Add(center + new Vector2(0, -radius).RotatedBy(rotation), Color.White, new(0, 0, 1));
		}

		Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
	}
}
