using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.WaterDeliveryHoles;

[Pipeline(typeof(WCSPipeline))]
public class WaterDeliveryHole_Slope_foreground : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;

	public float Rotation;

	public override void Update()
	{
		base.Update();
	}

	public override void Draw()
	{
		Texture2D tex = ModAsset.WaterDeliveryHole_Slope_foreground.Value;
		Vector2 offset = new Vector2(-3, 3).RotatedBy(Rotation);
		SpriteEffects effects = SpriteEffects.None;
		Ins.Batch.Draw(tex, Position + offset, null, Lighting.GetColor(Position.ToTileCoordinates()), Rotation + MathHelper.PiOver4, new Vector2(40, 28), 1f, effects);
	}
}
