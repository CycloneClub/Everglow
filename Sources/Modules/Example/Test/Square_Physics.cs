using Everglow.Commons.Enums;
using Everglow.Commons.Physics.PBEngine;
using Everglow.Commons.Physics.PBEngine.Collision.Colliders;
using Everglow.Commons.VFX;
using Everglow.Commons.VFX.Pipelines;

namespace Everglow.Example.Test;

[Pipeline(typeof(WCSPipeline))]
public class Square_Physics : CustomPhysicsObject
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawTiles;

	public Square_Physics(Collider2D collider, RigidBody2D rigidBody)
		: base(collider, rigidBody)
	{
	}

	public override void Update()
	{
		//if(Collider is null)
		//{
		//	Active = false;
		//	return;
		//}
	}

	public override void Draw()
	{
		Vector2 pos = Position;
		Color c = Lighting.GetColor(pos.ToTileCoordinates());
		Texture2D tex = ModAsset.Square_Physics.Value;
		Ins.Batch.Draw(tex, pos, null, c, Rotation, tex.Size() * 0.5f, 1f, 0);
	}
}