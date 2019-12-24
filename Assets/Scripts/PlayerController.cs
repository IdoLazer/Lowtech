using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public KeyCode typeOneKey;
    public KeyCode typeTwoKey;
    public KeyCode typeThreeKey;
    public float errorMargin = 0.2f;
    public int health = 3;
    public bool animated = false;
    public int streakBonus = 10;
    public int maxHealth = 4;
    public RuntimeAnimatorController[] runTimeAnimators;


    private Dictionary<string, KeyCode> waveTypeToKeyDict;
    private Dictionary<KeyCode, bool> isKeyCodePressed;
    private Animator animator;
    private int streak = 0;
    private int startHealth;

    public void Start()
    {
        startHealth = health;
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = runTimeAnimators[Mathf.Clamp(health - 1, 0, runTimeAnimators.Length - 1)];
        waveTypeToKeyDict = new Dictionary<string, KeyCode>()
        {
            {"1", typeOneKey }, {"2", typeTwoKey }, {"3", typeThreeKey}
        };
        isKeyCodePressed = new Dictionary<KeyCode, bool>()
        {
            {typeOneKey, false }, {typeTwoKey, false }, {typeThreeKey, false }
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (!animated)
            return;
        if (Input.GetKeyDown(typeOneKey))
        {
            animator.SetInteger("pressedKey", 1);
            StartCoroutine(PressedKey(typeOneKey));
        }
        if (Input.GetKeyDown(typeTwoKey))
        {
            animator.SetInteger("pressedKey", 2);
            StartCoroutine(PressedKey(typeTwoKey));
        }
        if (Input.GetKeyDown(typeThreeKey))
        {
            animator.SetInteger("pressedKey", 3);
            StartCoroutine(PressedKey(typeThreeKey));
        }
    }

    public bool PressedCorrectKeyForPulseType(string type)
    {
        return isKeyCodePressed[waveTypeToKeyDict[type]];
    }

    IEnumerator PressedKey(KeyCode key)
    {
        isKeyCodePressed[key] = true;
        yield return new WaitForSeconds(errorMargin);
        isKeyCodePressed[key] = false;
    }

    public void BackToIdle()
    {
        animator.SetInteger("pressedKey", 0);
    }

    public void Damage()
    {
        streak = 0;
        if (health > 0)
        {
            health -= 1;
            if (health == 0)
            {
                animated = false;
                streak = 0;
            }
            else
            {
                StartCoroutine(ChangeAnimator());
            }
            animator.SetTrigger("Damage");
            StartCoroutine(ChangeAnimator());
        }
    }

    private IEnumerator ChangeAnimator()
    {
        yield return new WaitForSeconds(0.3f);
        animator.runtimeAnimatorController = runTimeAnimators[Mathf.Clamp(health - 1, 0, runTimeAnimators.Length - 1)];
    }

    public void Success()
    {
        streak += 1;
        if (streak >= streakBonus)
        {
            if (health < maxHealth)
            {
                health += 1;
            }
            health += 1;
            streak = 0;
            animator.SetTrigger("Streak");
        }
    }

}
