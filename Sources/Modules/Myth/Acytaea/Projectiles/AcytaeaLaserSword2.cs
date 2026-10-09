using Everglow.Commons.DataStructures;
using Everglow.Myth.Acytaea.NPCs;
using Everglow.Myth.Acytaea.VFXs;
using Terraria.DataStructures;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;

namespace Everglow.Myth.Acytaea.Projectiles;

public class AcytaeaLaserSword2 : ModProjectile
{
	public override string Texture => "Everglow/Myth/Acytaea/Projectiles/AcytaeaSword_projectile";
	public override void SetDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 120;
		Projectile.extraUpdates = 0;
		Projectile.scale = 1f;
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.DamageType = DamageClass.Melee;

		Projectile.width = 80;
		Projectile.height = 80;
	}
	public Vector2 EndPos = Vector2.Zero;
	public NPC Owner = new NPC();
	public override void OnSpawn(IEntitySource source)
	{
		int index = (int)Projectile.ai[0];
		if (index >= 0 && index < 200)
		{
			Owner = Main.npc[index];
		}
		else
		{
			Projectile.Kill();
		}
	}
	public override void AI()
	{
		if (Owner == null || !Owner.active)
		{
			Projectile.Kill();
		}
		if (Owner.type == ModContent.NPCType<Acytaea_Boss>())
		{
			Projectile.velocity = new Vector2(70, 0).RotatedBy(Projectile.rotation);
			Projectile.Center = Owner.Center + Projectile.velocity;
		}
		else
		{
			Projectile.Kill();
		}
		CheckFrame();
		GenerateVFX();
		Projectile.hide = true;
	}
	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}
	public override bool ShouldUpdatePosition()
	{
		return false;
	}
	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		overPlayers.Add(index);
		base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
	}
	public void GenerateVFX()
	{
		for (int x = 0; x < 4; x++)
		{
			Vector2 newVec = new Vector2(0, Main.rand.NextFloat(4f, 12f)).RotatedByRandom(6.283);
			var positionVFX = EndPos + newVec * Main.rand.NextFloat(0.7f, 0.9f);

			var acytaeaFlame = new AcytaeaFlameDust
			{
				velocity = newVec * x * 0.3f,
				Active = true,
				Visible = true,
				position = positionVFX,
				maxTime = Main.rand.Next(14, 16),
				ai = new float[] { Main.rand.NextFloat(0.1f, 1f), Main.rand.NextFloat(-0.04f, 0.04f), Main.rand.NextFloat(18f, 30f) },
			};
			Ins.VFXManager.Add(acytaeaFlame);
		}
	}
	private void CheckFrame()
	{
		Projectile.frameCounter++;
		if (Projectile.frameCounter > 3)
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
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float width = 36;
		if (Projectile.timeLeft > 264)
		{
			width = 300 - Projectile.timeLeft;
		}
		if (Projectile.timeLeft < 60)
		{
			width = Projectile.timeLeft * 0.6f;
		}
		width *= 0.4f;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect dissolve = Commons.ModAsset.Dissolve.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		float dissolveDuration = Projectile.timeLeft / 60f - 0.2f;
		if (Projectile.timeLeft < 0)
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
		Texture2D tex = ModAsset.AcytaeaFlySword.Value;
		Rectangle projFrame = new Rectangle(0, Projectile.frame * Projectile.height, Projectile.width, Projectile.height);
		Main.spriteBatch.Draw(tex, Projectile.Center, projFrame, lightColor, Projectile.rotation + MathHelper.PiOver4, new Vector2(40), Projectile.scale, SpriteEffects.None, 0);
		Texture2D texglow = ModAsset.AcytaeaFlySword_glow.Value;
		Main.spriteBatch.Draw(texglow, Projectile.Center, projFrame, new Color(255, 255, 255, 0), Projectile.rotation + MathHelper.PiOver4, new Vector2(40), Projectile.scale, SpriteEffects.None, 0);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		Texture2D dark = Commons.ModAsset.StarSlash_black.Value;
		Vector2 endDrawPos = EndPos - Main.screenPosition;
		Main.spriteBatch.Draw(dark, endDrawPos, null, Color.White, MathHelper.PiOver2, star.Size() / 2f, width / 4.5f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(dark, endDrawPos, null, Color.White, 0, star.Size() / 2f, width / 4.5f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(dark, endDrawPos, null, Color.White, MathHelper.PiOver2 + (float)Main.timeForVisualEffects * 0.04f, star.Size() / 2f, width / 3f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(dark, endDrawPos, null, Color.White, (float)Main.timeForVisualEffects * 0.04f, star.Size() / 2f, width / 3f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(dark, endDrawPos, null, Color.White, 0, star.Size() / 2f, new Vector2(3f, width / 2.25f), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, endDrawPos, null, new Color(255, 20, 60, 0), MathHelper.PiOver2, star.Size() / 2f, width / 4.5f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, endDrawPos, null, new Color(255, 20, 60, 0), 0, star.Size() / 2f, width / 4.5f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, endDrawPos, null, new Color(255, 20, 60, 0), MathHelper.PiOver2 + (float)Main.timeForVisualEffects * 0.04f, star.Size() / 2f, width / 3f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, endDrawPos, null, new Color(255, 0, 60, 0), (float)Main.timeForVisualEffects * 0.04f, star.Size() / 2f, width / 3f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, endDrawPos, null, new Color(255, 0, 60, 0), 0, star.Size() / 2f, new Vector2(3f, width / 2.25f), SpriteEffects.None, 0);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		float range = 80 + MathF.Sin((float)Main.timeForVisualEffects * 0.04f) * 20;
		List<Vertex2D> rings = new List<Vertex2D>();
		for (int x = 0; x < 45; x++)
		{
			Vector2 arrow = new Vector2(range, 0).RotatedBy(x / 45d * MathHelper.TwoPi);
			Vector2 arrowFar = new Vector2(range + width * 2, 0).RotatedBy(x / 45d * MathHelper.TwoPi);
			rings.Add(endDrawPos + arrow, new Color(0, 0, 0, 0), new Vector3(x / 45f, 1, 0));
			rings.Add(endDrawPos + arrowFar, new Color(255, 0, 60, 0), new Vector3(x / 45f, 0, 0));
		}
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_thick.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, rings.ToArray(), 0, rings.Count - 2);
	}
	public override void OnKill(int timeLeft)
	{
		foreach (var proj in Main.projectile)
		{
			if (proj != null && proj.active)
			{
				if (proj.type == ModContent.ProjectileType<AcytaeaSword_following>())
				{
					return;
				}
			}
		}
		foreach (var npc in Main.npc)
		{
			if (npc.type == ModContent.NPCType<Acytaea_Boss>())
			{
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<AcytaeaSword_following>(), 0, 0f, -1, npc.whoAmI);
				break;
			}
		}
	}
}
