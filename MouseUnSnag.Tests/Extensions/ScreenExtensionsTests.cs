using System.Drawing;
using Moq;
using MouseUnSnag.Contracts;
using MouseUnSnag.Extensions;
using NUnit.Framework;

namespace MouseUnSnag.Tests.Extensions
{
	[TestFixture]
	public class ScreenExtensionsTests
	{
		[Test]
		[TestCase(468, 1548, 1008)]
		[TestCase(487, 1537, 1012)]
		[TestCase(0, 2160, 1080)]
		public void TestScreenMidpointYs(int top, int bottom, int expectedMidpoint)
		{
			//--Arrange
			var screenMock = new Mock<IScreen>();
			screenMock.Setup(x => x.Bounds)
				.Returns(new Rectangle(default, top, default, bottom - top));

			//--Act
			var actualMidpoint = screenMock.Object.GetScreenMidpointY();

			//--Assert
			Assert.AreEqual(expectedMidpoint, actualMidpoint);
		}
	}
}
