using UnityEngine;

namespace Klondike.Utils
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        public static T Singleton { get; private set; }

        protected virtual void SetupSingleton()
        {
            if (Singleton != null)
            {
                this.gameObject.SetActive(false);
                Destroy(this.gameObject);
            }
            else
            {
                DontDestroyOnLoad(this);
                Singleton = (T)this;
            }
        }
    }

}