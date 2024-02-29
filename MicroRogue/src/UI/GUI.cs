using SFML.System;
using SFML.Graphics;

namespace MicroRogue.UI
{
    internal class GUI
    {
        static public readonly Vector2f GUIOffset = new Vector2f(0, 432);

        public static void Draw(RenderWindow window)
        {
            Text HP = new Text($"Health: {Game.Player.Stats.Health}/{Game.Player.Stats.MaxHealth}", Game.FONT, 16);
            HP.Color = Game.COLOR_DARK;
            HP.Position = new Vector2f(16, 16) + GUIOffset;
            window.Draw(HP);

            Text CoinCount = new Text($"Coins: {Game.Coins}", Game.FONT, 16);
            CoinCount.Color = Game.COLOR_DARK;
            CoinCount.Position = new Vector2f(16, 48) + GUIOffset;
            window.Draw(CoinCount);
        }
    }
}
