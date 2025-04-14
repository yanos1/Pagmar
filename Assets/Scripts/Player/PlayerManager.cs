using System;
using Managers;
using Terrain;
using Unity.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        private PlayerMovement _playerMovement;
        private void Start()
        {
            CoreManager.Instance.Player = this;
            _playerMovement = GetComponent<PlayerMovement>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<IBreakable>() is { } breakable && _playerMovement.IsDashing)
            {
                breakable.OnHit((other.transform.position - gameObject.transform.position).normalized);
            }
        }
    }
}