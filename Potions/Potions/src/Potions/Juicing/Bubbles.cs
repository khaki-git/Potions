using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Potions.Juicing;

internal class Bubbles : MonoBehaviour
{
    private const int BUBBLES_PER_SECOND = 10;
    private const float BUBBLE_GROW_TIME = 0.4f;
    private const float BUBBLE_IDLE_TIME = 0.5f;
    private const float BUBBLE_POP_TIME = 0.08f;
    private const float POP_SCALE_MULTIPLIER = 1.4f;

    private float currentBubbles = 0;
    private AudioSource boilingSfx;
    
    public Transform referenceBubble;
    public bool __enabled = true;

    void Awake()
    {
        boilingSfx = GetComponent<AudioSource>();
        boilingSfx.Play();
    }
    
    private void Update()
    {
        boilingSfx.volume = currentBubbles/BUBBLES_PER_SECOND * 0.5f;
        var chance = Time.deltaTime * currentBubbles;

        if (Random.Range(0f, 1f) < chance)
        {
            StartCoroutine(Bubble());
        }

        var goalBubbles = BUBBLES_PER_SECOND;
        if (!__enabled) {goalBubbles = 0;}

        currentBubbles = Mathf.Lerp(currentBubbles, goalBubbles, Time.deltaTime / 13f);
    }

    private IEnumerator Bubble()
    {
        var bubble = Instantiate(referenceBubble);
        var location = Random.insideUnitCircle * 0.4f;
        var offset = transform.right * location.x + transform.up * location.y;
        var pos = transform.position + offset;
        
        var sizeVariation = Random.Range(0.8f, 1.2f);
        var goalScale = referenceBubble.lossyScale * sizeVariation;
        var rotationSpeed = Random.Range(-180f, 180f);
        
        bubble.localScale = Vector3.zero;
        bubble.position = pos;

        float t = 0f;
        while (t < BUBBLE_GROW_TIME)
        {
            yield return null;
            t += Time.deltaTime;
            float normalizedTime = t / BUBBLE_GROW_TIME;
            
            float easeOut = 1f - Mathf.Pow(1f - normalizedTime, 3f);
            float overshoot = normalizedTime < 0.8f ? easeOut : easeOut * (1f + Mathf.Sin((normalizedTime - 0.8f) * Mathf.PI * 5f) * 0.1f);
            
            bubble.localScale = Vector3.Lerp(Vector3.zero, goalScale, overshoot);
            
            float wobble = Mathf.Sin(normalizedTime * Mathf.PI * 4f) * 0.15f * (1f - normalizedTime);
            bubble.localScale += new Vector3(wobble * goalScale.x, -wobble * goalScale.y, wobble * goalScale.z);
        }
        
        bubble.localScale = goalScale;

        t = 0f;
        while (t < BUBBLE_IDLE_TIME)
        {
            yield return null;
            t += Time.deltaTime;
            float normalizedTime = t / BUBBLE_IDLE_TIME;
            
            bubble.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            
            float breathe = 1f + Mathf.Sin(normalizedTime * Mathf.PI * 6f) * 0.05f;
            bubble.localScale = goalScale * breathe;
        }

        t = 0f;
        Vector3 popScale = goalScale * POP_SCALE_MULTIPLIER;
        while (t < BUBBLE_POP_TIME)
        {
            yield return null;
            t += Time.deltaTime;
            float normalizedTime = t / BUBBLE_POP_TIME;
            
            float popEase = 1f - Mathf.Pow(1f - normalizedTime, 2f);
            bubble.localScale = Vector3.Lerp(goalScale, popScale, popEase);
        }
    
        Destroy(bubble.gameObject);
    }
} 