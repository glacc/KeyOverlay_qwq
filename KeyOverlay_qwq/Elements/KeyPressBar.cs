// SPDX-License-Identifier: GPL-3.0-or-later

using Glacc.UI;
using SFML.Graphics;
using SFML.System;

namespace Glacc.KeyOverlay_qwq.Elements
{
    internal class KeyPressBar : Element, IDisposable
    {
        public Key key;

        float barMovementPixelPerTick;
        Vector2f barMovementVector;

        List<RectangleShape> bars = new List<RectangleShape>();
        int currentBarIndex = -1;

        const float safeYPosition = -10f;

        RenderTexture texture;
        Sprite sprite;

        Sprite fadeMask;

        readonly static RenderStates renderStatesFadeMaskBlend;

        static KeyPressBar()
        {
            renderStatesFadeMaskBlend = new RenderStates(RenderStates.Default);

            renderStatesFadeMaskBlend.BlendMode.ColorSrcFactor = BlendMode.Factor.Zero;
            renderStatesFadeMaskBlend.BlendMode.ColorDstFactor = BlendMode.Factor.One;
            renderStatesFadeMaskBlend.BlendMode.ColorEquation = BlendMode.Equation.Add;

            renderStatesFadeMaskBlend.BlendMode.AlphaSrcFactor = BlendMode.Factor.One;
            renderStatesFadeMaskBlend.BlendMode.AlphaDstFactor = BlendMode.Factor.Zero;
            renderStatesFadeMaskBlend.BlendMode.AlphaEquation = BlendMode.Equation.Min;
        }

        public override void Update()
        {
            if (key.pressedThisFrame)
            {
                RectangleShape newBar = new RectangleShape();
                newBar.FillColor = key.keyColor;
                newBar.Position = new Vector2f(0f, key.py);
                newBar.Size = new Vector2f(key.width, 0f);

                if (currentBarIndex < 0)
                {
                    currentBarIndex = bars.Count;
                    bars.Add(newBar);
                }
            }

            if (!key.pressed)
                currentBarIndex = -1;
            else if (currentBarIndex >= 0)
            {
                RectangleShape currentBar = bars[currentBarIndex];
                currentBar.Position -= barMovementVector;
                currentBar.Size += barMovementVector;
            }

            int i = 0;
            int endIndex = (currentBarIndex >= 0) ? (bars.Count - 1) : bars.Count;
            while (i < endIndex)
            { 
                RectangleShape barToUpdate = bars[i];

                if (barToUpdate.Position.Y >= safeYPosition)
                    barToUpdate.Position -= barMovementVector;
                else
                    barToUpdate.Size -= barMovementVector;

                if (barToUpdate.Position.Y + barToUpdate.Size.Y < safeYPosition)
                {
                    int indexToSwapAndRemove = endIndex - 1;

                    RectangleShape temp = bars[indexToSwapAndRemove];
                    bars[indexToSwapAndRemove] = barToUpdate;
                    bars[i] = temp;

                    bars.RemoveAt(indexToSwapAndRemove);

                    currentBarIndex--;
                    endIndex--;

                    continue;
                }

                i++;
            }
        }

        public override Drawable?[] Draw()
        {
            texture.Clear(Color.Transparent);

            foreach (RectangleShape bar in bars)
                texture.Draw(bar);

            texture.Draw(fadeMask, renderStatesFadeMaskBlend);

            texture.Display();

            return [sprite];
        }

        public void Dispose()
        {
            texture.Dispose();
            sprite.Dispose();
            fadeMask.Dispose();
        }

        public KeyPressBar(Key key)
        {
            this.key = key;

            barMovementPixelPerTick = AppSettings.barSpeed / (1000f / (1000f / AppSettings.tickrate));
            barMovementVector = new Vector2f(0f, barMovementPixelPerTick);

            texture = new RenderTexture((uint)key.width, (uint)key.py);
            sprite = new Sprite(texture.Texture);
            sprite.Position = new Vector2f(key.px, 0f);

            fadeMask = new Sprite(Textures.fadeMaskTexture);
            fadeMask.Scale = new Vector2f(key.width / 100f, 1f);
        }
    }
}
