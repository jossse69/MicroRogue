using SFML.Graphics;
using System.Drawing;

namespace MicroRogue
{
    internal class Assetloader
    {
        public static Dictionary<string, Texture> LoadedTextures = new Dictionary<string, Texture>();

        public static void LoadTextures()
        {
            // get sprites directory
            string spritesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets/sprites");
            string[] files = Directory.GetFileSystemEntries(spritesDirectory);

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                LoadedTextures.Add(fileName, new Texture(file));
                Console.WriteLine("Loaded '"+ file + "'.");
            }
        }

        public static Sprite BuildSprite(string name, IntRect area)
        {
            Texture texture = LoadedTextures[name];
            return new Sprite(texture, area);
        }
    }
}