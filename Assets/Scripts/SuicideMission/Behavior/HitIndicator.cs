﻿using System;
using SuicideMission.Objects;
using SuicideMission.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace SuicideMission.Behavior
{
    public class HitIndicator : MonoBehaviour
    {
        [Header("Particle Effect")]
        [SerializeField] private GameObject hitParticle;

        [Header("Sprites")]
        [SerializeField] private Sprite[] hitSprites;

        [Header("Color")]
        [SerializeField] private bool colorActive;
        [SerializeField] private Gradient gradient;

        [Header("Color Flash")]
        [SerializeField] private bool colorFlashActive;
        [SerializeField] private Color flashColor = Color.red;
        [SerializeField] private int flashSpeed = 7;

        [Header("Sound Effect")]
        [SerializeField] private AudioClip soundEffect;
        [SerializeField] [Range(0, 1)] private float soundEffectVolume = 0.5f;

        [Header("Hit Background")]
        [SerializeField] private GameObject hitBackgroundObject;
        [SerializeField] private float minBackgroundAlpha;
        [SerializeField] private float maxBackgroundAlpha = 0.5f;

        private SpriteRenderer spriteRenderer;
        private Spaceship spaceship;
        private Image hitIndicatorImage;

        private int initialHealth;

        private bool colorFlashInvoked;
        private float colorFlashTimer;
        private Color initialColor;
        private Color colorToChange;

        private bool backgroundChangeInvoked;
        private float backgroundChangeTimer;
        private float initialBackgroundAlpha;

        private void Start()
        {
            spaceship = GetComponent<Spaceship>();
            initialHealth = spaceship.Health;
            colorToChange = flashColor;

            if (hitSprites.Length > 0 || colorActive || colorFlashActive)
                spriteRenderer = GetComponent<SpriteRenderer>();

            if (hitBackgroundObject != null) hitIndicatorImage = hitBackgroundObject.GetComponent<Image>();
        }

        private void Update()
        {
            if (backgroundChangeInvoked)
                backgroundChangeTimer += Time.deltaTime;
            if (colorFlashInvoked)
                colorFlashTimer += Time.deltaTime * flashSpeed;
        }

        public void IndicateHit()
        {
            if (hitParticle != null) ParticleUtils.PlayParticle(hitParticle, gameObject.transform.position, transform);

            if (hitSprites.Length > 0) ChangeSprite();

            if (colorActive) ChangeColor();

            if (colorFlashActive)
            {
                if (!colorFlashInvoked)
                {
                    colorFlashInvoked = true;
                    initialColor = spriteRenderer.color;
                    InvokeRepeating(nameof(FlashColor), 0, Time.deltaTime);
                }
                else
                {
                    colorToChange = flashColor;
                    colorFlashTimer = 0;
                }
            }

            if (soundEffect != null)
                AudioSource.PlayClipAtPoint(soundEffect, spaceship.cameraPosition, soundEffectVolume);

            if (hitBackgroundObject != null)
                if (!backgroundChangeInvoked)
                {
                    backgroundChangeInvoked = true;
                    initialBackgroundAlpha = hitIndicatorImage.color.a;
                    InvokeRepeating(nameof(ChangeIndicatorBackgroundColor), 0, Time.deltaTime);
                }
        }

        private void ChangeSprite()
        {
            var health = spaceship.Health;
            var index = hitSprites.Length - Mathf.CeilToInt(health / (initialHealth / (hitSprites.Length + 1)));
            index = Mathf.Min(index, hitSprites.Length);
            spriteRenderer.sprite = hitSprites[index];
        }

        private void ChangeColor()
        {
            var health = spaceship.Health;
            spriteRenderer.color = gradient.Evaluate((float) health / initialHealth);
        }

        private void ChangeIndicatorBackgroundColor()
        {
            var blinkingAlpha = Mathf.PingPong(backgroundChangeTimer, maxBackgroundAlpha);
            if (backgroundChangeTimer > maxBackgroundAlpha &&
                Math.Round(blinkingAlpha, 1) == Math.Round(minBackgroundAlpha, 1))
            {
                CancelInvoke(nameof(ChangeIndicatorBackgroundColor));
                blinkingAlpha = initialBackgroundAlpha;
                backgroundChangeInvoked = false;
                backgroundChangeTimer = 0;
            }

            var color = hitIndicatorImage.color;
            hitIndicatorImage.color = new Color(color.r, color.g, color.b, blinkingAlpha);
        }

        private void FlashColor()
        {
            var color = spriteRenderer.color;

            if (color.Equals(colorToChange))
            {
                colorToChange = initialColor;
                colorFlashTimer = 0;
            }

            if (initialColor.Equals(color) && colorToChange.Equals(initialColor))
            {
                CancelInvoke(nameof(FlashColor));
                colorFlashInvoked = false;
                colorFlashTimer = 0;
                colorToChange = flashColor;
            }
            else
            {
                spriteRenderer.color = Color.Lerp(color, colorToChange, colorFlashTimer);
            }
        }
    }
}