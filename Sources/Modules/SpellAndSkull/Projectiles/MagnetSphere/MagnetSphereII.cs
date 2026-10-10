using Everglow.Commons.Vertex;
using Everglow.SpellAndSkull.Common;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.SpellAndSkull.Projectiles.MagnetSphere;

public class MagnetSphereII : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.extraUpdates = 3;
		Projectile.timeLeft = 3000;
		Projectile.alpha = 0;
		Projectile.penetrate = 18;
		Projectile.scale = 1f;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 24;
		Projectile.DamageType = DamageClass.Magic;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Lighting.AddLight(Projectile.Center, 0, 1.8f * Projectile.scale, 1.4f * Projectile.scale);
		Projectile.velocity *= 0.999f;
		Projectile.scale = 0.6f + (float)Math.Sin(Main.timeForVisualEffects / 1.8f + Projectile.whoAmI) * 0.25f;
		Projectile.timeLeft -= player.ownedProjectileCounts[Projectile.type];
		if (Main.rand.NextBool(8))
		{
			foreach (NPC target in Main.npc)
			{
				if (target.active)
				{
					if (!target.friendly && !target.dontTakeDamage && target.CanBeChasedBy())
					{
						Vector2 v = target.Center - Projectile.Center;
						if (v.Length() < 400)
						{
							if (Main.rand.NextBool(6))
							{
								ScreenShaker Gsplayer = Main.player[Projectile.owner].GetModPlayer<ScreenShaker>();
								Gsplayer.FlyCamPosition = new Vector2(0, 11).RotatedByRandom(6.283);
								int HitType = ModContent.ProjectileType<MagnetSphereLighting>();
								var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), target.Center, Vector2.One, HitType, Projectile.damage * 1, Projectile.knockBack, Projectile.owner, Projectile.whoAmI, Projectile.rotation + Main.rand.NextFloat(6.283f));
								p.CritChance = Projectile.CritChance;
								SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, target.Center);
								Projectile.penetrate--;
								if (Projectile.penetrate < 0)
								{
									Projectile.Kill();
								}
							}
						}
					}
				}
			}
		}
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.ai[0] = 0;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		float timeValue = (float)(Main.timeForVisualEffects * 0.008f);
		float mulSize = 1f + MathF.Sin(timeValue * 15f + Projectile.whoAmI) * 0.15f;
		var baseColor = new Color(0, 199, 129, 0);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		SpellAndSkullUtils.DrawTexCircle(20 * mulSize, 10, baseColor, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail.Value, 0, 3);
		Texture2D Light = ModAsset.MagnetSphereII.Value;
		Texture2D Shade = ModAsset.NewWaterBoltShade.Value;
		Main.spriteBatch.Draw(Shade, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, Color.White, Projectile.rotation, Shade.Size() / 2f, 1.08f * Projectile.scale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(Light, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, baseColor, Projectile.rotation, Light.Size() / 2f, 0.8f * Projectile.scale, SpriteEffects.None, 0);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect sphere = ModAsset.SpherePerspective.Value;
		List<Vertex2D> triangleList = new List<Vertex2D>();
		float radius = 40 * mulSize;

		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(radius, radius), baseColor, new Vector3(-1, 1, timeValue)));
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(radius, -radius), baseColor, new Vector3(-1, -1, timeValue)));
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(-radius, -radius), baseColor, new Vector3(1, -1, timeValue)));

		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(radius, radius), baseColor, new Vector3(-1, 1, timeValue)));
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(-radius, -radius), baseColor, new Vector3(1, -1, timeValue)));
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(-radius, radius), baseColor, new Vector3(1, 1, timeValue)));

		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;

		sphere.Parameters["uTransform"].SetValue(model * projection);
		sphere.Parameters["circleCenter"].SetValue(new Vector3(0, 0, -2));
		sphere.Parameters["radiusOfCircle"].SetValue(1f);
		sphere.Parameters["uTime"].SetValue(timeValue * 0.4f);
		sphere.Parameters["uWarp"].SetValue(0.06f);

		sphere.CurrentTechnique.Passes[0].Apply();

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_hiveCyber.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.Textures[1] = Commons.ModAsset.Noise_rgb.Value;
		Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		return false;
	}

	public override void OnKill(int timeLeft)
	{
		ScreenShaker Gsplayer = Main.player[Projectile.owner].GetModPlayer<ScreenShaker>();
		Gsplayer.FlyCamPosition = new Vector2(0, 22).RotatedByRandom(6.283);
		SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.Center);
		for (int d = 0; d < 14; d++)
		{
			Vector2 BasePos = Projectile.Center - new Vector2(4) - Projectile.velocity;
			var d0 = Dust.NewDustDirect(BasePos, 0, 0, DustID.Electric, 0, 0, 0, default, 0.6f);
			d0.velocity = new Vector2(0, Main.rand.NextFloat(1.65f, 5.5f)).RotatedByRandom(6.283) * 3;
		}
		int HitType = ModContent.ProjectileType<MagnetSphereHit>();
		var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.One, HitType, (int)(Projectile.damage * 3f), Projectile.knockBack, Projectile.owner, 24, Projectile.rotation + Main.rand.NextFloat(6.283f));
		p.CritChance = Projectile.CritChance;
	}

	public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
	{
		Spark();
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		Spark();
	}

	private void Spark()
	{
		ScreenShaker Gsplayer = Main.player[Projectile.owner].GetModPlayer<ScreenShaker>();
		Gsplayer.FlyCamPosition = new Vector2(0, 22).RotatedByRandom(6.283);
		SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap, Projectile.Center);
		for (int d = 0; d < 10; d++)
		{
			Vector2 BasePos = Projectile.Center - new Vector2(4) - Projectile.velocity;
			var d0 = Dust.NewDustDirect(BasePos, 0, 0, DustID.Electric, 0, 0, 0, default, 0.6f);
			d0.velocity = new Vector2(0, Main.rand.NextFloat(1.65f, 5.5f)).RotatedByRandom(6.283);
		}
		Projectile.penetrate -= 5;
		if (Projectile.penetrate < 0)
		{
			Projectile.Kill();
		}

		int HitType = ModContent.ProjectileType<MagnetSphereHit>();
		var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.One, HitType, (int)(Projectile.damage * 2f), Projectile.knockBack, Projectile.owner, 18, Projectile.rotation + Main.rand.NextFloat(6.283f));
		p.CritChance = Projectile.CritChance;
		Projectile.damage = (int)(Projectile.damage * 1.2);
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Spark();
		if (Projectile.velocity.X != oldVelocity.X)
		{
			Projectile.velocity.X = -oldVelocity.X;
		}

		if (Projectile.velocity.Y != oldVelocity.Y)
		{
			Projectile.velocity.Y = -oldVelocity.Y;
		}

		Projectile.velocity *= 0.98f;
		Projectile.penetrate -= 5;
		if (Projectile.penetrate < 0)
		{
			Projectile.Kill();
		}

		return false;
	}
}
