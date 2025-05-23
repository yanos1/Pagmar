using Interfaces;
using UnityEngine;

namespace Terrain.Environment
{
    public class Fire : MonoBehaviour, IResettable
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Box>() is not null)
            {
                gameObject.SetActive(false);
            }
        }

        public void ResetToInitialState()
        {
            gameObject.SetActive(true);
        }
    }
}
