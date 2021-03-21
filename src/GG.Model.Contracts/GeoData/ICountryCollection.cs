using System.Collections.Generic;
using System.Threading.Tasks;

namespace GG.Model.Contracts.GeoData
{
	public interface ICountryCollection
	{
		IEnumerable<ICountryInfo> Countries { get; }

		Task Initialize();
	}
}
