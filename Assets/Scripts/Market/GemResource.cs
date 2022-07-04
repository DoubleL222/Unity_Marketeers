using Market.Generics;
using UnityEngine;

namespace Market
{
    public class GemResource : ResourceGeneric
    {
        public override int r_GetBuyValue()
        {
            return CurrencyManagerSingleton.Instance.BuyValueGem;
        }

        public override int r_GetSellValue()
        {
            return CurrencyManagerSingleton.Instance.SellValueGem;
        }

        public GemResource(ResourceType _type, Sprite _sprite) : base(_type, _sprite)
        {

        }

    }
}
