using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

public class Legume_Proj : TrailingProjectile
{
	public override void SetCustomDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = 6;
		Projectile.timeLeft = 3600;
		Projectile.DamageType = DamageClass.Summon;
		TrailTexture = Commons.ModAsset.Trail_8.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_8_black.Value;
		TrailLength = 12;
		TrailColor = new Color(0.25f, 0.175f, 0.15f, 0f);
		TrailBackgroundDarkness = 0.5f;
		TrailWidth = 12f;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.damage += 2;
		base.OnSpawn(source);
	}

	public override void Behaviors()
	{
		Projectile.velocity.Y += 0.5f;
		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 * 0.5f;
	}

	public override void DrawSelf()
	{
		var bullerColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		Texture2D ball = ModAsset.Legume_Proj.Value;
		Main.spriteBatch.Draw(ball, Projectile.Center - Main.screenPosition, null, bullerColor, (float)Main.time * 0.03f + Projectile.whoAmI, ball.Size() * 0.5f, 0.6f, SpriteEffects.None, 0);
		return;
	}

	public override void DestroyEntityEffect()
	{
		SoundEngine.PlaySound(SoundID.NPCHit4.WithVolumeScale(0.8f), Projectile.Center);
		for (int d = 0; d < 3; d++)
		{
			var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<Husk>());
			dust.velocity = Projectile.velocity.RotatedByRandom(MathHelper.TwoPi) * 0.3f;
			dust.scale = 1.5f;
		}
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0) => base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
	// public override void DrawTrail()
	// {
	// var unSmoothPos = new List<Vector2>();
	// for (int i = 0; i < Projectile.oldPos.Length; ++i)
	// {
	// if (Projectile.oldPos[i] == Vector2.Zero)
	// {
	// break;
	// }

	// unSmoothPos.Add(Projectile.oldPos[i]);
	// }
	// List<Vector2> smoothTrail_current = GraphicsUtils.CatmullRom(unSmoothPos); // 平滑
	// var SmoothTrail = new List<Vector2>();
	// for (int x = 0; x < smoothTrail_current.Count - 1; x++)
	// {
	// SmoothTrail.Add(smoothTrail_current[x]);
	// }
	// if (unSmoothPos.Count != 0)
	// {
	// SmoothTrail.Add(unSmoothPos[unSmoothPos.Count - 1]);
	// }

	// Vector2 halfSize = new Vector2(Projectile.width, Projectile.height) / 2f;
	// var bars = new List<Vertex2D>();
	// var bars2 = new List<Vertex2D>();
	// var bars3 = new List<Vertex2D>();
	// for (int i = 1; i < SmoothTrail.Count; ++i)
	// {
	// float mulFac = Timer / (float)ProjectileID.Sets.TrailCacheLength[Projectile.type];
	// if (mulFac > 1f)
	// {
	// mulFac = 1f;
	// }
	// float factor = i / (float)SmoothTrail.Count * mulFac;
	// float width = TrailWidthFunction(factor);
	// float timeValue = (float)Main.time * 0.0005f;

	// Vector2 drawPos = SmoothTrail[i] + halfSize;
	// Color drawC = TrailColor;
	// if (!SelfLuminous)
	// {
	// Color lightC = Lighting.GetColor((drawPos / 16f).ToPoint());
	// drawC.R = (byte)(lightC.R * drawC.R / 255f);
	// drawC.G = (byte)(lightC.G * drawC.G / 255f);
	// drawC.B = (byte)(lightC.B * drawC.B / 255f);
	// }
	// bars.Add(new Vertex2D(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 2f / 3f) * TrailWidth, drawC, new Vector3(-factor * 2 + timeValue, 1, width)));
	// bars.Add(new Vertex2D(drawPos, drawC, new Vector3(-factor * 2 + timeValue, 0.5f, width)));
	// bars2.Add(new Vertex2D(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 1f / 3f) * TrailWidth, drawC, new Vector3(-factor * 2 + timeValue, 0, width)));
	// bars2.Add(new Vertex2D(drawPos, drawC, new Vector3(-factor * 2 + timeValue, 0.5f, width)));
	// bars3.Add(new Vertex2D(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 0f / 3f) * TrailWidth, drawC, new Vector3(-factor * 2 + timeValue, 1, width)));
	// bars3.Add(new Vertex2D(drawPos, drawC, new Vector3(-factor * 2 + timeValue, 0.5f, width)));
	// }
	// SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
	// Main.spriteBatch.End();
	// Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	// Effect effect = TrailShader;
	// var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
	// var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
	// effect.Parameters["uTransform"].SetValue(model * projection);
	// effect.CurrentTechnique.Passes[0].Apply();
	// Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
	// Main.graphics.GraphicsDevice.Textures[0] = TrailTexture;
	// Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
	// if (bars.Count > 3)
	// {
	// Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
	// }

	// if (bars2.Count > 3)
	// {
	// Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
	// }

	// if (bars3.Count > 3)
	// {
	// Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars3.ToArray(), 0, bars3.Count - 2);
	// }

	// Main.spriteBatch.End();
	// Main.spriteBatch.Begin(sBS);
	// }
}
