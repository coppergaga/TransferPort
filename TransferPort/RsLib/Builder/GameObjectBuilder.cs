using UnityEngine;

namespace RsLib.Builder {
    public class GameObjectBuilder : BaseBuilder<GameObject> {
        public override void Build() {
            AppendPrefix();
            Append("|- ");
            Append(target.name);
            Append(" [ ");

            AppendKeyValue("active", target.activeSelf, true);

            AppendKeyValue("instanceID", target.GetInstanceID(), true);

            AppendKeyValue("layer", LayerMask.LayerToName(target.layer), true);
            Append("|").Append(target.layer);

            AppendKeyValue("tag", target.tag, true);

            AppendKeyValue("static", target.isStatic, true);
            CheckAddendSpace();
            Append("]");

            Component[] components = target.GetComponents<Component>();

            foreach (Component component in components) {
                otherBuilderInfos.Add(new OtherBuilderInfo(prefix + "      ", component));
            }

            for (var i = 0; i < target.transform.childCount; i++) {
                otherBuilderInfos.Add(new OtherBuilderInfo(prefix + "    ", target.transform.GetChild(i).gameObject));
            }

        }
    }
}