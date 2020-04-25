using System;
using OdinSerializer;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace AepsLabs.TileManager {

    /// <summary>
    /// Original Credit: https://stackoverflow.com/questions/58088009/serializing-tilemap
    /// </summary>
    [Serializable]
    public class WorldTileMap
    {
        [OdinSerialize]
        public float animationFrameRate;

        [OdinSerialize]
        public Vector3Int LocalPlace { get; set; }

        [OdinSerialize]
        public Vector3 WorldLocation { get; set; }

        [OdinSerialize]
        public string TileSprite { get; set; } //Path to the sprite on the filesystem

        // [OdinSerialize]
        // public TileBase TileBase { get; set; }
        //
        // [OdinSerialize]
        // public Tilemap TilemapMember { get; set; }

        [OdinSerialize]
        public string Name { get; set; }

        [OdinSerialize]
        public Color32 Color { get; set; }

        [OdinSerialize]
        public Tilemap.Orientation orientation;

        [OdinSerialize]
        public Matrix4x4 orientationMatrix;

        [OdinSerialize]
        public Vector3 tileAnchor;

        [OdinSerialize]
        public BoundsInt cellBounds;

        [OdinSerialize]
        public Bounds localBounds;

        [OdinSerialize]
        public Vector3Int origin;

        [OdinSerialize]
        public Vector3Int size;

        //Store individual tile data in here.
        [OdinSerialize]
        public WorldTile[] tiles;



    }
}
