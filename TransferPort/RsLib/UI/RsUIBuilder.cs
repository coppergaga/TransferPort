using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace RsLib
{

    public delegate void ComponentInitDelegate<in T>(T c, RsUIBuilder r) where T : Component;
    
    public class RsUIBuilder
    {
        private Dictionary<string, object> reference = new Dictionary<string, object>();
        private List<Task> tasks = new List<Task>();
        private Stack<GameObject> parentStack = new Stack<GameObject>();
        private GameObject currentObject;
        private GameObject currentParent;

        public RsUIBuilder Parent(GameObject currentParent)
        {
            this.currentParent = currentParent;
            parentStack.Push(currentParent);
            return this;
        }

        public RsUIBuilder Go(string name = null, string referenceName = null)
        {
            currentObject = UIGameObject(name, currentParent);
            TryAddReference(referenceName, currentObject);
            return this;
        }
        
        public RsUIBuilder Go(GameObject gameObject)
        {
            return Go(null, gameObject);
        }
        public RsUIBuilder Go(string referenceName, GameObject gameObject)
        {
            currentObject = gameObject;
            currentObject.transform.SetParent(currentParent.transform);
            TryAddReference(referenceName, currentObject);
            return this;
        }
        
        public RsUIBuilder Go(Func<GameObject, GameObject> create)
        {
            return Go(null, create);
        }
        
        public RsUIBuilder Go(string referenceName, Func<GameObject, GameObject> create)
        {
            currentObject = create(currentParent);
            currentObject.transform.SetParent(currentParent.transform);
            TryAddReference(referenceName, currentObject);
            return this;
        }

        public RsUIBuilder Add<T>()
            where  T : Component
        {
            return Add<T>(null, null);
        }
        public RsUIBuilder Add<T>(ComponentInitDelegate<T> create)
            where  T : Component
        {
            return Add<T>(null, create);
        }
        public RsUIBuilder Add<T>(string referenceName , ComponentInitDelegate<T> create )
            where  T : Component
        {
            tasks.Add(new Task(
                gameObject: currentObject,
                referenceName: referenceName,
                componentType: typeof(T),
                create:  create,
                actionType: ActionType.ADD
                ));
            return this;
        }
        
        public RsUIBuilder AddOrGet<T>()
            where  T : Component
        {
            return AddOrGet<T>(null);
        }
        public RsUIBuilder AddOrGet<T>(ComponentInitDelegate<T> create)
            where  T : Component
        {
            return AddOrGet(null, create);
        }
        public RsUIBuilder AddOrGet<T>(string referenceName, ComponentInitDelegate<T> create)
            where  T : Component
        {
            
            tasks.Add(new Task(
                gameObject: currentObject,
                referenceName: referenceName,
                componentType: typeof(T),
                create: create,
                actionType: ActionType.ADD_OR_GET
            ));
            return this;
        }
          
        public RsUIBuilder Get<T>()
            where  T : Component
        {
            return Get<T>(null);
        }
        public RsUIBuilder Get<T>(ComponentInitDelegate<T> create)
            where  T : Component
        {
            return Get<T>(null, create);
        }
        public RsUIBuilder Get<T>(string referenceName = null, ComponentInitDelegate<T> create = null)
            where  T : Component
        {
            tasks.Add(new Task(
                gameObject: currentObject,
                referenceName: referenceName,
                componentType: typeof(T),
                create:  create,
                actionType: ActionType.GET
            ));
            return this;
        }
        
        public RsUIBuilder DebugColor(Color color)
        {
            Graphic graphic;
            if (!currentObject.TryGetComponent(out graphic))
            {
                graphic = currentObject.AddComponent<Image>();
            }
            graphic.color = color;
            return this;
        }

        public void TryAddReference(string key, object obj)
        {
            if (key != null)
            {
                reference[key] = obj;
            }
        }

        public RsUIBuilder Child(Action<GameObject, RsUIBuilder> b)
        {
            parentStack.Push(currentObject);
            currentParent = currentObject;
            currentObject = null;
            b?.Invoke(currentParent, this);
            currentObject = parentStack.Pop();
            currentParent = parentStack.Count > 0 ? parentStack.Peek() : null;
            return this;
        }

        public T Reference<T>(string key) where T : class
        {
            if (reference.TryGetValue(key, out object a))
            {
                return a as T;
            }
            return null;
        }
        
        public void Build()
        {
            foreach (Task task in tasks)
            {
                GameObject taskGameObject = task.gameObject;
                if (task.componentType != null)
                {
                    Component component = null;
                    if (task.actionType == ActionType.GET)
                    {
                        component = taskGameObject.GetComponent(task.componentType);
                    } else if (task.actionType == ActionType.ADD)
                    {
                        component = taskGameObject.AddComponent(task.componentType);
                    }
                    else if (task.actionType == ActionType.ADD_OR_GET)
                    {
                        component = taskGameObject.GetComponent(task.componentType);
                        if (component == null)
                        {
                            component = taskGameObject.AddComponent(task.componentType);
                        }
                    }

                    if (task.referenceName != null)
                    {
                        reference[task.referenceName] = component;
                    }

                    task.instance = component;
                }
            }
            
            foreach (Task task in tasks)
            {
                task.create?.DynamicInvoke(task.instance, this);
            }
        }

        public GameObject CurrentObject()
        {
            return currentObject;
        }

        public void HorizontalLayout(bool expandWidth, bool expandHeight, bool controlWidth, bool controlHeight)
        {
            AddOrGet<HorizontalLayoutGroup>((c, u) =>
            {
                c.childForceExpandWidth = expandWidth;
                c.childForceExpandHeight = expandHeight;
                c.childControlWidth = controlWidth;
                c.childControlHeight = controlHeight;
            });
        }
        public void VerticalLayout(bool expandWidth, bool expandHeight, bool controlWidth, bool controlHeight)
        {
            AddOrGet<VerticalLayoutGroup>((c, u) =>
            {
                c.childForceExpandWidth = expandWidth;
                c.childForceExpandHeight = expandHeight;
                c.childControlWidth = controlWidth;
                c.childControlHeight = controlHeight;
            });
        }

        public class Task
        {
            public GameObject gameObject;
            public string referenceName;
            public Type componentType;
            public Delegate create;
            public ActionType actionType;

            public Component instance;

            public Task(GameObject gameObject, string referenceName, Type componentType, Delegate create, ActionType actionType)
            {
                this.gameObject = gameObject;
                this.referenceName = referenceName;
                this.componentType = componentType;
                this.create = create;
                this.actionType = actionType;
            }
        }
        
   
        
        
        public enum ActionType
        {
            ADD,
            GET,
            ADD_OR_GET,
        }
        
        
        
        
        public static GameObject BlockLine(GameObject parent = null, float height = 5f)
        {
            GameObject root = UIGameObject("BlockLine", parent);
            root.rectTransform().sizeDelta = new Vector2(height, height);
            LayoutElement layoutElement = root.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = height;
            layoutElement.minHeight = height;
            return root;
        }
        
        public static GameObject UIGameObject(string name = "UIGameObject",GameObject parent = null, bool active = true)
        {
            GameObject gameObject = new GameObject {
                name = name,
                layer = LayerMask.NameToLayer("UI")
            };
            if (parent != null)
            {
                gameObject.transform.SetParent(parent.transform, false);
            }
            gameObject.AddOrGet<RectTransform>();
            if (gameObject.activeSelf != active)
            {
                gameObject.SetActive(active);
            }
            return gameObject;
        }

        public static Image ImageGO(Sprite sprite = null, GameObject parent = null, bool setNativeSize = false)
        {
            GameObject gameObject = UIGameObject("Image", parent);
            Image image = gameObject.AddComponent<Image>();
            image.sprite = sprite;
            if (setNativeSize)
            {
                image.SetNativeSize();
            }
            return image;
        }

        public static MultiToggle CheckBoxGO(GameObject parent = null)
        {
            Vector2 size = new Vector2(22,22);
            GameObject root = UIGameObject("CheckBox",parent, false);
            root.rectTransform().sizeDelta = size;

            Image bg = ImageGO(RsUITuning.Images.CheckBorder, root);
            bg.name = "BG";
            bg.color = Color.black;
            bg.rectTransform.FullParent();
         

            Image check = ImageGO(RsUITuning.Images.Checked, root);
            bg.name = "check";
            check.rectTransform.FullParent();
            
            LayoutElement layoutElement = root.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = size.x;
            layoutElement.preferredHeight = size.y;
                
            MultiToggle multiToggle = root.AddComponent<MultiToggle>();
            multiToggle.toggle_image = check;

            multiToggle.states = new ToggleState[]
            {
                new ToggleState()
                {
                    Name = "off",
                    sprite = null,
                    additional_display_settings = new StatePresentationSetting[0],
                    color = Color.clear,
                },
                new ToggleState()
                {
                    Name = "on",
                    sprite = RsUITuning.Images.Checked,
                    additional_display_settings = new StatePresentationSetting[0],
                    color = Color.black,
                },
            };
            multiToggle.ChangeState(0);
            root.SetActive(true);
            return multiToggle;
        }

        public static LocText LocTextGo(LocString text,TextStyleSetting styleSetting = null, GameObject parent = null)
        {
            var root = UIGameObject("LocText", parent);
            root.SetActive(false);
            LocText locText = root.AddComponent<LocText>();
            locText.allowOverride = false;
            locText.textStyleSetting = styleSetting != null ? styleSetting : RsUITuning.ScriptableObjects.style_labelText;
            locText.key = text.key.IsValid() ? text.key.String : "";
            locText.text = text.text;
            
            root.SetActive(true);
            root.rectTransform().sizeDelta = new Vector2(100, locText.textStyleSetting.fontSize);
            root.rectTransform().SetSizeWithPreferred();
            return locText;
        }
        
        public static LocText LocTextTooltipGo(LocString text, string tooltip, TextStyleSetting styleSetting = null, GameObject parent = null)
        {
            var root = UIGameObject("LocText", parent);
            root.SetActive(false);
            LocText locText = root.AddComponent<LocText>();
            locText.allowOverride = false;
            locText.textStyleSetting = styleSetting != null ? styleSetting : RsUITuning.ScriptableObjects.style_labelText;
            locText.key = text.key.IsValid() ? text.key.String : "";
            locText.text = text.text;

            ToolTip cToolTip = root.AddComponent<ToolTip>();
            cToolTip.toolTip = tooltip;

            root.SetActive(true);
            root.rectTransform().sizeDelta = new Vector2(100, locText.textStyleSetting.fontSize);
            root.rectTransform().SetSizeWithPreferred();
            return locText;
        }

        public static GameObject LocTextCheckBoxGo(LocString text, GameObject parent = null)
        {
            GameObject root = UIGameObject("LocTextCheckBox", parent, false);

            {
                HorizontalLayoutGroup layoutGroup = root.AddComponent<HorizontalLayoutGroup>();
                layoutGroup.childForceExpandWidth = false;
                layoutGroup.childForceExpandHeight = true;
                layoutGroup.childControlHeight = true;
            }
            
            GameObject toggleParent = UIGameObject("layout", root);
            
            {
                LayoutElement layoutElement = toggleParent.AddComponent<LayoutElement>();
                layoutElement.minWidth = 22;
                layoutElement.minHeight = 22;
            }
            
            MultiToggle multiToggle = CheckBoxGO(toggleParent);

            {
                RectTransform toggleRect = multiToggle.rectTransform();
                toggleRect.anchorMin = new Vector2(0.5f,0.5f);
                toggleRect.anchorMax = new Vector2(0.5f, 0.5f);
            }

            LocTextGo(text, null, root);
            root.SetActive(true);
            return root;
        }
        
        

        public static GameObject ToggleEntry(
            LocString text, 
            ToolParameterMenu.ToggleState defaultState = ToolParameterMenu.ToggleState.Off,
            GameObject parent = null
        )
        {
            GameObject gameObject = Util.KInstantiateUI(RsUITuning.Prefabs.ToggleEntry, parent);
            LocText locText = gameObject.GetComponentInChildren<LocText>();
            if (text.key.IsValid())
            {
                locText.key = text.key.String;
            }
            locText.text = text.text;

            MultiToggle toggle = gameObject.GetComponentInChildren<MultiToggle>();
            switch (defaultState)
            {
                case ToolParameterMenu.ToggleState.On:
                    toggle.ChangeState(1);
                    break;
                case ToolParameterMenu.ToggleState.Disabled:
                    toggle.ChangeState(2);
                    break;
                default:
                    toggle.ChangeState(0);
                    break;
            }
            gameObject.SetActiveNR(true);
            return gameObject;
        }
        
        public static MultiToggle ToggleEntryToMultiToggle(
            LocString text, 
            ToolParameterMenu.ToggleState defaultState = ToolParameterMenu.ToggleState.Off,
            GameObject parent = null
        )
        {
            GameObject gameObject = Util.KInstantiateUI(RsUITuning.Prefabs.ToggleEntry, parent);
            LocText locText = gameObject.GetComponentInChildren<LocText>();
            if (text.key.IsValid())
            {
                locText.key = text.key.String;
            }
            locText.text = text.text;

            MultiToggle toggle = gameObject.GetComponentInChildren<MultiToggle>();
            switch (defaultState)
            {
                case ToolParameterMenu.ToggleState.On:
                    toggle.ChangeState(1);
                    break;
                case ToolParameterMenu.ToggleState.Disabled:
                    toggle.ChangeState(2);
                    break;
                default:
                    toggle.ChangeState(0);
                    break;
            }
            gameObject.SetActiveNR(true);
            return toggle;
        }

      
    }
}