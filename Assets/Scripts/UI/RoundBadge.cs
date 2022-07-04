using Entities;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RoundBadge : MonoBehaviour
    {
        public Text BadgeText;
        private ShipMovement _ship;
        public PlayerInput PlayerInput;
        private int _visitsLeft;

        // Use this for initialization
        void Start ()
        {
            _ship = GameObject.FindGameObjectWithTag("Ship").GetComponent<ShipMovement>();

            _visitsLeft = _ship.NumOfRoundsLeft;

            if (_ship)
                _ship.OnLeavePort += UpdateText;

            BadgeText.text = "" + _visitsLeft;
        }

        void UpdateText(int playerIndexOfPort)
        {
            if (playerIndexOfPort == PlayerInput.playerNumber - 1)
                BadgeText.text = "" + --_visitsLeft;
        }
    }
}