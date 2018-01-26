
using UnityEngine;

namespace Events
{
    public partial class Tools
    {
        static partial void RegesterUserImplementation(object user);
        static partial void UnRegesterUserImplementation(object user);

        public static void RegesterUser(object user)
        {
            RegesterUserImplementation(user);
        }
        public static void UnRegesterUser(object user)
        {
            UnRegesterUserImplementation(user);
        }

        public interface IEventMethodBase{        }
        public class MonoBehaviour_EventManagerBase : MonoBehaviour
        {
            public virtual void OnEnable()
            {
                Events.Tools.RegesterUser(this);
            }
            public virtual void OnDisable()
            {
                Events.Tools.UnRegesterUser(this);
            }
        }

    }
}