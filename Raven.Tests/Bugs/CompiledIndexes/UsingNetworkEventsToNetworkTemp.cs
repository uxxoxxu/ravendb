using System;
using System.ComponentModel.Composition.Hosting;
using Newtonsoft.Json.Linq;
using Raven.Abstractions;
using Xunit;
using System.Linq;

namespace Raven.Tests.Bugs.CompiledIndexes
{
	public class UsingNetworkEventsToNetworkTemp : LocalClientTest
	{
		[Fact]
		public void CanGetGoodResults()
		{
			using(var store = NewDocumentStore())
			{
				using(var s = store.OpenSession())
				{
					s.Store(new NetworkList
					{
						Network = "abc",
					});
					s.SaveChanges();
				}


				using(var s = store.OpenSession())
				{
					var list = s.Advanced.LuceneQuery<dynamic>("Aggregates/NetworkTest")
						.WaitForNonStaleResults(TimeSpan.FromMinutes(5))
						.ToList();

					Assert.Empty(store.DocumentDatabase.Statistics.Errors);

					var expected = new DateTime(2011,5,29).ToUniversalTime();
					Assert.Equal(expected, list[0].NetworkTimeStamp.UtcDateTime);
				}
			}
		}

		protected override void ModifyConfiguration(Database.Config.RavenConfiguration configuration)
		{
			configuration.Catalog.Catalogs.Add(new TypeCatalog(typeof (NetworkEventsToNetworkTemp)));
		}
	}
}