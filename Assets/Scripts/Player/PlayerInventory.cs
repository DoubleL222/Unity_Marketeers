using System;
using System.Collections.Generic;
using Market;
using Market.Generics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public class PlayerInventory : MonoBehaviour, IComparable<PlayerInventory> {

        public IronResource myIron;
        public WoodResource myWood;
        public GemResource myGems;
        public SpiceResource mySpices;
        private List<ResourceGeneric> myResources;
        public UI.PlayerCanvasController myCanvas;
        public int gold = 0;

        private void Awake()
        {
            myCanvas = transform.parent.GetComponentInChildren<UI.PlayerCanvasController>();
        }

        void Start()
        {
            gold = 0;
            myIron = CurrencyManagerSingleton.Instance.GetIronResource();
            myWood = CurrencyManagerSingleton.Instance.GetWoodResource();
            myGems = CurrencyManagerSingleton.Instance.GetGemResource();
            mySpices = CurrencyManagerSingleton.Instance.GetSpiceResource();
            myResources = new List<ResourceGeneric> { myIron, myWood, myGems, mySpices };
            
            //SetupFakeAmounts();
            //SetupFakeAmountsComparerDebug();
            myCanvas.UpdateResourceIcons();
        }

        public int GetResourceAmount(ResourceType _getType)
        {
            if (myResources != null && myResources.Count > 0)
            {
                var currResource = myResources.Find(rt => rt.r_GetType() == _getType);
                return currResource.r_GetAmount();
            }
            else
            {
                return -1;
            }
        }

        void SetupFakeAmounts(){
            gold = 90000;
            myWood.r_Increase_Amount(100);
            myIron.r_Increase_Amount(100);
            mySpices.r_Increase_Amount(100);
            myGems.r_Increase_Amount(100);
        }

        void SetupFakeAmountsComparerDebug()
        {
            // Make player 2 has 1 more gold than the others (winner), and make player 3 has 1 more gem than the others (second), and player 1 and 4 are equal.
            var playerNum = GetComponent<PlayerInput>().playerNumber;
            gold = 100 + (playerNum == 2 ? 1 : 0);
            myWood.r_Increase_Amount(100);
            myIron.r_Increase_Amount(100);
            mySpices.r_Increase_Amount(100);
            myGems.r_Increase_Amount(100 + (playerNum == 3 ? 1 : 0));
        }

        public bool HasResourceAmount(ResourceType _checkType, int _checkAmount)
        {
            ResourceGeneric currResource = myResources.Find(rt => rt.r_GetType() == _checkType);
            return currResource.r_HasResource_Amount(_checkAmount);
        }

        public bool HasGoldAmount(int _checkAmount)
        {
            if (gold >= _checkAmount)
            {
                return true;
            }
            return false;
        }

        public bool ChangeResourceAmount(ResourceType type, int amount)
        {
            if (amount > 0)
            {
                ResourceGeneric currResource = myResources.Find(rt => rt.r_GetType() == type);
                currResource.r_Increase_Amount(amount);
                UpdateResourceUI();
                return true;
            }
            else if (amount < 0)
            {
                if (HasResourceAmount(type, amount))
                {
                    ResourceGeneric currResource = myResources.Find(rt => rt.r_GetType() == type);
                    bool changeStatus = currResource.r_Decrease_By_Amount(-amount);
                    UpdateResourceUI();
                    return changeStatus;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public bool ChangeGold(int _amount)
        {
            if (_amount < 0)
            {
                if (HasGoldAmount(_amount))
                {
                    gold += _amount;
                    UpdateResourceUI();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (_amount > 0)
            {
                gold += _amount;
                UpdateResourceUI();
                return true;
            }
            return false;
        }

        public void UpdateResourceUI()
        {
            myCanvas.UpdateResourceIcons();
        }

        public void DisableAllCanvases()
        {
            myCanvas.DisableAllCanvases(false);
        }


        //public bool IsGoldTransactionValid(ResourceType type, int amount){
        //    return gold + amount > 0;
        //}

        //public bool IsResourceTransactionValid(ResourceType type, int amount)
        //{
        //    switch (type)
        //    {
        //        case ResourceType.Wood:
        //            return wood + amount > 0;
        //        case ResourceType.Iron:
        //            return iron + amount > 0;
        //        case ResourceType.Spice:
        //            return spice + amount > 0;
        //        case ResourceType.Gem:
        //            return gem + amount > 0;

        //        default:
        //            return false;
        //    }
        //}

        //public void ChangeGold(int amount)
        //{
        //    gold += amount;
        //}

        //public void ChangeResource(ResourceType type, int amount)
        //{
        //    switch (type)
        //    {
        //        case ResourceType.Wood:
        //            wood += amount;
        //            break;
        //        case ResourceType.Iron:
        //            iron += amount;
        //            break;
        //        case ResourceType.Spice:
        //            spice += amount;
        //            break;
        //        case ResourceType.Gem:
        //            gem += amount;
        //            break;
        //    }
        //}

        public int CompareTo(PlayerInventory y)
        {
            if (gold > y.gold)
                return -1;
            if (gold < y.gold)
                return 1;

            var xResourceWorth = myWood.r_GetAmount() * myWood.r_GetSellValue() +
                                 myIron.r_GetAmount() * myIron.r_GetSellValue() +
                                 mySpices.r_GetAmount() * mySpices.r_GetSellValue() +
                                 myGems.r_GetAmount() * myGems.r_GetSellValue();

            var yResourceWorth = y.myWood.r_GetAmount() * y.myWood.r_GetSellValue() +
                                 y.myIron.r_GetAmount() * y.myIron.r_GetSellValue() +
                                 y.mySpices.r_GetAmount() * y.mySpices.r_GetSellValue() +
                                 y.myGems.r_GetAmount() * y.myGems.r_GetSellValue();

            // Fix draws, by counting up the resources
            if (xResourceWorth > yResourceWorth)
                return -1;
            if (xResourceWorth < yResourceWorth)
                return 1;

            return Random.Range(0, 3) - 1;
        }
    }
}
