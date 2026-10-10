using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons;

namespace Everglow.Myth.TheFirefly.Projectiles.DreamWeaver;

internal class DreamWeaverArray : NoTextureProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 28;
		Projectile.height = 28;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 10000;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.tileCollide = false;
	}

	public override bool? CanCutTiles()
	{
		return false;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.Center = Projectile.Center * 0.7f + (player.Center + new Vector2(player.direction * 22, 12 * player.gravDir * (float)(0.2 + Math.Sin(Main.timeForVisualEffects / 18d) / 2d))) * 0.3f;
		Projectile.spriteDirection = player.direction;
		Projectile.velocity *= 0;
		if (player.itemTime > 0 && player.HeldItem.type == ModContent.ItemType<Items.Weapons.DreamWeaver>() && player.active && !player.dead)
		{
			Projectile.timeLeft = player.itemTime + 60;
			if (projPower < 30)
			{
				projPower++;
			}
		}
		else
		{
			projPower--;
			if (projPower < 0)
			{
				Projectile.Kill();
			}
		}
		Player.CompositeArmStretchAmount PCAS = Player.CompositeArmStretchAmount.Full;

		player.SetCompositeArmFront(true, PCAS, (float)(-Math.Sin(Main.timeForVisualEffects / 18d) * 0.6 + 1.2) * -player.direction);
		Vector2 vTOMouse = Main.MouseWorld - player.Center;
		player.SetCompositeArmBack(true, PCAS, (float)(Math.Atan2(vTOMouse.Y, vTOMouse.X) - Math.PI / 2d));
		Projectile.rotation = player.fullRotation;

		ringPos = ringPos * 0.9f + new Vector2(-72 * player.direction, -24 * player.gravDir) * 0.1f;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		DrawMagicArray();
		return false;
	}

	internal int projPower = 0;
	internal Vector2 ringPos = Vector2.Zero;

	public void DrawMagicArray()
	{
		Player player = Main.player[Projectile.owner];
		float timeValue = (float)Main.time * 0.05f;
		List<Vertex2D> bars = new List<Vertex2D>();
		Color drawColor = new Color(0, 0.4f, 1f, 0);
		Vector2 drawPos = player.MountedCenter + ringPos;
		for (int t = 0; t < 7; t++)
		{
			Vector2 drawPoint = new Vector2(0, projPower * 1.1f).RotatedBy(t / 7f * MathHelper.TwoPi);
			Vector2 pointVel = new Vector2(0, projPower * 0.1f).RotatedBy((t + 2.4f) / 7f * MathHelper.TwoPi) * 2.4f;
			float omega = 0.5f * projPower / 30f;
			float omegaStep = (0.03f + 0.01f * MathF.Sin(timeValue * 0.4f)) * projPower / 30f;
			for (int s = 0; s < 35; s++)
			{
				Vector2 velLeft = Vector2.Normalize(pointVel).RotatedBy(MathHelper.PiOver2) * projPower * 0.5f;
				float factor = s / 34f;
				float zValue = MathF.Sin(factor * MathF.PI);
				bars.Add(drawPos + drawPoint + velLeft, drawColor, new Vector3(factor + timeValue, 0, zValue));
				bars.Add(drawPos + drawPoint - velLeft, drawColor, new Vector3(factor + timeValue, 1, zValue));
				drawPoint += pointVel;
				pointVel = pointVel.RotatedBy(omega);
				omega -= omegaStep;
			}
		}
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect effect = Commons.ModAsset.Trailing.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_4.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}
}
