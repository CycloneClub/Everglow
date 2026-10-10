using Everglow.Commons.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using static Everglow.Yggdrasil.CorruptWormHive.Projectiles.Melee.TrueDeathSickle.TrueDeathSickle_Blade;

namespace Everglow.Yggdrasil.CorruptWormHive.Projectiles.Melee.TrueDeathSickle;

public class TrueDeathSickle_SickleBack : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public override string Texture => ModAsset.TrueDeathSickleHit_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 60;
		Projectile.extraUpdates = 0;
		Projectile.noEnchantmentVisuals = true;
		Projectile.localNPCHitCooldown = 60;
		Projectile.usesLocalNPCImmunity = true;
	}

	public Vector3 SpacePos;
	public Vector3 RotatedAxis;

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.ai[1] += MathHelper.Pi * 20;
		Projectile.ai[1] %= MathHelper.TwoPi;
	}

	public override bool ShouldUpdatePosition()
	{
		return false;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.Center = player.MountedCenter + new Vector2(0, -10);
		RotatedAxis = Vector3.Lerp(RotatedAxis, new Vector3(0, 0, -1), 0.1f);
		float aimRot = 5.3f;
		if (Projectile.spriteDirection == 1)
		{
			aimRot = 1.3f;
		}
		Projectile.ai[1] = MathHelper.Lerp(Projectile.ai[1], aimRot, 0.1f);
		Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 80, 0.1f);
		foreach (Projectile projectile in Main.projectile)
		{
			if (projectile.active && (projectile.type == ModContent.ProjectileType<TrueDeathSickle_Blade>() || projectile.type == ModContent.ProjectileType<TrueDeathSickle_RightClick>()))
			{
				if (projectile.timeLeft > Projectile.timeLeft)
				{
					Projectile.Kill();
					return;
				}
			}
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect dissolve = Commons.ModAsset.Dissolve.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(0)) * Main.GameViewMatrix.TransformationMatrix;
		float dissolveDuration = Projectile.timeLeft / 40f - 0.2f;
		dissolve.Parameters["uTransform"].SetValue(model * projection);
		dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_spiderNet.Value);
		dissolve.Parameters["duration"].SetValue(dissolveDuration);
		dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(0.2f, 0.1f, 0.4f, 1f));
		dissolve.Parameters["uNoiseSize"].SetValue(4f);
		dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
		dissolve.CurrentTechnique.Passes[0].Apply();

		DrawSickle();
		DrawSickle(1, 0, true);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public void DrawSickle(float fade = 1, float addRotation = 0, bool glowMask = false)
	{
		var bars = new List<Vertex2D>();
		float scaleValue;

		// Get the normal of rotated axis and Z axis.
		var normalOfRotatedAxisAndZ = Vector3.Normalize(Vector3.Cross(new Vector3(0, 0, -1), RotatedAxis));
		float angleOfZAndRotatedAxis = GetAngleBetweenVectors(new Vector3(0, 0, -1), RotatedAxis);

		// Texture coords
		var texCoordsOrig = new List<Vector3>
		{
			new Vector3(155, 58, 0),
			new Vector3(42, 174, 0),
			new Vector3(6, 174, 0),
			new Vector3(7, 13, 0),
			new Vector3(127, 15, 0),
		};

		// Roteted texcoords to get space coords.
		// Then rotated the space coords to sickel rotation.
		float sickleRot = Projectile.ai[1];

		// GetAngleBetweenVectors(StartSpacePos, SpacePos);
		sickleRot += MathHelper.Pi / 5f - MathHelper.Pi / 3f * Projectile.spriteDirection;
		sickleRot += addRotation;

		// This is a magic value that coorded in the handle, the rotated center of the sickle.
		var unrotatedCoord1 = new Vector3(34, 132, 0);
		if (Projectile.spriteDirection == 1)
		{
			(unrotatedCoord1.Y, unrotatedCoord1.X) = (unrotatedCoord1.X, unrotatedCoord1.Y);
		}
		var texCoordsRotated = new List<Vector3>();
		for (int i = 0; i < texCoordsOrig.Count; i++)
		{
			Vector3 unrotatedCoord = texCoordsOrig[i];
			if (Projectile.spriteDirection == 1)
			{
				(unrotatedCoord.Y, unrotatedCoord.X) = (unrotatedCoord.X, unrotatedCoord.Y);
			}
			Vector3 firstSpaceCoord = RodriguesRotate(unrotatedCoord, normalOfRotatedAxisAndZ, -angleOfZAndRotatedAxis);
			firstSpaceCoord -= RodriguesRotate(unrotatedCoord1, normalOfRotatedAxisAndZ, -angleOfZAndRotatedAxis);
			firstSpaceCoord.Y += 8;
			texCoordsRotated.Add(RodriguesRotate(firstSpaceCoord, RotatedAxis, sickleRot));
		}

		var texCoordsProjected = new List<Vector2>();
		for (int i = 0; i < texCoordsRotated.Count; i++)
		{
			// Set the sickle size by ai[0].
			texCoordsProjected.Add(Projection2D(texCoordsRotated[i], Vector2.zeroVector, 500, out scaleValue) * Projectile.ai[0] / 130f/* + sickleTip - projectedSickleTip*/);
		}
		float size = 174f;
		if (!glowMask)
		{
			AddVertexWCS(bars, texCoordsProjected[0] + Projectile.Center, texCoordsOrig[0] / size, fade);
			AddVertexWCS(bars, texCoordsProjected[1] + Projectile.Center, texCoordsOrig[1] / size, fade);
			AddVertexWCS(bars, texCoordsProjected[4] + Projectile.Center, texCoordsOrig[4] / size, fade);

			AddVertexWCS(bars, texCoordsProjected[4] + Projectile.Center, texCoordsOrig[4] / size, fade);
			AddVertexWCS(bars, texCoordsProjected[1] + Projectile.Center, texCoordsOrig[1] / size, fade);
			AddVertexWCS(bars, texCoordsProjected[3] + Projectile.Center, texCoordsOrig[3] / size, fade);

			AddVertexWCS(bars, texCoordsProjected[3] + Projectile.Center, texCoordsOrig[3] / size, fade);
			AddVertexWCS(bars, texCoordsProjected[1] + Projectile.Center, texCoordsOrig[1] / size, fade);
			AddVertexWCS(bars, texCoordsProjected[2] + Projectile.Center, texCoordsOrig[2] / size, fade);
		}
		else
		{
			Color color = new Color(1f, 1f, 1f, 0) * fade;
			Vector2 offset = Projectile.Center - Main.screenPosition;
			bars.Add(texCoordsProjected[0] + offset, color, texCoordsOrig[0] / size);
			bars.Add(texCoordsProjected[1] + offset, color, texCoordsOrig[1] / size);
			bars.Add(texCoordsProjected[4] + offset, color, texCoordsOrig[4] / size);

			bars.Add(texCoordsProjected[4] + offset, color, texCoordsOrig[4] / size);
			bars.Add(texCoordsProjected[1] + offset, color, texCoordsOrig[1] / size);
			bars.Add(texCoordsProjected[3] + offset, color, texCoordsOrig[3] / size);

			bars.Add(texCoordsProjected[3] + offset, color, texCoordsOrig[3] / size);
			bars.Add(texCoordsProjected[1] + offset, color, texCoordsOrig[1] / size);
			bars.Add(texCoordsProjected[2] + offset, color, texCoordsOrig[2] / size);
		}
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.TrueDeathSickle_proj.Value;
		if (glowMask)
		{
			Main.graphics.GraphicsDevice.Textures[0] = ModAsset.TrueDeathSickle_proj_glow.Value;
		}
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, bars.ToArray(), 0, bars.Count / 3);
		}
	}
}
