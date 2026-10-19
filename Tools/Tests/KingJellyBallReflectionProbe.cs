using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Everglow.Tools.Tests;

// Standalone GPU probe: deliberately does not load Terraria.Main or the game world.
internal static class KingJellyBallReflectionProbe
{
	public static int Main(string[] args)
	{
		using var probe = new ReflectionProbe(args[0], args[1], args.Contains("--legacy"));
		probe.RunOneFrame();
		return probe.Failures == 0 ? 0 : 1;
	}

	private sealed class ReflectionProbe : Game
	{
		private const int Size = 1024;
		private readonly string root;
		private readonly string configuration;
		private readonly bool legacy;
		private readonly GraphicsDeviceManager manager;

		public ReflectionProbe(string root, string configuration, bool legacy)
		{
			this.root = root;
			this.configuration = configuration;
			this.legacy = legacy;
			manager = new GraphicsDeviceManager(this)
			{
				GraphicsProfile = GraphicsProfile.HiDef,
				PreferredBackBufferWidth = Size,
				PreferredBackBufferHeight = Size,
			};
		}

		public int Failures { get; private set; }

		protected override void Initialize()
		{
			base.Initialize();
			string project = legacy ? "Sources/Everglow.Function" : "Sources/Modules/Yggdrasil";
			using var content = new ContentManager(Services, Path.Combine(root, project, "bin", configuration, "net8.0/Assets"));
			Effect effect = content.Load<Effect>(legacy
				? "VFX/Effects/ScreenReflection_NormalColor"
				: "YggdrasilTown/VFXs/KingJellyBall_SurfaceReflection");
			using var file = File.OpenRead(Path.Combine(root, "Sources/Modules/Yggdrasil/YggdrasilTown/NPCs/KingJellyBall/KingJellyBall_Reflect.png"));
			using var normals = Texture2D.FromStream(GraphicsDevice, file);
			using var source = new Texture2D(GraphicsDevice, Size, Size, false, SurfaceFormat.Vector4);
			var ramp = new Vector4[Size * Size];
			for (int y = 0; y < Size; y++)
			{
				for (int x = 0; x < Size; x++)
				{
					// Encode source coordinates in color, then read the real shader's mapping back.
					ramp[y * Size + x] = new Vector4((x + 0.5f) / Size, (y + 0.5f) / Size, 0.25f, 1);
				}
			}
			source.SetData(ramp);
			using var target = new RenderTarget2D(GraphicsDevice, Size, Size, false, SurfaceFormat.Vector4, DepthFormat.None);
			foreach (float scale in new[] { 1f, 0.3f })
			{
				Vector4[] plain = Render(effect, source, normals, target, scale, 0, 0);
				int delta = (int)(20 * scale);
				Vector4 left = Sample(plain, 512 - delta, 512);
				Vector4 right = Sample(plain, 512 + delta, 512);
				Vector4 top = Sample(plain, 512, 512 - delta);
				Vector4 bottom = Sample(plain, 512, 512 + delta);
				float width = 2 * delta / ((right.X - left.X) * Size);
				float height = 2 * delta / ((bottom.Y - top.Y) * Size);
				Check(width is > 0.8f and < 1.2f && height is > 0.8f and < 1.2f,
					$"Center scale={scale}: width={width:F3}, height={height:F3} (both 0.8..1.2)");

				float previous = float.NegativeInfinity;
				bool monotonic = true;
				for (int offset = (int)(-170 * scale); offset <= (int)(170 * scale); offset++)
				{
					float value = Sample(plain, 512, 512 + offset).Y;
					monotonic &= float.IsFinite(value) && value + 0.000001f >= previous;
					previous = value;
				}
				Check(monotonic, $"Vertical reflection does not fold, scale={scale}");
				Vector4 sideLeft = Sample(plain, 512 + (int)(160 * scale), 512);
				Vector4 sideRight = Sample(plain, 512 + (int)(180 * scale), 512);
				float sideWidth = 20 * scale / ((sideRight.X - sideLeft.X) * Size);
				Check(sideWidth > 0 && sideWidth < width - 0.05f, $"Side compresses: {sideWidth:F3} vs center {width:F3}");

				Vector4[] wave0 = Render(effect, source, normals, target, scale, 0, 0.65f);
				Vector4[] wave1 = Render(effect, source, normals, target, scale, 1, 0.65f);
				float movement = 0;
				float distortion = 0;
				bool smooth = true;
				for (int y = -100; y <= 100; y += 5)
				{
					for (int x = -100; x <= 100; x += 5)
					{
						int px = 512 + (int)(x * scale);
						int py = 512 + (int)(y * scale);
						Vector2 original = Coordinates(Sample(plain, px, py));
						Vector2 first = Coordinates(Sample(wave0, px, py));
						Vector2 second = Coordinates(Sample(wave1, px, py));
						movement = Math.Max(movement, Vector2.Distance(first, second));
						distortion = Math.Max(distortion, Vector2.Distance(original, first));
						smooth &= Sample(wave0, px + 1, py).X > Sample(wave0, px, py).X
							&& Sample(wave0, px, py + 1).Y > Sample(wave0, px, py).Y;
					}
				}
				Check(movement > 0.05f && distortion < 6 * scale,
					$"Ripples animate and stay subtle: movement={movement:F3}px, distortion={distortion:F3}px");
				Check(smooth, $"Ripples preserve local image orientation, scale={scale}");
			}

			Vector4[] reference = Render(effect, source, normals, target, 1, 0, 0);
			foreach (float zoom in new[] { 0.75f, 1.5f })
			{
				Vector4[] zoomed = Render(effect, source, normals, target, 1, 0, 0, zoom);
				Vector2 intersection = Coordinates(Sample(reference, 552, 532));
				Vector2 expected = new Vector2(512) + (intersection - new Vector2(512)) * zoom;
				Vector2 actual = Coordinates(Sample(zoomed, 512 + (int)(40 * zoom), 512 + (int)(20 * zoom)));
				Check(Vector2.Distance(expected, actual) < 1.5f, $"Zoom={zoom} transforms the intersection once");
			}

			Vector4[] edge = Render(effect, source, normals, target, 1, 0, 0, center: new Vector2(5, 512));
			Vector4 edgeSample = Sample(edge, 1, 512);
			Check(edgeSample.W < 0.1f && edgeSample.X < 0.01f, "Screen edge fades without sampling the opposite edge");
			Vector4[] grazing = Render(effect, source, normals, target, 1, 0, 0, radius: 300);
			Check(Sample(grazing, 770, 512).W < 0.01f, "Grazing and backward rays fade out");
			Console.WriteLine($"GPU regression failures: {Failures}");
			Exit();
		}

		private Vector4[] Render(Effect effect, Texture2D source, Texture2D normals, RenderTarget2D target,
			float scale, float time, float strength, float zoom = 1, Vector2? center = null, float radius = 240)
		{
			Vector2 origin = center ?? new Vector2(512);
			GraphicsDevice device = GraphicsDevice;
			device.SetRenderTarget(target);
			device.Clear(Color.Transparent);
			device.BlendState = BlendState.Opaque;
			device.DepthStencilState = DepthStencilState.None;
			device.RasterizerState = RasterizerState.CullNone;
			Matrix view = Matrix.CreateTranslation(-512, -512, 0) * Matrix.CreateScale(zoom, zoom, 1) * Matrix.CreateTranslation(512, 512, 0);
			effect.Parameters["uModel"]?.SetValue(Matrix.Identity);
			effect.Parameters["uMNormal"]?.SetValue(Matrix.Identity);
			effect.Parameters["uViewProj"]?.SetValue(view * Matrix.CreateOrthographicOffCenter(0, Size, Size, 0, 0, 500));
			effect.Parameters["uViewportSize"]?.SetValue(new Vector2(Size));
			effect.Parameters["uScreenDistanceMultipler"]?.SetValue(Vector2.One);
			effect.Parameters["uFresnelF0"]?.SetValue(Vector3.One);
			effect.Parameters["uKs"]?.SetValue(Vector3.One);
			effect.Parameters["uSurfaceCenter"]?.SetValue(origin);
			effect.Parameters["uSurfaceScale"]?.SetValue(scale);
			effect.Parameters["uTime"]?.SetValue(time);
			effect.Parameters["uRippleStrength"]?.SetValue(strength);
			effect.Parameters["uScreenTransform"]?.SetValue(view);
			effect.CurrentTechnique.Passes[0].Apply();
			device.Textures[0] = source;
			device.Textures[1] = normals;
			device.SamplerStates[0] = legacy ? SamplerState.PointWrap : SamplerState.LinearClamp;
			device.SamplerStates[1] = SamplerState.PointClamp;
			var vertices = new List<VertexPositionColorTexture>();
			for (int i = 0; i <= 150; i++)
			{
				float angle = i * MathHelper.TwoPi / 150;
				var direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
				vertices.Add(new VertexPositionColorTexture(new Vector3(origin, -150), Color.White, new Vector2(0.5f)));
				vertices.Add(new VertexPositionColorTexture(new Vector3(origin + direction * radius * scale, -10),
					Color.White, new Vector2(0.5f) + direction * 0.27f));
			}
			device.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
			device.SetRenderTarget(null);
			var data = new Vector4[Size * Size];
			target.GetData(data);
			return data;
		}

		private static Vector4 Sample(Vector4[] data, int x, int y) => data[y * Size + x];

		private static Vector2 Coordinates(Vector4 color) => new Vector2(color.X, color.Y) * Size;

		private void Check(bool condition, string message)
		{
			Console.WriteLine((condition ? "PASS " : "FAIL ") + message);
			if (!condition)
			{
				Failures++;
			}
		}
	}
}
