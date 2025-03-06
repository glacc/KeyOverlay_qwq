using Glacc.UI;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Glacc.KeyOverlay_qwq.Elements
{
    internal class Key : Element, IDisposable
    {
        public int px;
        public int py;

        public int width;
        public int height;

        public int borderSize;

        public int keyTextSize = 24;
        string m_displayedKeyText = "";
        string displayedKeyText
        {
            get => m_displayedKeyText;
            set
            {
                m_displayedKeyText = value;

                keyTextDrawable.DisplayedString = value;
            }
        }
        string? m_keyText = null;
        public string? keyText
        {
            get => m_keyText;
            set
            {
                m_keyText = value;

                if (value != null)
                    displayedKeyText = value;
            }
        }

        public Color keyColor;

        Keyboard.Key? m_keyCode = null;
        public Keyboard.Key? keycode
        {
            get => m_keyCode;
            set
            {
                m_keyCode = value;

                if (m_keyText == null)
                    displayedKeyText = value?.ToString() ?? "null";
            }
        }
        Mouse.Button? m_mouseButton = null;
        public Mouse.Button? mouseButton
        {
            get => m_mouseButton;
            set
            {
                m_mouseButton = value;

                if (m_keyText == null)
                    displayedKeyText = value?.ToString() ?? "null";
            }
        }

        int m_keyCount = 0;
        public int keyCount
        {
            get => m_keyCount;
        }

        public int fadeDuration = 100;
        float fadeTimeRemaining = 0;

        int lightingRange;

        bool m_pressed = false;
        public bool pressed
        {
            get => m_pressed;
        }
        bool pressedOld = false;
        bool m_pressedThisFrame = false;
        public bool pressedThisFrame
        {
            get => m_pressedThisFrame;
        }

        RectangleShape background = new RectangleShape();
        RectangleShape[] borderRectangles = new RectangleShape[4];

        Text keyTextDrawable = new Text();

        Texture? textureLighting;
        Sprite? pressLighting;
        void InitLightingTexture()
        {
            int top = lightingRange;
            int bottom = height + lightingRange;
            int left = lightingRange;
            int right = width + lightingRange;

            byte CalculateHollowLightingAlpha(int x, int y)
            {
                float distance;
                if (y < top)
                {
                    int diffY = top - y;
                    if (x >= left && x < right)
                        distance = diffY;
                    else
                    {
                        int diffX;
                        if (x < left)
                            diffX = left - x;
                        else
                            diffX = x - right;

                        distance = MathF.Sqrt((diffX * diffX) + (diffY * diffY));
                    }
                }
                else if (y >= bottom)
                {
                    int diffY = y - bottom;
                    if (x >= left && x < right)
                        distance = diffY;
                    else
                    {
                        int diffX;
                        if (x < left)
                            diffX = left - x;
                        else
                            diffX = x - right;

                        distance = MathF.Sqrt((diffX * diffX) + (diffY * diffY));
                    }
                }
                else
                {
                    if (x < left)
                        distance = left - x;
                    else if (x >= right)
                        distance = x - right;
                    else
                        distance = lightingRange;
                }

                distance = distance / lightingRange;
                distance = MathF.Pow(distance, 0.25f);
                distance = Math.Clamp(distance * 255f, 0f, 255f);

                return (byte)(255 - (int)distance);
            }

            int textureWidth = width + (lightingRange * 2);
            int textureHeight = height + (lightingRange * 2);
            int stride = textureWidth * 4;
            int pixelCount = textureWidth * textureHeight;

            byte[] pixels = new byte[pixelCount * 4];
            Span<byte> pixelsSpan = pixels;

            textureLighting?.Dispose();
            textureLighting = new Texture((uint)textureWidth, (uint)textureHeight);

            for (int y = 0; y < textureHeight; y++)
            {
                Span<byte> pixelsCurrentLine = pixelsSpan.Slice(y * stride, stride);

                int offsetCurrentLine = 0;

                for (int x = 0; x < textureWidth; x++)
                {
                    pixelsCurrentLine[offsetCurrentLine++] = 0xFF;
                    pixelsCurrentLine[offsetCurrentLine++] = 0xFF;
                    pixelsCurrentLine[offsetCurrentLine++] = 0xFF;
                    pixelsCurrentLine[offsetCurrentLine++] = CalculateHollowLightingAlpha(x, y);
                }
            }

            textureLighting.Update(pixels);

            pressLighting?.Dispose();
            pressLighting = new Sprite(textureLighting);
        }

        bool CheckKeyPress()
        {
            if (m_keyCode != null)
            {
                if (Keyboard.IsKeyPressed((Keyboard.Key)m_keyCode))
                    return true;
            }
            if (m_mouseButton != null)
            {
                if (Mouse.IsButtonPressed((Mouse.Button)m_mouseButton))
                    return true;
            }
            return false;
        }

        void UpdateBackgroundAndBorders()
        {
            Vector2f posTopLeft = new Vector2f(px, py);

            Vector2f sizeTopBottom = new Vector2f(width, borderSize);
            Vector2f sizeLeftRight = new Vector2f(borderSize, height);

            // Background
            background.Position = posTopLeft;
            background.Size = new Vector2f(width, height);

            // Top
            borderRectangles[0].Position = posTopLeft;
            borderRectangles[0].Size = sizeTopBottom;

            // Right
            borderRectangles[1].Position = new Vector2f(px + width - borderSize, py);
            borderRectangles[1].Size = sizeLeftRight;

            // Bottom
            borderRectangles[2].Position = new Vector2f(px, py + height - borderSize);
            borderRectangles[2].Size = sizeTopBottom;

            // Left
            borderRectangles[3].Position = posTopLeft;
            borderRectangles[3].Size = sizeLeftRight;

            foreach (RectangleShape rectangleShape in borderRectangles)
                rectangleShape.FillColor = keyColor;
        }

        public override void Update()
        {
            m_pressed = CheckKeyPress();

            m_pressedThisFrame = false;
            if (m_pressed)
            {
                fadeTimeRemaining = fadeDuration;

                if (!pressedOld)
                {
                    m_pressedThisFrame = true;
                    m_keyCount++;
                }
            }
            else
            {
                if (fadeTimeRemaining > 0f)
                    fadeTimeRemaining -= 1000f / AppSettings.tickrate;

                if (fadeTimeRemaining < 0f)
                    fadeTimeRemaining = 0f;
            }

            if (AppSettings.keyClearCounter != null)
            {
                if (Keyboard.IsKeyPressed((Keyboard.Key)AppSettings.keyClearCounter))
                    m_keyCount = 0;
            }

            pressedOld = m_pressed;
        }

        public override Drawable?[] Draw()
        {
            // Background and border position
            UpdateBackgroundAndBorders();

            // Background and lighting alpha
            float alphaKeyPressNormalizedTo255 = (fadeTimeRemaining / fadeDuration) * 255f;
            byte alphaBackground = (byte)(alphaKeyPressNormalizedTo255 * 0.5f);
            byte alphaLighting = (byte)(alphaKeyPressNormalizedTo255);

            UInt32 colorWithoutAlpha = keyColor.ToInteger() & 0xFFFFFF00;

            Color borderAndLightingColor = new Color(colorWithoutAlpha | alphaBackground);
            Color lightingColor = new Color(colorWithoutAlpha | alphaLighting);

            if (pressLighting != null)
            {
                pressLighting.Position = new Vector2f(px - lightingRange, py - lightingRange);
                pressLighting.Color = borderAndLightingColor;
            }
            background.FillColor = borderAndLightingColor;

            // Key text
            keyTextDrawable.CharacterSize = (uint)keyTextSize;
            Utils.UpdateTextOrigins(keyTextDrawable, TextAlign.Center);
            keyTextDrawable.Position = new Vector2f(px + (width / 2), py + (height / 2));

            return
                [
                    keyTextDrawable,
                    borderRectangles[0],
                    borderRectangles[1],
                    borderRectangles[2],
                    borderRectangles[3],
                    background,
                    pressLighting
                ];
        }

        public void Dispose()
        {
            textureLighting?.Dispose();
            pressLighting?.Dispose();
        }

        public Key(int width, int height, int px, int py)
        {
            this.px = px;
            this.py = py;

            this.width = width;
            this.height = height;

            borderSize = AppSettings.keyBorderSize;
            lightingRange = AppSettings.lightingRange;

            for (int i = 0; i < borderRectangles.Length; i++)
                borderRectangles[i] = new RectangleShape();

            keyTextDrawable.Font = Settings.font;

            InitLightingTexture();
        }

        public Key(int width, int height) : this(width, height, 0, 0) { }
    }
}
