using System;
using Player;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers
{
    public class HornDamageManager : MonoBehaviour
    {
        [SerializeField] private HornDamageHandlerUI ui;
        [SerializeField] private GameObject hornDamageTitle;
        public static HornDamageManager Instance;
        public bool allowHornDamage = true;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                allowHornDamage = !allowHornDamage;
                ui.gameObject.SetActive(!ui.gameObject.activeInHierarchy);
                hornDamageTitle.gameObject.SetActive(!hornDamageTitle.activeInHierarchy);
            }   
        }

        private void Awake()
        {
            Instance = this;
        }
        
        
    }
}