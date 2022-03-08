using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace VisualPinball.Unity.VisualScripting.Editor
{
	[Widget(typeof(AllCoilsEnabledEventUnit))]
	public sealed class AllCoilsEnabledEventUnitWidget : GleMultiUnitWidget<AllCoilsEnabledEventUnit>
	{
		public AllCoilsEnabledEventUnitWidget(FlowCanvas canvas, AllCoilsEnabledEventUnit unit) : base(canvas, unit)
		{
		}

		protected override IEnumerable<string> IdSuggestions(IGamelogicEngine gle) => gle.RequestedCoils.Select(coil => coil.Id);
	}
}
