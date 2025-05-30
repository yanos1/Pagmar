using System;
using System.Collections;
using Managers;
using SpongeScene;
using Unity.VisualScripting;
using UnityEngine;

namespace Terrain.Environment
{
    public class FallingGround : MonoBehaviour
    {
        private Rigidbody2D rb;
        private void OnEnable()
        {
            CoreManager.Instance.EventManager.AddListener(EventNames.PickUpFakeRune, OnPickUp);
            rb = GetComponent<Rigidbody2D>();
        }
        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.PickUpFakeRune, OnPickUp);
        }

        private void OnPickUp(object obj)
        {
            StartCoroutine(Break(null));
        }

        private IEnumerator Break(object obj)
        {
            yield return StartCoroutine(UtilityFunctions.ShakeObject(rb, 0.4f, 0.17f, 0.17f));
            gameObject.SetActive(false);
        }
    }
}