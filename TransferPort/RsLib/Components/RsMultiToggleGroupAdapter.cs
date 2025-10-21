using System;
using RsLib.Components;
using UnityEngine;

namespace RsLib.Adapter
{
    public class RsMultiToggleGroupCom : MonoBehaviour
    {
        [SerializeField] public MultiToggle[] toggles;
       
        private int m_selected = -1;
        
        public event Action<int> onSelected;
        
        public int selected => m_selected;
        
        protected void Awake()
        {
            for (var i = 0; i < toggles.Length; i++)
            {
                MultiToggle toggle = toggles[i];
                var i1 = i;
                toggle.onClick += () => { OnToggleClick(i1); };
            }
        }
        
        public void Select(int index, bool triggerOnSelected = false)
        {
            if (selected == index)
            {
                return;
            }

            SelectNoCheck(index, triggerOnSelected);
        }

        protected void SelectNoCheck(int index, bool triggerOnSelected = false)
        {
            m_selected = index;
            for (var i = 0; i < toggles.Length; i++)
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

            if (triggerOnSelected)
            {
                onSelected?.Invoke(index);
            }
        }
        
        protected void OnToggleClick(int index)
        {
            Select(index, true);
        }
    }
}