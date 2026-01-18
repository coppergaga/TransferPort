using System;
using System.Collections.Generic;
using RsLib.Adapter;
using UnityEngine;

namespace RsTransferPort {
    public class PriorityBar : MonoBehaviour {
        [SerializeField]
        private MultiToggleAdapter[] toggles;

        [SerializeField]
        private ToolTipAdapter help;

        public Action<int> OnPriorityClick;

        private readonly int[] statesCache = new int[9];

        public ToolTip Help => help;
        private void Start() {
            for (var index = 0; index < toggles.Length; index++) {
                MultiToggleAdapter multiToggle = toggles[index];
                int index1 = index + 1;
                multiToggle.onClick += delegate { OnMultClick(index1); };
            }
        }

        private void OnMultClick(int priority) {
            OnPriorityClick?.Invoke(priority);
        }

        public void SetPriorityState(int priority, int state) {
            if (priority >= 1 && priority <= 9) {
                toggles[priority - 1].ChangeState(state);
            }
        }

        public void SetAllPriorityState(int state) {
            foreach (MultiToggleAdapter toggle in toggles) {
                toggle.ChangeState(state);
            }
        }

        public void SetAllStateCache(int state) {
            for (var i = 0; i < statesCache.Length; i++) {
                statesCache[i] = 0;
            }
        }

        public void SetStateCache(int priority, int state) {
            if (priority >= 1 && priority <= 9) {
                statesCache[priority - 1] = state;
            }
        }

        public void SetStateCacheRange(ICollection<int> priorities, int state) {
            foreach (int priority in priorities) {
                SetStateCache(priority, state);
            }
        }

        public void ApplyStateCache() {
            for (var i = 0; i < statesCache.Length; i++) {
                toggles[i].ChangeState(statesCache[i]);
            }
        }
    }
}