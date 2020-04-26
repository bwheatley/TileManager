using System;
using OdinSerializer;
using UnityEngine;
using UnityEngine.Tilemaps;



namespace AepsLabs.TileManager {

    /// <summary>
    /// This is an individual tile being represented here.
    /// </summary>
    public class WorldTile {

        [OdinSerialize]
        public Tile Tile;

        public string SpriteName;

        [OdinSerialize]
        public Color32 Color { get; set; }

        //We might not care about this mofo
        [OdinSerialize]
        public Tile.ColliderType ColliderType;

        [OdinSerialize]
        public Vector3Int position;

        [OdinSerialize]
        public Vector3 worldPosition;

        [OdinSerialize]
        public Vector3 rotation;

        [OdinSerialize]
        public Vector3 scale;

        // [OdinSerialize]
        // public TileBase TileBase { get; set; }
        //
        // [OdinSerialize]
        // public Tilemap TilemapMember { get; set; }

        [OdinSerialize]
        public string Name { get; set; }


    }

}
