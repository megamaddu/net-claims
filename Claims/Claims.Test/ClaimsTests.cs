using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Claims.Test
{
	[TestClass]
	public class ClaimsTests
	{
		Claims.Models.Claims claims;
		string ticket = @"mialc1#0.f,e.8,12.2;0.1:TlM=.2:WFk=.4:bnM=.8:cHdz.10:MTIzNDU=.20:GVzdEBlbWFpbC5jb20=,e.8:dGVzdFZhbHVl;3000-06-30T18:38:36.480Z|ZXwVqjzrMJ7zx1U3Iy4HKgqcelYNXxALwLghS9iozwdmLQGAa4n1dk2ZcAUrox8T8x8nulHXlot9HUZYE5OrmVITDf1iMUdKcY63LB9cYf3zJlNleDvyF02vFpssGjoUhL6IEM5mZWgseGpvzozKLYR00CDwP+gR85WFhFen51NV6ua4OMYDD5eSE9pA+cBD8gox106V6V6nQHtL844P0EeBNiM0z2xTnUPD8wwD4t7PH09Q5We7N3YMoeG+fGmAUymShoOOHCDzUFWORJPrtVZiTmolDpbPsM0X5G5P8VNzyxHUGydvumcolLrVQnbSNW0jNyBIfBQ81rllp1mXOQ==";

		[TestInitialize]
		public void Setup()
		{
			var pubkey = new KeyManager.KeyManager("claimsTest", "dev", "../../testkeys")["claimsTest"].Pub.Result;
			var verifier = new Crypto.Verifier(pubkey, isPrivate: false);
			claims = new ClaimsFactory((data, sig) => verifier.Verify(data, sig), new Claims.Models.ClaimsAuthority { }).Parse(ticket);
		}

		[TestMethod]
		public void Verified()
		{
			Assert.IsTrue(claims.Verified);
		}

		[TestMethod]
		public void IsValid()
		{
			Assert.IsTrue(claims.IsValid());
		}

		[TestMethod]
		public void Has()
		{
			Assert.IsTrue(claims.Has("0.1"));
			Assert.IsTrue(claims.Has("0", "1"));
		}

		[TestMethod]
		public void Get()
		{
			var x = claims.Get("0.1", resolve: false).Result;
			Console.WriteLine(x);
		}

		[TestMethod]
		public void Resolve()
		{
			Assert.Inconclusive();
		}
	}
}
