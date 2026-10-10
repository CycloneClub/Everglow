using Microsoft.Xna.Framework;
using Terraria;
namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Melee;

public class AcroporaThumpEff : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public override string Texture => "Terraria/Images/Projectile_0";

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.timeLeft = 40;
		Projectile.scale = 1f;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.extraUpdates = 3;
		Projectile.DamageType = DamageClass.Melee;
		ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 100;
		oldPos = new Vector2[35];
		Projectile.drawLayer = ProjectileDrawLayerID.OverPlayers;
		Projectile.usesOwnerLight = true;
	}

	private Vector2[] oldPos = new Vector2[35];

	private Vector2 vec = Vector2.Zero;

	public override void AI()
	{
		Lighting.AddLight(Projectile.Center + Projectile.velocity * (40 - Projectile.timeLeft) * 0.2f, 0.24f, 0.36f, 0f);
		Player player = Main.player[Projectile.owner];
		if (Projectile.ai[0] == 0)
		{
			if (Projectile.timeLeft > 20)
			{
				vec = player.Center + (40f - Projectile.timeLeft) * Projectile.velocity * 0.2f;
			}
			Projectile.Center = vec;
		}
		else
		{
			vec = Projectile.Center;
			Projectile.friendly = true;
		}
		if (Projectile.timeLeft > 20)
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
			for (int i = oldPos.Length - 1; i > 0; --i)
			{
				oldPos[i] = oldPos[i - 1];
			}
			oldPos[0] = (40 - Projectile.timeLeft) * Projectile.velocity;
		}
		if (Projectile.timeLeft < 20)
		{
			Projectile.friendly = false;
		}
		Projectile.position += Projectile.velocity * 6;
	}



	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		return false;
	}

	public override void PostDraw(Player player, Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		var bars = new List<Vertex2D>();
		var barsII = new List<Vertex2D>();
		var OldPoses = new List<Vector2>();
		OldPoses.Add(oldPos[0] + Projectile.velocity);
		for (int i = 1; i < oldPos.Length; ++i)
		{
			if (oldPos[i] == Vector2.Zero)
			{
				break;
			}
			else
			{
				OldPoses.Add(oldPos[i]);
			}
		}
		List<Vector2> SmoothPos = GraphicsUtils.CatmullRom(OldPoses, Math.Max(32 - OldPoses.Count, 6));
		for (int i = 1; i < SmoothPos.Count; ++i)
		{
			var normalDir = SmoothPos[i - 1] - SmoothPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));

			var factor = (i - 1) / (float)SmoothPos.Count;
			var w = MathHelper.SmoothStep(1.14514f, 0f, factor);
			float width = MathHelper.SmoothStep(0, 60, Math.Min(1, factor * 3f)) * Projectile.scale;
			if (i > SmoothPos.Count * 0.75f)
			{
				width *= (i - SmoothPos.Count) / (float)SmoothPos.Count;
			}

			float k1 = 1;
			if (Projectile.timeLeft < 20)
			{
				k1 = Projectile.timeLeft / 20f;
			}
			if (Projectile.timeLeft < 38)
			{
				if (Projectile.velocity.X < 0)
				{
					bars.Add(new Vertex2D(vec + SmoothPos[i] + normalDir * width * k1, Color.White, new Vector3((float)Math.Sqrt(factor), 0, w)));
					bars.Add(new Vertex2D(vec + SmoothPos[i] + normalDir * -width * k1, Color.White, new Vector3((float)Math.Sqrt(factor), 1, w)));
				}
				else
				{
					bars.Add(new Vertex2D(vec + SmoothPos[i] + normalDir * width * k1, Color.White, new Vector3((float)Math.Sqrt(factor), 1, w)));
					bars.Add(new Vertex2D(vec + SmoothPos[i] + normalDir * -width * k1, Color.White, new Vector3((float)Math.Sqrt(factor), 0, w)));
				}
			}
			if (Projectile.timeLeft < 38)
			{
				barsII.Add(new Vertex2D(vec + SmoothPos[i] + normalDir * width * 2, Color.White, new Vector3((float)Math.Sqrt(factor), 1, w)));
				barsII.Add(new Vertex2D(vec + SmoothPos[i] + normalDir * -width * 2, Color.White, new Vector3((float)Math.Sqrt(factor), 0, w)));
			}
		}
		Texture2D t0 = ModAsset.AcroporaSpear.Value;
		Color c0 = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16));
		Main.spriteBatch.Draw(t0, Projectile.Center - Main.screenPosition, null, c0 * (Projectile.timeLeft / 40f), Projectile.rotation + (float)(Math.PI / 4f), t0.Size() / 2f, 1, SpriteEffects.None, 0);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * (Projectile.ai[0] == 0 ? Main.GameViewMatrix.ZoomMatrix : Main.Transform);
		Effect MeleeTrail = Commons.ModAsset.MeleeTrail.Value;
		MeleeTrail.Parameters["uTransform"].SetValue(model * projection);
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Acropora_TexShade.Value;
		MeleeTrail.Parameters["tex1"].SetValue(ModAsset.Acropora_Color.Value);
		MeleeTrail.CurrentTechnique.Passes[0].Apply();
		if (bars.Count >= 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
		MeleeTrail.Parameters["uTransform"].SetValue(model * projection);
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.AcroporaLight.Value;
		MeleeTrail.Parameters["tex1"].SetValue(ModAsset.Acropora_Color.Value);
		MeleeTrail.CurrentTechnique.Passes[0].Apply();
		if (bars.Count >= 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
		MeleeTrail = Commons.ModAsset.MeleeTrailFade.Value;
		MeleeTrail.Parameters["uTransform"].SetValue(model * projection);
		float k0 = (40 - Projectile.timeLeft) / 40f;
		MeleeTrail.Parameters["FadeValue"].SetValue((k0 * 1.2f).Sqrt());
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Acropora_TexBlood.Value;
		MeleeTrail.Parameters["tex1"].SetValue(ModAsset.Acropora_Color.Value);
		MeleeTrail.CurrentTechnique.Passes[0].Apply();
		if (bars.Count >= 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, barsII.ToArray(), 0, bars.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.Poisoned, 600);
	}
}
