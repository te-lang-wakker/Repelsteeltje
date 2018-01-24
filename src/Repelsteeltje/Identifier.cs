using Qowaiv;
using Qowaiv.Formatting;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Repelsteeltje
{
	public static class Identifier
	{
		private static readonly MD5 cryptor = MD5.Create();

		public static Uuid ForName(string name)
		{
			var str = StringFormatter.ToNonDiacritic((name ?? string.Empty).ToUpperInvariant().Trim());
			foreach (var ch in "- .,")
			{
				str = str.Replace(ch.ToString(), "");
			}
			var hash = cryptor.ComputeHash(Encoding.ASCII.GetBytes(str));
			return new Guid(hash);
		}
	}
}
