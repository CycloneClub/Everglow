using Everglow.Commons.DataStructures;
using Everglow.Commons.Graphics;
using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class HexaCrystalStaff_ProjExplosion : ModProjectile, IWarpProjectile_warpStyle2
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override string Texture => Commons.ModAsset.Empty_Mod;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 120;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.extraUpdates = 4;
		Timer = 0;
	}

	public int Timer = 0;

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		return false;
	}

	public override void OnSpawn(IEntitySource source)
	{
		foreach (var npc in Main.npc)
		{
			if (npc is not null && npc.active)
			{
				if (!npc.dontTakeDamage && !npc.friendly)
				{
					if ((npc.Center - Projectile.Center).Length() < 100)
					{
						npc.AddBuff(ModContent.BuffType<HexaCrystalWeak>(), 30);
						break;
					}
				}
			}
		}
	}

	public override void AI()
	{
		Timer++;
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		modifiers._combatTextHidden = true;
		modifiers.FinalDamage.Flat *= 0;
		base.ModifyHitNPC(target, ref modifiers);
	}

	public override void OnKill(int timeLeft)
	{
		base.OnKill(timeLeft);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D drawTex = Commons.ModAsset.Noise_turtleCrack.Value;
		var drawPos = Projectile.Center - Main.screenPosition;
		float value = MathF.Pow(Timer, 0.5f) * 10 + 30;
		var gradientColor = new GradientColor();
		gradientColor.colorList.Add((new Color(1f, 1f, 1f, 0), 0));
		gradientColor.colorList.Add((new Color(89, 194, 255, 0), 0.2f));
		gradientColor.colorList.Add((new Color(35, 116, 255, 0), 0.4f));
		gradientColor.colorList.Add((new Color(3, 100, 109, 0), 0.6f));
		gradientColor.colorList.Add((new Color(42, 42, 109, 0), 0.8f));
		gradientColor.colorList.Add((new Color(0, 0, 0, 0), 1f));
		var hexagon = new List<Vertex2D>();
		var hexagonSide = new List<Vertex2D>();
		var hexagonSideDark = new List<Vertex2D>();
		var hexagonSideRainbow = new List<Vertex2D>();
		float sideWidth = 30f - Timer / 4f;
		float colorValue = Timer / 120f;
		float rainBowFade = 0f;
		if (Timer < 30f)
		{
			rainBowFade = (30 - Timer) / 15f;
		}
		for (int i = 0; i <= 6; i++)
		{
			hexagonSideDark.Add(drawPos + new Vector2(value, 0).RotatedBy(i / 6f * MathHelper.TwoPi + Projectile.rotation), Color.White, new Vector3(i / 2f, 0.5f, 0));
			hexagonSideDark.Add(drawPos + new Vector2(value - sideWidth, 0).RotatedBy(i / 6f * MathHelper.TwoPi + Projectile.rotation), Color.White * 0.7f, new Vector3(i / 2f, 0, 0));

			hexagon.Add(drawPos + new Vector2(value, 0).RotatedBy(i / 6f * MathHelper.TwoPi + Projectile.rotation), gradientColor.GetColor(colorValue), new Vector3(i / 6f, 0.5f + Timer / 120f, 0));
			hexagon.Add(drawPos + new Vector2(value - 80, 0).RotatedBy(i / 6f * MathHelper.TwoPi + Projectile.rotation), gradientColor.GetColor(colorValue) * 0, new Vector3(i / 6f, Timer / 120f, 0));

			hexagonSide.Add(drawPos + new Vector2(value, 0).RotatedBy(i / 6f * MathHelper.TwoPi + Projectile.rotation), gradientColor.GetColor(MathF.Pow(colorValue, 1.5f)), new Vector3(i / 2f, 0.5f, 0));
			hexagonSide.Add(drawPos + new Vector2(value - sideWidth, 0).RotatedBy(i / 6f * MathHelper.TwoPi + Projectile.rotation), Color.Transparent, new Vector3(i / 2f, 0, 0));

			hexagonSideRainbow.Add(drawPos + new Vector2(value, 0).RotatedBy(i / 6f * MathHelper.TwoPi + Projectile.rotation), new Color(0.15f, 0.3f, 1f, 0) * rainBowFade, new Vector3(i / 6f, 0.75f, 0));
			hexagonSideRainbow.Add(drawPos + new Vector2(0, 0).RotatedBy(i / 6f * MathHelper.TwoPi + Projectile.rotation), new Color(0.15f, 0.3f, 1f, 0) * rainBowFade, new Vector3(i / 6f, 0, 0));
		}
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		if (hexagonSideDark.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Melee_Black.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, hexagonSideDark.ToArray(), 0, hexagonSideDark.Count - 2);
		}

		if (hexagonSideRainbow.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_SolarSpectrum.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, hexagonSideRainbow.ToArray(), 0, hexagonSideRainbow.Count - 2);
		}

		if (hexagon.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = drawTex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, hexagon.ToArray(), 0, hexagon.Count - 2);
		}
		if (hexagonSide.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Melee.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, hexagonSide.ToArray(), 0, hexagonSide.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public void DrawWarp(VFXBatch batch)
	{
		var hexagon = new List<Vertex2D>();
		float value = MathF.Pow(Timer, 0.5f) * 10 + 26;
		var drawPos = Projectile.Center - Main.screenPosition;
		for (int i = 0; i <= 6; i++)
		{
			Vector2 dir = new Vector2(0.5f, 0).RotatedBy(i / 6f * MathHelper.TwoPi + Projectile.rotation);
			var warpColor = new Color(dir.X + 0.5f, dir.Y + 0.5f, Projectile.timeLeft / 1200f, 1);
			var warpColorInner = new Color(dir.X + 0.5f, dir.Y + 0.5f, Projectile.timeLeft / 1200f, 1);
			hexagon.Add(drawPos + new Vector2(value, 0).RotatedBy(i / 6f * MathHelper.TwoPi + Projectile.rotation), warpColor, new Vector3(0.5f, 0.5f, 1));
			hexagon.Add(drawPos + new Vector2(0, 0).RotatedBy(i / 6f * MathHelper.TwoPi + Projectile.rotation), warpColorInner, new Vector3(0.5f, 0.5f, 0));
		}
		batch.Draw(Commons.ModAsset.Point.Value, hexagon, PrimitiveType.TriangleStrip);
	}
}
