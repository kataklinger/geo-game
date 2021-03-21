
namespace GG.Model.Contracts.Infrastructure
{
	public interface IAppSettings
	{
		bool IsLowMemory { get; }

		string DbSourceFile { get; }
		string DbLocalFile { get; }

		int DbSourceVersion { get; }
		int DbLocalVersion { get; set; }

		string AppVersion { get; }

		string SmallFlagFileNameFormat { get; }
		string LargeFlagFileNameFormat { get; }

		bool IsSoundOn { get; set; }
	}
}
