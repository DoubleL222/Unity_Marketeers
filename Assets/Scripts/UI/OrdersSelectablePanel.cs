using System.Collections.Generic;
using Systems;
using AssetStore.SplitScreenAudio.Code;
using Market;
using Market.Generics;
using UI.Generics;
using UnityEngine;

namespace UI
{
    public class OrdersSelectablePanel : MPSelectableGroup {
        public PlayerCanvasController myCanvas;
        private OrderButtonItem[] myOrderButtons;
        private List<Order> myOrders;
        private VirtualAudioSource _audioSource;

        public override void Awake()
        {
            base.Awake();
            myOrderButtons = GetComponentsInChildren<OrderButtonItem>();
            myOrders = new List<Order>();
        }

        // Use this for initialization
        public void Init(PlayerCanvasController canvas)
        {
            myCanvas = canvas;
            _audioSource = myCanvas.gameObject.GetComponent<VirtualAudioSource>();
        }

        void GetNewOrder()
        {
            myOrders.Add(CurrencyManagerSingleton.Instance.ReceiveOrder());
            if (myOrders.Count > myOrderButtons.Length)
            {
                myOrders.RemoveAt(myOrders.Count-1);
            }
        }

        void FillMyOrders()
        {
            CurrencyManagerSingleton.Instance.ReEavaluateAllOrders();
            while (CurrencyManagerSingleton.Instance.GetOrderCount() < myOrderButtons.Length){
                GetNewOrder();
            }
            //Call GetAllOrders to get all orders
            //Call GetOrders to only get the number of "allowed" orders
            List<Order> newOrders = CurrencyManagerSingleton.Instance.GetAllOrders(); 

            myOrders = new List<Order>();
            for (int i = 0; i < myOrderButtons.Length; i++){
                myOrders.Add(newOrders[i]);
            }
        }

        public override void Select()
        {
            base.Select();
            FillMyOrders();
            PrepareButtons();
        }

        void PrepareButtons()
        {
            for (int i = 0; i < myOrderButtons.Length; i++)
            {
                myOrderButtons[i].SetupOrder(myOrders[i], this);
            }
        }

        public void ProcessOrder(Order _processOrder)
        {
            Debug.Log("PROCESSING ORDERS"); 
            foreach (KeyValuePair<ResourceType, int> _kvp in _processOrder.ResourceList)
            {
                if (!myCanvas.myPlayerInventory.HasResourceAmount(_kvp.Key, _kvp.Value))
                {
                    Debug.Log("DOES NOT HAVE RESOURCE");
                    return;
                }
            }
            foreach (KeyValuePair<ResourceType, int> _kvp in _processOrder.ResourceList)
            {
                myCanvas.myPlayerInventory.ChangeResourceAmount(_kvp.Key, -_kvp.Value);
            }
            myCanvas.myPlayerInventory.ChangeGold(_processOrder.Reward);
            myCanvas.UpdateResourceIcons();
            CurrencyManagerSingleton.Instance.HandInOrder(_processOrder);
            _audioSource.clip = SoundManager.Instance.MoneyBag;
            _audioSource.Play();
            FillMyOrders();
            PrepareButtons();
            return;
        }

        public void ReEvaluateOrders(){
            foreach (var order in myOrders){
                CurrencyManagerSingleton.Instance.ReevaluateOrder(order);
            }
        }
    }
}
