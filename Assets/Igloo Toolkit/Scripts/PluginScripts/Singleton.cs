namespace Igloo.Common
{
    using UnityEngine;

    /// <summary>
    /// The singleton base class. All monobehaviour singleton class should derived from this class.
    /// </summary>
    /// <remarks>
    /// The Singleton class ensures that only one type of a singleton script exsists. If more than one script exists
    /// then one (or more) will be destroyed as part of the initSingletonInst function. 
    /// Making a script a singleton ensures that it can be easily referenced by any script, as there is no chance of
    /// the reference being anything other than the singleton. 
    /// </remarks>
    public class Singleton<T> : MonoBehaviour
        where T : Singleton<T>
    {
        /// <summary>
        /// The type instance
        /// </summary>
        public static T instance;

        /// <summary>
        /// Initializes a singleton instance.
        /// </summary>
        protected virtual void InitSingletonInst()
        {
            if (instance != null && instance != (T)this)
            {
                Destroy(this);
                return;
            }

            instance = (T)this;
            this.AwakeInternal();
        }

        /// <summary>
        /// Executes when gameObject instantiates, after it's initialized as a Singleton.
        /// </summary>
        protected virtual void AwakeInternal()
        {
        }

        /// <summary>
        /// Executes when gameObject instantiates.
        /// </summary>
        private void Awake()
        {
            this.InitSingletonInst();
        }
    }
}
