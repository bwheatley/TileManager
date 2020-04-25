using OdinSerializer;

namespace AepsLabs.TileManager {

    using OdinSerializer;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    public class SaveLoadTiles : SerializedMonoBehaviour
    {
        public Tilemap                        Tilemap;
        public Dictionary<Vector3, WorldTileMap> tiles;
        public Dictionary<Vector3, WorldTileMap> tiles2;

        private void SetWorldTiles()
        {
            DataFormat dataFormat = DataFormat.JSON;
            string     path       = Application.dataPath + "/data.json";
            var        bytes      = File.ReadAllBytes(path);
            tiles2 = SerializationUtility.DeserializeValue<Dictionary<Vector3, WorldTileMap>>(bytes, dataFormat);

            foreach (var t in tiles2)
            {
                // Tilemap.SetTile(new Vector3Int((int)t.Key.x, (int)t.Key.z, 0), t.Value.TileBase);
            }
        }
    }
}
