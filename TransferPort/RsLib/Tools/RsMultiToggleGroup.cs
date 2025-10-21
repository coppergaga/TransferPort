using System;
using System.Collections.Generic;
using UnityEngine;

namespace RsLib
{
    [Serializable]
    public class RsMultiToggleGroup
    {
        [SerializeField]
        private List<MultiToggle> toggles = new List<MultiToggle>();
        [SerializeField]
        private int m_selected = -1;
        
        public event Action<int> onSelected;
        
        public int selected => m_selected;
        
        public void Add(MultiToggle toggle)
        {
            toggles.Add(toggle);
            int i = toggles.Count - 1;
            toggle.onClick += () => { OnToggleClick(i); };
        }

        public void Select(int index)
        {
            if (selected == index)
            {
                return;
            }

            SelectNoCheck(index);
        }
        
        protected void SelectNoCheck(int index)
        {
            m_selected = index;
            for (var i = 0; i < toggles.Count; i++)
            {
                MultiToggle toggle = toggles[i];
                if (index != i)
                {
                    toggle.ChangeState(0);
                }
                else
                {
                    toggle.ChangeState(1);
                }
            }
            onSelected?.Invoke(index);
        }
        
        protected void OnToggleClick(int index)
        {
            Select(index);
        }
        
    }
}