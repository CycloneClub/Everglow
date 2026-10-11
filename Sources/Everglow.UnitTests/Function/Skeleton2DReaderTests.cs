using System.Text;
using Everglow.Commons.Skeleton2D.Reader;
using Spine;
using Terraria;

namespace Everglow.UnitTests.Function;

[TestClass]
[DoNotParallelize]
public class Skeleton2DReaderTests
{
	private const string AtlasText = """
		body.png
		size: 128,64
		format: RGBA8888
		filter: Linear,Linear
		repeat: none
		body
		  rotate: false
		  xy: 16,8
		  size: 16,8
		  orig: 16,8
		  offset: 0,0
		  index: -1
		""";

	private const string JsonText = """
		{
		  "bones": [{"name":"root"}, {"name":"tip", "parent":"root", "x":10}],
		  "slots": [{"name":"body", "bone":"tip", "attachment":"body"}],
		  "skins": [{"name":"default", "attachments":{"body":{"body":{"width":16,"height":8}}}}],
		  "animations": {"move":{"bones":{"root":{"translate":[{"time":0,"x":0},{"time":1,"x":20}]}}}}
		}
		""";

	private bool originalDedServ;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		originalDedServ = Main.dedServ;
		Main.dedServ = true;
	}

	[TestCleanup]
	public void Cleanup() => Main.dedServ = originalDedServ;

	[TestMethod]
	public void ReadSkeleton_WithoutTexture_PreservesAtlasDimensionsAndUVs()
	{
		var skeleton = Skeleton2DReader.ReadSkeleton(Encoding.UTF8.GetBytes(AtlasText), Encoding.UTF8.GetBytes(JsonText), null);
		var attachment = (RegionAttachment)skeleton.Skeleton.GetAttachment("body", "body");
		var region = (AtlasRegion)attachment.RendererObject;

		Assert.AreEqual(128, region.page.width);
		Assert.AreEqual(64, region.page.height);
		Assert.IsNull(region.page.rendererObject);
		Assert.AreEqual(0.125f, region.u);
		Assert.AreEqual(0.25f, region.u2);
		Assert.AreEqual(0.125f, region.v);
		Assert.AreEqual(0.25f, region.v2);
	}

	[TestMethod]
	public void ReadSkeleton_WithoutTexture_AnimatesWorldSpaceBones()
	{
		var skeleton = Skeleton2DReader.ReadSkeleton(Encoding.UTF8.GetBytes(AtlasText), Encoding.UTF8.GetBytes(JsonText));
		skeleton.AnimationState.SetAnimation(0, "move", false);
		skeleton.AnimationState.Update(0.5f);
		skeleton.AnimationState.Apply(skeleton.Skeleton);
		skeleton.Position = new Microsoft.Xna.Framework.Vector2(100, 200);

		Assert.AreEqual(110f, skeleton.Skeleton.RootBone.WorldX, 0.001f);
		Assert.AreEqual(120f, skeleton.Skeleton.FindBone("tip").WorldX, 0.001f);
		Assert.AreEqual(200f, skeleton.Skeleton.FindBone("tip").WorldY, 0.001f);
	}

	[TestMethod]
	public void ReadSkeleton_WithoutTextureAndAtlasDimensions_RejectsInvalidUVs()
	{
		string atlas = AtlasText.Replace("size: 128,64", "size: 0,0");
		Assert.ThrowsException<InvalidDataException>(() => Skeleton2DReader.ReadSkeleton(Encoding.UTF8.GetBytes(atlas), Encoding.UTF8.GetBytes(JsonText)));
	}

	[TestMethod]
	public void ReadSkeleton_WithoutTexture_AppliesScaleToAnimatedBones()
	{
		var skeleton = Skeleton2DReader.ReadSkeleton(Encoding.UTF8.GetBytes(AtlasText), Encoding.UTF8.GetBytes(JsonText), scale: 2f);
		skeleton.AnimationState.SetAnimation(0, "move", false);
		skeleton.AnimationState.Update(0.5f);
		skeleton.AnimationState.Apply(skeleton.Skeleton);
		skeleton.Skeleton.UpdateWorldTransform();

		Assert.AreEqual(40f, skeleton.Skeleton.FindBone("tip").WorldX, 0.001f);
	}
}
