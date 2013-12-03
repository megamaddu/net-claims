namespace Claims.Models
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Dynamic;
	using System.Threading.Tasks;

	public class Claims
	{
		public IDictionary<string, Claimset> Claimsets { get; internal set; }
		public DateTime? Expiration { get; private set; }
		public string Ticket { get; private set; }
		internal string Signature { get; private set; }
		internal string Encoded { get; private set; }
		internal Func<string, string, bool> Verifier { get; private set; }
		internal ClaimsAuthority ClaimsAuthority { get; private set; }

		public Claims(
			IDictionary<string, Claimset> claimsets = null,
			DateTime? expiration = null,
			string signature = null,
			string encoded = null,
			string ticket = null,
			Func<string, string, bool> verifier = null,
			ClaimsAuthority claimsAuthority = null)
		{
			this.Claimsets = claimsets;
			this.Expiration = expiration;
			this.Signature = signature;
			this.Encoded = encoded;
			this.Ticket = ticket;
			this.Verifier = verifier;
			this.ClaimsAuthority = claimsAuthority;
		}

		public bool IsValid()
		{
			return this.Verified && DateTime.UtcNow <= this.Expiration;
		}

		public bool Verified
		{
			get
			{
				return this.Verifier != null && this.Verifier(this.Encoded, this.Signature);
			}
		}

		private static dynamic Sanitize(string csid, string cid = null)
		{
			if (string.IsNullOrWhiteSpace(cid)) {
				Contract.Assert(!string.IsNullOrWhiteSpace(csid), "claims error -- when cid is not supplied csid must be a string in the form `0.0`, where both digits are hex values");
				var parts = csid.Split('.');
				csid = parts[0];
				cid = parts[1];
			}
			UInt32 pcsid;
			Contract.Assert(UInt32.TryParse(csid, System.Globalization.NumberStyles.HexNumber, null, out pcsid), "claims error -- csid must be a valid hex number");
			UInt64 pcid;
			Contract.Assert(UInt64.TryParse(cid, System.Globalization.NumberStyles.HexNumber, null, out pcid), "claims error -- cid must be a valid hex number");
			dynamic res = new ExpandoObject();
			res.csid = pcsid.ToString("x");
			res.cid = pcid.ToString("x");
			return res;
		}

		public bool Has(string csid, string cid = null)
		{
			var args = Sanitize(csid, cid);
			Claimset claimset;
			if (!this.Claimsets.TryGetValue(args.csid, out claimset) || claimset.Signature != this.Signature)
			{
				return false;
			}
			var pcid = UInt32.Parse(args.cid, System.Globalization.NumberStyles.HexNumber);
			UInt32 b = 1;
			while (b <= pcid)
			{
				if (b == (b & pcid) && claimset.Claims.ContainsKey(b.ToString("x")))
				{
					return true;
				}
				b *= 2;
			}
			return false;
		}

		public async Task<string> Get(string csid, string cid = null, bool resolve = true)
		{
			var args = Sanitize(csid, cid);
			Claimset claimset;
			if(!this.Claimsets.TryGetValue(args.csid, out claimset))
			{
				return null;
			}
			if (string.IsNullOrWhiteSpace(this.Signature) || this.Signature != claimset.Signature)
			{
				return null;
			}
			Claim claim;
			if (!claimset.Claims.TryGetValue(args.cid, out claim))
			{
				return resolve ? await this.Resolve(args.csid, args.cid) : null;
			}
			return claim.Value;
		}

		public async Task<string> Resolve(string csid, string cid = null)
		{
			Contract.Assert(this.ClaimsAuthority != null, "claims error -- unable to resolve, no resolver defined");
			var args = Sanitize(csid, cid);
			await this.ClaimsAuthority.Expand(this, string.Format("{0}.{1}", csid, cid));
			return await this.Get(csid, cid, resolve: false);
		}

		internal void Merge(Claims parsed)
		{
			foreach (var from in parsed.Claimsets)
			{
				Claimset claimset;
				if (!this.Claimsets.TryGetValue(from.Key, out claimset))
				{
					claimset = from.Value;
				}
				else
				{
					claimset.Merge(from.Value);
				}
			}
		}
	}
}
