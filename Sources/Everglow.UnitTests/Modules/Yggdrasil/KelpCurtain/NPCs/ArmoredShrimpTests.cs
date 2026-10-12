using System.Runtime.CompilerServices;
using Everglow.Yggdrasil.KelpCurtain.NPCs.DeathJadeLake;
using Terraria;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules.Yggdrasil.KelpCurtain.NPCs;

[TestClass]
[DoNotParallelize]
public class ArmoredShrimpTests
{
	[TestInitialize]
	public void Initialize() => Program.SavePath = string.Empty;

	[TestMethod]
	public void ShrimpDoesNotOverrideOnSpawn()
	{
		Assert.AreEqual(typeof(ModNPC), typeof(ArmoredShrimp).GetMethod(nameof(ModNPC.OnSpawn))!.DeclaringType,
			"Creating one shrimp must not trigger an automatic shoal or source-specific setup.");
	}

	[DataTestMethod]
	[DataRow(0f, true, true, 1)]
	[DataRow(1f, true, true, 1)]
	[DataRow(0f, false, true, -1)]
	[DataRow(0f, true, false, -1)]
	public void ShrimpCohesion_UsesLivingSameTypeNeighbours(float legacyMarker, bool sameType, bool alive, int expectedHeading)
	{
		var previousNpcs = Main.npc.ToArray();
		int previousNetMode = Main.netMode;
		bool previousDedServ = Main.dedServ;
		try
		{
			Main.netMode = Terraria.ID.NetmodeID.Server;
			Main.dedServ = true;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				Main.npc[i] = (NPC)RuntimeHelpers.GetUninitializedObject(typeof(NPC));
			}
			var shrimp = (ArmoredShrimp)RuntimeHelpers.GetUninitializedObject(typeof(ArmoredShrimp));
			NPC npc = CreateShrimpNpc(0, 1000, 0);
			npc.ai[1] = legacyMarker;
			typeof(ModType<NPC>).GetProperty("Entity")!.SetValue(shrimp, npc);
			Main.npc[0] = npc;
			NPC other = CreateShrimpNpc(1, sameType ? 1000 : 1001, 400);
			other.life = alive ? 10 : 0;
			Main.npc[1] = other;

			shrimp.AI();

			Assert.AreEqual(expectedHeading, npc.ai[2]);
			Assert.AreEqual(expectedHeading, npc.direction);
			Assert.AreEqual(2, Main.npc.Take(Main.maxNPCs).Count(n => n.active), "AI must not create more NPCs.");
		}
		finally
		{
			for (int i = 0; i < previousNpcs.Length; i++)
			{
				Main.npc[i] = previousNpcs[i];
			}
			Main.netMode = previousNetMode;
			Main.dedServ = previousDedServ;
		}
	}

	private static NPC CreateShrimpNpc(int slot, int type, float x)
	{
		var npc = (NPC)RuntimeHelpers.GetUninitializedObject(typeof(NPC));
		npc.type = type;
		npc.whoAmI = slot;
		npc.active = true;
		npc.life = 10;
		npc.width = 22;
		npc.height = 14;
		npc.position = new Microsoft.Xna.Framework.Vector2(x, 0);
		npc.ai = new float[4];
		npc.ai[2] = -1;
		npc.localAI = new float[4];
		npc.localAI[0] = 100; // Keep the random wander timer from firing in this deterministic AI step.
		return npc;
	}
}
