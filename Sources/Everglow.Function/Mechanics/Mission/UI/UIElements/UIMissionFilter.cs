using Everglow.Commons.DataStructures;
using Everglow.Commons.Mechanics.Mission.Core;
using Everglow.Commons.Mechanics.Mission.PlayerSide;
using Everglow.Commons.Mechanics.Mission.PlayerSide;
using Everglow.Commons.Mechanics.Mission.Presentation;
using Everglow.Commons.Mechanics.Mission.UI;
using Everglow.Commons.UI.UIElements;
using Everglow.Commons.Utilities;

namespace Everglow.Commons.Mechanics.Mission.UI.UIElements;

public class UIMissionFilter : BaseElement
{
	private const float RotationSnapThreshold = 0.01f;

	// Outer ring rotation state
	private float _outerRotation;
	private Vector2? _outerMouseDownPosition;
	private float _outerMouseClickRotation;
	private bool _outerHeld;
	private Vector2 _outerDispalcement;
	private Vector2 _outerClickPoint;
	private float? _outerClickTargetRotation;
	private float? _outerHoverTargetRotation;
	private float _outerRotationMisaligment;

	// Inner ring rotation state
	private float _innerRotation;
	private Vector2? _innerMouseDownPosition;
	private float _innerMouseClickRotation;
	private bool _innerHeld;
	private Vector2 _innerDispalcement;
	private Vector2 _innerClickPoint;
	private float? _innerClickTargetRotation;
	private float? _innerHoverTargetRotation;
	private float _innerRotationMisaligment;

	/// <summary>
	/// Available pool type selections (null represents "All" option)
	/// </summary>
	private static List<PlayerMissionState?> PoolTypeList { get; } = [null, .. Enum.GetValues<PlayerMissionState>()];

	/// <summary>
	/// Available mission type selections (null represents "All" option)
	/// </summary>
	private static List<MissionType?> MissionTypeList { get; } = [null, .. Enum.GetValues<MissionType>()];

	/// <summary>
	/// Currently selected pool type filter
	/// </summary>
	public PlayerMissionState? PoolTypeValue { get; private set; }

	/// <summary>
	/// Currently selected mission type filter
	/// </summary>
	public MissionType? MissionTypeValue { get; private set; }

	public bool SpectrumBlockedAtInner => _innerRotationMisaligment > 0.05f;

	public bool SpectrumBlockedAtOuter => _outerRotationMisaligment > 0.05f;

	private float MouseRotation => HitBox.Center.ToVector2().AngleTo(Main.MouseScreen);

	private float MouseHoldDisplacementLimitForAutoRotation => 10 * MissionContainer.Scale;

	private static MissionType? RotationToMissionType(float rotation)
	{
		var unit = MathHelper.TwoPi / MissionTypeList.Count;
		var standard = ((rotation % MathHelper.TwoPi) + MathHelper.TwoPi) % MathHelper.TwoPi;
		var index = (int)Math.Round(standard / unit) % MissionTypeList.Count;
		return MissionTypeList[index];
	}

	private MissionType? RotationToMissionTypeCheckGemMisalignment(float rotation)
	{
		var unit = MathHelper.TwoPi / MissionTypeList.Count;
		var standard = ((rotation % MathHelper.TwoPi) + MathHelper.TwoPi) % MathHelper.TwoPi;
		var index = (int)Math.Round(standard / unit) % MissionTypeList.Count;
		float angularMisalignment = MathF.Abs((standard + unit * 0.5f) % unit - unit * 0.5f);
		_outerRotationMisaligment = angularMisalignment;

		return MissionTypeList[index];
	}

	private static PlayerMissionState? RotationToPoolType(float rotation)
	{
		var unit = MathHelper.TwoPi / PoolTypeList.Count;
		var standard = ((rotation % MathHelper.TwoPi) + MathHelper.TwoPi) % MathHelper.TwoPi;
		var index = (int)Math.Round(standard / unit) % PoolTypeList.Count;
		return PoolTypeList[index];
	}

	private PlayerMissionState? RotationToPoolTypeCheckGemMisalignment(float rotation)
	{
		var unit = MathHelper.TwoPi / PoolTypeList.Count;
		var standard = ((rotation % MathHelper.TwoPi) + MathHelper.TwoPi) % MathHelper.TwoPi;
		var index = (int)Math.Round(standard / unit) % PoolTypeList.Count;
		float angularMisalignment = MathF.Abs((standard + unit * 0.5f) % unit - unit * 0.5f);
		_innerRotationMisaligment = angularMisalignment;

		return PoolTypeList[index];
	}

	private static float MissionTypeToRotation(MissionType? type) => MissionTypeList.IndexOf(type) * MathHelper.TwoPi / MissionTypeList.Count;

	private static float PoolTypeToRotation(PlayerMissionState? type) => PoolTypeList.IndexOf(type) * MathHelper.TwoPi / PoolTypeList.Count;

	private static bool DistanceWithinInnerRing(float distance)
	{
		var innerRadius1 = 229 / 2f;
		innerRadius1 *= MissionContainer.Scale;
		var innerRadius2 = 229 / 2f - 42;
		innerRadius2 *= MissionContainer.Scale;
		return distance < innerRadius1 && distance > innerRadius2;
	}

	private static bool DistanceWithinOuterRing(float distance)
	{
		var outerRadius1 = 337 / 2f;
		outerRadius1 *= MissionContainer.Scale;
		var outerRadius2 = 337 / 2f - 48;
		outerRadius2 *= MissionContainer.Scale;
		return distance < outerRadius1 && distance > outerRadius2;
	}

	private static float GetNaturalRotation(float targetRotation, float originRotation)
	{
		var rotationDiff = (targetRotation - originRotation) % MathHelper.TwoPi;
		if (MathF.Abs(rotationDiff) > MathF.PI)
		{
			if (rotationDiff > 0)
			{
				rotationDiff -= MathHelper.TwoPi;
			}
			else if (rotationDiff < 0)
			{
				rotationDiff += MathHelper.TwoPi;
			}
		}

		return rotationDiff;
	}

	/// <summary>
	/// Calculate the nearest snap rotation based on the current rotation and the count of options.
	/// </summary>
	/// <param name="currentRotation"></param>
	/// <param name="optionCount"></param>
	/// <returns></returns>
	/// <exception cref="InvalidDataException"></exception>
	private static float CalculateNearestSnapRotation(float currentRotation, float optionCount)
	{
		if (optionCount <= 0)
		{
			throw new InvalidDataException();
		}

		var unit = MathHelper.TwoPi / optionCount;
		var diff = currentRotation % MathHelper.TwoPi % unit;

		if (diff > 0)
		{
			if (diff > unit / 2)
			{
				diff = unit - diff;
			}
			else
			{
				diff = -diff;
			}
		}
		else if (diff < 0)
		{
			if (diff < -unit / 2)
			{
				diff = -unit - diff;
			}
			else
			{
				diff = -diff;
			}
		}
		return currentRotation + diff;
	}

	public override void OnInitialization()
	{
		base.OnInitialization();

		Events.OnMouseHover += ManageHeldLogic;
	}

	public override void Calculation()
	{
		base.Calculation();

		var scale = MissionContainer.Scale;

		Info.Width.SetValue(350 * scale);
		Info.Height.SetValue(350 * scale);
	}

	public override void Update(GameTime gt)
	{
		// Update mission type
		var missionType = RotationToMissionTypeCheckGemMisalignment(_outerRotation);
		if (MissionTypeValue != missionType)
		{
			MissionTypeValue = missionType;
			PlayerMissionManager.NeedRefresh = true;
		}

		// Update pool type
		var poolType = RotationToPoolTypeCheckGemMisalignment(_innerRotation);
		if (PoolTypeValue != poolType)
		{
			PoolTypeValue = poolType;
			PlayerMissionManager.NeedRefresh = true;
		}

		ManageAutoRotation();

		var distanceToCenter = Main.MouseScreen.Distance(HitBox.Center.ToVector2());

		// Outer ring hover logic: show mission type text and highlight the selected mission type
		if (DistanceWithinOuterRing(distanceToCenter))
		{
			var hoverMissionType = RotationToMissionType(MathHelper.Pi - MouseRotation + _outerRotation);
			_outerHoverTargetRotation = MissionTypeToRotation(hoverMissionType);
			MissionContainer.Instance.MouseText = hoverMissionType?.ToString() ?? "All";
		}
		else
		{
			_outerHoverTargetRotation = null;
		}

		// Inner ring hover logic: show mission type text and highlight the selected mission type
		if (DistanceWithinInnerRing(distanceToCenter))
		{
			var hoverPoolType = RotationToPoolType(MathHelper.Pi - MouseRotation + _innerRotation);
			_innerHoverTargetRotation = PoolTypeToRotation(hoverPoolType);
			MissionContainer.Instance.MouseText = hoverPoolType?.ToString() ?? "All";
		}
		else
		{
			_innerHoverTargetRotation = null;
		}
	}

	/// <summary>
	/// Manage the ring held logic: rotate the inner and outer ring with mouse movement when held.
	/// </summary>
	/// <param name="baseElement"></param>
	private void ManageHeldLogic(BaseElement baseElement)
	{
		// Reset held state when mouse is not clicking
		var mouseLeftClicking = Main.mouseLeft && !Main.mouseLeftRelease;
		if (!mouseLeftClicking)
		{
			_innerHeld = false;
			_outerHeld = false;
		}

		var distanceToCenter = Main.MouseScreen.Distance(HitBox.Center.ToVector2());

		// Outer ring held logic
		if (((DistanceWithinOuterRing(distanceToCenter) && mouseLeftClicking) || _outerHeld) && !_innerHeld)
		{
			RotateRingWithMouse(ref _outerMouseDownPosition, ref _outerMouseClickRotation, ref _outerRotation, ref _outerHeld);
		}
		else
		{
			// Outer on release
			ResetHeldState(ref _outerMouseDownPosition);
		}

		// Inner ring held logic
		if (((DistanceWithinInnerRing(distanceToCenter) && mouseLeftClicking) || _innerHeld) && !_outerHeld)
		{
			RotateRingWithMouse(ref _innerMouseDownPosition, ref _innerMouseClickRotation, ref _innerRotation, ref _innerHeld);
		}
		else
		{
			// Inner on release
			ResetHeldState(ref _innerMouseDownPosition);
		}
	}

	/// <summary>
	/// Clear the ring mouse held state when released.
	/// </summary>
	/// <param name="mouseDownPosition"></param>
	private void ResetHeldState(ref Vector2? mouseDownPosition)
	{
		mouseDownPosition = null;
	}

	/// <summary>
	/// Rotate the ring with mouse movement when held.
	/// </summary>
	/// <param name="mouseDownPosition"></param>
	/// <param name="mouseClickRotation"></param>
	/// <param name="rotation"></param>
	/// <param name="held"></param>
	private void RotateRingWithMouse(ref Vector2? mouseDownPosition, ref float mouseClickRotation, ref float rotation, ref bool held)
	{
		if (mouseDownPosition == null)
		{
			mouseDownPosition = Main.MouseScreen;
			mouseClickRotation = rotation;
			held = true;
		}

		var enterAngle = HitBox.Center.ToVector2().AngleTo(mouseDownPosition.Value);
		rotation = mouseClickRotation + MouseRotation - enterAngle;
	}

	/// <summary>
	/// Rotate the ring to the target rotation.
	/// </summary>
	/// <param name="targetRotation"></param>
	/// <param name="currentRotation"></param>
	private void RotateRingTo(float targetRotation, ref float currentRotation)
	{
		var rotationDiff = GetNaturalRotation(targetRotation, currentRotation);

		if (MathF.Abs(rotationDiff) > RotationSnapThreshold)
		{
			currentRotation += rotationDiff / 10;
		}
		else
		{
			currentRotation += rotationDiff;
		}
	}

	/// <summary>
	/// Manage auto-rotation of two rings.
	/// <para/>The auto-rotation is consist of two part: click-to-select and fix rotation.
	/// </summary>
	private void ManageAutoRotation()
	{
		// Inner spin
		if (_innerHeld)
		{
			_innerDispalcement = Main.MouseScreen - _innerClickPoint;
			_innerClickTargetRotation = null;
		}
		else
		{
			// TODO: _innerDispalcement will reset to Vector2.zero when resolution change. This caused the filter rotated.
			if (_innerDispalcement.Length() < MouseHoldDisplacementLimitForAutoRotation/* && _innerDispalcement.Length() != 0*/)
			{
				var clickedPoolType = RotationToPoolType(MathHelper.Pi - MouseRotation + _innerRotation);
				_innerClickTargetRotation = PoolTypeToRotation(clickedPoolType);
			}
			_innerDispalcement = new Vector2(1000);
			_innerClickPoint = Main.MouseScreen;

			// If there's no click target rotation, then fix the rotation to nearest snap.
			_innerClickTargetRotation ??= CalculateNearestSnapRotation(_innerRotation, PoolTypeList.Count);

			RotateRingTo(_innerClickTargetRotation.Value, ref _innerRotation);
		}

		// Outer spin
		if (_outerHeld)
		{
			_outerDispalcement = Main.MouseScreen - _outerClickPoint;
			_outerClickTargetRotation = null;
		}
		else
		{
			// TODO: _outerDispalcement will reset to Vector2.zero when resolution change. This caused the filter rotated.
			if (_outerDispalcement.Length() < MouseHoldDisplacementLimitForAutoRotation/* && _outerDispalcement.Length() != 0*/)
			{
				var clickedMissionType = RotationToMissionType(MathHelper.Pi - MouseRotation + _outerRotation);
				_outerClickTargetRotation = MissionTypeToRotation(clickedMissionType);
			}
			_outerDispalcement = new Vector2(1000);
			_outerClickPoint = Main.MouseScreen;

			// If there's no click target rotation, then fix the rotation to nearest snap.
			_outerClickTargetRotation ??= CalculateNearestSnapRotation(_outerRotation, MissionTypeList.Count);
			RotateRingTo(_outerClickTargetRotation.Value, ref _outerRotation);
		}
	}

	public override void Draw(SpriteBatch sb)
	{
		base.Draw(sb);

		var drawPos = new Vector2(Info.HitBox.X + Info.HitBox.Width / 2, Info.HitBox.Y + Info.HitBox.Height / 2);
		var scale = MissionContainer.Scale;

		// gems and panel _outer
		var typeTexture = ModAsset.MissionClassificationMarbleRing_Panel.Value;
		sb.Draw(typeTexture, drawPos, null, Color.White, _outerRotation, typeTexture.Size() / 2, scale, SpriteEffects.None, 0);
		var typeTexture_Gem = ModAsset.MissionClassificationMarbleRing_Gemstones.Value;
		sb.Draw(typeTexture_Gem, drawPos, null, Color.White * 0.9f, _outerRotation, typeTexture.Size() / 2, scale, SpriteEffects.None, 0);

		float outerGlowFade = (0.1f - _outerRotationMisaligment) * 10f;
		outerGlowFade = MathF.Max(outerGlowFade, 0);
		var typeTexture_Gem_select = ModAsset.MissionClassificationMarbleRing_GemstoneSelected.Value;
		var point = new Vector2(-148.5f, 0).RotatedBy(-_outerRotation) * scale;
		var frame = new Rectangle((int)(point.X - 20 + 168.5), (int)(point.Y - 20 + 168.5), 40, 40);
		if (!SpectrumBlockedAtOuter)
		{
			// gem glow _outer
			sb.Draw(typeTexture_Gem_select, drawPos + new Vector2(-148.5f, 0), frame, new Color(1f, 1f, 1f, 0) * outerGlowFade, _outerRotation, frame.Size() / 2, scale, SpriteEffects.None, 0);
		}

		SpriteBatchState sBS = GraphicsUtils.GetState(sb).Value;
		sb.End();
		sb.Begin(sBS);
		sb.sortMode = SpriteSortMode.Immediate;
		sb.blendState = BlendState.AlphaBlend;
		Effect goldShader = ModAsset.GoldenReflection.Value;
		goldShader.Parameters["sv_Pos_Y"].SetValue(drawPos.Y);
		goldShader.Parameters["uSize"].SetValue(0.006f);
		goldShader.Parameters["uHeatMap"].SetValue(ModAsset.GoldenHue_Dark.Value);
		goldShader.CurrentTechnique.Passes[0].Apply();
		var typeGlow = ModAsset.GoldRingTexture.Value;
		sb.Draw(typeGlow, drawPos, null, Color.White, _outerRotation, typeGlow.Size() / 2, scale, SpriteEffects.None, 0);
		sb.End();
		sb.Begin(sBS);

		// gem glow _inner
		var statusFilter = ModAsset.MissionDurationMarbleRing_Panel.Value;
		sb.Draw(statusFilter, drawPos, null, Color.White, _innerRotation, statusFilter.Size() / 2, scale, SpriteEffects.None, 0);
		var statusFilter_Gem = ModAsset.MissionDurationMarbleRing_Gemstones.Value;
		sb.Draw(statusFilter_Gem, drawPos, null, Color.White * 0.9f, _innerRotation, statusFilter_Gem.Size() / 2, scale, SpriteEffects.None, 0);

		// gem glow _inner
		float innerGlowFade = (0.1f - _innerRotationMisaligment) * 10f;
		innerGlowFade = MathF.Max(innerGlowFade, 0);
		var statusFilter_Gem_select = ModAsset.MissionDurationMarbleRing_GemstonesSelected.Value;
		point = new Vector2(-94.5f, 0).RotatedBy(-_innerRotation) * scale;
		frame = new Rectangle((int)(point.X - 20 + 114.5), (int)(point.Y - 20 + 114.5), 40, 40);
		if (!SpectrumBlockedAtInner)
		{
			sb.Draw(statusFilter_Gem_select, drawPos + new Vector2(-94.5f, 0), frame, new Color(1f, 1f, 1f, 0) * innerGlowFade, _innerRotation, frame.Size() / 2, scale, SpriteEffects.None, 0);
		}

		// StarEffect_Outer
		var star = ModAsset.StarSlash.Value;
		var star_dark = ModAsset.StarSlash_black.Value;
		var starColor = ColorDefinition.GetMissionTypeColor(MissionTypeValue);
		starColor.A = 0;
		var star_darkColor = ColorDefinition.GetMissionTypeColor(MissionTypeValue);
		star_darkColor = new Color(star_darkColor.A, star_darkColor.A, star_darkColor.A, star_darkColor.A);
		if (!SpectrumBlockedAtOuter)
		{
			sb.Draw(star_dark, drawPos + new Vector2(-148.5f, 0) * scale, null, star_darkColor * MathF.Pow(outerGlowFade, 4f), MathHelper.Pi / 6f, star_dark.Size() / 2, scale * new Vector2(0.4f, 0.2f) * MathF.Pow(outerGlowFade, 4f), SpriteEffects.None, 0);
			sb.Draw(star_dark, drawPos + new Vector2(-148.5f, 0) * scale, null, star_darkColor * MathF.Pow(outerGlowFade, 4f), -MathHelper.Pi / 6f, star_dark.Size() / 2, scale * new Vector2(0.4f, 0.2f) * MathF.Pow(outerGlowFade, 4f), SpriteEffects.None, 0);
			sb.Draw(star_dark, drawPos + new Vector2(-148.5f, 0) * scale, null, star_darkColor * MathF.Pow(outerGlowFade, 4f), MathHelper.PiOver2, star_dark.Size() / 2, scale * 0.4f * MathF.Pow(outerGlowFade, 4f), SpriteEffects.None, 0);

			sb.Draw(star, drawPos + new Vector2(-148.5f, 0) * scale, null, starColor * MathF.Pow(outerGlowFade, 4f), MathHelper.Pi / 6f, star.Size() / 2, scale * new Vector2(0.4f, 0.2f) * MathF.Pow(outerGlowFade, 4f), SpriteEffects.None, 0);
			sb.Draw(star, drawPos + new Vector2(-148.5f, 0) * scale, null, starColor * MathF.Pow(outerGlowFade, 4f), -MathHelper.Pi / 6f, star.Size() / 2, scale * new Vector2(0.4f, 0.2f) * MathF.Pow(outerGlowFade, 4f), SpriteEffects.None, 0);
			sb.Draw(star, drawPos + new Vector2(-148.5f, 0) * scale, null, starColor * MathF.Pow(outerGlowFade, 4f), MathHelper.PiOver2, star.Size() / 2, scale * 0.4f * MathF.Pow(outerGlowFade, 4f), SpriteEffects.None, 0);
		}

		// StarEffect_Inner
		starColor = ColorDefinition.GetMissionStateColor(PoolTypeValue);
		starColor.A = 0;
		star_darkColor = ColorDefinition.GetMissionStateColor(PoolTypeValue);
		star_darkColor = new Color(star_darkColor.A, star_darkColor.A, star_darkColor.A, star_darkColor.A);
		if (!SpectrumBlockedAtInner)
		{
			sb.Draw(star_dark, drawPos + new Vector2(-94.5f, 0) * scale, null, star_darkColor * MathF.Pow(innerGlowFade, 4f), MathHelper.Pi / 6f, star_dark.Size() / 2, scale * new Vector2(0.4f, 0.2f) * MathF.Pow(innerGlowFade, 4f), SpriteEffects.None, 0);
			sb.Draw(star_dark, drawPos + new Vector2(-94.5f, 0) * scale, null, star_darkColor * MathF.Pow(innerGlowFade, 4f), -MathHelper.Pi / 6f, star_dark.Size() / 2, scale * new Vector2(0.4f, 0.2f) * MathF.Pow(innerGlowFade, 4f), SpriteEffects.None, 0);
			sb.Draw(star_dark, drawPos + new Vector2(-94.5f, 0) * scale, null, star_darkColor * MathF.Pow(innerGlowFade, 4f), MathHelper.PiOver2, star_dark.Size() / 2, scale * 0.4f * MathF.Pow(innerGlowFade, 4f), SpriteEffects.None, 0);

			sb.Draw(star, drawPos + new Vector2(-94.5f, 0) * scale, null, starColor * MathF.Pow(innerGlowFade, 4f), MathHelper.Pi / 6f, star.Size() / 2, scale * new Vector2(0.4f, 0.2f) * MathF.Pow(innerGlowFade, 4f), SpriteEffects.None, 0);
			sb.Draw(star, drawPos + new Vector2(-94.5f, 0) * scale, null, starColor * MathF.Pow(innerGlowFade, 4f), -MathHelper.Pi / 6f, star.Size() / 2, scale * new Vector2(0.4f, 0.2f) * MathF.Pow(innerGlowFade, 4f), SpriteEffects.None, 0);
			sb.Draw(star, drawPos + new Vector2(-94.5f, 0) * scale, null, starColor * MathF.Pow(innerGlowFade, 4f), MathHelper.PiOver2, star.Size() / 2, scale * 0.4f * MathF.Pow(innerGlowFade, 4f), SpriteEffects.None, 0);
		}

		if (_innerHoverTargetRotation != null)
		{
			var statusFilter_Selected_black = ModAsset.MissionDurationMarbleRing_Seleted_black.Value;
			sb.Draw(statusFilter_Selected_black, drawPos, null, Color.White, _innerRotation - _innerHoverTargetRotation.Value, statusFilter_Selected_black.Size() / 2, scale, SpriteEffects.None, 0);
			var statusFilter_Selected = ModAsset.MissionDurationMarbleRing_Seleted.Value;
			sb.Draw(statusFilter_Selected, drawPos, null, new Color(1f, 1f, 1f, 0), _innerRotation - _innerHoverTargetRotation.Value, statusFilter_Selected.Size() / 2, scale, SpriteEffects.None, 0);
			var statusFilter_Selected_glow = ModAsset.MissionDurationMarbleRing_Seleted_glow.Value;
			sb.Draw(statusFilter_Selected_glow, drawPos, null, new Color(1f, 1f, 1f, 0) * (MathF.Sin((float)(Main.timeForVisualEffects * 0.05f)) + 1) * 0.15f, _innerRotation - _innerHoverTargetRotation.Value, statusFilter_Selected_glow.Size() / 2, scale, SpriteEffects.None, 0);
		}
		if (_outerHoverTargetRotation != null)
		{
			var typeTexture_Selected_black = ModAsset.MissionClassificationMarbleRing_Selected_black.Value;
			sb.Draw(typeTexture_Selected_black, drawPos, null, Color.White, _outerRotation - _outerHoverTargetRotation.Value, typeTexture_Selected_black.Size() / 2, scale, SpriteEffects.None, 0);
			var typeTexture_Selected = ModAsset.MissionClassificationMarbleRing_Selected.Value;
			sb.Draw(typeTexture_Selected, drawPos, null, new Color(1f, 1f, 1f, 0), _outerRotation - _outerHoverTargetRotation.Value, typeTexture_Selected.Size() / 2, scale, SpriteEffects.None, 0);
			var typeTexture_Selected_glow = ModAsset.MissionClassificationMarbleRing_Selected_glow.Value;
			sb.Draw(typeTexture_Selected_glow, drawPos, null, new Color(1f, 1f, 1f, 0) * (MathF.Sin((float)(Main.timeForVisualEffects * 0.05f)) + 1) * 0.15f, _outerRotation - _outerHoverTargetRotation.Value, typeTexture_Selected_glow.Size() / 2, scale, SpriteEffects.None, 0);
		}
	}
}
