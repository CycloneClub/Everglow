using Everglow.Commons.Physics.MassSpringSystem;
using Microsoft.Xna.Framework;
using Terraria;

namespace Everglow.UnitTests;

[TestClass]
public class UnitTest
{
	public TestContext TestContext { get; set; } = default!;

	[TestInitialize]
	public void Initialize()
	{
		// Prevent TypeInitializer throwing exception
		Program.SavePath = string.Empty;
	}

	[TestMethod]
	public void RopeTest()
	{
		var rope = Rope.Create_Fixed_StartAndEnd_WithKnots(
			new Vector2(0, 0),
			new Vector2(10, 0),
			11,
			0.1f, 1f);
		Assert.AreEqual(rope.Masses.Length, 11);
		for (int i = 0; i < rope.Masses.Length; i++)
		{
			var mass = rope.Masses[i];
			Assert.AreEqual(i, mass.Position.X);
		}

		var system = new MassSpringContainer();
		system.AddMassSpringMesh(rope);

		var solver = new EulerSolver(10);
		Enumerable.Repeat(0, 60).ToList().ForEach(_ =>
		{
			rope.ApplyForce_Gravity_Wind();
			solver.Step(system, 1f);
		});

		TestContext.WriteLine(string.Join(", ", from mass in rope.Masses select mass.Position.Y));
	}
}
