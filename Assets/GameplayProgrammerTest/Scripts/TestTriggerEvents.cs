using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestTriggerEvents : MonoBehaviour
{
    [System.Serializable]
    public struct CollisionEvent
    {
        public UnityEvent unityEvent;
        public List<GameObject> triggeringObjects;
        public List<string> triggeringTags;
    }

    public CollisionEvent[] collisionEvents;

    private void OnTriggerEnter(Collider other)
    {
        foreach (CollisionEvent collisionEvent in collisionEvents)
        {
            if(collisionEvent.unityEvent != null)
            {
                if(collisionEvent.triggeringObjects.Contains(other.gameObject) ||
                collisionEvent.triggeringTags.Contains(other.tag))
                {
                    collisionEvent.unityEvent.Invoke();
                }
            }
        }
    }
}
