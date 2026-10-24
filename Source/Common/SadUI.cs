using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Verse;

namespace SadSeal.Common.UI
{
    public static class SadUI
    {
        public static void DrawHeader(Listing_Standard listing, string titleKey, string descKey = null)
        {
            if (listing == null) return;

            Text.Font = GameFont.Medium;
            if (!string.IsNullOrEmpty(titleKey))
            {
                listing.Label(titleKey.CanTranslate() ? titleKey.Translate() : (TaggedString)titleKey);
            }
            Text.Font = GameFont.Small;
            if (!string.IsNullOrEmpty(descKey))
            {
                listing.Label(descKey.CanTranslate() ? descKey.Translate() : (TaggedString)descKey);
            }
            listing.Gap(10f);
        }

        public static void DrawPresetRow<TEnum>(
            Listing_Standard listing,
            TEnum currentPreset,
            ValueTuple<string, TEnum>[] presets,
            Action<TEnum> onSelect,
            Action onReset) where TEnum : Enum
        {
            DrawPresetRow(listing, currentPreset, presets, onSelect, onReset, null, 32f, 5f);
        }

        public static void DrawPresetRow<TEnum>(
            Listing_Standard listing,
            TEnum currentPreset,
            ValueTuple<string, TEnum>[] presets,
            Action<TEnum> onSelect,
            Action onReset,
            string resetLabel,
            float height = 32f,
            float spacing = 5f) where TEnum : Enum
        {
            if (listing == null || presets == null || presets.Length == 0) return;

            int totalButtons = presets.Length + (onReset != null ? 1 : 0);
            Rect presetRect = listing.GetRect(height);
            float totalSpacing = spacing * (totalButtons - 1);
            float btnWidth = (presetRect.width - totalSpacing) / totalButtons;

            Color oldColor = GUI.color;

            for (int i = 0; i < presets.Length; i++)
            {
                var (labelKey, presetValue) = presets[i];
                bool isActive = EqualityComparer<TEnum>.Default.Equals(currentPreset, presetValue);
                GUI.color = isActive ? Color.green : oldColor;

                string label = labelKey.CanTranslate() ? labelKey.Translate().Resolve() : labelKey;
                Rect btnRect = new Rect(presetRect.x + i * (btnWidth + spacing), presetRect.y, btnWidth, height);
                if (Widgets.ButtonText(btnRect, label))
                {
                    onSelect?.Invoke(presetValue);
                }
            }

            GUI.color = oldColor;

            if (onReset != null)
            {
                Rect resetRect = new Rect(presetRect.x + presets.Length * (btnWidth + spacing), presetRect.y, btnWidth, height);
                string resetText;
                if (!string.IsNullOrEmpty(resetLabel))
                {
                    resetText = resetLabel.CanTranslate() ? resetLabel.Translate().Resolve() : resetLabel;
                }
                else
                {
                    resetText = "Reset".CanTranslate() ? "Reset".Translate().Resolve() : "Reset";
                }

                if (Widgets.ButtonText(resetRect, resetText))
                {
                    onReset.Invoke();
                }
            }
        }

        public static void DrawIntRow(
            Rect rowRect,
            string label,
            int currentValue,
            int min,
            int max,
            ref string buffer,
            Action<int> onChanged,
            float labelWidth = 240f)
        {
            float textWidth = 70f;
            float sliderWidth = rowRect.width - labelWidth - textWidth - 20f;

            Rect labelRect = new Rect(rowRect.x, rowRect.y, labelWidth, rowRect.height);
            Widgets.Label(labelRect, label);

            Rect sliderRect = new Rect(rowRect.x + labelWidth + 5f, rowRect.y + 4f, sliderWidth, rowRect.height - 8f);
            int newSliderValue = Mathf.RoundToInt(Widgets.HorizontalSlider(sliderRect, currentValue, min, max, false, null, min.ToString(), max.ToString(), 1f));

            if (newSliderValue != currentValue)
            {
                buffer = newSliderValue.ToString();
                onChanged?.Invoke(newSliderValue);
            }

            Rect textRect = new Rect(rowRect.x + labelWidth + sliderWidth + 15f, rowRect.y + 2f, textWidth, 26f);
            if (buffer == null)
            {
                buffer = currentValue.ToString();
            }

            string newText = Widgets.TextField(textRect, buffer);
            if (newText != buffer)
            {
                buffer = newText;
                if (int.TryParse(newText, out int parsed))
                {
                    int clamped = Mathf.Clamp(parsed, min, max);
                    onChanged?.Invoke(clamped);
                }
            }
        }

        public static void DrawFloatRow(
            Rect rowRect,
            string label,
            float currentValue,
            float min,
            float max,
            ref string buffer,
            Action<float> onChanged,
            float labelWidth = 240f)
        {
            float textWidth = 70f;
            float sliderWidth = rowRect.width - labelWidth - textWidth - 20f;

            Rect labelRect = new Rect(rowRect.x, rowRect.y, labelWidth, rowRect.height);
            Widgets.Label(labelRect, label);

            Rect sliderRect = new Rect(rowRect.x + labelWidth + 5f, rowRect.y + 4f, sliderWidth, rowRect.height - 8f);
            float roundStep = (max - min) > 100f ? 50f : 1f;
            float newSliderValue = Widgets.HorizontalSlider(sliderRect, currentValue, min, max, false, null, min.ToString("F0"), max.ToString("F0"), roundStep);

            if (Math.Abs(newSliderValue - currentValue) > 0.01f)
            {
                float rounded = Mathf.Round(newSliderValue);
                buffer = rounded.ToString("F0", CultureInfo.InvariantCulture);
                onChanged?.Invoke(rounded);
            }

            Rect textRect = new Rect(rowRect.x + labelWidth + sliderWidth + 15f, rowRect.y + 2f, textWidth, 26f);
            if (buffer == null)
            {
                buffer = currentValue.ToString("F0", CultureInfo.InvariantCulture);
            }

            string newText = Widgets.TextField(textRect, buffer);
            if (newText != buffer)
            {
                buffer = newText;
                if (float.TryParse(newText, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed) ||
                    float.TryParse(newText, NumberStyles.Float, CultureInfo.CurrentCulture, out parsed))
                {
                    float clamped = Mathf.Clamp(parsed, min, max);
                    onChanged?.Invoke(clamped);
                }
            }
        }

        public static void DrawMultiplierRow(
            Rect rowRect,
            string label,
            float currentValue,
            ref string buffer,
            Action<float> onChanged,
            float min = 0f,
            float max = 100f,
            float labelWidth = 240f)
        {
            float textWidth = 65f;
            float sliderWidth = rowRect.width - labelWidth - textWidth - 20f;

            Rect labelRect = new Rect(rowRect.x, rowRect.y, labelWidth, rowRect.height);
            Widgets.Label(labelRect, label);

            Rect sliderRect = new Rect(rowRect.x + labelWidth + 5f, rowRect.y + 4f, sliderWidth, rowRect.height - 8f);
            float newSliderValue = Widgets.HorizontalSlider(sliderRect, currentValue, min, max, false, null, $"{min:0.##}x", $"{max:0.##}x", 0.05f);

            if (Math.Abs(newSliderValue - currentValue) > 0.001f)
            {
                float rounded = Mathf.Round(newSliderValue * 100f) / 100f;
                buffer = rounded.ToString("0.##", CultureInfo.InvariantCulture);
                onChanged?.Invoke(rounded);
            }

            Rect textRect = new Rect(rowRect.x + labelWidth + sliderWidth + 15f, rowRect.y + 2f, textWidth, 26f);
            if (buffer == null)
            {
                buffer = currentValue.ToString("0.##", CultureInfo.InvariantCulture);
            }

            string newText = Widgets.TextField(textRect, buffer);
            if (newText != buffer)
            {
                buffer = newText;
                if (float.TryParse(newText, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed) ||
                    float.TryParse(newText, NumberStyles.Float, CultureInfo.CurrentCulture, out parsed))
                {
                    float clamped = Mathf.Clamp(parsed, min, max);
                    onChanged?.Invoke(clamped);
                }
            }
        }
    }
}
