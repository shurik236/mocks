using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

namespace MockFramework
{
	public class ThingCache
	{
		private readonly IDictionary<string, Thing> dictionary
			= new Dictionary<string, Thing>();
		private readonly IThingService thingService;

		public ThingCache(IThingService thingService)
		{
			this.thingService = thingService;
		}

		public Thing Get(string thingId)
		{
			Thing thing;
			if (dictionary.TryGetValue(thingId, out thing))
				return thing;
			if (thingService.TryRead(thingId, out thing))
			{
				dictionary[thingId] = thing;
				return thing;
			}
			return null;
		}
	}

	[TestFixture]
	public class ThingCache_Should
	{
		private IThingService thingService;
		private ThingCache thingCache;

		private const string thingId1 = "TheDress";
		private Thing thing1 = new Thing(thingId1);

		private const string thingId2 = "CoolBoots";
		private Thing thing2 = new Thing(thingId2);

		[SetUp]
		public void SetUp()
		{
		    thingService = A.Fake<IThingService>();
		    A.CallTo(() => thingService.TryRead(thingId1, out thing1)).Returns(true);
		    A.CallTo(() => thingService.TryRead(thingId2, out thing2)).Returns(true);
			thingCache = new ThingCache(thingService);
		}

		//TODO: написать простейший тест, а затем все остальные
	    [TestCase(thingId1)]
	    [TestCase(thingId2)]
	    public void ThingTest(string thingId)
	    {
	        thingCache.Get(thingId).ShouldBeEquivalentTo(new Thing(thingId));
	    }

	    [TestCase(thingId1)]
        [TestCase(thingId2)]
        public void ThingTest2(string thingId)
	    {
	        Thing thing;
	        thingCache.Get(thingId);
	        thingCache.Get(thingId);
	        A.CallTo(() => thingService.TryRead(thingId, out thing)).MustHaveHappened(Repeated.Exactly.Once);
	    }

	    [Test]
	    public void NotForgetFirstThing_AfterAddingSecond()
	    {
	        thingCache.Get(thingId1);
	        thingCache.Get(thingId2);
	        thingCache.Get(thingId1);
	        A.CallTo(() => thingService.TryRead(thingId1, out thing1)).MustHaveHappened(Repeated.Exactly.Times(1));
	    }

	    [Test]
	    public void CashCorrectValue_AfterAddingSecond()
	    {
	        thingCache.Get(thingId1);
	        thingCache.Get(thingId2);
	        thingCache.Get(thingId1).ShouldBeEquivalentTo(thing1);
	    }

        [Test]
	    public void NullResult()
	    {
	        thingCache.Get("rt").ShouldBeEquivalentTo(null);
	    }

	    [Test]
	    public void CallService_WhenGivenNonCashedValue()
	    {
	        Thing thing;
	        thingCache.Get("rt");
	        A.CallTo(() => thingService.TryRead("rt", out thing)).MustHaveHappened(Repeated.Exactly.Once);
	    }
	}
}
