using Everglow.Commons.CustomTiles;
using Everglow.Commons.CustomTiles.Abstracts;
using Everglow.Commons.CustomTiles.Core;
using Everglow.Commons.Physics.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Travelers;

/// <summary>A lightweight body; it occupies no slot in Main.npc.</summary>
internal sealed class TravelerActor : IBox
{
	private RigidEntity ground;
	private Vector2 groundPosition;

	public Entity Body { get; } = new ActorBody { width = 16, height = 48 };

	public AABB Box => new(Position, Size);

	public float Gravity => 1;

	public Vector2 Position { get => Body.position; set => Body.position = value; }

	public Vector2 Size => Body.Size;

	public Vector2 Velocity { get => Body.velocity; set => Body.velocity = value; }

	public bool Ignore(RigidEntity entity) => !entity.Active;

	public void ReleaseSupport() => ground = null;

	private sealed class ActorBody : Entity
	{
	}

	public void Move(bool fallThrough = false)
	{
		Vector2 carry = ground?.Active == true ? ground.Position - groundPosition : Vector2.Zero;
		ground = null;
		bool previousHook = ColliderManager.EnableHook;
		Vector2 stride;
		try
		{
			// Resolve vanilla terrain once, then let the CustomTile manager resolve its bodies.
			ColliderManager.EnableHook = false;
			stride = Collision.TileCollision(Position, Velocity + carry, Body.width, Body.height, fallThrough);
		}
		finally
		{
			ColliderManager.EnableHook = previousHook;
		}
		if (Math.Abs(stride.Y - Velocity.Y - carry.Y) > 0.01f)
		{
			Body.velocity.Y = 0;
		}
		foreach (var collision in ColliderManager.Instance.Move(this, stride))
		{
			if (collision.Normal.Y > 0.7f)
			{
				ground = collision.Collider;
				groundPosition = ground.Position;
				Body.velocity.Y = 0;
			}
		}
	}
}
