using Everglow.Commons.DataStructures;
using Everglow.Commons.Graphics;
using Everglow.Yggdrasil.KelpCurtain.VFXs;
using Terraria.DataStructures;
using Terraria.Utilities;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;

public class ArcI_Current : ModProjectile
{
	public Projectile ParentProj;

	public NPC Target;

	public Vector2 TargetCenter;

	public List<Vector2> LightningWay = new List<Vector2>();

	public int Timer = 0;

	public Vector2 ToParent = Vector2.One;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 60;
		Projectile.extraUpdates = 2;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.friendly = true;
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.tileCollide = false;
		Projectile.hide = true;
	}

	public override string Texture => Commons.ModAsset.Trail_10_black_Mod;

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		overPlayers.Add(index);
	}

	public override void OnSpawn(IEntitySource source)
	{
		foreach (var proj in Main.projectile)
		{
			if (proj is not null && proj.active && proj.owner == Projectile.owner && proj.type == ModContent.ProjectileType<ArcI_proj>())
			{
				if ((proj.Center - Projectile.Center).Length() < 60)
				{
					ParentProj = proj;
					break;
				}
			}
		}
		base.OnSpawn(source);
	}

	public override void AI()
	{
		Timer++;
		if (Timer < 2)
		{
			return;
		}
		if (ParentProj is not null && ParentProj.active)
		{
			var toMuzzle = new Vector2(12, -4 * ParentProj.spriteDirection);
			toMuzzle = toMuzzle.RotatedBy(ParentProj.rotation);
			Projectile.Center = ParentProj.Center + toMuzzle;
		}
		if (Target is not null && Target.active)
		{
			TargetCenter = Target.Center;
			if (Timer == 2)
			{
				for (int k = 0; k < 12; k++)
				{
					var branch = new LightningDust_Trail
					{
						velocity = new Vector2(0, Main.rand.NextFloat(1, 10)).RotatedByRandom(Math.PI * 2),
						Active = true,
						Visible = true,
						position = TargetCenter,
						maxTime = Main.rand.Next(30, 40),
						scale = Main.rand.Next(10, 16),
						ai = new float[] { Main.rand.NextFloat(1f, 8f), Projectile.timeLeft },
					};
					Ins.VFXManager.Add(branch);
				}
				for (int k = 0; k < 60; k++)
				{
					var branch = new LightningDust_Trail
					{
						velocity = new Vector2(0, Main.rand.NextFloat(3, 6)).RotatedByRandom(Math.PI * 2),
						Active = true,
						Visible = true,
						position = TargetCenter,
						maxTime = Main.rand.Next(18, 25),
						scale = Main.rand.Next(4, 6),
						ai = new float[] { Main.rand.NextFloat(1f, 8f), Projectile.timeLeft },
					};
					Ins.VFXManager.Add(branch);
				}
			}
		}
		GenerateLightningWay();
		if (Projectile.timeLeft < 57)
		{
			Projectile.friendly = false;
		}
	}

	public void GenerateLightningWay()
	{
		LightningWay = new List<Vector2>();
		Vector2 mouseVec = Main.player[Projectile.owner].MouseWorld() - Projectile.Center;
		mouseVec = mouseVec.NormalizeSafe() * 32f;
		Vector2 cursorLightning = Projectile.Center + mouseVec;
		Vector2 totalDirection = TargetCenter - cursorLightning;
		totalDirection = totalDirection.NormalizeSafe();
		for (int k = 0; k < 200; k++)
		{
			LightningWay.Add(cursorLightning);
			Vector2 moveStep = TargetCenter - cursorLightning;
			moveStep = moveStep.NormalizeSafe() * 16;
			Vector2 randomVec = totalDirection.RotatedBy(MathHelper.PiOver2) * GetRandomValueMove(k) * Timer / 8f;
			moveStep += randomVec;
			cursorLightning += moveStep;
			if ((TargetCenter - cursorLightning).Length() < 24)
			{
				break;
			}
		}
		LightningWay.Add(cursorLightning);
		LightningWay.Add(TargetCenter);
	}

	public void UpdateLightningWay()
	{
		if (LightningWay.Count <= 2)
		{
			return;
		}
		Vector2 totalDirection = LightningWay[^1] - LightningWay[0];
		totalDirection = totalDirection.NormalizeSafe();
		for (int k = 0; k < LightningWay.Count(); k++)
		{
			LightningWay[k] += totalDirection.RotatedBy(MathHelper.PiOver2 + GetRandomValueMove(k) * 0.5f) * GetRandomValueMove(k);
		}
		if (ToParent != Vector2.One)
		{
			for (int k = 0; k < LightningWay.Count(); k++)
			{
				LightningWay[k] *= (Projectile.Center - TargetCenter) / ToParent;
			}
		}
		List<Vector2> lightning = new List<Vector2>();
		for (int k = 0; k < LightningWay.Count() - 1; k++)
		{
			lightning.Add(LightningWay[k]);
			if ((LightningWay[k] - LightningWay[k + 1]).Length() > 28)
			{
				lightning.Add((LightningWay[k] + LightningWay[k + 1]) * 0.5f);
			}
		}
		lightning.Add(LightningWay[^1]);
		LightningWay = [.. lightning];
		ToParent = Projectile.Center - TargetCenter;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (LightningWay.Count <= 2)
		{
			return false;
		}
		foreach (var pos in LightningWay)
		{
			Rectangle rectangle = new Rectangle((int)pos.X - 12, (int)pos.Y - 12, 24, 24);
			if (rectangle.Intersects(targetHitbox))
			{
				return true;
			}
		}
		return false;
	}

	public float GetRandomValueMove(float index)
	{
		float value = 0;
		for (int i = 0; i < 8; i++)
		{
			value += MathF.Sin((i + index) * MathF.Pow(2, i) * 0.5f + (float)Main.time / 264f + Projectile.ai[0]) * MathF.Pow(2, -i);
		}
		return value;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		if (LightningWay.Count <= 2)
		{
			return false;
		}
		List<Vertex2D> bars = new List<Vertex2D>();

		float width = Projectile.timeLeft / 60f;
		width = MathF.Pow(width, 2);
		float value = Timer / 60f;
		GradientColor gradientColor = new GradientColor();
		gradientColor.colorList.Add((new Color(1f, 1f, 1f, 0), 0));
		gradientColor.colorList.Add((new Color(140, 232, 255, 0), 0.2f));
		gradientColor.colorList.Add((new Color(252, 170, 255, 0), 0.4f));
		gradientColor.colorList.Add((new Color(255, 166, 114, 0), 0.6f));
		gradientColor.colorList.Add((new Color(255, 84, 81, 0), 0.8f));
		gradientColor.colorList.Add((new Color(0, 0, 0, 0), 1));
		Color drawColor = gradientColor.GetColor(value);

		for (int k = 0; k < LightningWay.Count(); k++)
		{
			Vector2 dir = Vector2.One;
			if (k < LightningWay.Count() - 1)
			{
				dir = LightningWay[k + 1] - LightningWay[k];
			}
			else
			{
				dir = LightningWay[k] - LightningWay[k - 1];
			}
			dir = dir.NormalizeSafe().RotatedBy(MathHelper.PiOver2) * 36 * width;
			Vector2 drawPos = LightningWay[k] - Main.screenPosition;
			bars.Add(drawPos + dir, drawColor, new Vector3(k / 30f, 0, 0));
			bars.Add(drawPos - dir, drawColor, new Vector3(k / 30f, 1, 0));
		}
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		if (bars.Count > 2)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_10.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		width *= 26;
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		Texture2D bloom = Commons.ModAsset.LightPoint2.Value;
		value = 1 - value;
		Vector2 scaleH = new Vector2(value, 0.5f + value * 0.5f) * MathF.Pow(width, 0.5f) / 10f;
		Main.EntitySpriteDraw(bloom, LightningWay[0] - Main.screenPosition, null, drawColor, 0, bloom.Size() * 0.5f, MathF.Pow(width, 0.2f) * 1f, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(star, LightningWay[0] - Main.screenPosition, null, drawColor, 0, star.Size() * 0.5f, scaleH, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(star, LightningWay[0] - Main.screenPosition, null, drawColor, MathHelper.PiOver2, star.Size() * 0.5f, scaleH, SpriteEffects.None, 0);

		Main.EntitySpriteDraw(bloom, LightningWay[^1] - Main.screenPosition, null, drawColor, 0, bloom.Size() * 0.5f, MathF.Pow(width, 0.2f) * 1f, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(star, LightningWay[^1] - Main.screenPosition, null, drawColor, 0, star.Size() * 0.5f, scaleH, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(star, LightningWay[^1] - Main.screenPosition, null, drawColor, MathHelper.PiOver2, star.Size() * 0.5f, scaleH, SpriteEffects.None, 0);
		return false;
	}
}
