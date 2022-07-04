using Systems;
using Market.Generics;
using UI.Generics;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TradeButtonItem : MPButton {

        public Text counterText;
        public Text resourceName;
        public Image resourceImage;
        public Image goldImage;

        public PlayerCanvasController myPlayersCanvas;
        public bool isGold = false;
        public ResourceType myType;

        public void ResetCounter()
        {
            counterText.text = "0";
        }

        public int GetCounter()
        {
            return int.Parse(counterText.text);
        }

        public void SetupTradeItem(ResourceType _myType, Sprite _resourceImage, Sprite _goldImage, PlayerCanvasController _playerCanvas)
        {
            counterText.text = "0";
            myType = _myType;
            resourceName.text = _myType.ToString();
            resourceImage.sprite = _resourceImage;
            goldImage.sprite = Systems.MarketeersGameManager.Instance.multiplierSprite;
            myPlayersCanvas = _playerCanvas;
            isGold = false;
        }

        public void SetupGold(Sprite _goldImage, PlayerCanvasController _playerCanvas)
        {
            isGold = true;
            counterText.text = "0";
            resourceName.text = "Gold";
            resourceImage.sprite = _goldImage;
            goldImage.sprite = Systems.MarketeersGameManager.Instance.multiplierSprite;
            myPlayersCanvas = _playerCanvas;
        }

        public override void CLICK_BUTTONX()
        {
            //Debug.Log("OVERRIDED");
            if (!isGold)
            {
                int currCounter = int.Parse(counterText.text);
                if (myPlayersCanvas.myPlayerInventory.HasResourceAmount(myType, currCounter + 1))
                {
                    currCounter++;
                    counterText.text = currCounter.ToString();
                }
            }
            else
            {
            
                int currCounter = int.Parse(counterText.text);
                //Debug.Log("CURR COUNTER: "+ currCounter);
                if (myPlayersCanvas.myPlayerInventory.HasGoldAmount(currCounter + 1))
                {
                    //Debug.Log("HAS ENOUGH GOLD");
                    currCounter++;
                
                    counterText.text = currCounter.ToString();
                }
            }
            base.CLICK_BUTTONX();
        }

        public override void CLICK_BUTTONB()
        {
            int currCounter = int.Parse(counterText.text);
            if (currCounter>0)
            {
                currCounter--;
                counterText.text = currCounter.ToString();
            }
            base.CLICK_BUTTONB();
        }
    }
}
