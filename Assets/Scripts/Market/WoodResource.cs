using Market.Generics;
using UnityEngine;

namespace Market
{
    public class WoodResource : ResourceGeneric
    {
        public override int r_GetBuyValue()
        {
            return CurrencyManagerSingleton.Instance.BuyValueWood;
        }

        public override int r_GetSellValue()
        {
            return CurrencyManagerSingleton.Instance.SellValueWood;
        }

        public WoodResource(ResourceType _type, Sprite _sprite) : base(_type, _sprite)
        {

        }
    }
}
