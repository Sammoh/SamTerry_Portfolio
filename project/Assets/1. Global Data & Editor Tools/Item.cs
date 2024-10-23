using UnityEngine;

namespace Sammoh.One
{
    public class Item : MonoBehaviour
    {
        public string itemName;
        public Color itemColor;

        public void Generate(Adjectives adjectives, Nouns nouns, Colors colors)
        {
            itemColor = colors.colors[Random.Range(0, colors.colors.Length)];

            string adjective = adjectives.adjectives[Random.Range(0, adjectives.adjectives.Length)];
            string noun = nouns.nouns[Random.Range(0, nouns.nouns.Length)];

            itemName = $"{adjective} {noun}";
            gameObject.name = ToLowerCamelCase(itemName);
        }

        private string ToLowerCamelCase(string str)
        {
            string[] words = str.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (i == 0)
                    words[i] = words[i].ToLower();
                else
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
            }

            return string.Join("", words);
        }
    }
}