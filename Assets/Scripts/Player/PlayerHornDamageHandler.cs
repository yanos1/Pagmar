using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Managers;
using MoreMountains.Feedbacks;
using Player;
using SpongeScene;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHornDamageHandler : MonoBehaviour, IResettable
{
    [Header("Damage System")] [SerializeField]
    private Canvas c;
    [SerializeField] private List<Sprite> hornStates; // From healthy to broken
    [SerializeField] private Image hornImage; // The current horn image
    [SerializeField] private MMFeedbacks takeDamageFeedbacks;
    [SerializeField] private MMF_Player lastHealthFeedbakcs; // Feedback when horn is fully broken

    [Header("Healing System")]
    [SerializeField] private Image healBar;
    [SerializeField] private float healInterval = 5f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI healthText; // Health number display

    [SerializeField] private ParticleSystem p;

    private int currentDamageIndex = 0; // 0 = full health, max = fully broken
    private float healTimer = 0f;
    private bool isHealing = false;
    private int deferredDamage = 0;
    private int lastDamageAmount;
    private const float damageCooldown = 0.35f;
    private float lastDamageTime;

    private PlayerManager player;
    private Coroutine healRoutine;
    private Coroutine healthTextRoutine;
    private bool lastHealthPlayed = false;

    public int currentIndex => currentDamageIndex;

    public int Health => hornStates.Count - currentDamageIndex - 1;

    private void Start()
    {
        player = GetComponent<PlayerManager>();
        hornImage.sprite = hornStates[0];
        UpdateVisual();
    }

    private void OnEnable()
    {
        CoreManager.Instance.EventManager.AddListener(EventNames.PickupBoneHeal, StartFullHeal);
    }

    private void OnDisable()
    {
        CoreManager.Instance.EventManager.RemoveListener(EventNames.PickupBoneHeal, StartFullHeal);
    }

    private void Update()
    {
        if (isHealing || player.IsDead) return;

        if (currentDamageIndex != 0)
        {
            healTimer += Time.deltaTime;
            healBar.fillAmount = healTimer / healInterval;

            if (healTimer >= healInterval)
            {
                healTimer = 0f;
                HealOne();
            }
        }

        if (lastHealthPlayed)
        {
            print($"rect transform pos {hornImage.rectTransform.position}");
            p.transform.position =
                UtilityFunctions.UIToWorldNearCamera(hornImage.rectTransform, c, UnityEngine.Camera.main);
        }
    }

    private void HealOne()
    {
        if (currentDamageIndex > 0)
        {
            currentDamageIndex--;
            UpdateVisual();
        }

        if (lastHealthPlayed)
        {
            lastHealthPlayed = false;
            lastHealthFeedbakcs.StopFeedbacks();
        }
    }

    private void UpdateVisual()
    {
        hornImage.sprite = hornStates[Mathf.Clamp(currentDamageIndex, 0, hornStates.Count - 1)];
        UpdateHealthTextSmooth(Health);

        if (currentDamageIndex == hornStates.Count - 1 && !lastHealthPlayed)
        {
            lastHealthFeedbakcs?.PlayFeedbacks();
            lastHealthPlayed = true;
        }
       
    }

    private void UpdateHealthTextSmooth(int targetValue)
    {
        healthText.text = targetValue.ToString();
        // if (healthText == null) return;

        // if (healthTextRoutine != null)
        //     StopCoroutine(healthTextRoutine);

        // healthTextRoutine = StartCoroutine(AnimateHealthText(targetValue));
    }

    private IEnumerator AnimateHealthText(int targetValue)
    {
        int startValue = 0;
        int.TryParse(healthText.text, out startValue);

        float duration = 0.4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            int value = Mathf.RoundToInt(Mathf.Lerp(startValue, targetValue, t));
            healthText.text = value.ToString();
            yield return null;
        }

        healthText.text = targetValue.ToString();
    }

    public void AddDamage(int amount, bool activateFeedbacks = true)
    {
        if (Mathf.Approximately(amount, lastDamageAmount) && Time.time - lastDamageTime < damageCooldown)
            return;

        if (isHealing)
        {
            deferredDamage += amount;
            return;
        }

        currentDamageIndex += amount;
        lastDamageTime = Time.time;

        if (currentDamageIndex >= hornStates.Count)
        {
            Die();
            return;
        }

        UpdateVisual();
        takeDamageFeedbacks?.PlayFeedbacks();
    }

    private void Die()
    {
        currentDamageIndex = hornStates.Count - 1;
        UpdateVisual();
        Debug.Log("Horn is fully broken. Dead.");
        player.Die();
    }

    public void StartFullHeal(object o)
    {
        if (healRoutine != null) StopCoroutine(healRoutine);
        healRoutine = StartCoroutine(HealGradually(3f));
    }

    private IEnumerator HealGradually(float duration)
    {
        isHealing = true;
        float elapsed = 0f;
        int startingIndex = currentDamageIndex;
        healBar.fillAmount = 1f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            int newIndex = Mathf.RoundToInt(Mathf.Lerp(startingIndex, 0, t));

            if (newIndex != currentDamageIndex)
            {
                currentDamageIndex = newIndex;
                lastHealthPlayed = false;
                UpdateVisual();
            }
            else
            {
                break;
            }
        
            yield return null;
        }

        currentDamageIndex = 0;
        lastHealthPlayed = false;
        UpdateVisual();
        isHealing = false;

        if (deferredDamage > 0)
        {
            AddDamage(deferredDamage, false);
            deferredDamage = 0;
        }
    }

    public void ResetToInitialState()
    {
        if (healRoutine != null) StopCoroutine(healRoutine);
        if (healthTextRoutine != null) StopCoroutine(healthTextRoutine);

        currentDamageIndex = 0;
        deferredDamage = 0;
        isHealing = false;
        healTimer = 0f;
        lastHealthPlayed = false;

        UpdateVisual();
        healBar.fillAmount = 1f;
      
        lastHealthFeedbakcs.StopFeedbacks();
        
    }
}
