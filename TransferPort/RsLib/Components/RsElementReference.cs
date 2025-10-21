using System;
using UnityEngine;

namespace RsLib.Components
{
    [Serializable]
    public struct RsElementReference
    {
        public string Name;
        public GameObject target;
    }
}