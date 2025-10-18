using Everglow.Commons.Enums;
using Everglow.Commons.Physics.PBEngine;
using Everglow.Commons.VFX;
using Everglow.Commons.VFX.Pipelines;

namespace Everglow.Commons.Physics;

[Pipeline(typeof(WCSPipeline))]
public class VisualPhysicsUnit : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawTiles;

	public PhysicsObject physicsObject;

	public int Timer = 0;

	public delegate void VPU_Draw(VisualPhysicsUnit vpu);

	public event VPU_Draw CustomDraw;

	public delegate void VPU_Update(VisualPhysicsUnit vpu);

	public event VPU_Update CustomUpdate;

	public override void OnSpawn()
	{
		base.OnSpawn();
	}

	public override void Update()
	{
		if (physicsObject is null || !physicsObject.IsActive)
		{
			Active = false;
			return;
		}
		Timer++;
		CustomUpdate?.Invoke(this);
	}

	public override void Draw()
	{
		if (physicsObject is null || !physicsObject.IsActive)
		{
			Active = false;
			return;
		}
		CustomDraw?.Invoke(this);
	}

	public override void Kill()
	{
		physicsObject.IsActive = false;
		base.Kill();
	}

	public void RegisterCustomCode(VPU_Draw vpu_draw, VPU_Update vpu_update)
	{
		CustomDraw += vpu_draw;
		CustomUpdate += vpu_update;
	}

	public void UnregisterCustomCode(VPU_Draw vpu_draw, VPU_Update vpu_update)
	{
		CustomDraw -= vpu_draw;
		CustomUpdate -= vpu_update;
	}
}