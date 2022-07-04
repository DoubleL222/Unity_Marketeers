using UnityEngine;
using UnityEngine.UI;

public class WinLoseScreen : MonoBehaviour {
    public Text _text;
    public void UpdateText(int place, int gold, bool showScreen = true)
    {
        switch (place)
        {
            case 1:
                _text.text = "You were one of the\nbest marketeers!\n\nYou made " + gold + " gold!";
                break;
            case 2:
                _text.text = "You were one of the\nbest marketeers!\n\nYou made " + gold + " gold!";
                break;
            case 3:
                _text.text = "You were one of the\nbest marketeers!\n\nYou made " + gold + " gold!";
                break;
            case 4:
                _text.text = "You were the worst marketeer!\n\nYou made " + gold + " gold!";
                break;
        }

        gameObject.SetActive(showScreen);
    }
}