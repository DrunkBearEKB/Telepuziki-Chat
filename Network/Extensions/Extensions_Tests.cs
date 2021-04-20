using System.Net;
using System.Net.Sockets;
using FakeItEasy;
using Network.Package.ExchangingPackages;
using NUnit.Framework;

namespace Network.Extensions
{
    [TestFixture]
    public class Extensions_Tests
    {
        [Test]
        public void SubEmptyArray()
        {
            byte[] array = new byte[0];
            Assert.AreEqual(array, array.Sub(0, 0));
        }
        
        [Test]
        public void SubArray()
        {
            byte[] array = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            
            Assert.AreEqual(
                new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 
                array.Sub(0, 10));
            Assert.AreEqual(
                new byte[] { 1, 2, 3, 4, 5 }, 
                array.Sub(1, 5));
            Assert.AreEqual(
                new byte[] { 7, 8, 9, 10 }, 
                array.Sub(7, 4));
        }
        
        [Test]
        public void SubFullArray()
        {
            byte[] array = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            Assert.AreEqual(array, array.Sub(0));
        }
    }
}