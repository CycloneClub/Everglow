using Everglow.Commons.DataStructures;
using Everglow.Commons.Physics.MassSpringSystem;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;

namespace Everglow.Example.Items;

public class ExampleRopeItem : ModItem
{
	public Rope ItemRope;

	public MassSpringContainer EularSys = new MassSpringContainer();

	public override void HoldItem(Player player)
	{
		if (ItemRope is null)
		{
			AddRope();
		}
		ItemRope.Masses[0].Position = Main.MouseWorld;
		//ItemRope.ApplyForce_Gravity();
		ItemRope.ApplyForce_VelocityDecay(0.2f);

		// Experimental codes.
		if (ItemRope is not null)
		{
			if (Main.mouseLeft && Main.mouseLeftRelease)
			{
				GlobalRopeSystem.EulerContainers.Remove(EularSys);
				EularSys = new MassSpringContainer();
				ItemRope = Rope.Grow_Vine(ItemRope, 1);
				EularSys.AddMassSpringMesh(ItemRope);
				GlobalRopeSystem.EulerContainers.Add(EularSys);
			}
			if (Main.mouseRight && Main.mouseRightRelease)
			{
				GlobalRopeSystem.EulerContainers.Remove(EularSys);
				EularSys = new MassSpringContainer();
				ItemRope = Rope.Cut_Vine(ItemRope, 1);
				EularSys.AddMassSpringMesh(ItemRope);
				GlobalRopeSystem.EulerContainers.Add(EularSys);
			}
		}
	}

	public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		if (Main.LocalPlayer.HeldItem == Item)
		{
			SpriteBatchState sBS = GraphicsUtils.GetState(spriteBatch).Value;
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			if (ItemRope is null)
			{
				AddRope();
			}
			Texture2D tex = Commons.ModAsset.TileBlock.Value;
			foreach (var mass in ItemRope.Masses)
			{
				spriteBatch.Draw(tex, mass.Position - Main.screenPosition, null, Color.White, 0, tex.Size() * 0.5f, 0.5f, SpriteEffects.None, 0);
			}
			List<Vertex2D> bars = new List<Vertex2D>();
			for (int k = 1; k < ItemRope.Masses.Length; k++)
			{
				Vector2 dir = ItemRope.Masses[k].Position - ItemRope.Masses[k - 1].Position;
				dir = dir.SafeNormalize(Vector2.Zero);
				Vector2 normal = new Vector2(-dir.Y, dir.X) * 16;
				Vector2 drawPos = ItemRope.Masses[k - 1].Position - Main.screenPosition;
				float value = k / (float)ItemRope.Masses.Length;
				bars.Add(drawPos + normal, Color.White, new Vector3(value, 0, 0));
				bars.Add(drawPos - normal, Color.White, new Vector3(value, 1, 0));
			}
			if (bars.Count > 0)
			{
				Main.graphics.graphicsDevice.Textures[0] = tex;
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
			spriteBatch.End();
			spriteBatch.Begin(sBS);
		}
	}

	public void AddRope()
	{
		ItemRope = Rope.Create_Vine(Main.MouseWorld, 30, 2, 1, 15);

		// Rope.Create_Fixed_StartPos(Main.MouseWorld, 20, 5, 0.5f, 20);
		EularSys.AddMassSpringMesh(ItemRope);
		GlobalRopeSystem.EulerContainers.Add(EularSys);
	}
}
