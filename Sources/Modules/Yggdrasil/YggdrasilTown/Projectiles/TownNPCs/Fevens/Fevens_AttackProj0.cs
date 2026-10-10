using Everglow.Commons.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs.Fevens;

public class Fevens_AttackProj0 : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.aiStyle = -1;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 3600;
		Projectile.tileCollide = false;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
	}

	internal int Target = -1;
	internal int TimeTokill = -1;
	public Queue<Vector2> OldPos0 = new Queue<Vector2>();
	public Queue<Vector2> OldPos1 = new Queue<Vector2>();

	public Vector2 Position0 => Projectile.Center + new Vector2(0, 20).RotatedBy((float)Main.time * 0.25f);

	public Vector2 Position1 => Projectile.Center + new Vector2(0, 30).RotatedBy((float)Main.time * 0.25f + MathHelper.Pi);

	public override void OnSpawn(IEntitySource source)
	{
		Target = (int)Projectile.ai[0];
	}

	public override void AI()
	{
		OldPos0.Enqueue(Position0);
		OldPos1.Enqueue(Position1);
		if (OldPos0.Count > 60)
		{
			OldPos0.Dequeue();
		}
		if (OldPos1.Count > 60)
		{
			OldPos1.Dequeue();
		}
		if (Target == -1)
		{
			Projectile.Kill();
			return;
		}

		if (TimeTokill > 0)
		{
			Projectile.velocity *= 0.01f;
		}

		TimeTokill--;
		if (TimeTokill < 0)
		{
			Lighting.AddLight(Projectile.Center, 1f, 0.01f, 0.02f);
		}
	}

	public override void OnKill(int timeLeft)
	{
		base.OnKill(timeLeft);
	}

	public void GenerateSmog(int Frequency)
	{
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		AmmoHit();
		return false;
	}

	public void AmmoHit()
	{
		TimeTokill = 60;
		Projectile.velocity = Projectile.oldVelocity;
		GenerateSmog(8);
		SoundEngine.PlaySound(SoundID.Item98.WithVolume(Main.rand.NextFloat(0.14f, 0.22f)).WithPitchOffset(Main.rand.NextFloat(0.7f, 0.9f)), Projectile.Center);
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		if (TimeTokill > 0)
		{
			return false;
		}
		else
		{
			var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
			Main.spriteBatch.Draw(texMain, Position0 - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), Projectile.rotation, texMain.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(texMain, Position1 - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), Projectile.rotation, texMain.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}

	public override void PostDraw(Player player, Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		DrawTrail(lightColor);
	}

	public void DrawTrail(Color light)
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float drawC = 1f;

		int trueL = OldPos0.Count;

		float additiveFactor = 0;
		for (int t = 0; t < 3; t++)
		{
			var oldPos0Array = OldPos0.ToArray();
			var bars = new List<Vertex2D>();
			var barsDark = new List<Vertex2D>();
			for (int i = 1; i < oldPos0Array.Length; ++i)
			{
				if (oldPos0Array[i] == Vector2.Zero)
				{
					break;
				}

				float width = 17;
				if (Projectile.timeLeft <= 30)
				{
					width *= Projectile.timeLeft / 30f;
				}

				var normalDir = new Vector2(0, 1).RotatedBy(t * MathHelper.TwoPi / 3f);

				var factor = i / (float)trueL;
				additiveFactor += (oldPos0Array[i - 1] - oldPos0Array[i]).Length() / 600f;
				float timer = (float)Main.time * 0.02f + Projectile.whoAmI * 0.77f;
				var color = Color.Lerp(new Color(1f, 0f, 0.1f, 0), new Color(0, 0, 0, 0), 1 - factor);
				bars.Add(new Vertex2D(oldPos0Array[i] + normalDir * -width - Main.screenPosition, color, new Vector3(-additiveFactor * 2 + timer, 0, 0)));
				bars.Add(new Vertex2D(oldPos0Array[i] + normalDir * width - Main.screenPosition, color, new Vector3(-additiveFactor * 2 + timer, 1, 0)));

				var colorDark = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0, 0, 0, 0), 1 - factor);
				barsDark.Add(new Vertex2D(oldPos0Array[i] + normalDir * -width - Main.screenPosition, colorDark, new Vector3(-additiveFactor * 2 + timer, 0, 0)));
				barsDark.Add(new Vertex2D(oldPos0Array[i] + normalDir * width - Main.screenPosition, colorDark, new Vector3(-additiveFactor * 2 + timer, 1, 0)));
			}
			if (bars.Count > 2)
			{
				Texture2D tex = Commons.ModAsset.Trail_2_black_thick.Value;
				Main.graphics.GraphicsDevice.Textures[0] = tex;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, barsDark.ToArray(), 0, barsDark.Count - 2);

				tex = Commons.ModAsset.Trail_2_thick.Value;
				Main.graphics.GraphicsDevice.Textures[0] = tex;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}

			bars = new List<Vertex2D>();
			barsDark = new List<Vertex2D>();
			var oldPos1Array = OldPos1.ToArray();
			for (int i = 1; i < oldPos1Array.Length; ++i)
			{
				if (oldPos1Array[i] == Vector2.Zero)
				{
					break;
				}

				float width = 17;
				if (Projectile.timeLeft <= 30)
				{
					width *= Projectile.timeLeft / 30f;
				}

				var normalDir = new Vector2(0, 1).RotatedBy(t * MathHelper.TwoPi / 3f);

				var factor = i / (float)trueL;
				additiveFactor += (oldPos1Array[i - 1] - oldPos1Array[i]).Length() / 600f;
				float timer = (float)Main.time * 0.02f + Projectile.whoAmI * 0.77f;
				var color = Color.Lerp(new Color(drawC * light.R / 255f * 1f, drawC * light.G / 255f * 0f, drawC * light.B / 255f * 0.2f, 0), new Color(0, 0, 0, 0), 1 - factor);
				bars.Add(new Vertex2D(oldPos1Array[i] + normalDir * -width - Main.screenPosition, color, new Vector3(-additiveFactor * 2 + timer, 0, 0)));
				bars.Add(new Vertex2D(oldPos1Array[i] + normalDir * width - Main.screenPosition, color, new Vector3(-additiveFactor * 2 + timer, 1, 0)));

				var colorDark = Color.Lerp(new Color(1f, 1f, 1f, 1f), new Color(0, 0, 0, 0), 1 - factor);
				barsDark.Add(new Vertex2D(oldPos1Array[i] + normalDir * -width - Main.screenPosition, colorDark, new Vector3(-additiveFactor * 2 + timer, 0, 0)));
				barsDark.Add(new Vertex2D(oldPos1Array[i] + normalDir * width - Main.screenPosition, colorDark, new Vector3(-additiveFactor * 2 + timer, 1, 0)));
			}
			if (bars.Count > 2)
			{
				Texture2D tex = Commons.ModAsset.Trail_2_black_thick.Value;
				Main.graphics.GraphicsDevice.Textures[0] = tex;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, barsDark.ToArray(), 0, barsDark.Count - 2);

				tex = Commons.ModAsset.Trail_2_thick.Value;
				Main.graphics.GraphicsDevice.Textures[0] = tex;
				Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}
}
