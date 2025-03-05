using Glacc.UI;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glacc.KeyOverlay_qwq.Elements
{
    internal class KeyPressBar : Element
    {
        public Key? key = null;

        public Color barColor;

        float barMovementPixelPerTick;
        Vector2f barMovementVector;

        List<RectangleShape> bars = new List<RectangleShape>();
        int currentBarIndex = -1;

        const float safeYPosition = -10f;

        public override void Update()
        {
            if (key == null)
                return;

            if (key.pressedThisFrame)
            {
                RectangleShape newBar = new RectangleShape();
                newBar.FillColor = key.keyColor;
                newBar.Position = new Vector2f(key.px, key.py);
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
            => bars.ToArray();

        public KeyPressBar(Key? key)
        {
            this.key = key;

            barMovementPixelPerTick = AppSettings.barSpeed / (1000f / (1000f / AppSettings.tickrate));
            barMovementVector = new Vector2f(0f, barMovementPixelPerTick);
        }
    }
}
