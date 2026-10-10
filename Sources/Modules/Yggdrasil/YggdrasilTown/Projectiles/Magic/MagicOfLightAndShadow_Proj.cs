using Everglow.Commons.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class MagicOfLightAndShadow_Proj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override string Texture => Commons.ModAsset.Empty_Mod;

	public override void SetStaticDefaults()
	{
		Item.staff[Type] = true;
	}

	public override void SetDefaults()
	{
		Projectile.timeLeft = 60;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60;
		ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1800000;
		Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCsAndTiles;
		Projectile.usesOwnerLight = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
	}

	public Vector2 MiddlePoint0;
	public Vector2 MiddlePoint1;
	public Vector2 EndPos = default;



	public override void AI()
	{
		if (Projectile.timeLeft == 59)
		{
		}
		else if (Projectile.timeLeft < 59)
		{
			var checkPos = Projectile.Center;
			var vel = Projectile.velocity.NormalizeSafe();
			float totalFade = 1f;
			if (Projectile.timeLeft < 30f)
			{
				totalFade = Projectile.timeLeft / 30f;
			}
			int penetradeTileState = 0;
			for (int i = 0; i < 1000; i++)
			{
				checkPos += vel * 16;
				Lighting.AddLight(checkPos, new Vector3(0.3f, 0.25f, 0.25f) * totalFade);
				if (Collision.SolidCollision(checkPos - new Vector2(4), 8, 8))
				{
					if (!Collision.SolidCollision(checkPos - new Vector2(4) - new Vector2(vel.X * 16, 0), 8, 8))
					{
						vel.X *= -1;
					}
					if (!Collision.SolidCollision(checkPos - new Vector2(4) - new Vector2(0, vel.Y * 16), 8, 8))
					{
						vel.Y *= -1;
					}
					penetradeTileState++;
				}
				if (penetradeTileState == 3)
				{
					break;
				}
			}
		}
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		if (MiddlePoint1 != default)
		{
			if (CollisionUtils.Intersect(target.Hitbox.Left(), target.Hitbox.Right(), target.Hitbox.Height, MiddlePoint0, MiddlePoint1, 30) && Projectile.timeLeft > 30)
			{
				modifiers.FinalDamage *= 0.5f;
				return;
			}
		}
		if (EndPos != default)
		{
			if (CollisionUtils.Intersect(target.Hitbox.Left(), target.Hitbox.Right(), target.Hitbox.Height, MiddlePoint1, EndPos, 30) && Projectile.timeLeft > 30)
			{
				modifiers.FinalDamage *= 0.25f;
				return;
			}
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (MiddlePoint0 != default)
		{
			if (CollisionUtils.Intersect(targetHitbox.Left(), targetHitbox.Right(), targetHitbox.Height, Projectile.Center, MiddlePoint0, 30) && Projectile.timeLeft > 30)
			{
				return true;
			}
		}
		else if (EndPos != default)
		{
			if (CollisionUtils.Intersect(targetHitbox.Left(), targetHitbox.Right(), targetHitbox.Height, Projectile.Center, EndPos, 30) && Projectile.timeLeft > 30)
			{
				return true;
			}
		}
		if (MiddlePoint1 != default)
		{
			if (CollisionUtils.Intersect(targetHitbox.Left(), targetHitbox.Right(), targetHitbox.Height, MiddlePoint0, MiddlePoint1, 30) && Projectile.timeLeft > 30)
			{
				return true;
			}
		}
		else if (EndPos != default)
		{
			if (CollisionUtils.Intersect(targetHitbox.Left(), targetHitbox.Right(), targetHitbox.Height, MiddlePoint0, EndPos, 30) && Projectile.timeLeft > 30)
			{
				return true;
			}
		}
		if (EndPos != default)
		{
			if (CollisionUtils.Intersect(targetHitbox.Left(), targetHitbox.Right(), targetHitbox.Height, MiddlePoint1, EndPos, 30) && Projectile.timeLeft > 30)
			{
				return true;
			}
		}
		return false;
	}

	public override bool ShouldUpdatePosition() => false;

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		var bars = new List<Vertex2D>();
		Vector2 checkPos = Projectile.Center;
		var vel = Vector2.Normalize(Projectile.velocity);
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float totalFade = 1f;
		if (Projectile.timeLeft < 30f)
		{
			totalFade = Projectile.timeLeft / 30f;
		}
		float timeValue = 0;
		var drawColor = new Color(0.8f, 0.6f, 0.3f, 0) * totalFade;
		int penetradeTileState = 0;
		for (int i = 0; i < 1000; i++)
		{
			float startFade = 1f;
			if (i < 10)
			{
				startFade = i / 10f;
			}
			checkPos += vel * 16;
			if (Collision.SolidCollision(checkPos - new Vector2(4), 8, 8))
			{
				if (penetradeTileState == 0)
				{
					MiddlePoint0 = checkPos;
				}
				if (penetradeTileState == 1)
				{
					MiddlePoint1 = checkPos;
				}
				Vector2 width2 = vel.RotatedBy(MathHelper.PiOver2) * 10;
				bars.Add(checkPos - width2 - Main.screenPosition, drawColor * startFade, new Vector3(i / 40f + timeValue, 0, 0));
				bars.Add(checkPos + width2 - Main.screenPosition, drawColor * startFade, new Vector3(i / 40f + timeValue, 1, 0));
				bars.Add(checkPos - width2 - Main.screenPosition, Color.Transparent, new Vector3(i / 40f + timeValue, 0, 0));
				bars.Add(checkPos + width2 - Main.screenPosition, Color.Transparent, new Vector3(i / 40f + timeValue, 1, 0));
				Texture2D star = Commons.ModAsset.StarSlash.Value;
				Main.spriteBatch.Draw(star, checkPos - Main.screenPosition, null, drawColor * startFade, vel.ToRotation() + MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(totalFade, 1f), SpriteEffects.None, 0);
				if (!Collision.SolidCollision(checkPos - new Vector2(4) - new Vector2(vel.X * 16, 0), 8, 8))
				{
					vel.X *= -1;
				}
				if (!Collision.SolidCollision(checkPos - new Vector2(4) - new Vector2(0, vel.Y * 16), 8, 8))
				{
					vel.Y *= -1;
				}
				if (penetradeTileState < 2)
				{
					Main.spriteBatch.Draw(star, checkPos - Main.screenPosition, null, drawColor * startFade, vel.ToRotation() + MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(totalFade, 1f), SpriteEffects.None, 0);
				}
				width2 = vel.RotatedBy(MathHelper.PiOver2) * 10;
				bars.Add(checkPos - width2 - Main.screenPosition, Color.Transparent, new Vector3(i / 40f + timeValue, 0, 0));
				bars.Add(checkPos + width2 - Main.screenPosition, Color.Transparent, new Vector3(i / 40f + timeValue, 1, 0));
				penetradeTileState++;
			}
			Vector2 width = vel.RotatedBy(MathHelper.PiOver2) * 10;
			bars.Add(checkPos - width - Main.screenPosition, drawColor * startFade, new Vector3(i / 40f + timeValue, 0, 0));
			bars.Add(checkPos + width - Main.screenPosition, drawColor * startFade, new Vector3(i / 40f + timeValue, 1, 0));
			if (penetradeTileState == 3)
			{
				break;
			}
		}
		EndPos = checkPos;
		if (bars.Count > 2)
		{
			Main.spriteBatch.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_8.Value;
			Main.spriteBatch.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}
}
