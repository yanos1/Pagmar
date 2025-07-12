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
    [Header("Damage System")]
    [SerializeField] private Canvas c;
    [SerializeField] private List<Sprite> hornStates; // From healthy to broken
    [SerializeField] private Image hornImage;
    [SerializeField] private MMFeedbacks takeDamageFeedbacks;
    [SerializeField] private MMFeedbacks revealFeedbacks; 
    [SerializeField] private MMF_Player lowHealthFeedbacks;

    [Header("Healing System")]
    [SerializeField] private Image healBar;
    [SerializeField] private float healInterval;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private MMF_Player healthWarningTextFeedbacks;
    [SerializeField] private ParticleSystem lowHealthParticles;

    private int currentDamageIndex = 0;
    private float healTimer = 0f;
    private bool isHealing = false;
    private int deferredDamage = 0;
    private int lastDamageAmount;
    private int lowHealthThreshold = 5;
    private const float damageCooldown = 0.35f;
    private float lastDamageTime;

    private PlayerManager player;
    private Coroutine healthTextRoutine;
    private bool lowHealthPlayed = false;
    private bool hasBeenRevealed = false;
    private bool shownHealthWarning = false;
    private UnityEngine.Camera mainCamera;

    public int currentIndex => currentDamageIndex;
    public int Health => hornStates.Count - currentDamageIndex - 1;

    private void Start()
    {
        player = GetComponent<PlayerManager>();
        InjuryFeedbacks.Instance.Init(Health, healInterval);
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

        if (lowHealthPlayed)
        {
            lowHealthParticles.transform.position =
                UtilityFunctions.GetTopLeftCornerWorldPosition(mainCamera, 2, 2);
        }
    }

    private void HealOne()
    {
        if (currentDamageIndex > 0)
        {
            currentDamageIndex--;
            InjuryFeedbacks.Instance.Heal(1);
            UpdateVisual();
        }

        if (currentDamageIndex < lowHealthThreshold)
        {
            lowHealthPlayed = false;
            lowHealthFeedbacks?.StopFeedbacks();
            lowHealthParticles.Stop();
            UpdateVisual();
        }
    }

    private void UpdateVisual()
    {
        hornImage.sprite = hornStates[Mathf.Clamp(currentDamageIndex, 0, hornStates.Count - 1)];
        UpdateHealthTextSmooth(Health);

        if (currentDamageIndex >= lowHealthThreshold)
        {
            int overThreshold = currentDamageIndex - lowHealthThreshold;

            var emission = lowHealthParticles.emission;
            var main = lowHealthParticles.main;

            if (overThreshold == 0)
            {
                emission.rateOverTime = 5f;
                main.startLifetime = 0.3f;
            }
            else if (overThreshold == 1)
            {
                emission.rateOverTime = 10f;
                main.startLifetime = 0.5f;
            }
            else if (overThreshold >= 2)
            {
                emission.rateOverTime = 20f;
                main.startLifetime = 1f;
            }

            if (!lowHealthPlayed)
            {
                lowHealthParticles.gameObject.SetActive(true);
                lowHealthParticles.Play();
                lowHealthFeedbacks?.PlayFeedbacks();
                lowHealthPlayed = true;
                mainCamera = UnityEngine.Camera.main;
            }
        }
    }

    private void UpdateHealthTextSmooth(int targetValue)
    {
        healthText.text = targetValue.ToString();
    }

    public void AddDamage(int amount, bool activateFeedbacks = true)
    {
        if (!hasBeenRevealed)
        {
            hasBeenRevealed = true;
            revealFeedbacks.PlayFeedbacks();
        }

        if (Mathf.Approximately(amount, lastDamageAmount) && Time.time - lastDamageTime < damageCooldown)
            return;

        if (isHealing)
        {
            deferredDamage += amount;
            return;
        }

        InjuryFeedbacks.Instance.ApplyDamage(amount);

        currentDamageIndex += amount;
        lastDamageTime = Time.time;

        if (!shownHealthWarning && currentDamageIndex >= lowHealthThreshold)
        {
            shownHealthWarning = true;
            healthWarningTextFeedbacks?.PlayFeedbacks();
        }

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
        if (isHealing) return;

        isHealing = true;

        currentDamageIndex = 0;
        InjuryFeedbacks.Instance.Heal(Health);
        lowHealthPlayed = false;
        lowHealthParticles.Stop();
        lowHealthParticles.gameObject.SetActive(false);
        lowHealthFeedbacks?.StopFeedbacks();
        healBar.fillAmount = 1f;

        UpdateVisual();
        isHealing = false;

        if (deferredDamage > 0)
        {
            AddDamage(deferredDamage, false);
            InjuryFeedbacks.Instance.ApplyDamage(deferredDamage);
            deferredDamage = 0;
        }
    }

    public void ResetToInitialState()
    {
        if (healthTextRoutine != null) StopCoroutine(healthTextRoutine);

        currentDamageIndex = 0;
        deferredDamage = 0;
        isHealing = false;
        healTimer = 0f;
        lowHealthPlayed = false;

        UpdateVisual();
        healBar.fillAmount = 1f;
        lowHealthParticles.Stop();
        lowHealthParticles.gameObject.SetActive(false);
        lowHealthFeedbacks?.StopFeedbacks();
    }
}
