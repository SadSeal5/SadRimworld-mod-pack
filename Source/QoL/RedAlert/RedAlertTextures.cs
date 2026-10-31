using UnityEngine;
using Verse;

namespace SadSeal.MegaMod.QoL.RedAlert
{
    public static class RedAlertTextures
    {
        private static Texture2D iconActive;
        private static Texture2D iconInactive;

        public static Texture2D IconActive
        {
            get
            {
                if (iconActive == null)
                {
                    iconActive = ContentFinder<Texture2D>.Get("UI/Icons/RedAlert_Active", false)
                                 ?? ContentFinder<Texture2D>.Get("UI/Icons/RedAlert", false)
                                 ?? GenerateAlertIcon(new Color(0.92f, 0.2f, 0.2f), Color.white);
                }
                return iconActive;
            }
        }

        public static Texture2D IconInactive
        {
            get
            {
                if (iconInactive == null)
                {
                    iconInactive = ContentFinder<Texture2D>.Get("UI/Icons/RedAlert_Inactive", false)
                                   ?? GenerateAlertIcon(new Color(0.6f, 0.6f, 0.6f), new Color(0.85f, 0.85f, 0.85f));
                }
                return iconInactive;
            }
        }

        private static Texture2D GenerateAlertIcon(Color ringColor, Color innerColor)
        {
            int size = 32;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color transparent = new Color(0, 0, 0, 0);
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            float outerR = 13.5f;
            float innerR = 10.5f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), center);
                    if (d <= outerR)
                    {
                        if (d > innerR)
                        {
                            tex.SetPixel(x, y, ringColor);
                        }
                        else
                        {
                            // Draw an alert exclamation mark inside
                            float dx = Mathf.Abs(x - center.x);
                            bool isBar = dx <= 1.5f && y >= 12 && y <= 20;
                            bool isDot = dx <= 1.5f && y >= 7 && y <= 9;

                            if (isBar || isDot)
                            {
                                tex.SetPixel(x, y, innerColor);
                            }
                            else
                            {
                                tex.SetPixel(x, y, ringColor * 0.35f);
                            }
                        }
                    }
                    else
                    {
                        tex.SetPixel(x, y, transparent);
                    }
                }
            }
            tex.Apply();
            return tex;
        }
    }
}
