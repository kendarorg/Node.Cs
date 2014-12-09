
namespace Node.Cs
{
	public interface INodeConsole
	{
		void Write(string formatString, params object[] formatParameters);
		void WriteLine(string formatString, params object[] formatParameters);
		string ReadLine();

	}
}
