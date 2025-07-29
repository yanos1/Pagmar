namespace Atmosphere
{
    using UnityEngine;

    public class SpawnAndFlyToUI : MonoBehaviour
    {
        public GameObject effectPrefab;             // The visual prefab to instantiate (e.g. sparkle)
        public RectTransform targetUI;              // UI target (RectTransform)
        public Canvas canvas;                       // Canvas containing the UI
        public Camera uiCamera;                     // Camera rendering the canvas (usually Camera.main)
        public float speed = 5f;                    // Speed of movement
        public float destroyDistance = 0.1f;        // How close before destroying

        public void SpawnEffect(Vector3 worldStartPos)
        {
            GameObject instance = Instantiate(effectPrefab, worldStartPos, Quaternion.identity);
            StartCoroutine(MoveToUI(instance));
        }

        private System.Collections.IEnumerator MoveToUI(GameObject obj)
        {
            Vector3 targetWorldPos = GetWorldPositionFromUI(targetUI);

            while (obj != null && Vector3.Distance(obj.transform.position, targetWorldPos) > destroyDistance)
            {
                targetWorldPos = GetWorldPositionFromUI(targetUI); // Keep updating if UI moves
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetWorldPos, speed * Time.deltaTime);
                yield return null;
            }

            if (obj != null)
                Destroy(obj);
        }

        private Vector3 GetWorldPositionFromUI(RectTransform uiElement)
        {
            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, uiElement.position);
            Ray ray = uiCamera.ScreenPointToRay(screenPos);
            return ray.GetPoint(5f); // Adjust depth (5 units in front of camera)
        }
    }

}