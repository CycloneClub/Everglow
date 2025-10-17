using Everglow.Commons.Enums;
using Everglow.Commons.Interfaces;
using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Commons.VFX;
using Everglow.Commons.VFX.Pipelines;

namespace Everglow.Commons.Physics.PBEngine;

/// <summary>
/// Custom PhysicsObject, allow modifying Draw().
/// </summary>
public abstract class CustomPhysicsObject : PhysicsObject, IVisual
{
	public virtual bool Active { get; set; } = true;

	public virtual bool Visible { get; set; } = true;

	public virtual CodeLayer DrawLayer { get; }

	public CustomPhysicsObject(Collider2D collider, RigidBody2D rigidBody)
		: base(collider, rigidBody)
	{
		Collider = collider;
		RigidBody = rigidBody;
	}

	public virtual void Draw()
	{
	}

	public virtual void Kill()
	{
		Active = false;
	}

	public virtual void Update()
	{
	}
}