using UnityEngine;

namespace RsLib.Components {
    public class RsHierarchyReferences : MonoBehaviour {
        [SerializeField]
        public GameObject[] references;

        private void Awake() {
            if (references == null) {
                Debug.LogWarning("RsHierarchyReferences.references is null");
                references = new GameObject[0];
            }
        }

        public bool HasReference(string name) {
            foreach (GameObject reference in this.references) {
                if (reference.name == name)
                    return true;
            }
            return false;
        }

        public T GetReference<T>(string name) where T : Component {
            foreach (GameObject reference in this.references) {
                if (reference != null && reference.name == name) {
                    T component = reference.GetComponent<T>();
                    if (component != null) {
                        return component;
                    }
                }
            }
            Debug.LogError($"Could not find UI reference '{name}' or convert to specified type)");
            return default;
        }

        public GameObject GetReference(string name) {
            foreach (GameObject reference in this.references) {
                if (reference != null && reference.name == name)
                    return reference;
            }
            Debug.LogWarning($"Couldn't find reference to object named {name} Make sure the name matches the field in the inspector.");
            return null;
        }
    }
}