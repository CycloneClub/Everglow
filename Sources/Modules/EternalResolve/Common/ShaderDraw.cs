using Everglow.Commons.VFX;
using Everglow.Commons.Enums;
namespace Everglow.EternalResolve.Common;

public abstract class ShaderDraw : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;
	public Vector2 position;
	public Vector2 velocity;
	public float[] ai;
	public ShaderDraw() { }
	public ShaderDraw(Vector2 position, Vector2 velocity, params float[] ai)
	{
		this.position = position;
		this.velocity = velocity;
		this.ai = ai; // 可以认为params传入的都是右值，可以直接引用
	}
}
