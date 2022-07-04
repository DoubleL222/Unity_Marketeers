using Systems;
using Entities;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ShipIndicator : MonoBehaviour
    {
        private ShipMovement _ship;
        public Text PlayerNumberText;
        public Image BadgeImage;

        void Start()
        {
            _ship = FindObjectOfType<ShipMovement>();
            _ship.OnLeavePort += UpdateBadge;
            UpdateBadge(0);
        }

        void UpdateBadge(int playerIndexOfPort)
        {
            PlayerNumberText.text = "" + _ship.GetCurrentTargetPlayerNumber();
            BadgeImage.color = MarketeersGameManager.Instance.playerColors[_ship.GetCurrentTargetPlayerNumber()];
        }
    }
}