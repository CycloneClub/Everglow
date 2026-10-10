using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class CobaltClub_falling : TrailingProjectile
{
	public override string Texture => "Everglow/" + ModAsset.CobaltClub_Path;

	public override void SetCustomDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCsAndTiles;
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 120;
		Projectile.timeLeft = 140;
		Projectile.extraUpdates = 1;
		Projectile.tileCollide = true;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
		TrailTexture = Commons.ModAsset.Trail_2_thick.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_2_black.Value;
		TrailColor = new Color(0, 0.2f, 0.6f, 0f);
		TrailWidth = 15;
	}

	private bool hasHitTile = false;

	public override void OnSpawn(IEntitySource source)
	{
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		hasHitTile = true;
		Projectile.tileCollide = false;
		Projectile.position += Projectile.velocity * 2;
		Projectile.velocity *= 0;
		Projectile.timeLeft = 120;
		return false;
	}


	private int timeCounter = 0;

	public override void AI()
	{
		if (!hasHitTile)
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
		Timer++;
		if (TimeAfterEntityDestroy >= 0 && TimeAfterEntityDestroy <= 2)
		{
			Projectile.Kill();
		}

		TimeAfterEntityDestroy--;
		if (TimeAfterEntityDestroy < 0)
		{
		}
		else
		{
			Projectile.velocity *= 0f;
			return;
		}
		if (Projectile.timeLeft > 138)
		{
			Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X) - MathHelper.PiOver4 * 3;
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return base.PreDraw(player, ref lightColor);
	}

	public override void DrawSelf()
	{
		Texture2D texture = ModAsset.CobaltClub_falling.Value;

		Color lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f));

		if (Projectile.timeLeft < 120)
		{
			SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Effect dissolve = Commons.ModAsset.Dissolve.Value;
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
			float dissolveDuration = Projectile.timeLeft / 60f - 0.2f;
			if (Projectile.timeLeft > 60)
			{
				dissolveDuration = 1f;
			}
			dissolve.Parameters["uTransform"].SetValue(model * projection);
			dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_spiderNet.Value);
			dissolve.Parameters["duration"].SetValue(dissolveDuration);
			dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(0.1f, 0.1f, 0.9f, 1f));
			dissolve.Parameters["uNoiseSize"].SetValue(4f);
			dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
			dissolve.CurrentTechnique.Passes[0].Apply();

			Main.spriteBatch.Draw(texture, Projectile.Center, null, lightColor, Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}
		else
		{
			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		}
	}

	public override void DrawTrail()
	{
		base.DrawTrail();
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Projectile.friendly = false;
	}
}
