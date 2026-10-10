using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Magic.BoneFeatherMagic;

public class BoneFeather_spine : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = true;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = false;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 240;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
		Projectile.extraUpdates = 4;
		Projectile.aiStyle = -1;
	}

	public override void AI()
	{
		Projectile.velocity = Vector2.Zero;
		Beta += Main.rand.NextFloat(-0.002f, 0.002f);
		Omega += Beta;
		if (Math.Abs(Omega) > 0.3f)
		{
			Omega *= 0.95f;
		}
		if (Math.Abs(Beta) > 0.02f)
		{
			Beta *= 0.95f;
		}
		if (Projectile.timeLeft % 4 == 0 && Projectile.timeLeft > 120)
		{
			VelocityPerStep = VelocityPerStep.RotatedBy(Omega);
			float length = 20f;
			if (Projectile.timeLeft < 180)
			{
				length = (Projectile.timeLeft - 120) / 3f;
			}
			VelocityPerStep = Vector2.Normalize(VelocityPerStep) * length;
			Projectile.position += VelocityPerStep;
			OldPos.Add(Projectile.Center);
		}
		if (Projectile.timeLeft < 120)
		{
			Projectile.extraUpdates = 0;
		}
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.hide = true;
		VelocityPerStep = Vector2.Normalize(Projectile.velocity) * 20f;
	}

	public float Beta = 0;
	public float Omega = 0;
	public Vector2 VelocityPerStep;
	public List<Vector2> OldPos = new List<Vector2>();

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		foreach (Vector2 v in OldPos)
		{
			int topX = (int)(v.X - Projectile.width * 0.5f);
			int topY = (int)(v.Y - Projectile.height * 0.5f);
			Rectangle r0 = new Rectangle(topX, topY, Projectile.width, Projectile.height);
			if (Rectangle.Intersect(r0, targetHitbox) != Rectangle.emptyRectangle)
			{
				return true;
			}
		}
		return false;
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		if (target.velocity.X > 0)
		{
			modifiers.HitDirectionOverride = 1;
		}
		if (target.velocity.X < 0)
		{
			modifiers.HitDirectionOverride = 1;
		}
		if (target.velocity.X == 0)
		{
			Player player = Main.player[Projectile.owner];
			if (target.Center.X < player.Center.X)
			{
				modifiers.HitDirectionOverride = -1;
			}
			else
			{
				modifiers.HitDirectionOverride = 1;
			}
		}
		base.ModifyHitNPC(target, ref modifiers);
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCsAndTiles.Add(index);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		if (OldPos.Count < 2)
		{
			return false;
		}
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int i = 1; i < OldPos.Count; i++)
		{
			Vector2 normalize = (OldPos[i] - OldPos[i - 1]) * 1f;
			Vector2 normalizeRight = normalize.RotatedBy(MathHelper.PiOver2);
			Color newLightColor = Lighting.GetColor((int)(OldPos[i].X / 16f), (int)(OldPos[i].Y / 16f));
			bars.Add(OldPos[i] + normalize - normalizeRight, newLightColor, new Vector3(0, 0, 0));
			bars.Add(OldPos[i] + normalize + normalizeRight, newLightColor, new Vector3(1, 0, 0));
			bars.Add(OldPos[i] - normalize - normalizeRight, newLightColor, new Vector3(0, 1, 0));

			bars.Add(OldPos[i] - normalize - normalizeRight, newLightColor, new Vector3(0, 1, 0));
			bars.Add(OldPos[i] + normalize + normalizeRight, newLightColor, new Vector3(1, 0, 0));
			bars.Add(OldPos[i] - normalize + normalizeRight, newLightColor, new Vector3(1, 1, 0));
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect dissolve = Commons.ModAsset.Dissolve.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		float dissolveDuration = Projectile.timeLeft / 80f * 1.4f - 0.2f;
		if (Projectile.timeLeft > 80)
		{
			dissolveDuration = 1.2f;
		}
		dissolve.Parameters["uTransform"].SetValue(model * projection);
		dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_spiderNet.Value);
		dissolve.Parameters["duration"].SetValue(dissolveDuration);
		dissolve.Parameters["uDissolveColor"].SetValue(new Vector4(0.9f, 0.9f, 0.6f, 1f));
		dissolve.Parameters["uNoiseSize"].SetValue(4f);
		dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[0], Projectile.ai[1]));
		dissolve.CurrentTechnique.Passes[0].Apply();
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.BoneFeather_spine.Value;
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, bars.ToArray(), 0, bars.Count / 3);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		return false;
	}
}
