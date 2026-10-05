using System.Net;
using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class RockQuake_Proj : ModProjectile, IWarpProjectile_warpStyle2
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override string Texture => Commons.ModAsset.Empty_Mod;

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
		Projectile.hide = true;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60;
		ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1800000;
	}

	public override void OnSpawn(IEntitySource source)
	{
	}

	public Vector2 StartHit = default;
	public Vector2 EndHit = default;
	public Vector2 EndPos = default;

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCsAndTiles.Add(index);
		base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
	}

	public override void AI()
	{
		if (Projectile.timeLeft == 59)
		{
			ShakerManager.AddShaker(StartHit, Projectile.velocity.NormalizeSafe(), 10, 120, 100, 0.9f, 0.9f, 120);
			var checkPos = StartHit;
			var vel = Projectile.velocity.NormalizeSafe();
			int penetradeTileState = 0;
			for (int i = 0; i < 1000; i++)
			{
				checkPos += vel * 16;
				if (Collision.SolidCollision(checkPos - new Vector2(4), 8, 8) && penetradeTileState == 0)
				{
					StartHit = checkPos;
					penetradeTileState = 1;
				}
				if (!Collision.SolidCollision(checkPos - new Vector2(4), 8, 8) && penetradeTileState == 1)
				{
					EndHit = checkPos;
					penetradeTileState = 2;
				}
				if (penetradeTileState == 1)
				{
					Collision.HitTiles(checkPos - new Vector2(8), Projectile.velocity, 16, 16);
					float addRot = MathHelper.PiOver2 + Main.rand.NextFloat(-0.4f, 0.4f);
					if (i % 2 == 0)
					{
						addRot *= -1;
					}
					float rot = vel.ToRotation() + addRot;
					var dustVFX = new RockQuakeCone
					{
						Active = true,
						Visible = true,
						Position = checkPos + new Vector2(Main.rand.Next(8, 30), 0).RotatedBy(rot),
						Rotation = rot,
						MaxTime = Main.rand.Next(70, 120),
						Scale = Main.rand.Next(8, 17),
						ai = new float[] { Main.rand.NextFloat(1f, 8f) },
					};
					Ins.VFXManager.Add(dustVFX);
				}
				if (penetradeTileState == 2)
				{
					break;
				}
			}
		}
		else if (Projectile.timeLeft < 59)
		{
			var checkPos = StartHit;
			var vel = Projectile.velocity.NormalizeSafe();
			float totalFade = 1f;
			if (Projectile.timeLeft < 30f)
			{
				totalFade = Projectile.timeLeft / 30f;
			}
			for (int i = 0; i < 1000; i++)
			{
				checkPos += vel * 16;
				Lighting.AddLight(checkPos, new Vector3(0.3f, 0.25f, 0.25f) * totalFade);
				if ((checkPos - EndHit).Length() < 16f)
				{
					break;
				}
			}
		}
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		if (StartHit != default && EndHit != default)
		{
			if (CollisionUtils.Intersect(target.Hitbox.Left(), target.Hitbox.Right(), target.Hitbox.Height, StartHit, EndHit, 120) && Projectile.timeLeft > 30)
			{
				modifiers.FinalDamage *= 3;
			}
			else
			{
				modifiers.FinalDamage *= 0.5f;
			}
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (EndPos != default)
		{
			if (CollisionUtils.Intersect(targetHitbox.Left(), targetHitbox.Right(), targetHitbox.Height, Projectile.Center, EndPos, 30) && Projectile.timeLeft > 30)
			{
				return true;
			}
			if (CollisionUtils.Intersect(targetHitbox.Left(), targetHitbox.Right(), targetHitbox.Height, StartHit, EndHit, 120) && Projectile.timeLeft > 30)
			{
				return true;
			}
		}
		return false;
	}

	public override bool ShouldUpdatePosition() => false;

	public override bool PreDraw(ref Color lightColor)
	{
		var bars = new List<Vertex2D>();
		Vector2 checkPos = Projectile.Center;
		var vel = Vector2.Normalize(Projectile.velocity);
		Vector2 width = vel.RotatedBy(MathHelper.PiOver2) * 30;
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float totalFade = 1f;
		if (Projectile.timeLeft < 30f)
		{
			totalFade = Projectile.timeLeft / 30f;
		}
		float timeValue = (float)Main.time * 0.01f;
		var drawColor = new Color(0.4f, 0.3f, 0.3f, 0) * totalFade;
		int penetradeTileState = 0;
		for (int i = 0; i < 1000; i++)
		{
			checkPos += vel * 16;
			if (Collision.SolidCollision(checkPos - new Vector2(4), 8, 8) && penetradeTileState == 0)
			{
				StartHit = checkPos;
				penetradeTileState = 1;
			}
			if (!Collision.SolidCollision(checkPos - new Vector2(4), 8, 8) && penetradeTileState == 1)
			{
				EndHit = checkPos;
				penetradeTileState = 2;
			}
			float startFade = 1f;
			if (i < 10)
			{
				startFade = i / 10f;
			}
			bars.Add(checkPos - width - Main.screenPosition, drawColor * startFade, new Vector3(i / 40f + timeValue, 0, 0));
			bars.Add(checkPos + width - Main.screenPosition, drawColor * startFade, new Vector3(i / 40f + timeValue, 1, 0));
			if (penetradeTileState == 2)
			{
				for (int j = 0; j < 10; j++)
				{
					checkPos += vel * 16;
					float fade = (9 - j) / 10f;
					bars.Add(checkPos - width - Main.screenPosition, drawColor * fade, new Vector3((i + j) / 40f + timeValue, 0, 0));
					bars.Add(checkPos + width - Main.screenPosition, drawColor * fade, new Vector3((i + j) / 40f + timeValue, 1, 0));
				}
				EndPos = checkPos;
				break;
			}
		}
		if (bars.Count > 2)
		{
			Main.spriteBatch.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_8.Value;
			Main.spriteBatch.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		var bars = new List<Vertex2D>();
		Vector2 checkPos = Projectile.Center;
		var vel = Vector2.Normalize(Projectile.velocity);
		Vector2 width = vel.RotatedBy(MathHelper.PiOver2) * 30;
		float timeValue = (float)Main.time * 0.03f;
		float totalFade = 1f;
		if (Projectile.timeLeft < 30f)
		{
			totalFade = Projectile.timeLeft / 30f;
		}

		int penetradeTileState = 0;
		for (int i = 0; i < 1000; i++)
		{
			checkPos += vel * 16;
			if (Collision.SolidCollision(checkPos - new Vector2(4), 8, 8) && penetradeTileState == 0)
			{
				penetradeTileState = 1;
			}
			if (!Collision.SolidCollision(checkPos - new Vector2(4), 8, 8) && penetradeTileState == 1)
			{
				penetradeTileState = 2;
			}
			float startFade = 1f;
			if (i < 10)
			{
				startFade = i / 10f;
			}
			var drawColor = new Color(-vel.X / 2f + 0.5f, -vel.Y / 2f + 0.5f, totalFade * startFade, 0);
			bars.Add(checkPos - width - Main.screenPosition, drawColor, new Vector3(i / 40f + timeValue, 0, 1));
			bars.Add(checkPos + width - Main.screenPosition, drawColor, new Vector3(i / 40f + timeValue, 1, 1));
			if (penetradeTileState == 2)
			{
				for (int j = 0; j < 10; j++)
				{
					checkPos += vel * 16;
					float fade = (9 - j) / 10f;
					drawColor = new Color(-vel.X / 2f + 0.5f, -vel.Y / 2f + 0.5f, totalFade * startFade * fade, 0);
					bars.Add(checkPos - width - Main.screenPosition, drawColor, new Vector3((i + j) / 40f + timeValue, 0, 1));
					bars.Add(checkPos + width - Main.screenPosition, drawColor, new Vector3((i + j) / 40f + timeValue, 1, 1));
				}
				break;
			}
		}
		if (bars.Count > 2)
		{
			Main.graphics.graphicsDevice.RasterizerState = RasterizerState.CullNone;
			spriteBatch.Draw(Commons.ModAsset.Trail_12.Value, bars, PrimitiveType.TriangleStrip);
		}
	}
}
