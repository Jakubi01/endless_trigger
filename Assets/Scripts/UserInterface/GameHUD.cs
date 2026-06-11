using Character.Player;
using Components;
using Items.Weapon;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class GameHUD : UserInterface
    {
        private const int InvalidTenths = int.MinValue;

        [SerializeField] private Slider hpBarSlider;
        [SerializeField] private Slider expBarSlider;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private Image swordAttackCooldownImage;
        [SerializeField] private Image spearAttackCooldownImage;
        [SerializeField] private TMP_Text swordAttackCooldownText;
        [SerializeField] private TMP_Text spearAttackCooldownText;

        private PlayerCharacter _player;
        private HealthComponent _healthComponent;
        private PlayerLevelComponent _levelComponent;
        private EnemySpawner _enemySpawner;
        private SwordAttack _swordAttack;
        private SpearAttack _spearAttack;
        private int _lastTimerTenths = InvalidTenths;
        private int _lastShotgunCooldownTenths = InvalidTenths;
        private int _lastSniperCooldownTenths = InvalidTenths;

        public void Initialize(PlayerCharacter player, EnemySpawner enemySpawner)
        {
            UnsubscribeStatEvents();

            _player = player;
            _enemySpawner = enemySpawner;
            CachePlayerReferences();
            SubscribeStatEvents();
            EnsureCooldownWidgets();
            RefreshStats();
            RefreshTimeBasedValues(forceTextRefresh: true);
        }

        private void Update()
        {
            RefreshTimeBasedValues(forceTextRefresh: false);
        }

        private void OnDestroy()
        {
            UnsubscribeStatEvents();
        }

        private void CachePlayerReferences()
        {
            if (!_player) return;

            _healthComponent = _player.GetComponent<HealthComponent>();
            _levelComponent = _player.GetComponent<PlayerLevelComponent>();
            _swordAttack = _player.GetComponentInChildren<SwordAttack>();
            _spearAttack = _player.GetComponentInChildren<SpearAttack>();
        }

        private void SubscribeStatEvents()
        {
            if (_healthComponent)
            {
                _healthComponent.HealthChanged += RefreshHealth;
            }

            if (_levelComponent)
            {
                _levelComponent.ExperienceChanged += RefreshExperience;
            }
        }

        private void UnsubscribeStatEvents()
        {
            if (_healthComponent)
            {
                _healthComponent.HealthChanged -= RefreshHealth;
            }

            if (_levelComponent)
            {
                _levelComponent.ExperienceChanged -= RefreshExperience;
            }
        }

        private void RefreshStats()
        {
            RefreshHealth();
            RefreshExperience();
        }

        private void RefreshTimeBasedValues(bool forceTextRefresh)
        {
            RefreshTimer(forceTextRefresh);
            RefreshCooldown(swordAttackCooldownImage, swordAttackCooldownText, _swordAttack, ref _lastShotgunCooldownTenths, forceTextRefresh);
            RefreshCooldown(spearAttackCooldownImage, spearAttackCooldownText, _spearAttack, ref _lastSniperCooldownTenths, forceTextRefresh);
        }

        private void RefreshHealth()
        {
            if (!hpBarSlider || !_healthComponent) return;

            hpBarSlider.minValue = 0f;
            hpBarSlider.maxValue = _healthComponent.MaxHealth;
            hpBarSlider.value = _healthComponent.CurrentHealth;
        }

        private void RefreshExperience()
        {
            if (_levelComponent)
            {
                if (expBarSlider)
                {
                    expBarSlider.minValue = 0f;
                    expBarSlider.maxValue = Mathf.Max(1, _levelComponent.RequiredExperiencePerLevel);
                    expBarSlider.value = _levelComponent.CurrentExperience;
                }

                if (levelText)
                {
                    levelText.text = $"Lv {_levelComponent.Level}";
                }
            }
            else if (levelText)
            {
                levelText.text = "Lv 0";
            }
        }

        private void RefreshTimer(bool forceTextRefresh)
        {
            if (!timerText || !_enemySpawner) return;

            float remainingTime = _enemySpawner.TimeUntilGameStart > 0f
                ? _enemySpawner.TimeUntilGameStart
                : _enemySpawner.CurrentWaveRemainingTime;

            SetTimeTextIfChanged(timerText, remainingTime, ref _lastTimerTenths, forceTextRefresh);
        }

        private void RefreshCooldown(Image cooldownImage, TMP_Text cooldownText, WeaponAttackBase weaponAttack, ref int lastTenths, bool forceTextRefresh)
        {
            float remainingCooldown = weaponAttack ? weaponAttack.RemainingCooldown : 0f;

            if (cooldownImage)
            {
                cooldownImage.fillAmount = weaponAttack ? weaponAttack.CooldownProgress : 0f;
            }

            if (cooldownText)
            {
                SetTimeTextIfChanged(cooldownText, remainingCooldown, ref lastTenths, forceTextRefresh);
            }
        }

        private void SetTimeTextIfChanged(TMP_Text text, float seconds, ref int lastTenths, bool force)
        {
            int tenths = Mathf.Max(0, Mathf.CeilToInt(seconds * 10f));
            if (!force && tenths == lastTenths) return;

            lastTenths = tenths;
            text.SetText("{0:0.0}", tenths * 0.1f);
        }

        private void EnsureCooldownWidgets()
        {
            if (!swordAttackCooldownImage)
            {
                swordAttackCooldownImage = CreateCooldownWidget("ShotgunCooldown", "SG", new Vector2(-126f, 64f), out swordAttackCooldownText);
            }
            else if (!swordAttackCooldownText)
            {
                swordAttackCooldownText = ResolveOrCreateCooldownText(swordAttackCooldownImage, "SG");
            }

            if (!spearAttackCooldownImage)
            {
                spearAttackCooldownImage = CreateCooldownWidget("SniperCooldown", "SR", new Vector2(-48f, 64f), out spearAttackCooldownText);
            }
            else if (!spearAttackCooldownText)
            {
                spearAttackCooldownText = ResolveOrCreateCooldownText(spearAttackCooldownImage, "SR");
            }

            ConfigureCooldownImage(swordAttackCooldownImage);
            ConfigureCooldownImage(spearAttackCooldownImage);
        }

        private Image CreateCooldownWidget(string widgetName, string label, Vector2 anchoredPosition, out TMP_Text cooldownText)
        {
            GameObject widget = new GameObject(widgetName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            widget.transform.SetParent(transform, false);

            RectTransform rectTransform = widget.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.one;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(64f, 64f);
            rectTransform.anchoredPosition = anchoredPosition;

            Image image = widget.GetComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.65f);

            cooldownText = ResolveOrCreateCooldownText(image, label);
            return image;
        }

        private TMP_Text ResolveOrCreateCooldownText(Image cooldownImage, string label)
        {
            TMP_Text text = cooldownImage.GetComponentInChildren<TMP_Text>();
            if (text)
            {
                return text;
            }

            GameObject textObject = new GameObject($"{label}CooldownText", typeof(RectTransform));
            textObject.transform.SetParent(cooldownImage.transform, false);

            RectTransform rectTransform = textObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            text = textObject.AddComponent<TextMeshProUGUI>();
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 20f;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.white;
            text.raycastTarget = false;
            text.text = "0.0";
            return text;
        }

        private void ConfigureCooldownImage(Image cooldownImage)
        {
            if (!cooldownImage) return;

            cooldownImage.type = Image.Type.Filled;
            cooldownImage.fillMethod = Image.FillMethod.Radial360;
            cooldownImage.fillOrigin = (int)Image.Origin360.Top;
            cooldownImage.fillClockwise = false;
            cooldownImage.fillAmount = 0f;
        }
    }
}
