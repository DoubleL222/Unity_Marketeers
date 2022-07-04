using UnityEngine;
using UnityEngine.Events;

namespace UI.Generics
{
    public class MPButton : MPSelectableObject
    {
        [Header("Color Settings")]
        public Color clicked_color;
        public Color button_X_clicked_color;
        public Color button_B_clicked_color;

        [Header("Executed every click")]
        public UnityEvent On_Click_Ations;
        [Header("Executed On Button One click")]
        public UnityEvent Button_X_Actions;
        [Header("Executed On Button B (two) click")]
        public UnityEvent Button_B_Actions;

        [Header("Button click color durration")]
        public float click_duration = 0.2f;

        private bool isClicked = false;
        private float nextResetTime = float.MaxValue;

        public virtual void CLICK()
        {
            if (!isClicked)
            {
                if (On_Click_Ations != null)
                {
                    On_Click_Ations.Invoke();
                }
                SetClickedColor(clicked_color);
                isClicked = true;
            }
        }

        public virtual void CLICK_BUTTONB()
        {
            //Debug.Log("Clicked B");
            if (!isClicked)
            {
                if (Button_B_Actions != null)
                {
                    Button_B_Actions.Invoke();
                }
                SetClickedColor(button_B_clicked_color);
                isClicked = true;
            }
        }

        public virtual void CLICK_BUTTONX()
        {
            //Debug.Log("Clicked X");
            if (!isClicked)
            {
                if (Button_X_Actions != null)
                {
                    Button_X_Actions.Invoke();
                }
                SetClickedColor(button_X_clicked_color);
                isClicked = true;
            }
        }

        private void SetClickedColor(Color _selColor)
        {
            SetNewColor(_selColor);
            nextResetTime = Time.time + click_duration;
        }

        public override void Update()
        {
            if (isClicked)
            {
                if (nextResetTime <= Time.time)
                {
                    SetPrevColor();
                    isClicked = false;
                }
            }
        }
    }
}