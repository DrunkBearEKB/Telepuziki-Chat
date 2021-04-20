using System;
using System.Collections.Generic;
using System.Linq;
using Network.Extensions;
using NUnit.Framework;

namespace Network.Package
{
    [TestFixture]
    public class PackageCreator_Tests
    {
        [Test]
        public void PackageCreator1()
        {
            //TODO Test for PackageCreator1()
        }
        
        [Test]
        public void PackageCreator2()
        {
            //TODO Test for PackageCreator1()
        }
        
        [Test]
        public void PackageCreator3()
        {
            //TODO Test for PackageCreator3()
        }
        
        [Test]
        public void GetRawFormattedDataOfText()
        {
            Assert.AreEqual(
                new byte[] { 
                    57, 0, 0, 0, 
                    1, 
                    8, 97, 0, 98, 0, 99, 0, 100, 0, 
                    16, 101, 0, 102, 0, 103, 0, 104, 0, 105, 0, 106, 0, 107, 0, 108, 0, 
                    10, 49, 0, 50, 0, 58, 0, 51, 0, 48, 0, 
                    18, 116, 0, 101, 0, 109, 0, 112, 0, 32, 0, 116, 0, 101, 0, 109, 0, 112, 0 }, 
                PackageCreator.GetRawFormattedData(
                    PackageType.Text, "abcd", "efghijkl", "12:30", "temp temp"));
        }
        
        [Test]
        public void GetRawFormattedDataOfVoice()
        {
            Assert.AreEqual(
                new byte[] { 
                    62, 0, 0, 0, 
                    2, 
                    8, 97, 0, 98, 0, 99, 0, 100, 0, 
                    16, 101, 0, 102, 0, 103, 0, 104, 0, 105, 0, 106, 0, 107, 0, 108, 0, 
                    10, 49, 0, 50, 0, 58, 0, 51, 0, 48, 0, 
                    18, 116, 0, 101, 0, 109, 0, 112, 0, 32, 0, 116, 0, 101, 0, 109, 0, 112, 0, 11, 12, 13, 14, 15 }, 
                PackageCreator.GetRawFormattedData(
                    PackageType.Voice, 
                    new string[] {"abcd", "efghijkl", "12:30", "temp temp"}, 
                    new byte[]{ 11, 12, 13, 14, 15}));
        }
        
        [Test]
        public void GetRawFormattedDataOfFile()
        {
            //TODO Test for GetRawFormattedDataOfFile()
        }
        
        [Test]
        public void GetRawFormattedDataOfOnline()
        {
            Assert.AreEqual(
                new byte[] { 
                    27, 0, 0, 0, 
                    5, 
                    8, 97, 0, 98, 0, 99, 0, 100, 0, 
                    16, 101, 0, 102, 0, 103, 0, 104, 0, 105, 0, 106, 0, 107, 0, 108, 0 }, 
                PackageCreator.GetRawFormattedData(
                    PackageType.Online, "abcd", "efghijkl"));
        }
        
        [Test]
        public void GetRawFormattedDataOfDisconnect()
        {
            Assert.AreEqual(
                new byte[] { 
                    27, 0, 0, 0, 
                    6, 
                    8, 97, 0, 98, 0, 99, 0, 100, 0, 
                    16, 101, 0, 102, 0, 103, 0, 104, 0, 105, 0, 106, 0, 107, 0, 108, 0 }, 
                PackageCreator.GetRawFormattedData(
                    PackageType.Disconnect, "abcd", "efghijkl"));
        }

        [Test]
        public void GetRawFormattedDataOfHistoryRequest()
        {
            Assert.AreEqual(
                new byte[] { 
                    27, 0, 0, 0, 
                    7, 
                    8, 97, 0, 98, 0, 99, 0, 100, 0, 
                    16, 101, 0, 102, 0, 103, 0, 104, 0, 105, 0, 106, 0, 107, 0, 108, 0 }, 
                PackageCreator.GetRawFormattedData(
                    PackageType.HistoryRequest, "abcd", "efghijkl"));
        }
        
        [Test]
        public void GetRawFormattedDataOfHistoryAnswer()
        {
            //TODO Test for GetRawFormattedDataOfHistoryAnswer()
        }
        
        [Test]
        public void Parse1()
        {
            byte[] bytes = new byte[] {
                57, 0, 0, 0, 
                1, 
                8, 97, 0, 98, 0, 99, 0, 100, 0,
                16, 101, 0, 102, 0, 103, 0, 104, 0, 105, 0, 106, 0, 107, 0, 108, 0,
                10, 49, 0, 50, 0, 58, 0, 51, 0, 48, 0,
                18, 116, 0, 101, 0, 109, 0, 112, 0, 32, 0, 116, 0, 101, 0, 109, 0, 112, 0 };

            List<byte[]> parsed = PackageCreator.Parse(bytes);
            
            Assert.NotNull(parsed);
            Assert.AreEqual(4, parsed.Count);
            Assert.AreEqual(
                new byte[] { 97, 0, 98, 0, 99, 0, 100, 0 }, 
                parsed[0]);
            Assert.AreEqual(
                new byte[] { 101, 0, 102, 0, 103, 0, 104, 0, 105, 0, 106, 0, 107, 0, 108, 0 }, 
                parsed[1]);
            Assert.AreEqual(
                new byte[] { 49, 0, 50, 0, 58, 0, 51, 0, 48, 0 },
                parsed[2]);
            Assert.AreEqual(
                new byte[] { 116, 0, 101, 0, 109, 0, 112, 0, 32, 0, 116, 0, 101, 0, 109, 0, 112, 0 },
                parsed[3]);
        }
        
        [Test]
        public void Parse2()
        {
            byte[] bytes = new byte[] {
                27, 0, 0, 0, 
                7, 
                8, 97, 0, 98, 0, 99, 0, 100, 0, 
                16, 101, 0, 102, 0, 103, 0, 104, 0, 105, 0, 106, 0, 107, 0, 108, 0 };

            List<byte[]> parsed = PackageCreator.Parse(bytes);
            
            Assert.NotNull(parsed);
            Assert.AreEqual(2, parsed.Count);
            Assert.AreEqual(
                new byte[] { 97, 0, 98, 0, 99, 0, 100, 0 }, 
                parsed[0]);
            Assert.AreEqual(
                new byte[] { 101, 0, 102, 0, 103, 0, 104, 0, 105, 0, 106, 0, 107, 0, 108, 0 }, 
                parsed[1]);
        }
    }
}