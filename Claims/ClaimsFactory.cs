using System;
using Claims.Models;

namespace Claims
{
	public class ClaimsFactory
	{
		protected Func<string, string, bool> _verifier;
		protected ClaimsAuthority _claimsAuthority;

		public ClaimsFactory(Func<string, string, bool> verifier, ClaimsAuthority claimsAuthority)
		{
			_verifier = verifier;
			_claimsAuthority = claimsAuthority;
		}

		public Claims.Models.Claims Parse(string ticket)
		{
			return Parser.Parse(ticket, _verifier, _claimsAuthority);
		}

		public Claims.Models.Claims From(string json)
		{
			return Parser.From(json, _verifier, _claimsAuthority);
		}
	}
}
