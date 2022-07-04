using UnityEngine;

namespace Market.Generics
{
    public enum ResourceType
    {
        Iron,
        Wood,
        Spice,
        Gem,
    }

    public abstract class ResourceGeneric
    {
        protected int r_Amount = 0;
        private Sprite r_Sprite;
        private ResourceType r_Type;

        protected ResourceGeneric(ResourceType _type, Sprite _sprite)
        {
            r_Type = _type;
            r_Sprite = _sprite;
            r_Amount = 0;
        }

        public ResourceType r_GetType()
        {
            return r_Type;
        }


        public int r_GetAmount()
        {
            return r_Amount;
        }

        public bool r_HasResource()
        {
            if (r_Amount > 0)
            {
                return true;
            }
            return false;
        }

        public bool r_HasResource_Amount(int check_amount)
        {
            if (r_Amount >= check_amount)
            {
                return true;
            }
            return false;
        }

        //return true if successful, returns false if not possible
        public bool r_Decrease_By_Amount(int decrease_amount)
        {
            if (r_HasResource_Amount(decrease_amount))
            {
                r_Amount -= decrease_amount;
                return true;
            }
            return false;
        }

        public bool r_Decrease_One()
        {
            if (r_HasResource())
            {
                r_Amount--;
                return true;
            }
            return false;
        }

        public void r_Increase_Amount(int add_Amount)
        {
            r_Amount += add_Amount;
        }

        public void r_Increase_One()
        {
            r_Amount += 1;
        }


        //Value will be received live from the CurrencyManagerScript because it varies during the game and we don't want to be changing it for each instance of the resource 
        public virtual int r_GetBuyValue()
        {
            return 1;
        }

        public virtual int r_GetSellValue()
        {
            return 1;
        }
    }
}