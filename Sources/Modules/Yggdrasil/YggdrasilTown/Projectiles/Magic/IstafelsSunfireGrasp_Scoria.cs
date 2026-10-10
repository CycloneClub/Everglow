using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class IstafelsSunfireGrasp_Scoria : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public const int ProjectileSize = 20;
	public const int TimeLeftMax = 400;
	public const float Gravity = 0.2f;
	public const int ProjectileVelocityYMax = 16;

	public const int ContactDamage = 25;
	public const int BuffDuration = 960;

	public const int FlowDistance = 40;
	public const int NodeCount = 100;

	/// <summary>
	/// The timeleft where scoria is cooled. After the scoria is cooled, it can no longer deal damage, and its appearance will also be adjusted.
	/// </summary>
	public const int CooledTimeLeft = 100;

	/// <summary>
	/// A random value used for visual effects, initialized in <see cref="OnSpawn"/>.
	/// </summary>
	private ref float VFXRand => ref Projectile.localAI[0];

	/// <summary>
	/// State symbol, default to <c>true</c>, set to <c>false</c> when the projectile firstly colliding with a tile.
	/// <list type="number">
	/// <item>
	/// 	If <c>false</c>, the scoria is moving like a stone.
	/// </item>
	/// <item>
	/// 	If <c>true</c>, the scoria try to flowing based on gravity and terrain.
	/// </item>
	/// </list>
	/// </summary>
	private bool HasNotCollideTile { get; set; } = true;

	/// <summary>
	/// State symbol, default to <c>true</c>, set to <c>false</c> when the projectile stop flowing.
	/// </summary>
	private bool ScoriaAI { get; set; } = true;

	/// <summary>
	/// The progress of scoria life. <c>Progress = Projectile.timeleft / TimeLeftMax;</c>
	/// </summary>
	private float Progress => Math.Clamp(Projectile.timeLeft / (float)TimeLeftMax, 0, 1);

	/// <summary>
	/// Left part shape nodes.
	/// </summary>
	private Vector2[] NodesLeft { get; } = new Vector2[NodeCount];

	/// <summary>
	/// Right part shape nodes.
	/// </summary>
	private Vector2[] NodesRight { get; } = new Vector2[NodeCount];

	/// <summary>
	/// Where the scoria stop flow left at.
	/// </summary>
	private Vector2 EndPosLeft { get; set; } = Vector2.Zero;

	/// <summary>
	/// Where the scoria stop flow right at.
	/// </summary>
	private Vector2 EndPosRight { get; set; } = Vector2.Zero;

	public override void SetDefaults()
	{
		Projectile.width = ProjectileSize;
		Projectile.height = ProjectileSize;

		Projectile.DamageType = DamageClass.Default;

		Projectile.timeLeft = TimeLeftMax * 2;

		Projectile.friendly = true;
		Projectile.hostile = false;

		Projectile.tileCollide = true;
		Projectile.penetrate = -1;
		Projectile.localNPCHitCooldown = 20;

		Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCsAndTiles;
		Projectile.usesOwnerLight = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		VFXRand = Main.rand.NextFloat(3, 7);
	}

	public override void AI()
	{
		Projectile.ai[0]++;
		Lighting.AddLight(Projectile.Center, new Color(1f, 0.8f, 0f).ToVector3() * Progress);
		foreach (var pos in NodesLeft.Concat(NodesRight))
		{
			Lighting.AddLight(pos, new Color(1f, 0.8f, 0f).ToVector3() * Progress);
		}

		if (HasNotCollideTile)
		{
			Projectile.rotation += Projectile.velocity.X * 0.1f;

			// Simulate gravity
			Projectile.velocity.Y += Gravity;
			if (Projectile.velocity.Y > ProjectileVelocityYMax)
			{
				Projectile.velocity.Y = ProjectileVelocityYMax;
			}

			if (Main.rand.NextBool(10))
			{
				var d0 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.RedTorch, Projectile.velocity.X, Projectile.velocity.Y, Scale: 1.2f);
				d0.noGravity = true;
			}
		}
		else
		{
			// Try to flow scoria to the both sides.
			if (ScoriaAI)
			{
				// Calculate EndPosLeft and EndPosRight
				var leftLength = EndPosLeft - Projectile.Center;
				if (Math.Abs(leftLength.X) + Math.Abs(leftLength.Y) <= FlowDistance && !Collision.SolidCollision(EndPosLeft, 1, 1))
				{
					if (Collision.SolidCollision(EndPosLeft + Vector2.UnitY * 8, 1, 1))
					{
						EndPosLeft += Vector2.UnitX * -2;
					}
					else
					{
						EndPosLeft += Vector2.UnitY * 2;
					}
				}

				var rightLength = EndPosRight - Projectile.Center;
				if (Math.Abs(rightLength.X) + Math.Abs(rightLength.Y) <= FlowDistance && !Collision.SolidCollision(EndPosRight, 1, 1))
				{
					if (Collision.SolidCollision(EndPosRight + Vector2.UnitY * 8, 1, 1))
					{
						EndPosRight += Vector2.UnitX * 2;
					}
					else
					{
						EndPosRight += Vector2.UnitY * 2;
					}
				}

				CalculateNodes(NodesLeft, EndPosLeft);
				CalculateNodes(NodesRight, EndPosRight);
			}

			// Generate dusts before scoria cooled.
			if (Projectile.timeLeft > CooledTimeLeft)
			{
				foreach (var nodePos in NodesLeft.Concat(NodesRight).Where(x => x != Vector2.Zero))
				{
					if (Main.rand.NextBool(800))
					{
						var d0 = Dust.NewDustDirect(nodePos, 2, 2, DustID.RedTorch, Projectile.velocity.X, Projectile.velocity.Y, Scale: 1.2f);
						d0.noGravity = true;
					}
				}
			}
		}
	}

	/// <summary>
	/// Calculates the positions of nodes for the scoria's flow effect.
	/// </summary>
	/// <param name="nodes">The array of nodes to update.</param>
	/// <param name="endPos">The target end position for the nodes.</param>
	private void CalculateNodes(Vector2[] nodes, Vector2 endPos)
	{
		for (int i = 0; i < NodeCount; i++)
		{
			if (nodes[i] == Vector2.Zero)
			{
				if ((nodes[i - 1] - endPos).Length() is > 4 and < 20)
				{
					nodes[i] = endPos;
					Projectile.ai[1] = 0;
				}
				else if ((nodes[i - 1] - endPos).Length() is >= 20 and < 60)
				{
					nodes[i] = (endPos + nodes[i - 1]) * 0.5f;
					nodes[i + 1] = endPos;
					Projectile.ai[1] = 0;
				}
				else if ((nodes[i - 1] - endPos).Length() >= 60 && i > 4)
				{
					ScoriaAI = false;
					Projectile.ai[1] = 0;
				}
				else if ((nodes[i - 1] - endPos).Length() <= 4)
				{
					Projectile.ai[1]++;
					if (i < 8 && Projectile.ai[1] > 4)
					{
						float randomRot = Main.rand.NextFloat(MathHelper.TwoPi);
						for (int j = 0; j < 9; j++)
						{
							nodes[j] = endPos + new Vector2(j - 4).RotatedBy(randomRot) * 5;
							ScoriaAI = false;
						}
					}
				}

				break;
			}
		}
	}

	public override bool? CanCutTiles() => true;

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (HasNotCollideTile)
		{
			// Stick on the tile after colliding.
			Projectile.velocity = Vector2.Zero;
			Projectile.tileCollide = false;
			HasNotCollideTile = false;

			// Initial nodes
			NodesLeft[0] = Projectile.Center;
			NodesRight[0] = Projectile.Center;

			EndPosLeft = Projectile.Center + Vector2.UnitX * -2;
			EndPosRight = Projectile.Center + Vector2.UnitX * 2;

			// Draw dusts on collide tile
			for (int i = 0; i < 40; i++)
			{
				var offset = new Vector2(Main.rand.NextFloat(16), 0).RotatedBy(Main.rand.NextFloat() * MathHelper.TwoPi);
				var d0 = Dust.NewDustDirect(Projectile.Center + offset, 4, 4, DustID.LavaMoss, Scale: 1.4f);
				d0.noGravity = true;
			}
			for (int i = 0; i < 20; i++)
			{
				var d0 = Dust.NewDustDirect(Projectile.Center, Projectile.width / 2, Projectile.height / 2, DustID.Torch, Scale: 1.2f);
				d0.noGravity = true;
			}
		}

		return false;
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		// Let the projectile deals true damage, minus base damage 1
		modifiers.FinalDamage.Flat += ContactDamage - 1;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(ModContent.BuffType<Charred>(), BuffDuration);
	}

	public override void OnKill(int timeLeft)
	{
		foreach (var pos in NodesLeft.Concat(NodesRight))
		{
			for (int i = 0; i < 2; i++)
			{
				var d0 = Dust.NewDustDirect(pos, 0, 0, DustID.Wraith, Projectile.velocity.X, Projectile.velocity.Y, Scale: 1f);
				d0.noGravity = true;
				d0.velocity = d0.velocity.RotatedByRandom(MathHelper.TwoPi);
			}
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (Projectile.timeLeft < CooledTimeLeft)
		{
			return false;
		}

		for (int i = 0; i < NodesLeft.Length - 1; i++)
		{
			if (NodesLeft[i + 1] == default)
			{
				return false;
			}
			float value = 0;
			if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), NodesLeft.ToArray()[i], NodesLeft.ToArray()[i + 1], 20, ref value))
			{
				return true;
			}
		}
		return false;
	}



	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		if (HasNotCollideTile)
		{
			var effect = ModAsset.IstafelsSunfireGrasp_ScoriaVFX.Value;
			effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
			effect.Parameters["uScale"].SetValue(10f);
			effect.Parameters["uRand"].SetValue(VFXRand);
			effect.Parameters["uProgress"].SetValue(Progress);
			effect.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_Sand.Value);
			effect.CurrentTechnique.Passes["NotCollided"].Apply();

			var drawPos = Projectile.Center - Main.screenPosition;
			var drawSize = Projectile.width;
			var drawColor = Color.Lerp(lightColor, Color.White, Progress * Progress);
			var vertices = new List<Vertex2D>
			{
				{ drawPos + new Vector2(-1, -1).RotatedBy(Projectile.rotation) * drawSize, drawColor, new(0, 0, 0) },
				{ drawPos + new Vector2(1, -1).RotatedBy(Projectile.rotation) * drawSize, drawColor, new(1, 0, 0) },
				{ drawPos + new Vector2(-1, 1).RotatedBy(Projectile.rotation) * drawSize, drawColor, new(0, 1, 0) },
				{ drawPos + new Vector2(1, 1).RotatedBy(Projectile.rotation) * drawSize, drawColor, new(1, 1, 0) },
			};

			Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(Texture).Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
		}
		else
		{
			var verticesLeft = CalcNodesLeft(NodesLeft);
			var verticesRight = CalcNodesRight(NodesRight);

			// Combine left nodes and right nodes into one collection
			DrawVertices(
				verticesLeft.Bars.Skip(2).Reverse().Select(x =>
				{
					x.texCoord.X *= -1;
					return x;
				}).Concat(verticesRight.Bars).ToList(),
				verticesLeft.BarsReflect.Skip(2).Reverse().Select(x =>
				{
					x.texCoord.X *= -1;
					return x;
				}).Concat(verticesRight.BarsReflect).ToList());
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		return false;
	}

	private (IEnumerable<Vertex2D> Bars, IEnumerable<Vertex2D> BarsReflect) CalcNodesLeft(Vector2[] nodes)
	{
		var bars = new List<Vertex2D>();

		var barsReflect = new List<Vertex2D>();

		int length = 0;
		for (int i = 0; i < nodes.Length - 1; i++)
		{
			if (nodes[i + 1] != Vector2.Zero)
			{
				length++;
			}
		}

		for (int i = 0; i < nodes.Length - 1; i++)
		{
			float width = 12 + 2 * MathF.Sin(i * 1.1f);
			float offset = 0f;
			if (length - i - 1 < 2)
			{
				width *= (length - i - 1) / 2f;
				width += 2f;
				offset += 3f;
			}
			width *= i / (float)length * 0.3f + 0.7f;
			if (nodes[0].Y != nodes[1].Y && nodes[1] != Vector2.Zero)
			{
				nodes[0].Y = nodes[1].Y;
			}
			if (nodes[i + 1] != Vector2.Zero)
			{
				Vector2 normal = (nodes[i + 1] - nodes[i]).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);
				AddVertex(bars, nodes[i] + normal * width + new Vector2(0, offset), new Vector3(i * 0.1f, 1, 0));
				AddVertex(bars, nodes[i] - normal * width + new Vector2(0, offset), new Vector3(i * 0.1f, 0f, 0));

				AddVertexReflect(barsReflect, nodes[i] + normal * width + new Vector2(0, offset), new Vector3(i * 0.1f, 1, 0));
				AddVertexReflect(barsReflect, nodes[i] - normal * width + new Vector2(0, offset), new Vector3(i * 0.1f, 0f, 0));
			}
			else
			{
				break;
			}
		}

		return (bars, barsReflect);
	}

	private (IEnumerable<Vertex2D> Bars, IEnumerable<Vertex2D> BarsReflect) CalcNodesRight(Vector2[] nodes)
	{
		var bars = new List<Vertex2D>();

		var barsReflect = new List<Vertex2D>();

		int length = 0;
		for (int i = 0; i < nodes.Length - 1; i++)
		{
			if (nodes[i + 1] != Vector2.Zero)
			{
				length++;
			}
		}

		for (int i = 0; i < nodes.Length - 1; i++)
		{
			float width = 12 + 2 * MathF.Sin(i * 1.1f);
			float offset = 0f;
			if (length - i - 1 < 2)
			{
				width *= (length - i - 1) / 2f;
				width += 2f;
				offset += 3f;
			}
			width *= i / (float)length * 0.3f + 0.7f;
			if (nodes[0].Y != nodes[1].Y && nodes[1] != Vector2.Zero)
			{
				nodes[0].Y = nodes[1].Y;
			}
			if (nodes[i + 1] != Vector2.Zero)
			{
				Vector2 normal = (nodes[i + 1] - nodes[i]).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);
				AddVertex(bars, nodes[i] + normal * width + new Vector2(0, offset), new Vector3(i * 0.1f, 0, 0));
				AddVertex(bars, nodes[i] - normal * width + new Vector2(0, offset), new Vector3(i * 0.1f, 1f, 0));

				AddVertexReflect(barsReflect, nodes[i] + normal * width + new Vector2(0, offset), new Vector3(i * 0.1f, 0, 0));
				AddVertexReflect(barsReflect, nodes[i] - normal * width + new Vector2(0, offset), new Vector3(i * 0.1f, 1f, 0));
			}
			else
			{
				break;
			}
		}

		return (bars, barsReflect);
	}

	/// <summary>
	/// Draws the vertices for the scoria's flow effect using a custom shader.
	/// </summary>
	/// <param name="bars">The vertices for the main flow.</param>
	/// <param name="barsReflect">The vertices for the reflected flow.</param>
	private void DrawVertices(IEnumerable<Vertex2D> bars, IEnumerable<Vertex2D> barsReflect)
	{
		if (bars.Count() > 2)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			var effect = ModAsset.IstafelsSunfireGrasp_ScoriaVFX.Value;
			effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.002f);
			effect.Parameters["uScale"].SetValue(.5f);
			effect.Parameters["uRand"].SetValue(VFXRand);
			effect.Parameters["uProgress"].SetValue(Progress);
			effect.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_Sand.Value);
			effect.CurrentTechnique.Passes[0].Apply();

			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_lava.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count() - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, barsReflect.ToArray(), 0, barsReflect.Count() - 2);

			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.EmptyCrystal.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count() - 2);
		}
	}

	/// <summary>
	/// Adds a vertex to the list for rendering, with color based on lighting and progress.
	/// </summary>
	/// <param name="bars">The list of vertices to add to.</param>
	/// <param name="position">The position of the vertex.</param>
	/// <param name="texCoord">The texture coordinate of the vertex.</param>
	public void AddVertex(List<Vertex2D> bars, Vector2 position, Vector3 texCoord)
	{
		var tilePos = position.ToTileCoordinates();
		var baseColor = Lighting.GetColor(tilePos);
		var color = Color.Lerp(baseColor * 0.6f, new Color(1f, 1f, 0.4f, 0.9f), Progress);
		bars.Add(position - Main.screenPosition, color, texCoord);
	}

	/// <summary>
	/// Adds a vertex to the list for rendering, with enhanced lighting for reflection effects.
	/// </summary>
	/// <param name="bars">The list of vertices to add to.</param>
	/// <param name="position">The position of the vertex.</param>
	/// <param name="texCoord">The texture coordinate of the vertex.</param>
	public void AddVertexReflect(List<Vertex2D> bars, Vector2 position, Vector3 texCoord)
	{
		bars.Add(position - Main.screenPosition, Lighting.GetColor(position.ToTileCoordinates()) * 1.5f, texCoord);
	}
}
