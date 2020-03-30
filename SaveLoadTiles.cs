namespace AepsLabs.TileManager {

    using Sirenix.OdinInspector;
    using Sirenix.Serialization;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    public class SaveLoadTiles : SerializedMonoBehaviour
    {
        public Tilemap                        Tilemap;
        public Dictionary<Vector3, WorldTile> tiles;
        public Dictionary<Vector3, WorldTile> tiles2;

        [Button]
        private void GetWorldTiles()
        {
            tiles = new Dictionary<Vector3, WorldTile>();

            foreach (Vector3Int pos in Tilemap.cellBounds.allPositionsWithin)
            {
                var localPlace = new Vector3Int(pos.x, pos.y, pos.z);

                if (!Tilemap.HasTile(localPlace)) continue;

                var tile = new WorldTile
                {
                    LocalPlace    = localPlace,
                    WorldLocation = Tilemap.CellToWorld(localPlace),
                    TileBase      = Tilemap.GetTile(localPlace),
                    TilemapMember = Tilemap,
                    Name          = localPlace.x + "," + localPlace.y,
                };

                tiles.Add(tile.WorldLocation, tile);
            }

            string     path       = Application.dataPath + "/data.json";
            DataFormat dataFormat = DataFormat.JSON;
            var        bytes      = SerializationUtility.SerializeValue(tiles, dataFormat);
            File.WriteAllBytes(path, bytes);

            //tiles = new Dictionary<Vector3, WorldTile>();
        }

        [Button]
        private void SetWorldTiles()
        {
            DataFormat dataFormat = DataFormat.JSON;
            string     path       = Application.dataPath + "/data.json";
            var        bytes      = File.ReadAllBytes(path);
            tiles2 = SerializationUtility.DeserializeValue<Dictionary<Vector3, WorldTile>>(bytes, dataFormat);

            foreach (var t in tiles2)
            {
                Tilemap.SetTile(new Vector3Int((int)t.Key.x, (int)t.Key.z, 0), t.Value.TileBase);
            }
        }
    }
}
