using Everglow.Commons.Enums;
using Everglow.Commons.VFX;
using Everglow.Commons.VFX.Pipelines;

namespace Everglow.Commons.MEAC.VFX;

[Pipeline(typeof(WCSPipeline_PointWrap))]
public class MeleeProj_3D_Dust : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector3 Position_Space;

	public Queue<Vector3> OldPos_Space;

	public Vector3 RotAxis;

	public MeleeProj_3D ParentProj;

	public float Rotation;

	public float RotSpeed;

	public float Scale;

	public float Timer;

	public float MaxTime;

	public float[] ai;

	public int EnchantmentType = 0;

	public bool SelfLuminous;

	public delegate void CustomDraw(MeleeProj_3D_Dust dust);

	public event CustomDraw DustDraw;

	public delegate void CustomBehavior(MeleeProj_3D_Dust dust);

	public event CustomBehavior DustBehavior;

	public override void Update()
	{
		Timer++;
		MeleeProj_3D.RotateMainAxis(RotSpeed, RotAxis, ref Position_Space);
		DustBehavior?.Invoke(this);
		if (Timer > MaxTime)
		{
			Active = false;
			return;
		}
	}

	public override void Draw()
	{
		DustDraw?.Invoke(this);
	}

	public override void Kill()
	{
		UnregisterBehavior(DustBehavior);
		UnregisterDraw(DustDraw);
		base.Kill();
	}

	/// <summary>
	/// Allow you register a custom logic.
	/// </summary>
	/// <param name="behavoir">Callback to invoke when this dust updates.</param>
	public void RegisterBehavior(CustomBehavior behavoir)
	{
		DustBehavior += behavoir;
	}

	public void RegisterDraw(CustomDraw draw)
	{
		DustDraw += draw;
	}

	/// <summary>
	/// Unregister a custom logic.
	/// </summary>
	/// <param name="behavoir">Previously registered update callback to remove.</param>
	public void UnregisterBehavior(CustomBehavior behavoir)
	{
		DustBehavior -= behavoir;
	}

	public void UnregisterDraw(CustomDraw draw)
	{
		DustDraw -= draw;
	}
}
