using System;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Reflection;
using GG.Model.Contracts.Infrastructure;
using Microsoft.Phone.Info;
using Microsoft.Phone.Marketplace;
using Windows.ApplicationModel.Store;

namespace GG.App
{
	public class AppSettings : IAppSettings
	{
		private const string IsSoundOnKey = "IsSoundOn";
		private const string DbLocalVersionKey = "DbLocalVersion";

		private const int LowMemoryLimit = 398458880;

		private IsolatedStorageSettings _appSettings = IsolatedStorageSettings.ApplicationSettings;

		private int _dbLocalVersion;

		private bool _isSoundOn;

		public AppSettings()
		{
			DbSourceFile = "Data/VectorData.sqlite";
			DbLocalFile = "VectorData.sqlite";

			DbSourceVersion = 19;

			if (!_appSettings.TryGetValue(IsSoundOnKey, out _isSoundOn))
			{
				_isSoundOn = true;
				_appSettings.Add(IsSoundOnKey, _isSoundOn);
			}

			if (!_appSettings.TryGetValue(DbLocalVersionKey, out _dbLocalVersion))
			{
				_dbLocalVersion = -1;
				_appSettings.Add(DbLocalVersionKey, _dbLocalVersion);
			}

			SmallFlagFileNameFormat = "Data/Flags/Small/{0}.png";
			LargeFlagFileNameFormat = "Data/Flags/Large/{0}.png";

			IsLowMemory = DeviceStatus.ApplicationMemoryUsageLimit < LowMemoryLimit;
		}

		public bool IsLowMemory { get; private set; }

		public string DbSourceFile { get; private set; }
		public string DbLocalFile { get; set; }

		public int DbSourceVersion { get; private set; }
		public int DbLocalVersion
		{
			get { return _dbLocalVersion; }
			set
			{
				if (_dbLocalVersion != value)
				{
					_dbLocalVersion = value;
					_appSettings[DbLocalVersionKey] = value;
					_appSettings.Save();
				}
			}
		}

		public string AppVersion { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

		public string SmallFlagFileNameFormat { get; private set; }
		public string LargeFlagFileNameFormat { get; private set; }

		public bool IsSoundOn
		{
			get { return _isSoundOn; }
			set
			{
				if (_isSoundOn != value)
				{
					_isSoundOn = value;
					_appSettings[IsSoundOnKey] = value;
					_appSettings.Save();
				}
			}
		}
	}
}
