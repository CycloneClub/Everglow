using System.Reflection.Metadata;
using Everglow.Commons.DataStructures;
using Everglow.Commons.Graphics;
using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;

public class MossySpellArray : NoTextureProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 28;
		Projectile.height = 28;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 10000;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.tileCollide = false;
		LeafFade.colorList.Add((new Color(0.7f, 1f, 0.2f, 0), 0));
		LeafFade.colorList.Add((new Color(0.6f, 1f, 0.4f, 0), 0.5f));
		LeafFade.colorList.Add((new Color(0.3f, 0.5f, 0.3f, 0), 0.8f));
		LeafFade.colorList.Add((new Color(0.2f, 0.1f, 0.0f, 0), 0.9f));
		LeafFade.colorList.Add((new Color(0f, 0f, 0f, 0), 1f));

		LeafFade_dark.colorList.Add((new Color(0f, 0f, 0f, 1), 0));
		LeafFade_dark.colorList.Add((new Color(0f, 0f, 0f, 0.5f), 0.9f));
		LeafFade_dark.colorList.Add((new Color(0f, 0f, 0f, 0), 1f));
	}

	public GradientColor LeafFade = new();

	public GradientColor LeafFade_dark = new();

	public float ArrayScale = 0f;

	public bool Killing = false;

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
		if (player.itemTime > 0 && player.HeldItem.type == ModContent.ItemType<MossySpell>() && player.active && !player.dead)
		{
			Projectile.timeLeft = player.itemTime + 60;
			if (projPower < 30)
			{
				projPower++;
			}
			ArrayScale = projPower;
			Killing = false;
		}
		else
		{
			projPower--;
			if (projPower < 0)
			{
				Projectile.Kill();
			}
			Killing = true;
		}
		Player.CompositeArmStretchAmount PCAS = Player.CompositeArmStretchAmount.Full;

		player.SetCompositeArmFront(true, PCAS, (float)(-Math.Sin(Main.timeForVisualEffects / 18d) * 0.6 + 1.2) * -player.direction);
		Vector2 vTOMouse = Main.MouseWorld - player.Center;
		player.SetCompositeArmBack(true, PCAS, (float)(Math.Atan2(vTOMouse.Y, vTOMouse.X) - Math.PI / 2d));
		Projectile.rotation = player.fullRotation;

		ringPos = ringPos * 0.9f + new Vector2(-72 * player.direction, -24 * player.gravDir) * 0.1f;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
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
		var stems_black = new List<Vertex2D>();
		var leaves_black = new List<Vertex2D>();

		var stems = new List<Vertex2D>();
		var leaves = new List<Vertex2D>();

		Vector2 drawPos = player.MountedCenter + ringPos;
		float sideCount = 3;

		for (int k = 0; k < 3; k++)
		{
			float animationTime0 = (timeValue * 12 + k * 20) % 60;
			for (int t = 0; t < sideCount; t++)
			{
				Vector2 drawPoint = new Vector2(0, 0).RotatedBy(t / sideCount * MathHelper.TwoPi);
				Vector2 pointVel = new Vector2(0, ArrayScale * 0.1f).RotatedBy((t + 2.4f + k / 3f) / sideCount * MathHelper.TwoPi) * 2.4f;
				int maxStep = (int)(animationTime0 / 2f);
				for (int s = 0; s < maxStep; s++)
				{
					Vector2 velLeft = Vector2.Normalize(pointVel).RotatedBy(MathHelper.PiOver2) * ArrayScale * 0.5f;
					float factor = s / (float)(maxStep - 1);
					if (maxStep - 1 == 0)
					{
						factor = 0;
					}
					float zValue = MathF.Sin(factor * MathF.PI);
					var killingFade = 0f;
					if (Killing)
					{
						killingFade = (ArrayScale - projPower) / ArrayScale;
					}
					var drawColor = LeafFade.GetColor(animationTime0 / 60f + (1 - factor) * 0.2f + killingFade);
					var drawColor_black = LeafFade_dark.GetColor(animationTime0 / 60f + (1 - factor) * 0.2f + killingFade);
					stems_black.Add(drawPos + drawPoint + velLeft, drawColor_black, new Vector3(factor + timeValue * -0.05f, 0, zValue));
					stems_black.Add(drawPos + drawPoint - velLeft, drawColor_black, new Vector3(factor + timeValue * -0.05f, 1, zValue));

					stems.Add(drawPos + drawPoint + velLeft, drawColor, new Vector3(factor + timeValue * -0.05f, 0, zValue));
					stems.Add(drawPos + drawPoint - velLeft, drawColor, new Vector3(factor + timeValue * -0.05f, 1, zValue));
					drawPoint += pointVel;
					if (s % 4 == k)
					{
						float sideLeafValue = MathF.Sin(MathF.Pow(factor, 0.35f) * MathF.PI);
						Vector2 leafStalk = drawPos + drawPoint;
						Vector2 leafVel = pointVel.RotatedBy(1.2f);
						int maxLeafStep = (int)(maxStep * sideLeafValue) / 2;
						for (int le = 0; le < maxLeafStep; le++)
						{
							float factorLeave = le / (float)(maxLeafStep - 1);
							if (maxLeafStep - 1 == 0)
							{
								factorLeave = 0;
							}
							float zValueLeave = MathF.Cos(factorLeave * MathHelper.PiOver2);
							Vector2 leafVelLeft = Vector2.Normalize(leafVel).RotatedBy(MathHelper.PiOver2) * ArrayScale * 0.5f;
							if (le == 0)
							{
								leaves_black.Add(leafStalk + leafVelLeft, drawColor_black, new Vector3(factorLeave + timeValue * -0.05f, 0, 0));
								leaves_black.Add(leafStalk - leafVelLeft, drawColor_black, new Vector3(factorLeave + timeValue * -0.05f, 1, 0));

								leaves.Add(leafStalk + leafVelLeft, drawColor, new Vector3(factorLeave + timeValue * -0.05f, 0, 0));
								leaves.Add(leafStalk - leafVelLeft, drawColor, new Vector3(factorLeave + timeValue * -0.05f, 1, 0));
							}
							leaves_black.Add(leafStalk + leafVelLeft, drawColor_black, new Vector3(factorLeave + timeValue * -0.05f, 0, zValueLeave));
							leaves_black.Add(leafStalk - leafVelLeft, drawColor_black, new Vector3(factorLeave + timeValue * -0.05f, 1, zValueLeave));

							leaves.Add(leafStalk + leafVelLeft, drawColor, new Vector3(factorLeave + timeValue * -0.05f, 0, zValueLeave));
							leaves.Add(leafStalk - leafVelLeft, drawColor, new Vector3(factorLeave + timeValue * -0.05f, 1, zValueLeave));
							if (le == maxLeafStep - 1)
							{
								leaves_black.Add(leafStalk + leafVelLeft, drawColor_black, new Vector3(factorLeave + timeValue * -0.05f, 0, 0));
								leaves_black.Add(leafStalk - leafVelLeft, drawColor_black, new Vector3(factorLeave + timeValue * -0.05f, 1, 0));

								leaves.Add(leafStalk + leafVelLeft, drawColor, new Vector3(factorLeave + timeValue * -0.05f, 0, 0));
								leaves.Add(leafStalk - leafVelLeft, drawColor, new Vector3(factorLeave + timeValue * -0.05f, 1, 0));
							}
							leafStalk += leafVel;
						}

						leafStalk = drawPos + drawPoint;
						leafVel = pointVel.RotatedBy(-1.2f);
						for (int le = 0; le < maxLeafStep; le++)
						{
							float factorLeave = le / (float)(maxLeafStep - 1);
							if (maxLeafStep - 1 == 0)
							{
								factorLeave = 0;
							}
							float zValueLeave = MathF.Cos(factorLeave * MathHelper.PiOver2);
							Vector2 leafVelLeft = Vector2.Normalize(leafVel).RotatedBy(MathHelper.PiOver2) * ArrayScale * 0.5f;
							if (le == 0)
							{
								leaves_black.Add(leafStalk + leafVelLeft, drawColor_black, new Vector3(factorLeave + timeValue * -0.05f, 0, 0));
								leaves_black.Add(leafStalk - leafVelLeft, drawColor_black, new Vector3(factorLeave + timeValue * -0.05f, 1, 0));

								leaves.Add(leafStalk + leafVelLeft, drawColor, new Vector3(factorLeave + timeValue * -0.05f, 0, 0));
								leaves.Add(leafStalk - leafVelLeft, drawColor, new Vector3(factorLeave + timeValue * -0.05f, 1, 0));
							}
							leaves_black.Add(leafStalk + leafVelLeft, drawColor_black, new Vector3(factorLeave + timeValue * -0.05f, 0, zValueLeave));
							leaves_black.Add(leafStalk - leafVelLeft, drawColor_black, new Vector3(factorLeave + timeValue * -0.05f, 1, zValueLeave));

							leaves.Add(leafStalk + leafVelLeft, drawColor, new Vector3(factorLeave + timeValue * -0.05f, 0, zValueLeave));
							leaves.Add(leafStalk - leafVelLeft, drawColor, new Vector3(factorLeave + timeValue * -0.05f, 1, zValueLeave));
							if (le == maxLeafStep - 1)
							{
								leaves_black.Add(leafStalk + leafVelLeft, drawColor_black, new Vector3(factorLeave + timeValue * -0.05f, 0, 0));
								leaves_black.Add(leafStalk - leafVelLeft, drawColor_black, new Vector3(factorLeave + timeValue * -0.05f, 1, 0));

								leaves.Add(leafStalk + leafVelLeft, drawColor, new Vector3(factorLeave + timeValue * -0.05f, 0, 0));
								leaves.Add(leafStalk - leafVelLeft, drawColor, new Vector3(factorLeave + timeValue * -0.05f, 1, 0));
							}
							leafStalk += leafVel;
						}
					}
				}
			}
		}
		Texture oldTex = Main.graphics.GraphicsDevice.Textures[0];
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect effect = Commons.ModAsset.Trailing.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_8_black.Value;
		if (stems_black.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, stems_black.ToArray(), 0, stems_black.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, stems_black.ToArray(), 0, stems_black.Count - 2);
		}
		if (leaves_black.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, leaves_black.ToArray(), 0, leaves_black.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, leaves_black.ToArray(), 0, leaves_black.Count - 2);
		}

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_8.Value;
		if (stems.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, stems.ToArray(), 0, stems.Count - 2);
		}
		if (leaves.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, leaves.ToArray(), 0, leaves.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		Main.graphics.GraphicsDevice.Textures[0] = oldTex;
	}
}
