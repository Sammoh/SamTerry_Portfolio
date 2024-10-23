using UnityEngine;

[CreateAssetMenu(fileName = "Colors", menuName = "GlobalData/Colors", order = 1)]
public class Colors : ScriptableObject
{
    public Color[] colors = new Color[10];
    
    private void OnValidate()
    {
        for (int i = 0; i < colors.Length; i++)
        {
            if (colors[i].a != 1.0f || IsInvalidColor(colors[i]))
            {
                Debug.LogWarning("Invalid color detected. Alpha must be 100%, and color must not be black, white, gray, or red.");
            }
        }
    }

    private bool IsInvalidColor(Color color)
    {
        // Implement the check for black, white, grays, or red hues
        float max = Mathf.Max(color.r, color.g, color.b);
        float min = Mathf.Min(color.r, color.g, color.b);

        if (max == 0) return true; // black
        if (max == 1 && min == 1) return true; // white
        if (max - min < 0.1f) return true; // gray shades
        if (color.r > color.g && color.r > color.b) return true; // red hues

        return false;
    }
}