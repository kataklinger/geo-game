
namespace GG.Model.Game.Selection
{
	enum ContinentSelectorFlags
	{
		None = 0x00,
		IgnoreBorderExlusion = 0x01,
		MaritimeBorders = 0x02,
		LimitByNeighbors = 0x04,
		LimitByContinent = 0x08,
		RangeLimits = 0x10
	}
}
