using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Claims.Test
{
	[TestClass]
	public class ClaimsTests
	{
		Claims.Models.Claims claims;
		string ticket = @"mialc1#0.f,e.8,12.2;0.1:TlM=.2:WFk=.4:bnM=.8:cHdz.10:MTIzNDU=.20:GVzdEBlbWFpbC5jb20=,e.8:dGVzdFZhbHVl;3000-06-30T18:38:36.480Z|v3L+usYEyvnxuHIiQykmLIzkO3dcwa5NETeoQXliRsC8oh6IO05G4pLQlf8PoXeUQjz2FGfOiUTtOe+0/aU3E8dCJ6cBgk8Iyju4bNBuOC1Sz6hDL75IAdugHZsSGa2c70+ktWgWXkEtwHdIgyUlQir1oHCNFSw2jyqGoV0EobI=";

		[TestInitialize]
		public void Setup()
		{
			claims = new ClaimsFactory(new { }).Parse(ticket);
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
			Assert.Inconclusive();
		}

		[TestMethod]
		public void Get()
		{
			Assert.Inconclusive();
		}

		[TestMethod]
		public void Resolve()
		{
			Assert.Inconclusive();
		}
	}
}
