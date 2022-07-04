using Systems;
using System.Collections.Generic;
using Market.Generics;
using UI.Generics;
using UnityEngine;

namespace UI
{
    public class TradeSelectablePanel : MPSelectableGroup
    {
        public MPButton confirmButton;
        public MPSelectableGroup adressGroup;
        public TradeButtonItem[] myTradeItems;
        private int _selectedPlayerId = -1;
        private PlayerCanvasController _myController;

        public void SetNewPlayerId(int playerId, Color playerColor)
        {
            _selectedPlayerId = playerId;
            confirmButton.normal_color = playerColor;
            confirmButton.SetNewColor(playerColor);
            adressGroup.selected_color = playerColor;
            adressGroup.Select();
        }

        public void TryToSendOutOrder()
        {
            int goldAmount = 0;
            Dictionary<ResourceType, int> tradeOrder = new Dictionary<ResourceType, int>();
            foreach (TradeButtonItem _currItem in myTradeItems)
            {
                if (!_currItem.isGold)
                {
                    if (_currItem.GetCounter() > 0)
                    {
                        tradeOrder.Add(_currItem.myType, _currItem.GetCounter());
                    }
                }
                else
                {
                    goldAmount = _currItem.GetCounter();
                }
            }
            ProcessOrder(tradeOrder, goldAmount);
            ResetCounters();
        }

        bool ProcessOrder(Dictionary<ResourceType, int> TradeOrders, int goldAmount)
        {
            foreach (KeyValuePair<ResourceType, int> _currOrder in TradeOrders)
            {
                if (!_myController.myPlayerInventory.HasResourceAmount(_currOrder.Key, _currOrder.Value))
                    return false;
            }
            if (!_myController.myPlayerInventory.HasGoldAmount(goldAmount))
                return false;

            foreach (KeyValuePair<ResourceType, int> _currOrder in TradeOrders)
            {
                _myController.myPlayerInventory.ChangeResourceAmount(_currOrder.Key, -_currOrder.Value);
            }
            _myController.myPlayerInventory.ChangeGold(-goldAmount);
            _myController.SendShipToPlayer(_selectedPlayerId, goldAmount, TradeOrders);
            _myController.UpdateResourceIcons();
            return true;
        }

        void ResetCounters()
        {
            foreach (TradeButtonItem currItem in myTradeItems)
            {
                currItem.ResetCounter();
            }
        }

        public override void Activate()
        {
            base.Activate();
            ResetCounters();
            PlayerDestinationButton[] myDestinationButtons = GetComponentsInChildren<PlayerDestinationButton>();
            int playerIndex = 1;
            foreach (PlayerDestinationButton currButton in myDestinationButtons)
            {
                if (_myController.myPlayer.playerNumber == playerIndex)
                {
                    playerIndex++;
                
                }
                currButton.SetupPlayerButton(playerIndex, Systems.MarketeersGameManager.Instance.playerColors[playerIndex], this);

                playerIndex++;
            }

        }

        public override void Awake()
        {
            myTradeItems = GetComponentsInChildren<TradeButtonItem>(true);
        
            base.Awake();
        }

        public void Init(PlayerCanvasController controller)
        {
            _myController = controller;
        }
    }
}