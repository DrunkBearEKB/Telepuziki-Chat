using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crypting
{
	[TestFixture]
	public class CrypterShould

	{
        private const string SimpleMessage = "I love C #!";

        [Test]
		public void SimpleString()
		{
			var coder = new Coder("SimplePassword");
			var message = SimpleMessage;				
			var dataBytes = Encoding.Default.GetBytes(message);
			var cryptedDataBytes = coder.Encrypt(dataBytes);
			var decryptedDataBytes = coder.Decrypt(cryptedDataBytes);
			string decryptedMessage = Encoding.Default.GetString(
				decryptedDataBytes,
				0,
				decryptedDataBytes.Length
			);
			Assert.AreEqual(message, decryptedMessage);
		}

		public void BigMessage()
		{
			var coder = new Coder("SimplePassword");
			var message = SimpleMessage +
				" Thanks to all the teachers who taught me this semester!" +
				" Please put 4 or 5 ..." +
				"I love C #!" +
				" Thanks to all the teachers who taught me this semester!" +
				" Please put 4 or 5 ..." +
				"I love C #!" +
				" Thanks to all the teachers who taught me this semester!" +
				" Please put 4 or 5 ...";
			var dataBytes = Encoding.Default.GetBytes(message);
			var cryptedDataBytes = coder.Encrypt(dataBytes);
			var decryptedDataBytes = coder.Decrypt(cryptedDataBytes);
			string decryptedMessage = Encoding.Default.GetString(
				decryptedDataBytes,
				0,
				decryptedDataBytes.Length
			);
			Assert.AreEqual(message, decryptedMessage);
		}

		[Test]
		public void EmptySituation()
		{
			var message = "";
			var coder = new Coder("SimplePassword");
			var dataBytes = Encoding.Default.GetBytes(message);
			var cryptedDataBytes = coder.Encrypt(dataBytes);
			var decryptedDataBytes = coder.Decrypt(cryptedDataBytes);
			string decryptedMessage = Encoding.Default.GetString(
				decryptedDataBytes,
				0,
				decryptedDataBytes.Length
			);
			Assert.AreEqual(message, decryptedMessage);
		}

		[Test]
		public void WorksWithManyParameters()
		{
			var coder = new Coder("SimplePassword", "SimpleSaltValue", "MD5", 3);
			var message = SimpleMessage +
				" Thanks to all the teachers who taught me this semester!" +
				" Please put 4 or 5 ...";
			var dataBytes = Encoding.Default.GetBytes(message);
			var cryptedDataBytes = coder.Encrypt(dataBytes);
			var decryptedDataBytes = coder.Decrypt(cryptedDataBytes);
			string decryptedMessage = Encoding.Default.GetString(
				decryptedDataBytes,
				0,
				decryptedDataBytes.Length
			);
			Assert.AreEqual(message, decryptedMessage);
		}
	}
}