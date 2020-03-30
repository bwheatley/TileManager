using UnityEngine;
using UnityEngine.Tilemaps;

namespace AepsLabs.TileManager {

    /// <summary>
    /// Original Credit: https://stackoverflow.com/questions/58088009/serializing-tilemap
    /// </summary>
    public class WorldTile
    {
        public Vector3Int LocalPlace { get; set; }
        public Vector3 WorldLocation { get; set; }
        public TileBase TileBase { get; set; }
        public Tilemap TilemapMember { get; set; }
        public string Name { get; set; }
        public Color32 Color { get; set; }
        public Tilemap.Orientation Orientation;
    }
}
