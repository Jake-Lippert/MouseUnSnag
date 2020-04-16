using System.Diagnostics;
using System.Threading;

namespace MouseUnSnag
{
	class Program
	{
		static void Main(string[] args)
		{
			// Make sure the MouseUnSnag.exe has only one instance running at a time.
			if (new Mutex(true, "__MouseUnSnag_EXE__", out var createdNew) == null || !createdNew)
			{
				Debug.WriteLine("Already running!! Quitting this instance...");
				return;
			}
			else
			{
				new MouseEvents().Run();
			}
		}
	}
}