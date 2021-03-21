using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Geo.Abstractions.Interfaces;
using Geo.IO.Wkb;
using GG.Model.Contracts.GeoData;
using GG.Model.Contracts.Infrastructure;
using PCLStorage;
using SQLitePCL;

namespace GG.Model.GeoData
{
	public class CountryCollection : ICountryCollection
	{
		private readonly IAppSettings _settings;
		private readonly IResourceDataProvider _resourceDataProvider;

		private Dictionary<string, CountryInfo> _countries = new Dictionary<string, CountryInfo>();

		public CountryCollection(IAppSettings settings, IResourceDataProvider resourceDataProvider)
		{
			_settings = settings;
			_resourceDataProvider = resourceDataProvider;
		}

		public IEnumerable<ICountryInfo> Countries { get { return _countries.Values.ToList(); } }

		public async Task Initialize()
		{
			string dbFile = await EnsureDbFile();

			LoadGeoData(dbFile);
			LoadBorderData(dbFile);
		}

		private async Task<string> EnsureDbFile()
		{
			if (await FileSystem.Current.LocalStorage.CheckExistsAsync(_settings.DbLocalFile) == ExistenceCheckResult.NotFound
				|| _settings.DbLocalVersion != _settings.DbSourceVersion)
			{
				var file = await CopyFile(_settings.DbSourceFile, _settings.DbLocalFile);
				_settings.DbLocalVersion = _settings.DbSourceVersion;

				return file;
			}
			else
				return (await FileSystem.Current.LocalStorage.GetFileAsync(_settings.DbLocalFile)).Path;
		}

		private async Task<string> CopyFile(string srcFile, string dstFile)
		{
			var file = await FileSystem.Current.LocalStorage.CreateFileAsync(dstFile, CreationCollisionOption.ReplaceExisting);
			using (var dst = await file.OpenAsync(FileAccess.ReadAndWrite))
			{
				using (var src = _resourceDataProvider.GetDataStream(srcFile))
				{
					var buffer = new byte[4096];
					int bytes = -1;

					while ((bytes = src.Read(buffer, 0, buffer.Length)) > 0)
						dst.Write(buffer, 0, bytes);
				}
			}

			return file.Path;
		}

		private void LoadGeoData(string dbFile)
		{
			WkbReader geoReader = new WkbReader();

			Geo.GeoContext.Current.LongitudeWrapping = true;

			using (var vectorConn = new SQLiteConnection(dbFile, SQLiteOpen.READONLY))
			{
				using (var cmd = vectorConn.Prepare(
					"SELECT geometry_hi, geometry_low, name_long, continent, long_wrap, z_index, tolerance_low, tolerance_hi FROM countries_v"))
				{
					while (cmd.Step() == SQLiteResult.ROW)
					{
						var name = cmd.GetText(2).Trim();

						IGeometry geometry = null;
						using (var geo = new MemoryStream(cmd.GetBlob(_settings.IsLowMemory ? 1 : 0)))
							geometry = geoReader.Read(geo);

						var continent = Continent.Unspecified;
						Enum.TryParse<Continent>(cmd.GetText(3).Replace(" ", string.Empty), out continent);

						var longWrap = cmd.GetInteger(4) != 0;
						var zIndex = (int)cmd.GetInteger(5);

						var toleranceLow = cmd.GetFloat(6);
						var toleranceHi = cmd.GetFloat(7);

						var fileName = NormalizeName(name);

						var country = new CountryInfo(continent, name, geometry,
							new Uri(string.Format(_settings.SmallFlagFileNameFormat, fileName), UriKind.Relative),
							new Uri(string.Format(_settings.LargeFlagFileNameFormat, fileName), UriKind.Relative),
							longWrap, zIndex, toleranceLow, toleranceHi);

						_countries[name.ToLower()] = country;
					}
				}
			}
		}

		private string NormalizeName(string name)
		{
			return name
				.Replace("ã", "a")
				.Replace("é", "e")
				.Replace("ç", "c")
				.Replace("ô", "o");
		}

		private void LoadBorderData(string dbFile)
		{
			using (var vectorConn = new SQLiteConnection(dbFile, SQLiteOpen.READONLY))
			{
				using (var cmd = vectorConn.Prepare("SELECT country2, land, maritime, excluded FROM countries_borders WHERE country1 = @name"))
				{
					foreach (var country1 in _countries.Values)
					{
						cmd.Reset();
						cmd.Bind("@name", country1.AdministrativeName);

						while (cmd.Step() == SQLiteResult.ROW)
						{
							CountryInfo country2;

							if (_countries.TryGetValue(cmd.GetText(0).ToLower(), out country2))
							{
								var land = cmd.GetInteger(1) != 0;
								var maritime = cmd.GetInteger(2) != 0;
								var excluded = cmd.GetInteger(3) != 0;

								country1.AddBorder(country2, land, maritime, excluded);
							}
						}
					}
				}
			}
		}
	}
}
