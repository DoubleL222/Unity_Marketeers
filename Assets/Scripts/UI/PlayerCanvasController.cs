using System.Collections.Generic;
using Market;
using Market.Generics;
using Player;
using UI.Generics;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerCanvasController : MonoBehaviour
    {
        public PlayerInput myPlayer;
        public PlayerInventory myPlayerInventory;
        public float nextInteractionDelay = 0.2f;
        public MarketSelectablePanel marketPanel;
        public TradeSelectablePanel tradingPanel;
        public OrdersSelectablePanel ordersPanel;
        public MPSelectableGroup panelTabs;
        public GameObject pressAToTOpenShopImage;
        public GameObject playerHUD;

        private GameObject myGoldResource;
        private Dictionary<ResourceType, GameObject> myResources;


        //private Image[] canvasIcons;
        private SmallShipControl mySmallShip;
        private MPSelectableGroup currentlySelectedGroup;
        private bool anyObjectSelected = false;
        private bool inMarketArea = false;
        private List<MPSelectableGroup> allPanels;
        private WowCamera myWowCamera;
        private PlayerMovement myMovement;
        private float nextInteractionTime = float.MinValue;
        private Queue<Dictionary<ResourceType, int>> queuedOrders = new Queue<Dictionary<ResourceType, int>>();
        private int sendDestination;
        private int sendGold;

        public void EnteredMarketArea(bool _entered)
        {
            if (myWowCamera == null)
            {
                myWowCamera = transform.parent.GetComponentInChildren<WowCamera>();
            }
            if (pressAToTOpenShopImage != null)
            {
                pressAToTOpenShopImage.SetActive(_entered);
            }
            myWowCamera.ToggleMapView(!_entered);
            inMarketArea = _entered;
            if (!_entered)
            {
                DisableAllCanvases(false);
            }
        }

        public void DisableAllCanvases(bool _enable)
        {
            panelTabs.gameObject.SetActive(false);
            DisableShipCanvases(_enable);
            DisableTradePanel(_enable);
        }
        public void DisableTradePanel(bool _enable)
        {
            if (currentlySelectedGroup == tradingPanel)
            {
                panelTabs.gameObject.SetActive(false);
                EnableTradeCanvas(false);
                //canvasIcons[0].enabled = _enable;
                myMovement.MovementStopped = _enable;
                anyObjectSelected = _enable;
            }
        }

        public void DisableShipCanvases(bool _enable)
        {
            if (currentlySelectedGroup == ordersPanel || currentlySelectedGroup == marketPanel)
            {
                panelTabs.gameObject.SetActive(false);
                EnableOrderCanvas(false);
                EnableMarketCanvas(false);
                myMovement.MovementStopped = _enable;
                anyObjectSelected = _enable;
                //canvasIcons[1].enabled = _enable;
                //canvasIcons[2].enabled = _enable;
            }
        }

        public void EnableOrderCanvas(bool _enable)
        {
            ordersPanel.Deselect();
            ordersPanel.transform.parent.gameObject.SetActive(_enable);
            ordersPanel.gameObject.SetActive(_enable);

            if (_enable)
            {
                panelTabs.gameObject.SetActive(true);
                panelTabs.SelectNew(2);
                EnableTradeCanvas(false);
                EnableMarketCanvas(false);
                //canvasIcons[1].enabled = _enable;
                myMovement.MovementStopped = true;
                anyObjectSelected = true;
                currentlySelectedGroup = ordersPanel;
                ordersPanel.Activate();
            }
        }


        public void EnableMarketCanvas(bool _enable)
        {
            ordersPanel.Deselect();

            marketPanel.gameObject.SetActive(_enable);
            if (_enable)
            {
                panelTabs.gameObject.SetActive(true);
                panelTabs.SelectNew(1);
                EnableTradeCanvas(false);
                EnableOrderCanvas(false);
                //canvasIcons[2].enabled = _enable;
                myMovement.MovementStopped = true;
                anyObjectSelected = true;
                currentlySelectedGroup = marketPanel;
                marketPanel.Activate();
                marketPanel.UpdatePrices();
            }
        }

        public void EnableTradeCanvas(bool _enable)
        {
            ordersPanel.Deselect();

            tradingPanel.gameObject.SetActive(_enable);
            if (_enable)
            {
                panelTabs.gameObject.SetActive(true);
                panelTabs.SelectNew(0);
                EnableMarketCanvas(false);
                EnableOrderCanvas(false);
                //canvasIcons[0].enabled = _enable;
                myMovement.MovementStopped = true;
                anyObjectSelected = true;
                currentlySelectedGroup = tradingPanel;
                tradingPanel.Activate();
            }
        }

        void DisableCurrentCanvas()
        {
            //currentlySelectedGroup.gameObject.SetActive(false);
        }

        private void Awake()
        {
            mySmallShip = transform.parent.GetComponentInChildren<SmallShipControl>();

            myPlayer.OnDpadX += xD => { InputXChange(xD); };
            myPlayer.OnDpadY += yD => { InputYChange(yD); };
            myPlayer.OnHorizontal += xS => { InputXChange(xS); };
            myPlayer.OnVertical += yS => { InputYChange(-yS); };

            myPlayer.OnButton1Held += new System.Action(() =>
                { ButtonOneHeld(); }
            );
            myPlayer.OnButton2Held += new System.Action(() =>
                { ButtonTwoClick(); }
            );
            myPlayer.OnButton3Held += new System.Action(() =>
                { ButtonThreeHeld(); }
            );
            myPlayer.OnButton4Held += new System.Action(() =>
                { ButtonFourClick(); }
            );
            myPlayer.OnButton1Down += new System.Action(() =>
                { ButtonOneClick(); }
            );
            myPlayer.OnButton2Down += new System.Action(() =>
                { ButtonTwoClick(); }
            );
            myPlayer.OnButton3Down += new System.Action(() =>
                { ButtonThreeClick(); }
            );
            myPlayer.OnButton4Down += new System.Action(() =>
                { ButtonFourClick(); }
            );
            myPlayer.OnButton5Down += new System.Action(() =>
                { ButtonFiveClick(); }
            );
            myPlayer.OnZoomOutDown += new System.Action(() =>
                { ButtonSixClick(); }
            );
            mySmallShip.onArrivedToDestination += new System.Action(() =>
                { ExecuteOrder(); }
            );
            myMovement = transform.parent.GetComponentInChildren<PlayerMovement>();

            SetupResourceObjects();
            ordersPanel = GetComponentInChildren<OrdersSelectablePanel>(true);
            marketPanel = GetComponentInChildren<MarketSelectablePanel>(true);
            tradingPanel = GetComponentInChildren<TradeSelectablePanel>(true);
            allPanels = new List<MPSelectableGroup>();
            allPanels.Add(ordersPanel);
            allPanels.Add(marketPanel);
            allPanels.Add(tradingPanel);
        }

        void SetupResourceObjects()
        {
            var allResources = System.Enum.GetValues(typeof(ResourceType));
            int index = 0;
            myResources = new Dictionary<ResourceType, GameObject>();
            Transform parentOfTexts = playerHUD.transform.GetChild(0);
            foreach (ResourceType _currType in allResources)
            {
                myResources.Add(_currType, parentOfTexts.GetChild(index).GetChild(0).gameObject);
                index++;
            }
            //goldToAdd += parentOfTexts.GetChild(index).GetChild(0).gameObject.guiText;
            myGoldResource = parentOfTexts.GetChild(index).GetChild(0).gameObject;
        }

        public void UpdateResourceIcons()
        {
            foreach (KeyValuePair<ResourceType, GameObject> _kvp in myResources)
            {
                _kvp.Value.GetComponent<Text>().text = myPlayerInventory.GetResourceAmount(_kvp.Key).ToString();
            }
            myGoldResource.GetComponent<Text>().text = myPlayerInventory.gold.ToString();
        }

        //CHECK IF SHIP IS AT PLAYER WITH THIS CANVAS
        private bool IsShipAtPlayer()
        {
            return Systems.MarketeersGameManager.Instance.IsShipAtPlayer(myPlayer.playerNumber);
        }

        private void InputYChange(float _change)
        {
            if (anyObjectSelected)
            {
                if (Time.time >= nextInteractionTime)
                {
                    currentlySelectedGroup.InputYChanged(_change);
                    nextInteractionTime = Time.time + nextInteractionDelay;
                }
            }
        }
        private void InputXChange(float _change)
        {
            if (anyObjectSelected)
            {
                if (Time.time >= nextInteractionTime)
                {
                    currentlySelectedGroup.InputXChanged(-_change);
                    nextInteractionTime = Time.time + nextInteractionDelay;
                }
            }
        }
        private void ButtonOneHeld()
        {
            if (anyObjectSelected)
            {
                if (Time.time >= nextInteractionTime)
                {
                    currentlySelectedGroup.PlayerButtonXClicked();
                    nextInteractionTime = Time.time + nextInteractionDelay / 4.0f;
                }
            }
        }
        private void ButtonOneClick()
        {
            if (anyObjectSelected)
            {
                if (Time.time >= nextInteractionTime)
                {
                    currentlySelectedGroup.PlayerButtonXClicked();
                    nextInteractionTime = Time.time + nextInteractionDelay;
                }
            }
            else
            {
                if (inMarketArea)
                {
                    nextInteractionTime = Time.time + nextInteractionDelay;

                    if (IsShipAtPlayer())
                    {
                        EnableMarketCanvas(true);
                        myMovement.MovementStopped = true;
                    }
                    else if (!mySmallShip.getLeftPort())
                    {
                        EnableTradeCanvas(true);
                        myMovement.MovementStopped = true;
                    }

                }
            }
        }
        private void ButtonTwoClick()
        {
            DisableAllCanvases(false);
        }
        private void ButtonThreeHeld()
        {
            if (anyObjectSelected)
            {
                if (Time.time >= nextInteractionTime)
                {
                    currentlySelectedGroup.PlayerButtonBClicked();
                    nextInteractionTime = Time.time + nextInteractionDelay / 4.0f;
                }
            }
        }
        private void ButtonThreeClick()
        {
            if (anyObjectSelected)
            {
                if (Time.time >= nextInteractionTime)
                {
                    currentlySelectedGroup.PlayerButtonBClicked();
                    nextInteractionTime = Time.time + nextInteractionDelay;
                }
            }
        }
        private void ButtonFourClick()
        {
            if (anyObjectSelected)
            {
                if (Time.time >= nextInteractionTime)
                {
                    currentlySelectedGroup.PlayerButtonYClicked();
                    nextInteractionTime = Time.time + nextInteractionDelay;
                }
            }
        }
        private void ButtonFiveClick()
        {
            //RIGHT BUMPER
            if (anyObjectSelected)
            {
                if (currentlySelectedGroup == marketPanel)
                {
                    if (!mySmallShip.getLeftPort())
                    {
                        EnableTradeCanvas(true);
                    }
                    else
                    {
                        EnableOrderCanvas(true);
                    }
                }
                else if (currentlySelectedGroup == ordersPanel)
                {
                    if (IsShipAtPlayer())
                        EnableMarketCanvas(true);
                }
                else if (currentlySelectedGroup == tradingPanel)
                {
                    if (IsShipAtPlayer())
                        EnableOrderCanvas(true);
                }
            }

        }
        private void ButtonSixClick()
        {
            //RIGHT BUMPER

            if (anyObjectSelected)
            {
                if (currentlySelectedGroup == marketPanel)
                {
                    if (IsShipAtPlayer())
                        EnableOrderCanvas(true);
                }
                else if (currentlySelectedGroup == ordersPanel)
                {
                    if (!mySmallShip.getLeftPort())
                    {
                        EnableTradeCanvas(true);
                    }
                    else
                    {
                        EnableMarketCanvas(true);
                    }
                }
                else if (currentlySelectedGroup == tradingPanel)
                {
                    if (IsShipAtPlayer())
                        EnableMarketCanvas(true);
                }
                //if (Time.time >= nextInteractionTime)
                //{
                //    currentlySelectedGroup.PlayerButtonLBClicked();
                //    nextInteractionTime = Time.time + nextInteractionDelay;
                //}
            }
        }
        void Start()
        {
            //SETUP MARKET ITEMS
            marketPanel.Init();
            var allResources = System.Enum.GetValues(typeof(ResourceType));
            int index = 0;
            MarketButtomItem[] myButtons = marketPanel.MyTradeItems;
            foreach (ResourceType currType in allResources)
            {
                myButtons[index].SetupMarketItem(currType, CurrencyManagerSingleton.Instance.ResourceSprites[currType], CurrencyManagerSingleton.Instance.GoldSprite, this);
                index++;
            }

            //SETUP TRADE ITEMS
            tradingPanel.Init(this);

            TradeButtonItem[] tradingItems = tradingPanel.myTradeItems;
            tradingItems[0].SetupGold(CurrencyManagerSingleton.Instance.GoldSprite, this);
            index = 1;
            foreach (ResourceType currType in allResources)
            {
                tradingItems[index].SetupTradeItem(currType, CurrencyManagerSingleton.Instance.ResourceSprites[currType], CurrencyManagerSingleton.Instance.GoldSprite, this);
                index++;
            }

            //SETUP ORDER ITEMS
            ordersPanel.Init(this);

            UpdateResourceIcons();
            DisableAllCanvases(false);
            //EnableTradeCanvas(true);

            /*
            canvasIcons = new Image[3];
            canvasIcons[0] = transform.GetChild(0).Find("IconImageHandShake").gameObject.GetComponent<Image>();
            canvasIcons[1] = transform.GetChild(0).Find("IconImageCrates").gameObject.GetComponent<Image>();
            canvasIcons[2] = transform.GetChild(0).Find("IconImageShip").gameObject.GetComponent<Image>();
            */
            //canvasIcons[1] //order
            //    canvasIcons[2] //market

        }
        public void SendShipToPlayer(int sentDestination, int _sendGold, Dictionary<ResourceType, int> _tradeOrder)
        {
            queuedOrders.Enqueue(_tradeOrder);
            sendGold = _sendGold;
            sendDestination = sentDestination;
            mySmallShip.sendShip(sentDestination - 1);
            DisableTradePanel(false);
        }
        void ExecuteOrder()
        {
            Systems.MarketeersGameManager.Instance.SendResourcesToPlayer(queuedOrders.Dequeue(), sendGold, sendDestination);
        }

        // Update is called once per frame
        void Update()
        {
            //goldToAdd
        }
    }
}