using UI.Generics;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerDestinationButton : MPButton {
        public int playerId;
        public Color myPlayerColor;
        public TradeSelectablePanel myParentPanel;

        public void SetupPlayerButton(int _playerID, Color _color, TradeSelectablePanel _panel)
        {
            playerId = _playerID;
            myPlayerColor = _color;
            myParentPanel = _panel;
            transform.GetChild(0).GetComponent<Image>().color = myPlayerColor;
        }

        public override void CLICK_BUTTONX()
        {
            Debug.Log("CLICKED ON PLAYER DESTINATION");
            base.CLICK_BUTTONX();
            myParentPanel.SetNewPlayerId(playerId, myPlayerColor);
        }
    }
}
