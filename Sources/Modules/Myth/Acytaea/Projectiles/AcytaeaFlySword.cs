using Everglow.Myth.Acytaea.Buffs;
using Everglow.Myth.Acytaea.VFXs;
using Everglow.Myth.Misc.Projectiles.Weapon.Magic.FreezeFeatherMagic;
using Terraria.Audio;

namespace Everglow.Myth.Acytaea.Projectiles;

public class AcytaeaFlySword : ModProjectile
{
	public override string Texture => "Everglow/Myth/Acytaea/Projectiles/AcytaeaSword_projectile";
	public override void SetDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 600;
		Projectile.extraUpdates = 0;
		Projectile.scale = 1f;
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.tileCollide = true;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.DamageType = DamageClass.Melee;

		Projectile.width = 80;
		Projectile.height = 80;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
	}
	public int TimeTokill = -1;
	public Vector2 Aim = Vector2.Zero;
	public override void AI()
	{
		if (TimeTokill >= 0 && TimeTokill <= 2)
			Projectile.Kill();
		if (TimeTokill <= 80 && TimeTokill > 0)
			Projectile.velocity = Projectile.oldVelocity;
		TimeTokill--;
		if (TimeTokill >= 0)
		{
			Projectile.velocity *= 0.1f;
		}
		else
		{
			CheckFrame();
			Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
			if (Projectile.timeLeft > 540)
			{
				Vector2 toAim = Aim - Projectile.Center - Projectile.velocity;
				toAim *= 0.1f;
				if (toAim.Length() > 30f)
				{
					toAim = Vector2.Normalize(toAim) * 30f;
				}
				Projectile.velocity = toAim * 0.05f + Projectile.velocity * 0.95f;
			}
			else
			{
				Projectile.velocity.Y += 0.5f;
			}
			foreach (Player p in Main.player)
			{
				if (p != null && p.active && !p.dead)
				{
					if (Rectangle.Intersect(Projectile.Hitbox, p.Hitbox) != Rectangle.emptyRectangle)
					{
						AmmoHit();
					}
				}
			}
		}
	}
	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		AmmoHit();
		Projectile.tileCollide = false;
		return false;
	}
	public void AmmoHit()
	{
		SoundEngine.PlaySound(SoundID.DD2_GoblinBomb.WithPitchOffset(-1), Projectile.Center);
		if (TimeTokill > 0)
		{
			return;
		}
		Player player = Main.player[Projectile.owner];
		TimeTokill = 60;
		Projectile.tileCollide = false;
		Projectile.friendly = false;
		Projectile.ignoreWater = true;
		Projectile.velocity = Projectile.oldVelocity;
		SoundEngine.PlaySound((SoundID.DD2_WitherBeastCrystalImpact.WithVolume(0.3f)).WithPitchOffset(Main.rand.NextFloat(-0.4f, 0.4f)), Projectile.Center);
		for (int x = 0; x < 25; x++)
		{
			Vector2 newVec = new Vector2(0, Main.rand.NextFloat(4f, 12f)).RotatedByRandom(6.238f);
			var positionVFX = Projectile.Center + newVec * Main.rand.NextFloat(0.7f, 0.9f);

			var acytaeaFlame = new AcytaeaFlameDust
			{
				velocity = newVec,
				Active = true,
				Visible = true,
				position = positionVFX,
				maxTime = Main.rand.Next(14, 16),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.04f, 0.04f), Main.rand.NextFloat(18f, 30f) },
			};
			Ins.VFXManager.Add(acytaeaFlame);
		}
		//Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AcytaeaFlySwordExplosion>(), Projectile.damage, Projectile.knockBack, player.whoAmI, 14);
		Projectile.position -= Projectile.velocity;
	}
	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}
	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		AmmoHit();
		target.AddBuff(ModContent.BuffType<AcytaeaInferno>(), 450);
		base.OnHitPlayer(target, info);
	}
	public void DrawTrail(Texture2D tex, Color color, float width = 36)
	{
		var c0 = color;
		var bars = new List<Vertex2D>();
		int trueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
				break;

			trueL++;
		}
		if (TimeTokill > 0)
		{
			trueL = Math.Min(trueL, TimeTokill);
			if (trueL == 0)
			{
				return;
			}
		}
		for (int i = 1; i < trueL; ++i)
		{
			float width2 = width;
			if (i < 10)
				width2 *= i / 10f;
			if (Projectile.oldPos[i] == Vector2.Zero)
				break;

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			if (normalDir == Vector2.zeroVector)
			{
				continue;
			}
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			var factor = i / (float)trueL;
			var w = MathHelper.Lerp(1f, 0.05f, factor);
			float x0 = factor * 0.6f - (float)(Main.timeForVisualEffects / 35d) + 10000;
			if (i > 2)
			{
				if ((Projectile.oldPos[i - 2] - Projectile.oldPos[i - 1]) == Vector2.zeroVector)
				{
					bars.Add(new Vertex2D(Projectile.oldPos[i - 1] + new Vector2(Projectile.width / 2f) - Main.screenPosition, c0, new Vector3(x0, 0.5f, w)));
					bars.Add(new Vertex2D(Projectile.oldPos[i - 1] + new Vector2(Projectile.width / 2f) - Main.screenPosition, c0, new Vector3(x0, 0.5f, w)));
				}
			}
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width2 * (1 - factor) + new Vector2(Projectile.width / 2f) - Main.screenPosition, c0, new Vector3(x0, 1, w)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width2 * (1 - factor) + new Vector2(Projectile.width / 2f) - Main.screenPosition, c0, new Vector3(x0, 0, w)));
		}
		Texture2D t = tex;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.Textures[0] = t;
		if (bars.Count > 3)
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
	}
	private void CheckFrame()
	{
		Projectile.frameCounter++;
		if (Projectile.frameCounter > 6)
		{
			if (Projectile.frame < 3)
			{
				Projectile.frame++;
			}
			else
			{
				Projectile.frame = 0;
			}
			Projectile.frameCounter = 0;
		}
	}
	public override void PostDraw(Color lightColor)
	{
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		DrawTrail(Commons.ModAsset.Trail_2_black_thick.Value, Color.White * 0.7f);
		DrawTrail(Commons.ModAsset.Trail_6.Value, new Color(1f, 0f, 0.4f, 0f));
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect dissolve = Commons.ModAsset.Dissolve.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		float dissolveDuration = TimeTokill / 60f - 0.2f;
		if (TimeTokill < 0)
		{
			dissolveDuration = 1.2f;
		}
		dissolve.Parameters["uTransform"].SetValue(model * projection);
		dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_spiderNet.Value);
		dissolve.Parameters["duration"].SetValue(dissolveDuration);
		dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(1f, 0f, 0f, 1f));
		dissolve.Parameters["uNoiseSize"].SetValue(2f);
		dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
		dissolve.CurrentTechnique.Passes[0].Apply();
		Texture2D tex = ModAsset.AcytaeaFlySword_red.Value;
		Rectangle projFrame = new Rectangle(0, Projectile.frame * Projectile.height, Projectile.width, Projectile.height);
		Main.spriteBatch.Draw(tex, Projectile.Center, projFrame, new Color(255, 0, 215, 155), Projectile.rotation + MathHelper.PiOver4, new Vector2(40), Projectile.scale, SpriteEffects.None, 0);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	}
}
