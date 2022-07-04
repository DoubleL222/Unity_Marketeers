using Market.Generics;
using UnityEngine;

namespace Market
{
    public class SpiceResource : ResourceGeneric
    {
        public override int r_GetBuyValue()
        {
            return CurrencyManagerSingleton.Instance.BuyValueSpice;
        }

        public override int r_GetSellValue()
        {
            return CurrencyManagerSingleton.Instance.SellValueSpice;
        }

        public SpiceResource(ResourceType _type, Sprite _sprite) : base(_type, _sprite)
        {
        
        }

    }
}
