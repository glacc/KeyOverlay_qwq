using Glacc.UI;
using SFML.Graphics;
using SFML.System;

namespace Glacc.KeyOverlay_qwq.Elements
{
    internal class KeyCounter : Element, IDisposable
    {
        public int px;
        public int py;

        public Key key;

        public int fontWidth;
        public int fontSize;

        public Font font;

        List<KeyCounterDigit> digits = new List<KeyCounterDigit>();

        int textureWidth;
        int textureHeight;

        float textureOffsetX = 0f;

        const float moveSpeedReference = 0.2f;
        const int tickRateReference = 100;
        float textureMoveSpeed = 0.2f;

        RenderTexture renderTexture;
        Sprite sprite;

        readonly static int maxDigits;

        static KeyCounter()
        {
            int maxValueOfInt = int.MaxValue;

            maxDigits = 0;
            while (maxValueOfInt > 0)
            {
                maxValueOfInt /= 10;
                maxDigits++;
            }
        }

        public override void Update()
        {
            int numOfDigit = 0;
            int keyCount = key.keyCount;
            while (keyCount > 0)
            {
                int numCurrentDigit = keyCount % 10;

                if (digits.Count <= numOfDigit)
                {
                    // int digitX = textureWidth - (fontWidth / 2) - (fontWidth * (digits.Count - 1));
                    int digitX = textureWidth - fontWidth - (fontWidth * digits.Count);
                    int digitY = textureHeight / 2;

                    digits.Add(new KeyCounterDigit(digitX, digitY, fontSize, fontSize + (fontSize / 2), font, numCurrentDigit));
                }
                else
                    digits[numOfDigit].number = numCurrentDigit;

                keyCount /= 10;
                numOfDigit++;
            }

            int validDigits = numOfDigit;

            while (numOfDigit < digits.Count)
            {
                KeyCounterDigit currentDigit = digits[numOfDigit];

                if (!currentDigit.hasMovedToEnd)
                {
                    currentDigit.moveToEnd = true;

                    numOfDigit++;

                    continue;
                }

                digits.RemoveAt(numOfDigit);
            }

            foreach (KeyCounterDigit digit in digits)
                digit.Update();

            float textWidth = fontWidth * Math.Max(validDigits, 1);
            float textureTargetOffsetX = -textureWidth + (textWidth / 2f) + (fontWidth / 2f);

            textureOffsetX = textureOffsetX + (textureTargetOffsetX - textureOffsetX) * textureMoveSpeed;
        }

        public override Drawable?[] Draw()
        {
            renderTexture.Clear(Color.Transparent);

            foreach (KeyCounterDigit digit in digits)
                digit.Draw(renderTexture);

            renderTexture.Display();

            sprite.Position = new Vector2f(px + textureOffsetX, py - (textureHeight / 2));

            return [sprite];
        }

        public void Dispose()
        {
            sprite.Dispose();
            renderTexture.Dispose();
        }

        public KeyCounter(int px, int py, Key key, int fontWidth, int fontSize, Font font)
        {
            this.px = px;
            this.py = py;

            this.key = key;

            this.fontWidth = fontWidth;
            this.fontSize = fontSize;

            this.font = font;

            textureWidth = fontWidth * maxDigits;
            textureHeight = fontSize;

            textureOffsetX = -textureWidth;

            renderTexture = new RenderTexture((uint)textureWidth, (uint)textureHeight);
            sprite = new Sprite(renderTexture.Texture);

            textureMoveSpeed = Math.Clamp(AppSettings.counterSpeed * (tickRateReference / (float)AppSettings.tickrate), 0.00000001f, 1.0f);
        }
    }
}
