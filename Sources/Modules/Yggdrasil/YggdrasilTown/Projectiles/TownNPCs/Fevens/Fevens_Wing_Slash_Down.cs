using Everglow.Commons.DataStructures;
using Everglow.Commons.Utilities.BuffHelpers;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs.Fevens;

public class Fevens_Wing_Slash_Down : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public override string Texture => Commons.ModAsset.StabbingProjectile_Mod;

	private Vector2 startCenter;
	public bool HitTile = false;

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.timeLeft = (int)Projectile.ai[0] + 121;
		startCenter = Projectile.Center;
	}

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 60;
		Projectile.timeLeft = 120;
		Projectile.extraUpdates = 2;
		ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 180000;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
	}

	public override bool PreAI()
	{
		if (Projectile.timeLeft > 120)
		{
			return false;
		}
		if (Projectile.timeLeft < 110 && Projectile.timeLeft > 60)
		{
			Projectile.hostile = true;
		}
		else
		{
			Projectile.hostile = false;
		}
		return base.PreAI();
	}

	public override bool ShouldUpdatePosition()
	{
		if (Projectile.timeLeft > 120)
		{
			return false;
		}
		return base.ShouldUpdatePosition();
	}

	public void SmashEffect()
	{
		Projectile.timeLeft = 70;
		Projectile.extraUpdates = 1;
		Vector2 checkPos = Projectile.position;
		int count = 0;
		while (Collision.SolidCollision(checkPos, Projectile.width, Projectile.height))
		{
			count++;
			if (count > 100)
			{
				break;
			}
			checkPos.Y -= 10;
		}
		Projectile.position = checkPos + new Vector2(0, -60);
		for (int g = 0; g < 16; g++)
		{
			Vector2 newVelocity = new Vector2(0, -Main.rand.NextFloat(15f, 42f)).RotatedBy(Main.rand.NextFloat(-1.57f, 1.57f));
			Vector2 pos = Projectile.Center + new Vector2(0, 100) - newVelocity * 1;
			var somg = new Fevens_WingSmash
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos,
				MaxTime = Main.rand.Next(25, 68),
				Scale = Main.rand.NextFloat(10f, 50f),
				ai = new float[] { 0, 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public void Enpower()
	{
		Projectile.velocity = new Vector2(0, 100);
	}

	public override void AI()
	{
		if (Projectile.timeLeft > 120)
		{
			return;
		}
		if (Projectile.timeLeft == 120)
		{
			Enpower();
		}
		if (!HitTile)
		{
			if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
			{
				SmashEffect();
				HitTile = true;
			}
		}
		else
		{
			Projectile.velocity *= 0;
		}
		if (Projectile.timeLeft == 40)
		{
			Vector2 backPos = Vector2.zeroVector;
			foreach (NPC npc in Main.npc)
			{
				if (npc != null && npc.active && npc.type == ModContent.NPCType<NPCs.TownNPCs.Fevens>())
				{
					if ((npc.Center - Projectile.Center).Length() < 3000)
					{
						backPos = npc.Center;
					}
				}
			}
			if (backPos != Vector2.zeroVector)
			{
				Vector2 vel = (backPos - Projectile.Center) * 0.2f;
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ModContent.ProjectileType<Fevens_Wing_Slash>(), 30, 2, default, 2);
			}
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (Projectile.timeLeft > 120)
		{
			return false;
		}
		if (CollisionUtils.Intersect(targetHitbox.Left(), targetHitbox.Right(), targetHitbox.Height, startCenter, startCenter * 0.4f + Projectile.Center * 0.6f, 2) && Projectile.timeLeft > 30)
		{
			return true;
		}
		return false;
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		target.AddBuff(ModContent.BuffType<ShortImmune12>(), 10);
		base.OnHitPlayer(target, info);
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		if (Projectile.timeLeft > 120)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			Vector2 start = Projectile.Center + new Vector2(0, -400);
			Vector2 end = start;
			int count = 0;
			while (!Collision.SolidCollision(end, 4, 4))
			{
				count++;
				if (count > 1000)
				{
					break;
				}
				end.Y += 10;
			}
			var predictColor = new Color(0f, 0.001f, 0.6f, 0f);
			var linePredict = new List<Vertex2D>
			{
				new Vertex2D(start + new Vector2(-10, 0) - Main.screenPosition, predictColor, new Vector3(0.4f, 0.5f, 0)),
				new Vertex2D(start + new Vector2(10, 0) - Main.screenPosition, predictColor, new Vector3(0.6f, 0.5f, 0)),
				new Vertex2D(end + new Vector2(-10, 0) - Main.screenPosition, predictColor, new Vector3(0.4f, 0.5f, 0)),
				new Vertex2D(end + new Vector2(10, 0) - Main.screenPosition, predictColor, new Vector3(0.6f, 0.5f, 0)),
			};
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.StarSlash.Value;
			if (linePredict.Count > 3)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, linePredict.ToArray(), 0, linePredict.Count - 2);
			}

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
			return false;
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float value0 = (120 - Projectile.timeLeft) / 120f;
		float value1 = MathF.Pow(value0, 0.5f);
		float width = (1 - MathF.Cos(value1 * 2f * MathF.PI)) * 18f;
		var normalizedVelocity = new Vector2(0, 1);
		Vector2 normalize = normalizedVelocity.RotatedBy(Math.PI / 2d) * width;
		Color shadow = Color.White * 0.4f;
		float endCoordY = 1f;
		if (HitTile)
		{
			endCoordY = 0.5f;
		}
		var bars = new List<Vertex2D>
		{
			new Vertex2D(startCenter + normalize - Main.screenPosition, shadow, new Vector3(0, 0, 0)),
			new Vertex2D(startCenter - normalize - Main.screenPosition, shadow, new Vector3(0, 1, 0)),
			new Vertex2D(Projectile.Center + normalize - Main.screenPosition, shadow, new Vector3(endCoordY, 0, 0)),
			new Vertex2D(Projectile.Center - normalize - Main.screenPosition, shadow, new Vector3(endCoordY, 1, 0)),
		};
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Star2_black.Value;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		Color light = Color.Lerp(new Color(1f, 1f, 1f, 0), new Color(0f, 0.3f, 1f, 0), value0 * 3) * width;
		if (value0 > 0.5f)
		{
			light = Color.Lerp(light, new Color(0f, 0.02f, 0.2f, 0), value0 * 3) * width;
		}
		light *= width / 10f;
		normalize *= width * width / 45f;
		bars = new List<Vertex2D>()
		{
			new Vertex2D(startCenter + normalize - Main.screenPosition, light, new Vector3(0, 0, 0)),
			new Vertex2D(startCenter - normalize - Main.screenPosition, light, new Vector3(0, 1, 0)),
			new Vertex2D(Projectile.Center + normalize - Main.screenPosition, light, new Vector3(endCoordY, 0, 0)),
			new Vertex2D(Projectile.Center - normalize - Main.screenPosition, light, new Vector3(endCoordY, 1, 0)),
		};
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Textures_Star.Value;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public void DrawWarp(VFXBatch sb)
	{
		if (Projectile.timeLeft > 120)
		{
			return;
		}
		float time = (float)(Main.time * 0.03);
		float value0 = (120 - Projectile.timeLeft) / 120f;
		float value1 = MathF.Pow(value0, 0.5f);
		float width = (1 - MathF.Cos(value1 * 2f * MathF.PI)) * 5f;
		Vector2 normalizedVelocity = Projectile.velocity.SafeNormalize(Vector2.zeroVector);
		Vector2 normalize = normalizedVelocity.RotatedBy(Math.PI / 2d) * width;
		Vector2 start = startCenter - Main.screenPosition;
		Vector2 end = Projectile.Center - Main.screenPosition;
		var middle = Vector2.Lerp(start, end, 0.5f);
		float rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
		Color alphaColor = Color.White;
		alphaColor.A = 0;
		alphaColor.R = (byte)((rotation + Math.PI) % 6.283 / 6.283 * 255);
		alphaColor.G = 5;
		var bars = new List<Vertex2D>
		{
			new Vertex2D(start - normalize, new Color(alphaColor.R, alphaColor.G / 9, 0, 0), new Vector3(1 + time, 0.3f, 0)),
			new Vertex2D(start + normalize, new Color(alphaColor.R, alphaColor.G / 9, 0, 0), new Vector3(1 + time, 0.7f, 0)),
			new Vertex2D(middle - normalize, new Color(alphaColor.R, alphaColor.G / 3, 0, 0), new Vector3(0.5f + time, 0.3f, 0.5f)),
			new Vertex2D(middle + normalize, new Color(alphaColor.R, alphaColor.G / 3, 0, 0), new Vector3(0.5f + time, 0.7f, 0.5f)),
			new Vertex2D(end, alphaColor, new Vector3(0f + time, 0.5f, 1)),
			new Vertex2D(end, alphaColor, new Vector3(0f + time, 0.5f, 1)),
		};
		sb.Draw(Commons.ModAsset.Trail_1.Value, bars, PrimitiveType.TriangleStrip);
	}
}
