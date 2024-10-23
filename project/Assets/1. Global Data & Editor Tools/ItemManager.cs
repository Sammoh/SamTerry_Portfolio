using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// The item manager is responsible for generating and deleting items from Global Data objects.
/// </summary>
namespace Sammoh.One
{
    public class ItemManager : MonoBehaviour
    {
        [SerializeField] private Adjectives adjectives;
        [SerializeField] private Nouns nouns;
        [SerializeField] private Colors colors;

        private GameObject[] currentItems;

        private void Start()
        {
            GenerateItems();
        }

        private void OnDisable()
        {
            DeleteItems();
        }

        public void GenerateItems()
        {
            var orSo = 20 + Random.Range(0, 4);

            currentItems = new GameObject[orSo];

            for (int i = 0; i < orSo; i++)
            {
                GameObject newItem = new GameObject("Item");
                Item itemComponent = newItem.AddComponent<Item>();
                itemComponent.Generate(adjectives, nouns, colors);

                currentItems[i] = newItem;
            }
        }

        private void DeleteItems()
        {
            foreach (var item in currentItems)
            {
                Destroy(item);
            }

            currentItems = null;
        }
    }
}