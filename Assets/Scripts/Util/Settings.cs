using UnityEngine;

public class Settings {

    public static readonly Theme[] themes = new Theme[] {
            new Theme {
                name = "Dark",
                skyboxColor = new Color32(0x10, 0x0F, 0x0F, 0xFF),
                nodeColor = new Color32(0x17, 0xE2, 0xDA, 0xA1),
                nodeSelectedColor = new Color32(0xD7, 0x8F, 0x32, 0xD9),
                labelColor = new Color32(0xE3, 0xE3, 0xE3, 0xFF),
                edgeColor = new Color32(0xF3, 0xF3, 0xF3, 0x64)
            },
            new Theme {
                name = "Light",
                skyboxColor = new Color32(0x02, 0x44, 0x5F, 0xFF),
                nodeColor = new Color32(0x10, 0xAA, 0x51, 0xD2),
                nodeSelectedColor = new Color32(0xE4, 0xE6, 0x2D, 0xD4),
                labelColor = new Color32(0x9E, 0xCC, 0xC7, 0xFF),
                edgeColor = new Color32(0xD9, 0x68, 0x3E, 0xC6)
            }
        };

    public bool autoRotationEnabled = false;
    public float autoRotationSpeed = 15f;
    public bool edgesActive = true;
    public string graphName = "Miserables";
    public bool labelsActive = true;
    public int themeIndex = 0;
}
