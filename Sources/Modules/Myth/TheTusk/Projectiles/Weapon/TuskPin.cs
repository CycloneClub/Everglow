using Everglow.Commons.DataStructures;
using Everglow.Myth.Misc.Dusts;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.TheTusk.Projectiles.Weapon;

public class TuskPin : ModProjectile
{
	public override void OnSpawn(IEntitySource source)
	{
		Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X) - MathHelper.PiOver2;
	}

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 120;
		Projectile.timeLeft = 120;
		Projectile.extraUpdates = 1;
		Projectile.tileCollide = true;
	}

	private bool HasHitTile = false;

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		HasHitTile = true;
		Projectile.tileCollide = false;
		Projectile.position += Projectile.velocity * 2;
		Projectile.velocity *= 0;
		Projectile.timeLeft = 30;
		return false;
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCsAndTiles.Add(index);
		base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
	}

	private int timeCounter = 0;

	public override void AI()
	{
		Projectile.hide = true;
		if (!HasHitTile)
		{
			if (Projectile.timeLeft < 60)
			{
				timeCounter++;
				if (timeCounter < 200)
				{
					Projectile.timeLeft = 50;
				}
				Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 45;
			}
		}
		base.AI();
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = ModAsset.TuskPin.Value;
		Texture2D textureWhite = ModAsset.TuskPinWhite.Value;
		Texture2D textureBlack = ModAsset.TuskPinDark.Value;

		if (Projectile.timeLeft < 65)
		{
			SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Effect dissolve = Commons.ModAsset.Dissolve.Value;
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
			float dissolveDuration = Projectile.timeLeft / 30f - 0.2f;
			if (Projectile.timeLeft > 30)
			{
				dissolveDuration = 1f;
			}
			dissolve.Parameters["uTransform"].SetValue(model * projection);
			dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_spiderNet.Value);
			dissolve.Parameters["duration"].SetValue(dissolveDuration);
			dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(0.5f, 0, 0.2f, 1f));
			dissolve.Parameters["uNoiseSize"].SetValue(4f);
			dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
			dissolve.CurrentTechnique.Passes[0].Apply();

			Main.spriteBatch.Draw(texture, Projectile.Center, null, lightColor, Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}
		else
		{
			if (Projectile.timeLeft < 85)
			{
				Main.EntitySpriteDraw(textureBlack, Projectile.Center - Main.screenPosition, null, Color.White * (Math.Min(1, (85 - Projectile.timeLeft) / 5f)), Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None);
			}
			if (Projectile.timeLeft < 80 && Projectile.timeLeft > 75)
			{
				Main.EntitySpriteDraw(textureWhite, Projectile.Center - Main.screenPosition, null, lightColor * Math.Min(1, (80 - Projectile.timeLeft) / 5f), Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None);
			}
			if (Projectile.timeLeft < 75 && Projectile.timeLeft > 70)
			{
				Main.EntitySpriteDraw(textureWhite, Projectile.Center - Main.screenPosition, null, Color.Lerp(lightColor, Color.Red, Math.Min(1, (75 - Projectile.timeLeft) / 5f)), Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None);
			}
			if (Projectile.timeLeft < 70 && Projectile.timeLeft > 65)
			{
				Main.EntitySpriteDraw(textureWhite, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.Red, Color.Transparent, Math.Min(1, (70 - Projectile.timeLeft) / 5f)), Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None);
			}
		}

		return false;
	}
}
