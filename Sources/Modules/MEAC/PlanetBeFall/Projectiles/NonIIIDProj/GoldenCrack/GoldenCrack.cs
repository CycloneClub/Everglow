using Everglow.Commons.IIID;
using Everglow.Commons.Vertex;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Utilities;

namespace Everglow.MEAC.PlanetBeFall.Projectiles.NonIIIDProj.GoldenCrack
{
	public class Tree
	{
		public int cnt;
		public int brunchnum;
		public Node[] Nodes;
		public Node root;
		public UnifiedRandom random;
		public Vector2 vel;

		public Tree(UnifiedRandom random, Vector2 vel)
		{
			cnt = 0;
			brunchnum = 0;
			root = null;
			this.random = random;
			this.vel = vel;
		}

		public class Node
		{
			public int num;
			public bool ismaster;
			public bool isbrunch;
			public float rad;
			public float size;
			public float length;
			public bool isterminal;
			public bool isfork;
			public Vector2 position;
			public List<Node> children;

			public Node(int num, float rad, float size, float length, bool isterminal, bool ismaster, bool isbrunch, bool isfork, Vector2 position)
			{
				this.num = num;
				this.ismaster = ismaster;
				this.isbrunch = isbrunch;
				this.rad = rad;
				this.size = size;
				this.length = length;
				this.isterminal = isterminal;
				this.isfork = isfork;
				this.position = position;
				children = new List<Node>();
			}
		}

		public Vector2 Position(Vector2 pos, Vector2 vel, Node node)
		{
			return pos + (vel.ToRotation() + node.rad).ToRotationVector2() * node.length;
		}

		private float Rand(float range)
		{
			return random.NextFloatDirection() * range;
		}

		public static float Rand()
		{
			UnifiedRandom random = Main.rand;
			double u = -2 * Math.Log(random.NextDouble());
			double v = 2 * Math.PI * random.NextDouble();
			return Math.Clamp((float)Math.Max(0, Math.Sqrt(u) * Math.Cos(v) * 0.3 + 0.5) + 0.4f, 0.7f, 1f);
		}

		public void Generate()
		{
			root = new Node(0, 0, Rand(), 300 * Rand(), false, true, false, false, Vector2.Zero);
			root = Buildmaster(root, 5);
			Nodes = new Node[cnt + 1];
		}

		private Node Buildmaster(Node node, int dep)
		{
			cnt++;

			if (dep == 10)
			{
				node.isterminal = true;
				return node;
			}

			if (Main.rand.NextBool(5) || dep == 5)
			{
				var rad = Rand(MathHelper.Pi / 4f);
				var brunch = new Node(cnt, node.rad > 0 ? Math.Abs(rad) : -Math.Abs(rad), node.size * Rand(), node.length * Rand(), false, false, true, false, Position(node.position, vel, node));
				node.isfork = true;
				node.children.Add(Buildbrunch(brunch, 0));
			}

			// 参数修改了
			var master = new Node(cnt, Rand(MathHelper.Pi / 6f), node.size * Rand(), node.length * Rand() * 0.9f, false, true, false, false, Position(node.position, vel, node));
			node.children.Add(Buildmaster(master, dep + 1));
			return node;
		}

		private Node Buildbrunch(Node node, int dep)
		{
			cnt++;
			brunchnum++;
			if (dep == 2)
			{
				node.isterminal = true;
				return node;
			}
			var child = new Node(cnt, Rand(MathHelper.Pi / 6f), node.size * Rand(), node.length * Rand() * 0.9f, false, false, true, false, Position(node.position, vel, node));
			node.children.Add(Buildbrunch(child, dep + 1));
			return node;
		}

		public void Array(Node node)
		{
			Nodes[node.num] = node;
			foreach (var child in node.children)
			{
				Array(child);
			}
		}
	}

	public class GoldenCrack : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 6;
			Projectile.height = 20;
			Projectile.aiStyle = -1;
			Projectile.timeLeft = 180;
			Projectile.tileCollide = false;
			Projectile.scale = 0.8f;
			Projectile.alpha = 60;
			Projectile.hostile = false;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.hide = true;
		}

		private Tree tree;
		private Vector2[] nodePosition;
		private int j;
		private int terminal;
		private bool back = false;
		private bool[] terminals;

		public override void AI()
		{
			if (Projectile.ai[0] == 0)
			{
				tree = new Tree(Main.rand, Projectile.velocity);
				tree.Generate();
				if (tree.root != null)
				{
					tree.Array(tree.root);
				}

				nodePosition = new Vector2[tree.Nodes.Length + tree.brunchnum];
				terminals = new bool[tree.Nodes.Length + tree.brunchnum];
				for (int i = 0; i < nodePosition.Length; i++)
				{
					nodePosition[i] = tree.Nodes[j].position;
					terminals[i] = false;
					if (tree.Nodes[j].isterminal)
					{
						terminal = j;
						terminals[i] = true;
						if (tree.Nodes[j].isbrunch)
						{
							back = true;
						}
						else
						{
							j--;
						}
					}
					if (!back)
					{
						j++;
					}
					else
					{
						j--;
					}
					if (back && tree.Nodes[j].isfork)
					{
						back = false;
						j = terminal + 1;
					}
				}
				Projectile.ai[0]++;
			}
			if (Projectile.timeLeft >= 28)
			{
				Projectile.ai[1] += 1f / 60f;
			}
			else
			{
				Projectile.ai[1] -= 1f / 24f;
			}

			if (Projectile.ai[1] >= 1)
			{
				Projectile.ai[1] = 1;
			}
			Projectile.ai[0]++;
		}

		public override bool PreDraw(Player player, ref Color lightColor)
		{
			var vertices = new List<Vertex2D>();
			Color color = Color.Gold;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(0, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			for (int i = 0; i < nodePosition.Length - 1; i++)
			{
				if (nodePosition[i] + Projectile.Center != Vector2.Zero)
				{
					if (terminals[i])
					{
						vertices.Add(new Vertex2D(nodePosition[i] + Projectile.Center - Main.screenPosition, color, new Vector3(1f, 0f, 1f)));

						// vertices.Add(new Vertex2D(Main.LocalPlayer.Center - Main.screenPosition, new Vector3(1f, 0f, 1f), new Color(200, 100, 200)));
					}
					else
					{
						Vector2 normalDir = nodePosition[i] - nodePosition[i + 1];
						normalDir = Vector2.Normalize(new Vector2(0f - normalDir.Y, normalDir.X));
						float width = Math.Clamp(Projectile.ai[0] * 25 - nodePosition[i].Length(), 0, 10) * Projectile.ai[1];
						vertices.Add(new Vertex2D(nodePosition[i] + Projectile.Center + (i == 0 ? 6 : 1) * normalDir * width * (float)Math.Pow(0.9f, i) - Main.screenPosition, color, new Vector3(1f, 0f, 1f)));
						vertices.Add(new Vertex2D(nodePosition[i] + Projectile.Center - (i == 0 ? 6 : 1) * normalDir * width * (float)Math.Pow(0.9f, i) - Main.screenPosition, color, new Vector3(1f, 0f, 1f)));
					}
				}
			}
			Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
			if (vertices.Count > 2)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			return false;
		}
	}
}
