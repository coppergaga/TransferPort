using System;

namespace RsTransferPort
{
    public abstract class SingleManager<T> where T : class
    {
        protected static T _instance;

        public SingleManager()
        {
            // Game.Instance.OnLoad
            App.OnPreLoadScene += OnPreLoadScene;
            OnSpawn();
        }

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    // _instance = Game.Instance.gameObject.AddOrGet<WirelessLogicPortManager>();
                    _instance = Activator.CreateInstance<T>();

                return _instance;
            }
        }

        protected virtual void OnSpawn()
        {
        }

        private void OnPreLoadScene()
        {
            App.OnPreLoadScene -= OnPreLoadScene;
            OnCleanUp();
            _instance = null;
        }

        protected virtual void OnCleanUp()
        {
        }
    }
}