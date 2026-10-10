using Everglow.Commons.DataStructures;
using Terraria.DataStructures;

namespace Everglow.Myth.TheTusk.Projectiles;

public class TuskBloodPool_open : ModProjectile
{
	public override string Texture => ModAsset.Empty_Mod;

	public List<Point> DissolvingTile;

	public override void SetDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCsAndTiles;
		Projectile.tileCollide = false;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.timeLeft = 180;
		Projectile.extraUpdates = 3;
	}

	public override void OnSpawn(IEntitySource source)
	{
		DissolvingTile = new List<Point>();
	}

	public override void AI()
	{
		if (Projectile.timeLeft == 180)
		{
			foreach (Point point in DissolvingTile)
			{
				if (point.X < 20 || point.X > Main.maxTilesX - 20)
				{
					Projectile.active = false;
					return;
				}
				if (point.Y < 20 || point.Y > Main.maxTilesY - 20)
				{
					Projectile.active = false;
					return;
				}
			}
		}
	}


	public override bool PreDraw(Player player, ref Color lightColor)
	{
		float timeValue = (Projectile.timeLeft - 150f) / 30f;
		timeValue = Math.Max(timeValue, 0);
		Color drawColor = new Color(0.5f + timeValue * 0.5f, 0.8f * timeValue * timeValue * timeValue, timeValue * timeValue, 1);
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, null);
		Effect dissolve = Commons.ModAsset.DissolveWithLight.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		float dissolveDuration = Projectile.timeLeft / 180f * 1.2f - 0.2f;
		dissolve.Parameters["uTransform"].SetValue(model * projection);
		dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_hiveCyber.Value);
		dissolve.Parameters["duration"].SetValue(dissolveDuration);
		dissolve.Parameters["uLightColor"].SetValue(drawColor.ToVector4());
		dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(0.3f, 0.0f, 0.0f, 0.7f));
		dissolve.Parameters["uNoiseSize"].SetValue(6f);
		dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
		dissolve.CurrentTechnique.Passes[0].Apply();
		Texture2D tileTex = Commons.ModAsset.TileBlock.Value;
		foreach (Point point in DissolvingTile)
		{
			Main.spriteBatch.Draw(tileTex, point.ToWorldCoordinates(), new Rectangle(point.X * 16, point.Y * 16, 16, 16), drawColor, 0, tileTex.Size() * 0.5f, 1, SpriteEffects.None, 0);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}
}
