using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;
using UnityEngine.Windows;

namespace Managers
{
  

    public class ResetManager : MonoBehaviour
    {
        private IResettable[] resettables;

        void Awake()
        {
            resettables = FindObjectsOfType<MonoBehaviour>().OfType<IResettable>().ToArray();
        }

        public void ResetAll()
        {
            foreach (var r in resettables)
            {
                r.ResetToInitialState();
            }
        }
    }


}