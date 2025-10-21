using System;
using UnityEngine;

namespace RsTransferPort
{
    [CreateAssetMenu(fileName = "LineCenterAsset", menuName = "TransferPor/Line Center Asset",order = 1)]
    public class LineCenterAsset : ScriptableObject
    {
        public Sprite gas;
        public Sprite liquid;
        public Sprite solid;
        public Sprite power;
        public Sprite logic;
        public Sprite rp;

        public Sprite GetSpriteByBuildingType(BuildingType type)
        {
            switch (type)
            {
                case BuildingType.Gas: return gas;
                case BuildingType.Liquid: return liquid;
                case BuildingType.Solid: return solid;
                case BuildingType.Power: return power;
                case BuildingType.Logic: return logic;
                case BuildingType.HEP: return rp;
                default: return null;
            }
        }

    }
}