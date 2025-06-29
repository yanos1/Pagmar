using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class DoSpineFlash : MonoBehaviour
{
    private Coroutine flashCoroutine;
    [SerializeField] private EnemySpineControl spineControl;
    [SerializeField] private SpineControl playerSpineControl;
    [SerializeField] private float flashDuration = 0.07f;
    [SerializeField] private Color flashColor = new Color(0.1f, 0.1f, 0.1f, 1f);
    [SerializeField] private int numOfFlashes = 1;
        
    
    public void OnRammedFeedback()
    {
        if (flashCoroutine == null)
            flashCoroutine = StartCoroutine(SpineFlash(flashDuration, flashColor, numOfFlashes));
    }

    private IEnumerator SpineFlash(float duration, Color flashColor, int flashes)
    {
        if (spineControl == null && playerSpineControl == null)
        {
            Debug.LogError("No SpineControl or PlayerSpineControl assigned.");
            yield break;
        }
        var skeleton = playerSpineControl != null ? playerSpineControl.skeletonAnimation.Skeleton : spineControl.skeletonAnimation.Skeleton;
        var originalColors = new Dictionary<Spine.Slot, Color>();

        foreach (var slot in skeleton.Slots)
            originalColors[slot] = slot.GetColor();

        for (int i = 0; i < flashes; i++)
        {
            // Set flash color
            foreach (var slot in skeleton.Slots)
                slot.SetColor(flashColor);

            yield return new WaitForSeconds(duration);

            // Restore original color
            foreach (var kvp in originalColors)
                kvp.Key.SetColor(kvp.Value);

            yield return new WaitForSeconds(duration);
        }
        flashCoroutine = null;
    }
    public void RestoreOriginalColors()
    {
        if (spineControl == null && playerSpineControl == null)
        {
            Debug.LogError("No SpineControl or PlayerSpineControl assigned.");
            return;
        }

        var skeleton = playerSpineControl != null ? playerSpineControl.skeletonAnimation.Skeleton : spineControl.skeletonAnimation.Skeleton;

        foreach (var slot in skeleton.Slots)
        {
            slot.SetColor(Color.white); // Or your desired default color
        }
    }

}