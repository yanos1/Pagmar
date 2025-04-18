using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Loader
{
    public class GameLoaderUI : MonoBehaviour
    {
        [SerializeField] private Image bar;

        private int _progress;
        private int _targetProgress = 100;

        public bool IsNotFinished => _progress < _targetProgress;

        public void AddProgress(int progress)
        {
            SetProgress(_progress + progress);
        }

        public void FinishLoading()
        {
            
        }

        public void SetProgress(int progress)
        {
            _progress = progress;
            UpdateUI();
        }

        private void UpdateUI()
        {
            var percent = (float)_progress / _targetProgress;
            var percentClamp = Mathf.Clamp01(percent);
            bar.fillAmount = percentClamp;
            Debug.Log(percent);
        }

        private void Reset()
        {
            bar = GetComponent<Image>();
        }

        public void DestroyUI()
        {
            bar.gameObject.SetActive(false);
        }
    }
}