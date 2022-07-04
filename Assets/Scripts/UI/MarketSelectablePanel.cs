using UI.Generics;
using Debug = UnityEngine.Debug;

namespace UI
{
    public class MarketSelectablePanel : MPSelectableGroup
    {
        public MarketButtomItem[] MyTradeItems;

        public void Init()
        {
            MyTradeItems = GetComponentsInChildren<MarketButtomItem>(true);
        }

        public override void Activate()
        {
            base.Activate();
            UpdatePrices();
        }

        public void UpdatePrices()
        {
            foreach (MarketButtomItem currItem in MyTradeItems)
            {
                currItem.UpdatePrices();
            }
        }
    }
}