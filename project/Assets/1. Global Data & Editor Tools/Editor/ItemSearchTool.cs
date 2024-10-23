using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

/// <summary>
/// ItemSearchTool is a custom editor window that allows the user to search for items by name and color.
/// </summary>

namespace Sammoh.One
{
    public class ItemSearchTool : EditorWindow
    {
        private string searchQuery = "";
        private Color filterColor = Color.white; // Default to white
        private bool filterByColor = false;
        private float tolerance = 0.01f;

        private Item[] items;
        private Vector2 scrollPosition; // To handle scrolling

        [MenuItem("Tools/Item Search/Item Editor", false, 1)]
        public static void ShowWindow()
        {
            GetWindow<ItemSearchTool>("Item Editor");
        }

        [MenuItem("Tools/Item Search/Generate Items", false, 1)]
        public static void GenerateItems()
        {
            // Get the ItemManager
            var manager = FindObjectOfType<ItemManager>();
            manager.GenerateItems();
        }

        private void OnEnable()
        {
            RefreshItems();
        }

        private void OnGUI()
        {
            GUILayout.Label("Search", EditorStyles.boldLabel);
            searchQuery = EditorGUILayout.TextField("Name", searchQuery);
            filterColor = EditorGUILayout.ColorField("Color", filterColor);
            filterByColor = EditorGUILayout.Toggle("Filter by Color", filterByColor);

            if (GUILayout.Button("Refresh"))
            {
                RefreshItems();
            }

            // Begin the scrollable area
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DisplayItems();

            // End the scrollable area
            EditorGUILayout.EndScrollView();
        }

        private void DisplayItems()
        {
            // Check if items is not empty
            if (items == null || items.Length == 0)
            {
                Debug.LogError("There are no items in the scene!");
                return;
            }

            foreach (var item in items)
            {
                if (item != null && MatchesSearch(item) && MatchesColor(item))
                {
                    GUI.color = item.itemColor;
                    if (GUILayout.Button(item.itemName))
                    {
                        Selection.activeGameObject = item.gameObject;
                    }

                    GUI.color = Color.white;
                }
            }
        }

        // Refreshing items only matters when new objects are added or removed.
        private void RefreshItems()
        {
            items = FindObjectsOfType<Item>();
        }

        private bool MatchesSearch(Item item)
        {
            // bypass the search matching if the search query is empty, otherwise check if the item name contains the search query
            return string.IsNullOrEmpty(searchQuery) ||
                   item.itemName.Contains(searchQuery, System.StringComparison.OrdinalIgnoreCase);
        }

        private bool MatchesColor(Item item)
        {
            // bypass the color matching if the filter is not enabled
            return !filterByColor || ColorsAreEqual(item.itemColor, filterColor);
        }

        // check the equality of two colors with a tolerance.
        // The tolerance could be adjusted to match the colors more strictly.
        private bool ColorsAreEqual(Color color1, Color color2)
        {
            return Mathf.Abs(color1.r - color2.r) < tolerance &&
                   Mathf.Abs(color1.g - color2.g) < tolerance &&
                   Mathf.Abs(color1.b - color2.b) < tolerance &&
                   Mathf.Abs(color1.a - color2.a) < tolerance;
        }

    }
}
#endif
