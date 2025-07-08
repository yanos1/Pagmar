using System;
using System.Collections.Generic;

namespace Managers
{
    public class EventManager
    {
        private Dictionary<EventNames, List<Action<object>>> _activeListeners = new();

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
        PickUpFakeRune = 4,
        EndGame=5,
        Falling = 6,
        StopFalling=7,

        GameOver=8,
        ShowScoreBoard=9,
        ToMainMenu=10,
        StartTimer=11,

        EnterCutScene=12,

        GayserFinished=13,
        StartEnemyChase=14,
        PickupBoneHeal=15,
        ChangeAmbience=16,
        AllowCutSceneInput=17,

        EndCutScene=18,
        EnterSlowMotion=19,
        PlayerMeetSmall=20,
        BigPickUpHeal = 21,
        ChangeMusic = 22,
        StopMusic = 23,
        StartLoadNextScene = 24,
    }
}