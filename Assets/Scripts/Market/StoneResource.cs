using Market.Generics;
using UnityEngine;

namespace Market
{
    public class IronResource : ResourceGeneric
    {
        public override int r_GetBuyValue()
        {
            return CurrencyManagerSingleton.Instance.BuyValueIron;
        }

        public override int r_GetSellValue()
        {
            return CurrencyManagerSingleton.Instance.SellValueIron;
        }

        public IronResource(ResourceType _type, Sprite _sprite) : base(_type, _sprite)
        {

        }
    }
}
