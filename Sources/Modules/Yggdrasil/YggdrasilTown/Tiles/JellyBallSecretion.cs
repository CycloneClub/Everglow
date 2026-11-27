using System.Collections.Generic;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria.Audio;
using Terraria.GameContent.Drawing;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class JellyBallSecretion : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		TileID.Sets.NegatesFallDamage[Type] = true;
		Main.tileMergeDirt[Type] = false;

		Main.tileBlendAll[Type] = false;
		Main.tileBlockLight[Type] = false;
		Main.tileShine2[Type] = false;
		Main.tileMerge[Type][ModContent.TileType<JellyBallNerve>()] = true;
		Main.tileMerge[Type][ModContent.TileType<JellyBallNerve_Thin>()] = true;
		Main.tileMerge[Type][ModContent.TileType<JellyBallSecretion_Fresh>()] = true;

		DustType = ModContent.DustType<JellyBallGel>();
		HitSound = SoundID.NPCHit1;

		AddMapEntry(new Color(57, 70, 181));
	}

	internal static bool IsJelly(Point p) => WorldGen.InWorld(p.X, p.Y, 1)
		&& Main.tile[p.X, p.Y].HasUnactuatedTile && Main.tile[p.X, p.Y].TileType == ModContent.TileType<JellyBallSecretion>();

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (Main.dedServ)
		{
			return true;
		}

		Lighting.AddLight(i, j, 0, 0.5f, 0.94f);
		var tile = Main.tile[i, j];
		var ripples = ModContent.GetInstance<JellyRipples>();
		var point = new Point(i, j);
		if (tile.IsHalfBlock || tile.Slope != SlopeType.Solid || !tile.HasUnactuatedTile || !TileDrawing.IsVisible(tile) || !ripples.TryGetWave(point, out Vector2 squeeze))
		{
			return true;
		}

		Vector2 offset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
		Vector2 position = point.ToVector2() * 16 - Main.screenPosition + offset;
		var texture = Main.instance.TilesRenderer.GetTileDrawTexture(tile, i, j);
		var light = tile.IsTileFullbright ? Color.White : Lighting.GetColor(i, j);
		var tint = Color.Lerp(light, Color.White, Math.Min(squeeze.Length() * 0.035f, 0.22f));
		var frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
		// Positive squeeze compresses an axis. Shared faces stay anchored so the tiles stay joined.
		float left = WorldGen.SolidTile(i - 1, j) ? 0f : squeeze.X;
		float right = WorldGen.SolidTile(i + 1, j) ? 0f : squeeze.X;
		float top = WorldGen.SolidTile(i, j - 1) ? 0f : squeeze.Y;
		float bottom = WorldGen.SolidTile(i, j + 1) ? 0f : squeeze.Y;
		var scale = new Vector2(1f - (left + right) / 16f, 1f - (top + bottom) / 16f);
		spriteBatch.Draw(texture, position + new Vector2(left, top), frame, tint, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		return false;
	}

	public override void RandomUpdate(int i, int j)
	{
		//Tile tile = Main.tile[i, j];
		//Tile tileLeft = Main.tile[i + 1, j];
		//Tile tileRight = Main.tile[i - 1, j];
		//if (tile.Slope == SlopeType.Solid && tileLeft.TileType == Type && tileRight.TileType == Type && tileLeft.HasTile && tileRight.HasTile)
		//{
		//	if (Main.rand.NextBool(1))
		//	{
		//		Tile t2 = Main.tile[i, j + 1];
		//		if (!t2.HasTile)
		//		{
		//			t2.TileType = (ushort)ModContent.TileType<JellyBeadString>();
		//			t2.HasTile = true;
		//		}
		//	}
		//	if (Main.rand.NextBool(1))
		//	{
		//		Tile t2 = Main.tile[i, j - 1];
		//		if (!t2.HasTile)
		//		{
		//			t2.TileType = (ushort)ModContent.TileType<HaloGrass>();
		//			t2.TileFrameX = (short)(18 * Main.rand.Next(6));
		//			t2.HasTile = true;
		//		}
		//	}
		//}
	}
}

public class JellyRipples : ModSystem
{
	private const int MaxPulses = 12;
	private const int TilesPerPulsePerTick = 96;
	private const int RingDownTicks = 40;
	private const int TilesPerAmplitudeHalving = 10;
	private static readonly float TransmissionPerTile = MathF.Pow(0.5f, 1f / TilesPerAmplitudeHalving);
	private readonly List<Pulse> pulses = [];

	private sealed class Pulse
	{
		public readonly Queue<Point> Frontier = [];
		public readonly Dictionary<Point, (ulong Tick, float Transmission)> Arrivals = [];
		public Vector2 Squeeze;
		public ulong LastArrival;

		public void Visit(Point point, ulong now, float transmission)
		{
			if (JellyBallSecretion.IsJelly(point) && Arrivals.TryAdd(point, (now, transmission)))
			{
				Frontier.Enqueue(point);
				LastArrival = now;
			}
		}
	}

	public override void ClearWorld() => pulses.Clear();

	public override void PostUpdateEverything()
	{
		ulong now = Main.GameUpdateCount;
		for (int i = pulses.Count - 1; i >= 0; i--)
		{
			var pulse = pulses[i];
			// Advance at most one connected layer per tick, sharing large layers across frames.
			int count = Math.Min(pulse.Frontier.Count, TilesPerPulsePerTick);
			for (int n = 0; n < count; n++)
			{
				var p = pulse.Frontier.Dequeue();
				if (!JellyBallSecretion.IsJelly(p))
				{
					continue;
				}

				float transmission = pulse.Arrivals[p].Transmission * TransmissionPerTile;
				pulse.Visit(new Point(p.X - 1, p.Y), now, transmission);
				pulse.Visit(new Point(p.X + 1, p.Y), now, transmission);
				pulse.Visit(new Point(p.X, p.Y - 1), now, transmission);
				pulse.Visit(new Point(p.X, p.Y + 1), now, transmission);
			}

			if (pulse.Frontier.Count == 0 && now - pulse.LastArrival >= RingDownTicks)
			{
				pulses.RemoveAt(i);
			}
		}
	}

	internal void Emit(Point origin, Point direction, float speed)
	{
		if (pulses.Count >= MaxPulses)
		{
			return;
		}

		// Compression along the impact axis pushes both perpendicular free faces outward.
		var axes = direction.X == 0 ? new Vector2(-0.65f, 1f) : new Vector2(1f, -0.65f);
		var pulse = new Pulse { Squeeze = axes * MathHelper.Clamp(speed * 0.8f, 2f, 8f) };
		pulse.Visit(origin, Main.GameUpdateCount, 1f);
		pulses.Add(pulse);
	}

	internal bool TryGetWave(Point tile, out Vector2 squeeze)
	{
		squeeze = Vector2.Zero;
		foreach (var pulse in pulses)
		{
			if (!pulse.Arrivals.TryGetValue(tile, out var arrival))
			{
				continue;
			}

			ulong age = Main.GameUpdateCount - arrival.Tick;
			if (age < RingDownTicks)
			{
				squeeze += pulse.Squeeze * (arrival.Transmission * MathF.Exp(-0.085f * age) * MathF.Sin(0.56f * age));
			}
		}

		squeeze = Vector2.Clamp(squeeze, new Vector2(-5f), new Vector2(5f));
		return squeeze.LengthSquared() > 0.01f;
	}
}

public class JellyBallSecretionPlayer : ModPlayer
{
	private const float ImpactSpeed = 6f;
	private const float Restitution = 0.9f;
	private Vector2 incoming;
	private Vector2 startPosition;
	private float stepDistance;

	public override void PreUpdateMovement()
	{
		incoming = Player.velocity;
		startPosition = Player.position;
	}

	public override void PostUpdate()
	{
		if (Player.dead || Player.ghost || Main.dedServ)
		{
			return;
		}

		bool impact = false;
		bool bounced = false;
		Point source = default;
		if (HitFace(incoming.X, Player.position.X - startPosition.X) && TryContact(true, Math.Sign(incoming.X), out var wall))
		{
			bounced = Math.Abs(incoming.X) >= ImpactSpeed;
			if (bounced && Player.whoAmI == Main.myPlayer)
			{
				Player.velocity.X = -incoming.X * Restitution;
			}

			impact = true;
			source = wall;
			ModContent.GetInstance<JellyRipples>().Emit(wall, new Point(Math.Sign(incoming.X), 0), Math.Abs(incoming.X));
		}

		if (HitFace(incoming.Y, Player.position.Y - startPosition.Y) && TryContact(false, Math.Sign(incoming.Y), out var floor))
		{
			bool bounceY = Math.Abs(incoming.Y) >= ImpactSpeed;
			bounced |= bounceY;
			if (bounceY && Player.whoAmI == Main.myPlayer)
			{
				Player.velocity.Y = -incoming.Y * Restitution;
				if (incoming.Y * Player.gravDir > 0)
				{
					Player.fallStart = (int)(Player.position.Y / 16f);
				}
			}

			impact = true;
			source = floor;
			ModContent.GetInstance<JellyRipples>().Emit(floor, new Point(0, Math.Sign(incoming.Y)), Math.Abs(incoming.Y));
		}

		if (impact)
		{
			stepDistance = 0;
			if (bounced)
			{
				SoundEngine.PlaySound(SoundID.NPCHit1, source.ToWorldCoordinates());
			}
		}
		else if (Math.Abs(Player.velocity.Y) < 0.05f && TryContact(false, (int)Player.gravDir, out var ground))
		{
			stepDistance += Math.Abs(Player.velocity.X);
			if (stepDistance >= 24f)
			{
				stepDistance %= 24f;
				ModContent.GetInstance<JellyRipples>().Emit(ground, new Point(0, (int)Player.gravDir), 2f);
			}
		}
		else
		{
			stepDistance = 0;
		}
	}

	private static bool HitFace(float speed, float travelled) =>
		Math.Abs(speed) >= 2f && travelled * Math.Sign(speed) < Math.Abs(speed) - 0.001f;

	private bool TryContact(bool horizontal, int direction, out Point contact)
	{
		float face = horizontal
			? (direction > 0 ? Player.Right.X : Player.Left.X)
			: (direction > 0 ? Player.Bottom.Y : Player.Top.Y);
		float start = horizontal ? Player.Top.Y : Player.Left.X;
		float end = horizontal ? Player.Bottom.Y : Player.Right.X;
		int fixedTile = (int)MathF.Floor((face + direction * 0.1f) / 16f);
		for (int t = (int)MathF.Floor((start + 0.01f) / 16f); t <= (int)MathF.Floor((end - 0.01f) / 16f); t++)
		{
			contact = horizontal ? new Point(fixedTile, t) : new Point(t, fixedTile);
			if (JellyBallSecretion.IsJelly(contact))
			{
				return true;
			}
		}

		contact = default;
		return false;
	}
}
