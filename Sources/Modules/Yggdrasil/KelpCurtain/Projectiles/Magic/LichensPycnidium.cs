using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;

public class LichensPycnidium : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.friendly = false;
		Projectile.width = 15;
		Projectile.height = 15;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 200;
		Projectile.magic = true;
		base.SetDefaults();
	}

	public struct SubLichen
	{
		public float rotation;
		public NPC attachTarget;
		public float maturity;
		public float ai0;
		public Vector2 attachPos;
		public float scale;
		public bool active;
	}

	public List<SubLichen> Lichens;

	public void AddLichens(float rot, NPC attach, float ai0)
	{
		var subLichen = default(SubLichen);
		subLichen.rotation = rot;
		subLichen.attachTarget = attach;
		subLichen.maturity = 0;
		subLichen.ai0 = ai0;
		subLichen.attachPos = Vector2.zeroVector;
		subLichen.scale = 0;
		subLichen.active = true;
		Lichens.Add(subLichen);
	}

	public override void OnSpawn(IEntitySource source)
	{
		Lichens = new List<SubLichen>();

		base.OnSpawn(source);
	}

	public SubLichen UpdateLichen(SubLichen lichen)
	{
		float rotValue = 0.4f * MathF.Sin(lichen.ai0 * 95f + lichen.attachTarget.whoAmI + (float)Main.time * 0.03f);
		float lengthValue = 24 * MathF.Sin(lichen.ai0 + lichen.attachTarget.whoAmI * 0.25f + (float)Main.time * 0.042f);
		Vector2 aimPos = new Vector2(30 + lengthValue + lichen.maturity / 5f, 0).RotatedBy(lichen.ai0 + rotValue);
		if (!lichen.attachTarget.noGravity)
		{
			aimPos -= new Vector2(0, 45 * lichen.scale);
		}
		lichen.attachPos = Vector2.Lerp(lichen.attachPos, aimPos, 0.15f);
		lichen.maturity++;
		if (lichen.maturity > 300)
		{
			if (Main.rand.NextFloat(lichen.maturity - 500, lichen.maturity - 200) > 150)
			{
				Explode(lichen);
				lichen.active = false;
			}
		}
		float value = lichen.maturity / 300f;
		if (value > 1)
		{
			value = 1;
		}
		lichen.scale = value * value * 0.5f;
		return lichen;
	}

	public override void AI()
	{
		for (int t = 0; t < Lichens.Count; t++)
		{
			if (Lichens[t].attachTarget == null || Lichens[t].attachTarget.active == false)
			{
				Lichens.RemoveAt(t);
				continue;
			}
			if (!Lichens[t].active)
			{
				Lichens.RemoveAt(t);
				continue;
			}
			else
			{
				Lichens[t] = UpdateLichen(Lichens[t]);
			}
		}
		if (Lichens.Count > 0)
		{
			Projectile.timeLeft = 60;
		}
		Projectile.Center = Main.player[Projectile.owner].Center;
		base.AI();
	}

	public void Explode(SubLichen lichen)
	{
		if (lichen.active)
		{
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), lichen.attachPos + lichen.attachTarget.Center, Vector2.zeroVector, ModContent.ProjectileType<Pycnidium_explosion>(), 12, Projectile.knockBack * 4f, Projectile.owner, 30);
		}
	}

	public SubLichen SetRotation(SubLichen lichen, float value)
	{
		lichen.rotation = value;
		return lichen;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D texture = ModAsset.LichensPycnidium.Value;
		var bars = new List<Vertex2D>();
		foreach (var lichen in Lichens)
		{
			if (!lichen.active)
			{
				continue;
			}
			Vector2 center = lichen.attachTarget.Center + lichen.attachPos;
			Color c0 = lightColor;
			c0.A = 0;
			var color = Color.Lerp(c0, new Color(0.1f, 0.7f, 0.7f, 0), lichen.maturity / 100f);
			if (lichen.maturity > 100)
			{
				color = Color.Lerp(new Color(0.1f, 0.7f, 0.7f, 0), new Color(0.4f, 0.7f, 0.4f, 0), (lichen.maturity - 100f) / 100f);
			}
			if (lichen.maturity > 200)
			{
				color = Color.Lerp(new Color(0.4f, 0.7f, 0.4f, 0), new Color(0.8f, 0.8f, 0.0f, 0), (lichen.maturity - 200f) / 100f);
			}
			Main.spriteBatch.Draw(texture, center - Main.screenPosition, null, color, 0, texture.Size() / 2f, lichen.scale, SpriteEffects.None, 0);

			Vector2 joint = lichen.attachTarget.Center;
			Vector2 jointVel = new Vector2(7, 0).RotatedBy(lichen.ai0);
			var joints = new List<Vector2>();
			var jointVels = new List<Vector2>();
			for (int t = 0; t < 120; t++)
			{
				joints.Add(joint);
				jointVels.Add(jointVel);
				joint += jointVel;
				jointVel = Vector2.Lerp(jointVel, Vector2.Normalize(center - joint - jointVel) * 1.5f, 0.35f);
				float toTargetLength = (center - joint).Length();

				if (toTargetLength < 2)
				{
					SetRotation(lichen, jointVel.ToRotation() + MathHelper.PiOver2);
					joints.Add(joint);
					jointVels.Add(jointVel);
					break;
				}
			}
			float width = 2f * (lichen.scale * 0.5f + 0.5f);
			color.A = 255;
			for (int t = 0; t < joints.Count; t++)
			{
				Vector2 velLeft = jointVels[t].NormalizeSafe().RotatedBy(MathHelper.PiOver2);
				float mulWid = 1f;
				if (t < 10)
				{
					mulWid = t / 10f;
				}
				if (t == 0)
				{
					bars.Add(new Vertex2D(joints[t] + velLeft * width * mulWid - Main.screenPosition, Color.Transparent, new Vector3(0, 0.7f, 0)));
					bars.Add(new Vertex2D(joints[t] - velLeft * width * mulWid - Main.screenPosition, Color.Transparent, new Vector3(0, 0.3f, 0)));
				}
				bars.Add(new Vertex2D(joints[t] + velLeft * width * mulWid - Main.screenPosition, Color.Lerp(c0, color, t / (float)joints.Count), new Vector3(0, 0.7f, 0)));
				bars.Add(new Vertex2D(joints[t] - velLeft * width * mulWid - Main.screenPosition, Color.Lerp(c0, color, t / (float)joints.Count), new Vector3(0, 0.3f, 0)));
				if (t == joints.Count - 1)
				{
					bars.Add(new Vertex2D(joints[t] + velLeft * width * mulWid - Main.screenPosition, Color.Transparent, new Vector3(0, 0.7f, 0)));
					bars.Add(new Vertex2D(joints[t] - velLeft * width * mulWid - Main.screenPosition, Color.Transparent, new Vector3(0, 0.3f, 0)));
				}
			}
		}
		if (bars.Count > 2)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Main.graphics.GraphicsDevice.Textures[0] = ModAsset.Mycelium.Value;
			Main.graphics.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
		return false;
	}

	public void DrawShell(VFXBatch batch)
	{
		foreach (var lichen in Lichens)
		{
			if (!lichen.active)
			{
				continue;
			}
			Vector2 center = lichen.attachTarget.Center + lichen.attachPos;
			float timeValue = (float)(Main.timeForVisualEffects * 0.008f + lichen.ai0 * 0.142f);
			float mulSize = 1f + MathF.Sin(timeValue * 4f + lichen.ai0) * 0.07f;
			var baseColor = new Color(0.3f, 1f, 0.6f, 1);
			var triangleList = new List<Vertex2D>();
			float radius = 36 * mulSize * lichen.scale;

			triangleList.Add(new Vertex2D(center - new Vector2(radius, radius).RotatedBy(lichen.rotation), baseColor, new Vector3(-1, 1, timeValue)));
			triangleList.Add(new Vertex2D(center - new Vector2(radius, -radius).RotatedBy(lichen.rotation), baseColor, new Vector3(-1, -1, timeValue)));
			triangleList.Add(new Vertex2D(center - new Vector2(-radius, -radius).RotatedBy(lichen.rotation), baseColor, new Vector3(1, -1, timeValue)));

			triangleList.Add(new Vertex2D(center - new Vector2(radius, radius).RotatedBy(lichen.rotation), baseColor, new Vector3(-1, 1, timeValue)));
			triangleList.Add(new Vertex2D(center - new Vector2(-radius, -radius).RotatedBy(lichen.rotation), baseColor, new Vector3(1, -1, timeValue)));
			triangleList.Add(new Vertex2D(center - new Vector2(-radius, radius).RotatedBy(lichen.rotation), baseColor, new Vector3(1, 1, timeValue)));

			batch.Draw(ModAsset.LichenNoise.Value, triangleList, PrimitiveType.TriangleList);
		}
	}
}

internal class LichensPycnidiumManager : ILoadable
{
	public void Load(Mod mod)
	{
		var hookManager = Ins.HookManager;
		hookManager.AddHook(CodeLayer.PostDrawProjectiles, DrawLichensPycnidium);
	}

	private void DrawLichensPycnidium()
	{
		float timeValue = (float)(Main.timeForVisualEffects * 0.008f);
		Ins.Batch.Begin(BlendState.NonPremultiplied, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		Effect sphere = ModAsset.Pycnidium_SpherePerspective.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;

		sphere.Parameters["uTransform"].SetValue(model * projection);
		sphere.Parameters["circleCenter"].SetValue(new Vector3(0, 0, 2));
		sphere.Parameters["radiusOfCircle"].SetValue(1f);
		sphere.Parameters["uTime"].SetValue(timeValue * 0.4f);
		sphere.Parameters["uWarp"].SetValue(0.06f);

		sphere.CurrentTechnique.Passes[0].Apply();

		foreach (Projectile proj in Main.projectile)
		{
			if (proj.active)
			{
				if (proj.ModProjectile is LichensPycnidium modProj2)
				{
					modProj2.DrawShell(Ins.Batch);
				}
			}
		}
		Ins.Batch.End();
	}

	public void Unload()
	{
	}
}
