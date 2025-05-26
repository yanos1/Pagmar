using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Player
{
    public class HornDamageHandlerUI : MonoBehaviour
    {
        [SerializeField] private Slider damageSlider;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private float updateSpeed = 3f;

        private Coroutine updateCoroutine;

        public void UpdateUI(float targetDamage)
        {
            SetDisplay(targetDamage);
        }
        

        private void SetDisplay(float damage)
        {
            damageSlider.value = damage / 100f;
            damageText.text = $"{Mathf.RoundToInt(damage)}%";
        }
    }
}