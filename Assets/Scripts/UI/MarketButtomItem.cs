using Market;
using AssetStore.SplitScreenAudio.Code;
using Market.Generics;
using UI.Generics;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MarketButtomItem : MPButton
    {
        [Header("Public references")]
        public Image goldImage1;
        public Image goldImage2;
        public Image resourceImage;
        public Text sellPrice;
        public Text buyPrice;
        public Text resourceName;

        private ResourceType representedResource;
        private PlayerCanvasController myPlayersCanvas;
        private VirtualAudioSource _audioSource;

        public void SetupMarketItem(ResourceType _myType, Sprite _resourceSprite, Sprite _goldSprite, PlayerCanvasController _myCanvas)
        {
            myPlayersCanvas = _myCanvas;
            representedResource = _myType;
            resourceImage.sprite = _resourceSprite;
            goldImage1.sprite = _goldSprite;
            goldImage2.sprite = _goldSprite;
            resourceName.text = _myType.ToString();
            UpdatePrices();

            _audioSource = myPlayersCanvas.gameObject.GetComponent<VirtualAudioSource>();
        }

        public void UpdatePrices()
        {
            sellPrice.text = CurrencyManagerSingleton.Instance.ResourceValues[representedResource][0].ToString();
            buyPrice.text = CurrencyManagerSingleton.Instance.ResourceValues[representedResource][1].ToString();
        }

        //BUY THE RESOURCE
        public override void CLICK_BUTTONB()
        {
            base.CLICK_BUTTONB();
            if (myPlayersCanvas.myPlayerInventory.HasGoldAmount(int.Parse(buyPrice.text)))
            {
                base.CLICK_BUTTONB();
                myPlayersCanvas.myPlayerInventory.ChangeGold(-int.Parse(buyPrice.text));
                myPlayersCanvas.myPlayerInventory.ChangeResourceAmount(representedResource, 1);
                CurrencyManagerSingleton.Instance.ChangeResource(representedResource, true, 0.005f);
                //Play moneybag sound
                //_audioSource.clip = SoundManager.Instance.MoneyBag;
                //_audioSource.Play();
            }
            else
            {
                //DOES NOT HAVE GOLD
                base.CLICK_BUTTONB();
            }
            myPlayersCanvas.UpdateResourceIcons();
            UpdatePrices();
        }

        //SELL THE RESOURCE
        public override void CLICK_BUTTONX()
        {
            if (myPlayersCanvas.myPlayerInventory.HasResourceAmount(representedResource, 1))
            {
                base.CLICK_BUTTONX();
                myPlayersCanvas.myPlayerInventory.ChangeGold(int.Parse(sellPrice.text));
                myPlayersCanvas.myPlayerInventory.ChangeResourceAmount(representedResource, -1);
                CurrencyManagerSingleton.Instance.ChangeResource(representedResource, false, 0.005f);
                //Play moneybag sound
                //_audioSource.clip = SoundManager.Instance.MoneyBag;
                //_audioSource.Play();
                //TODO REDUCE PLAYERS RESOURCE
            }
            else
            {
                base.CLICK_BUTTONX();
            }
            myPlayersCanvas.UpdateResourceIcons();
            UpdatePrices();
        }

        public override void CLICK()
        {
            base.CLICK();
        }
    }
}