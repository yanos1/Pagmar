using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpongeScene.Managers
{
    public class EventsManager : MonoBehaviour
    {
        private Dictionary<EventNames, List<Action<object>>> _activeListeners;

        private void Awake()
        {
            _activeListeners = new();
        }

        public void AddListener(EventNames eventName, Action<object> listener)
        {
            if (_activeListeners.TryGetValue(eventName, out var listOfEvents))
            {
                listOfEvents.Add(listener);

                return;
            }

            _activeListeners.Add(eventName, new List<Action<object>> { listener });
        }

        public void RemoveListener(EventNames eventName, Action<object> listener)
        {
            if (_activeListeners.TryGetValue(eventName, out var listOfEvents))
            {
                listOfEvents.Remove(listener);

                if (listOfEvents.Count <= 0)
                {
                    _activeListeners.Remove(eventName);
                }
            }
        }

        public void InvokeEvent(EventNames eventName, object obj)
        {
            if (_activeListeners.TryGetValue(eventName, out var listOfEvents))
            {
                for (int i = 0; i < listOfEvents.Count; i++)
                {
                    listOfEvents[i].Invoke(obj);
                }
            }
        }
    }

    public enum EventNames
    {
        None = 0,
        StartNewScene =1,
        Die=2,
        ReachedCheckPoint=3,
        StartGame=4,
        EndGame=5,
        Falling = 6,
        StopFalling=7,

        GameOver=8,
        ShowScoreBoard=9,
        ToMainMenu=10,
        StartTimer=11,

        StartEndCutScene=12,
    }
}