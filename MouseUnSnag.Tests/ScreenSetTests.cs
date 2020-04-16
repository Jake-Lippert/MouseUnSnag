using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Moq;
using MouseUnSnag.Contracts;
using NUnit.Framework;

namespace MouseUnSnag.Tests
{
	[TestFixture]
	public class ScreenSetTests
	{
		ScreenSet _screenSet;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			var screenMocks = new (int left, int top, int width, int height, uint dpi)[]
			{
				(-1920, 468, 1920, 1080, 91),
				(3840, 487, 1680, 1050, 90),
				(5520, 487, 1680, 1050, 90),
				(0, 0, 3840, 2160, 163)
			}.Select((s, i) =>
			{
				var screenMock = new Mock<IScreen>();
				screenMock.Setup(x => x.Index).Returns(i);
				screenMock.Setup(x => x.Bounds).Returns(new Rectangle(s.left, s.top, s.width, s.height));
				screenMock.Setup(x => x.Dpi).Returns(s.dpi);

				return screenMock;
			}).ToList();
			_screenSet = new ScreenSet(screenMocks.Select(x => x.Object).ToArray());
			screenMocks.ForEach(s => s.Setup(x => x.ScreenSet).Returns(_screenSet));

			Debug.WriteLine(_screenSet);
		}

		[Test]
		public void TestDetectsAllScreens()
		{
			//--Act
			var screens = _screenSet.AllScreens;

			//--Assert
			Assert.AreEqual(4, screens.Length);

			(int width, int height) GetScreenSize(IScreen screen) => (screen.Bounds.Width, screen.Bounds.Height);
			(int left, int top, int right, int bottom) GetScreenBounds(IScreen screen) => (screen.Bounds.Left, screen.Bounds.Top, screen.Bounds.Right, screen.Bounds.Bottom);

			Assert.AreEqual((1920, 1080), GetScreenSize(screens[0]));
			Assert.AreEqual((-1920, 468, 0, 1548), GetScreenBounds(screens[0]));

			Assert.AreEqual((1680, 1050), GetScreenSize(screens[1]));
			Assert.AreEqual((3840, 487, 5520, 1537), GetScreenBounds(screens[1]));

			Assert.AreEqual((1680, 1050), GetScreenSize(screens[2]));
			Assert.AreEqual((5520, 487, 7200, 1537), GetScreenBounds(screens[2]));

			Assert.AreEqual((3840, 2160), GetScreenSize(screens[3]));
			Assert.AreEqual((0, 0, 3840, 2160), GetScreenBounds(screens[3]));
		}

		[Test]
		[TestCase(468, 113)]
		[TestCase(470, 116)]
		[TestCase(500, 170)]
		[TestCase(530, 224)]
		[TestCase(560, 278)]
		[TestCase(590, 331)]
		[TestCase(620, 385)]
		[TestCase(650, 439)]
		[TestCase(680, 492)]
		[TestCase(710, 546)]
		[TestCase(740, 600)]
		[TestCase(770, 654)]
		[TestCase(800, 707)]
		[TestCase(830, 761)]
		[TestCase(860, 815)]
		[TestCase(890, 869)]
		[TestCase(920, 922)]
		[TestCase(950, 976)]
		[TestCase(980, 1030)]
		[TestCase(1010, 1084)]
		[TestCase(1040, 1137)]
		[TestCase(1070, 1191)]
		[TestCase(1100, 1245)]
		[TestCase(1130, 1299)]
		[TestCase(1160, 1352)]
		[TestCase(1190, 1406)]
		[TestCase(1220, 1460)]
		[TestCase(1250, 1513)]
		[TestCase(1280, 1567)]
		[TestCase(1310, 1621)]
		[TestCase(1340, 1675)]
		[TestCase(1370, 1728)]
		[TestCase(1400, 1782)]
		[TestCase(1430, 1836)]
		[TestCase(1460, 1890)]
		[TestCase(1490, 1943)]
		[TestCase(1520, 1997)]
		[TestCase(1547, 2045)]
		public void TestAdjustsPoint03(int yScreen0, int yScreen3)
		{
			//--Arrange
			var cursor = new Point(-1, yScreen0);
			var mouse = new Point(1, yScreen0);

			//--Act
			var newCursor = _screenSet.GetAdjustedPoint(cursor, mouse);

			//--Assert
			Assert.AreEqual((mouse.X, yScreen3), (newCursor.X, newCursor.Y));
		}

		[Test]
		[TestCase(0, 468)]
		[TestCase(8, 468)]
		[TestCase(62, 468)]

		[TestCase(113, 468)]
		[TestCase(116, 470)]
		[TestCase(170, 500)]
		[TestCase(224, 530)]
		[TestCase(278, 560)]
		[TestCase(331, 590)]
		[TestCase(385, 620)]
		[TestCase(439, 650)]
		[TestCase(492, 680)]
		[TestCase(546, 710)]
		[TestCase(600, 740)]
		[TestCase(654, 770)]
		[TestCase(707, 800)]
		[TestCase(761, 830)]
		[TestCase(815, 860)]
		[TestCase(869, 890)]
		[TestCase(922, 920)]
		[TestCase(976, 950)]
		[TestCase(1030, 980)]
		[TestCase(1084, 1010)]
		[TestCase(1137, 1040)]
		[TestCase(1191, 1070)]
		[TestCase(1245, 1100)]
		[TestCase(1299, 1130)]
		[TestCase(1352, 1160)]
		[TestCase(1406, 1190)]
		[TestCase(1460, 1220)]
		[TestCase(1513, 1250)]
		[TestCase(1567, 1280)]
		[TestCase(1621, 1310)]
		[TestCase(1675, 1340)]
		[TestCase(1728, 1370)]
		[TestCase(1782, 1400)]
		[TestCase(1836, 1430)]
		[TestCase(1890, 1460)]
		[TestCase(1943, 1490)]
		[TestCase(1997, 1520)]
		[TestCase(2045, 1547)]

		[TestCase(2050, 1547)]
		[TestCase(2104, 1547)]
		[TestCase(2159, 1547)]
		public void TestAdjustsPoint30(int yScreen3, int yScreen0)
		{
			//--Arrange
			var cursor = new Point(1, yScreen3);
			var mouse = new Point(-1, yScreen3);

			//--Act
			var newCursor = _screenSet.GetAdjustedPoint(cursor, mouse);

			//--Assert
			Assert.AreEqual((mouse.X, yScreen0), (newCursor.X, newCursor.Y));
		}

		[Test]
		[TestCase(-1000, 470, -1000, 460)]
		[TestCase(-1000, 1540, -1000, 1560)]
		[TestCase(-1910, 1000, -1930, 1000)]
		public void TestShouldNotJump(int cursorX, int cursorY, int mouseX, int mouseY)
		{
			//--Arrange
			var cursor = new Point(cursorX, cursorY);
			var mouse = new Point(mouseX, mouseY);

			//--Act
			var shouldJump = _screenSet.CheckJumpCursor(cursor, mouse, out var newCursor);

			//--Assert
			Assert.IsFalse(shouldJump);
			Assert.AreEqual(cursor, newCursor);
		}
	}
}
