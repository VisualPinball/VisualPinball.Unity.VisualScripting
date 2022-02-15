using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace VisualPinball.Unity.VisualScripting.Editor
{
	[Widget(typeof(AllSwitchesEnabledEventUnit))]
	public sealed class AllSwitchesEnabledEventUnitWidget : GleMultiUnitWidget<AllSwitchesEnabledEventUnit>
	{
		public AllSwitchesEnabledEventUnitWidget(FlowCanvas canvas, AllSwitchesEnabledEventUnit unit) : base(canvas, unit)
		{
		}
		protected override IEnumerable<string> IdSuggestions(IGamelogicEngine gle) => gle.RequestedSwitches.Select(sw => sw.Id);
	}
}
