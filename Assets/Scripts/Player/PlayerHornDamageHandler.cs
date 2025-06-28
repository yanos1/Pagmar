using System;
using UnityEngine;
using System.Collections;
using Interfaces;
using Managers;
using Player;

public class PlayerHornDamageHandler : MonoBehaviour, IResettable
{
    [Range(0f, 100f)]
    public float currentDamage = 0f;

    [SerializeField] private float passiveHealRate = 2.7f; // % per second
    [SerializeField] private HornDamageHandlerUI damageUI;

    private bool isDead = false;
    private Coroutine healingCoroutine;

    private float lastDamageTime = -1f;
    private float lastDamageValue = -1f;

    private bool gradualHeal;
    private const float damageCooldown = 0.35f;

    private void OnEnable()
    {
        CoreManager.Instance.EventManager.AddListener(EventNames.PickupBoneHeal, Heal);
    }

    private void OnDisable()
    {
        CoreManager.Instance.EventManager.RemoveListener(EventNames.PickupBoneHeal, Heal);
    }

    private void Update()
    {
        if (isDead) return;

        if (currentDamage > 0f && !gradualHeal)
        {
            PassiveHeal();
        }
    }

    public void AddDamage(float addedDamage = 12)
    {
        if (isDead) return;

        // Block if same damage value and within cooldown
        if (Mathf.Approximately(addedDamage, lastDamageValue) && Time.time - lastDamageTime < damageCooldown)
            return;

        currentDamage += addedDamage;
        currentDamage = Mathf.Clamp(currentDamage, 0f, 100f);
        damageUI?.UpdateUI(currentDamage);

        lastDamageTime = Time.time;
        lastDamageValue = addedDamage;

        if (currentDamage >= 100f)
        {
            Debug.Log("DIE!!!");
            isDead = true;
            CoreManager.Instance.Player.Die();
        }
    }

    private void PassiveHeal()
    {
        currentDamage -= passiveHealRate * Time.deltaTime;
        currentDamage = Mathf.Clamp(currentDamage, 0f, 100f);
        damageUI?.UpdateUI(currentDamage);
    }

    public void Heal(object o)
    {
        if (o is int healAmount)
        {
            if (healingCoroutine != null)
                StopCoroutine(healingCoroutine);

            Debug.Log("HEAL!!!");
            healingCoroutine = StartCoroutine(GradualHeal(healAmount, 2f));
        }
    }

    private IEnumerator GradualHeal(int healAmount, float duration)
    {
        gradualHeal = true;

        float startDamage = currentDamage;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            currentDamage = Mathf.Lerp(startDamage, Mathf.Max(startDamage - healAmount, 0), elapsed / duration);
            damageUI?.UpdateUI(currentDamage);
            yield return null;
        }

        currentDamage = Mathf.Clamp(currentDamage, 0f, 100f);
        damageUI?.UpdateUI(currentDamage);
        healingCoroutine = null;
        gradualHeal = false;
    }

    public void ResetToInitialState()
    {
        isDead = false;
        currentDamage = 0f;
        lastDamageValue = -1f;
        lastDamageTime = -1f;
        damageUI?.UpdateUI(currentDamage);

        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
            healingCoroutine = null;
        }

        gradualHeal = false;
    }
}
