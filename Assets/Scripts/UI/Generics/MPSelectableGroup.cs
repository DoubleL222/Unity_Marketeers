using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Generics
{
    public class MPSelectableGroup : MPSelectableObject {
        private List<MPSelectableObject> my_Selectable_Children;
        private int selectedIndex = 0;

        public bool Vertical = true;

        private bool properlyInitialized = false;
        //    private bool isSelected = false;

        public bool RemoveOBJ(MPSelectableObject _remove)
        {
            return my_Selectable_Children.Remove(_remove);
        }
        // Use this for initialization
        void RecursivellyLookForSelectableChildren(Transform _nextTransform)
        {
            MPSelectableObject newSelectableObject = null;
            newSelectableObject = _nextTransform.gameObject.GetComponent<MPSelectableObject>();
            if (newSelectableObject != null)
            {
                if (newSelectableObject.GetType() == typeof(MPSelectableGroup) && _nextTransform != transform)
                {
                    my_Selectable_Children.Add(newSelectableObject);
                    return;
                }
            }
            if(newSelectableObject != null && _nextTransform != transform)
                my_Selectable_Children.Add(newSelectableObject);
            if (_nextTransform.childCount > 0)
            {
                for (int i = 0; i < _nextTransform.childCount; i++)
                {
                    RecursivellyLookForSelectableChildren(_nextTransform.GetChild(i));
                }
            }
            return;

        }
        public override void Awake()
        {
            base.Awake();
            //MPSelectableObject[] myChildren;
            //myChildren = GetComponentsInChildren<MPSelectableObject>(true);
            my_Selectable_Children = new List<MPSelectableObject>();
            RecursivellyLookForSelectableChildren(transform);
            //Debug.Log(my_Selectable_Children.Count + " My Children");
            //for (int i = 0; i < myChildren.Length; i++)
            //{
            
            //    if (myChildren[i] != this)
            //    {
            //        //Debug.Log(myChildren[i].transform.position);
            //        my_Selectable_Children.Add(myChildren[i]);
            //    }
            //}
            if (Vertical)
            {
                //my_Selectable_Children.Sort((m1, m2) => (int)(m2.gameObject.GetComponent<RectTransform>().position.y - m1.gameObject.GetComponent<RectTransform>().position.y));
            }
            else
            {
                //my_Selectable_Children.Sort((m1, m2) => (int)(m2.gameObject.GetComponent<RectTransform>().position.x - m1.gameObject.GetComponent<RectTransform>().position.x));
            }
            for (int i = 0; i < my_Selectable_Children.Count; i++)
            {
                //Debug.Log("Button Positions " + my_Selectable_Children[i].GetComponent<RectTransform>().anchoredPosition);
            }
            if (my_Selectable_Children.Count > 0)
            {
                properlyInitialized = true;
            }
            //Debug.Log("My selected children count: " + my_Selectable_Children.Count);
        }

        public virtual void Activate()
        {
            selectedIndex = 0;
            Select();
        }

        public override void Deselect()
        {
            if (my_Selectable_Children != null)
            {
                foreach (MPSelectableObject _currObj in my_Selectable_Children)
                {
                    _currObj.Deselect();
                }
            }
            base.Deselect();
            
        }

        public override void Select()
        {
            isSelected = true;
            my_Selectable_Children[selectedIndex].Select();
            base.Select();
        }

        public void Click()
        {
            if (my_Selectable_Children[selectedIndex].GetType() == typeof(MPButton) || my_Selectable_Children[selectedIndex].GetType().BaseType == typeof(MPButton))
            {
                ((MPButton) (my_Selectable_Children[selectedIndex])).CLICK();
            }
            if (my_Selectable_Children[selectedIndex].GetType() == typeof(MPSelectableGroup) || my_Selectable_Children[selectedIndex].GetType().BaseType == typeof(MPSelectableGroup))
            {
                ((MPSelectableGroup)(my_Selectable_Children[selectedIndex])).Click();
            }
        }
        public void PlayerButtonAClicked()
        {
        
        }
        public void PlayerButtonBClicked()
        {
            if (my_Selectable_Children[selectedIndex].GetType() == typeof(MPButton) || my_Selectable_Children[selectedIndex].GetType().BaseType == typeof(MPButton))
            {
                ((MPButton)(my_Selectable_Children[selectedIndex])).CLICK_BUTTONB();
            }
            if (my_Selectable_Children[selectedIndex].GetType() == typeof(MPSelectableGroup) || my_Selectable_Children[selectedIndex].GetType().BaseType == typeof(MPSelectableGroup))
            {
                ((MPSelectableGroup)(my_Selectable_Children[selectedIndex])).PlayerButtonBClicked();
            }
        }
        public void PlayerButtonXClicked()
        {
            if (my_Selectable_Children[selectedIndex].GetType() == typeof(MPButton) || my_Selectable_Children[selectedIndex].GetType().BaseType == typeof(MPButton))
            {
                ((MPButton)(my_Selectable_Children[selectedIndex])).CLICK_BUTTONX();
            }
            if (my_Selectable_Children[selectedIndex].GetType() == typeof(MPSelectableGroup) || my_Selectable_Children[selectedIndex].GetType().BaseType == typeof(MPSelectableGroup))
            {
                ((MPSelectableGroup)(my_Selectable_Children[selectedIndex])).PlayerButtonXClicked();
            }
        }
        public void PlayerButtonYClicked()
        {
        
        }

        public bool IsProperlyInitialized()
        {
            return properlyInitialized;
        }

        public void InputYChanged(float _change)
        {
            if (Vertical)
            {
                if (_change > 0)
                {
                    SelectNew(selectedIndex - 1);
                }
                else if (_change < 0)
                {
                    SelectNew(selectedIndex + 1);
                }
            }
            else
            {
                if (my_Selectable_Children[selectedIndex].GetType() == typeof(MPSelectableGroup))
                {
                    ((MPSelectableGroup)(my_Selectable_Children[selectedIndex])).InputYChanged(_change);
                }
            }
        }

        public void InputXChanged(float _change)
        {
            if (!Vertical)
            {
                if (_change > 0)
                {
                    SelectNew(selectedIndex - 1);
                }
                else if (_change < 0)
                {
                    SelectNew(selectedIndex + 1);
                }
            }
            else
            {
                if (my_Selectable_Children[selectedIndex].GetType() == typeof(MPSelectableGroup))
                {
                    ((MPSelectableGroup)(my_Selectable_Children[selectedIndex])).InputXChanged(_change);
                }
            }
        }

        public void SelectNew(int _newIndex)
        {
            int prevIndex = selectedIndex;
        
            selectedIndex = _newIndex;
            if (selectedIndex > (my_Selectable_Children.Count - 1))
            {
                selectedIndex = 0;
            }
            else if (selectedIndex < 0)
            {
                selectedIndex = my_Selectable_Children.Count - 1;
            }
            my_Selectable_Children[prevIndex].Deselect();
            my_Selectable_Children[selectedIndex].Select();
        }

        private void OnDisable()
        {
            Deselect();
        }

        internal void PlayerButtonLBClicked()
        {
            Debug.Log("LEFT BUMPER");
        }
    }
}
