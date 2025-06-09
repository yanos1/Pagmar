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

    // 🔸 Track time of last damage
    private float lastDamageTime = -1f;
    private bool garualHeal;
    private const float damageCooldown = 0.35f;


    private void OnEnable()
    {
        CoreManager.Instance.EventManager.AddListener(EventNames.PickupBoneHeal, Heal);
    }
    
    private void OnDisable()
    {
        CoreManager.Instance.EventManager.RemoveListener(EventNames.PickupBoneHeal, Heal);
    }

    void Update()
    {
        if (isDead) return;
        if (currentDamage >= 0f && !garualHeal)
        {
            PassiveHeal();
        }
    }

    public void AddDamage(float addedDamage = 15)
    {
        if (isDead) return;
        
        // 🔸 Check damage cooldown
        if (Time.time - lastDamageTime < damageCooldown)
            return;

        currentDamage += addedDamage;
        currentDamage = Mathf.Clamp(currentDamage, 0f, 100f);
        damageUI?.UpdateUI(currentDamage);

        // 🔸 Record the time of this damage
        lastDamageTime = Time.time;

        if (currentDamage >= 100f)
        {
            Debug.Log("DIE!!!");
            isDead = true;
            CoreManager.Instance.EventManager.InvokeEvent(EventNames.Die, null);
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
            print("HEAL!!!");
            healingCoroutine = StartCoroutine(GradualHeal(healAmount, 2));
        }
       
    }

    private IEnumerator GradualHeal(int healAmount, float duration)
    {
        garualHeal = true;
        float startDamage = currentDamage;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            currentDamage = Mathf.Lerp(startDamage, Mathf.Max(startDamage -healAmount,0), elapsed / duration);
            damageUI?.UpdateUI(currentDamage);
            yield return null;
        }

        currentDamage = 0f;
        damageUI?.UpdateUI(currentDamage);
        healingCoroutine = null;
        garualHeal = false;
    }

    public void ResetToInitialState()
    {
        isDead = false;
        currentDamage = 0f;
        damageUI?.UpdateUI(currentDamage);
        if (healingCoroutine is not null)
        {
            StopCoroutine(healingCoroutine);
            healingCoroutine = null;
        }
        garualHeal = false;

    }
}
