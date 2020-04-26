using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using Tile = UnityEngine.Tilemaps.Tile;
using Debug = UnityEngine.Debug;
using System.Diagnostics;
using System.IO;
using OdinSerializer;
using UnityEngine.U2D;

namespace AepsLabs.TileManager {

    public class TileTools : MonoBehaviour {


        /// <summary>
        ///
        /// </summary>
        /// <param name="theTile"></param>
        public static Tile GetTile(Vector3Int theTile, Tilemap theMap) {
            return (Tile)theMap.GetTile(theTile);
        }

        /// <summary>
        /// Set a tile on a tilemap
        /// </summary>
        /// <param name="tilePos">Location to paint a tile</param>
        /// <param name="tile">What is the tile to be painted</param>
        /// <param name="tileMap">Tilemap to paint on</param>
        public static void SetTile(Vector3Int tilePos, Tilemap tileMap, Tile tile ) {
            tileMap.SetTile(tilePos, tile);
        }

        public static void SetTile(Vector3Int tilePos, Tilemap tileMap, Tile[] tile ) {
            SetTileRandom(tilePos, tileMap, tile);
        }


        public static void SetTileRandom(Vector3Int tilePos, Tilemap tileMap, Tile[] randomTile) {
            var tile = randomTile[Random.Range(0, randomTile.Length)];

            SetTile(tilePos, tileMap, tile);
        }

        public static void SetTileBlockLegacy(Vector3Int tilePosStart, Vector3Int tilePosStop, Tile tile,
            Tilemap                                      tileMap, bool debug = false) {
            SetTileBlockLegacy(tilePosStart, tilePosStop, tile, tileMap, new Tile[0], debug);
        }

        /// <summary>
        /// Loop through a bunch of tile positions and set all of the tiles to thd defined tile
        /// Less efficient then the bulk option but around for history sake
        /// </summary>
        /// <param name="tilePosStart"></param>
        /// <param name="tilePosStop"></param>
        /// <param name="tile"></param>
        /// <param name="tileMap"></param>
        public static void SetTileBlockLegacy(Vector3Int tilePosStart, Vector3Int tilePosStop, Tile tile, Tilemap tileMap, Tile[] randomTiles, bool debug = false) {
            var startX = tilePosStart.x;
            var startY= tilePosStart.y;
            var endX = tilePosStop.x;
            var endY = tilePosStop.y;

            Stopwatch _sw = new Stopwatch();
            if (debug) {
                Debug.Log(string.Format("TileTools:SetTileBlock Start X{0},Y{1} - End X{2},Y{3}", startX, startY, endX,
                    endY));
                // Start timer to check performance
                _sw.Start();
            }

            for (int x = startX; x <= endX; x++) {
                for (int y = startY; y <= endY; y++) {
                    if (randomTiles.Length > 0) {
                        tileMap.SetTile(new Vector3Int(x, y, 0), randomTiles[Random.Range(0, randomTiles.Length)]);
                    }
                    else {
                        tileMap.SetTile(new Vector3Int(x, y, 0), tile);
                    }
                }
            }

            if (debug) {
                // Stop timer
                _sw.Stop();

                var m_testResult = string.Format("[PerformanceTester] Execution time for loop: {0}ms",
                    _sw.ElapsedMilliseconds);
                Debug.Log(m_testResult);
            }
        }


        /// <summary>
        /// Take in a tilemap and save out to a binary format, that can be reloaded later. Currently only supports 2D
        /// TODO add support for z axis
        /// </summary>
        /// <param name="tileMap">Tilemap to Save out to binary</param>
        /// <param name="path">Path to save to</param>
        /// <param name="rows">Number of rows in the tilemap</param>
        /// <param name="columns">Number of columns in the tilemaps</param>
        /// <param name="dataFormat">JSON, Binary, Other</param>
        /// <param name="spriteAtlas">If you use a spriteAtlas for your tilesheets, this will allow you to reference the child sprite </param>
        public static void SaveTilemap(Tilemap tileMap, string path, int columns, int rows,
            DataFormat dataFormat = DataFormat.Binary, SpriteAtlas spriteAtlas = null) {
            WorldTile[,] tiles = new WorldTile[columns,rows];

            for (int x = 0; x <= columns; x++) {
                for (int y = 0; y <= rows; y++) {
                    var localPlace = new Vector3Int(x, y, 0);

                    //If there is no tile here, fuck off
                    if (!tileMap.HasTile(localPlace)) continue;

                    var spriteLoc = tileMap.GetSprite(localPlace);
                    var _theTile = tileMap.GetTile(localPlace);
                    // TileData _theTileData = _theTile.GetTileData(localPlace, tileMap, _theTileData);

                    //TODO button up the sprite path since it could break when using
                    //TODO support different scaling for tiles
                    // a path w/o an extension
                    var tile = new WorldTile
                    {
                        Name = localPlace.x + "," + localPlace.y,
                        position    = localPlace,
                        scale = new Vector3(1,1,1),
                        worldPosition = tileMap.GetCellCenterWorld(localPlace),
                        Color = tileMap.GetColor(localPlace),
                        rotation =  tileMap.GetTransformMatrix(localPlace).rotation.eulerAngles,
                        SpriteName = spriteLoc.name,
                    };

                    //Set the tile that's going to be serialized
                    tiles[x, y] = tile;
                }
            }
            var        bytes      = SerializationUtility.SerializeValue(tiles, dataFormat);
            File.WriteAllBytes(path, bytes);
        }
#if UNITY_EDITOR
#endif


        /// <summary>
        /// Pass in a tileMap to restore your data to
        /// Along with a Path to a Saved data object and load your data!
        /// MUST have sprites in a SpriteAtlas to work
        /// TODO support raw sprites, but spritesheets must always be in a SpriteAtlas
        /// </summary>
        /// <param name="tileMapGO">GameObject with a TileMap Attached</param>
        /// <param name="myPath">Path to the data object saved with SaveTileMap</param>
        /// <param name="dataFormat">The format the data is stored</param>
        /// <param name="spriteAtlas">The spriteAtlas where your sprites are stored</param>
        public static void LoadTileMap(GameObject tileMapGO, string myPath, DataFormat dataFormat, SpriteAtlas spriteAtlas
        ) {
		var bytes = File.ReadAllBytes(myPath);
		var data = SerializationUtility.DeserializeValue<WorldTile[,]>(bytes, dataFormat);

		Tilemap tileMap = tileMapGO.GetComponent<Tilemap>();

        //Loop over the deserialized data, and paint the tilemap
		for ( int x = 0; x <= data.GetUpperBound(0); x++ ) {
			for ( int y = 0; y <= data.GetUpperBound(1); y++ ) {

				//Skip if there is no data for a position
				if (data[x,y] == null) continue;

				///The tile we're building has no concept of it's position
				/// We NEED to know it's position for when we call back to SetTile to pain the fucker
				var myTile = ScriptableObject.CreateInstance<Tile>();

				//Position where the tile will live in the tilemap
				var pos = new Vector3Int(x,y,0);
				tileMap.SetTileFlags(pos, TileFlags.None );

                // TODO support non-spriteatlas based sprites
                // If you don't have a sprite don't error
                if (data[x, y].SpriteName != null) {
                    string spriteName = data[x, y].SpriteName;
                    spriteName.Replace("(Clone)", "");

                    myTile.sprite = spriteAtlas.GetSprite(data[x, y].SpriteName);

                    //Rename the tile properly
                    myTile.name        = spriteName;
                    myTile.sprite.name = spriteName;
                }

                TileTools.SetTile(pos, tileMap , myTile  );
				TileTools.SetColor(pos, tileMap, data[x,y].Color  );
            }
		}

		//Refresh the tilemaps
		tileMap.RefreshAllTiles();
		Debug.Log(String.Format("GameData:LoadMap data name {0}", data.Length));
        }



        public static void SetTileBlock(Vector3Int tilePosStart, Vector3Int tilePosStop, Tile tile, Tilemap tileMap, bool debug = false) {
            SetTileBlock(tilePosStart, tilePosStop, tile, tileMap, new Tile[0], debug);
        }

        static Vector2Int CalculateStartEnd( int _startNum, int _endNum ) {
            int _startNumber, _endNumber;

            if ( _startNum < _endNum ) {
                _startNumber = _startNum;
                _endNumber   = _endNum;
            }
            else {
                _startNumber = _endNum;
                _endNumber   = _startNum;
            }
            return new Vector2Int( _startNumber, _endNumber );
        }

        /// <summary>
        /// Loop through a bunch of tile positions and set all of the tiles to thd defined tile
        /// </summary>
        /// <param name="tilePosStart"></param>
        /// <param name="tilePosStop"></param>
        /// <param name="tile"></param>
        /// <param name="tileMap"></param>
        /// <param name="randomTiles">If random tiles is passed in then randomly paint all tiles</param>
        public static void SetTileBlock(Vector3Int tilePosStart, Vector3Int tilePosStop, Tile tile, Tilemap tileMap, Tile[] randomTiles, bool debug = false, bool performanceDebug = false) {
            var _myStartX = CalculateStartEnd( tilePosStart.x, tilePosStop.x );
            var _myStartY = CalculateStartEnd( tilePosStart.y, tilePosStop.y );
            // var _myStartZ = CalculateStartEnd( tilePosStart.z, tilePosStop.z );

            var startX = _myStartX.x;
            var startY = _myStartY.x;
            var endX   = _myStartX.y;
            var endY   = _myStartY.y;


            Stopwatch _sw = new Stopwatch();
            if (debug) {
                Debug.Log(string.Format("TileTools:SetTileBlock Start X{0},Y{1} - End X{2},Y{3}", startX, startY, endX,
                    endY));
                if (performanceDebug) {
                    // Start timer to check performance
                    _sw.Start();
                }
            }

            // BoundsInt myB = new BoundsInt(-1, -1, 0, 3, 3, 1);
            // BoundsInt bounds = new BoundsInt(startX, startY, 0, endX, endY, 1);
            //BoundsInt(origin, size);
            BoundsInt bounds = new BoundsInt(new Vector3Int(startX,startY,0), new Vector3Int(Mathf.Clamp(endX-startX, 1, int.MaxValue), Mathf.Clamp(endY-startY, 1, int.MaxValue ), 1) );

            // TileBase[] tileArray = new TileBase[endX * endY];
            TileBase[] tileArray = new TileBase[bounds.size.x * bounds.size.y * bounds.size.z];
            if (debug) {
                Debug.Log(string.Format("tileArray Size {0} Bounds X {1} Y {2} Z {3}", tileArray.Length, bounds.size.x,
                    bounds.size.y, bounds.size.z));
            }

            for (int index = 0; index < tileArray.Length; index++)
            {
                //If random tiles passed in pick a random tile
                if (randomTiles.Length > 0) {
                    tileArray[index] = randomTiles[Random.Range(0, randomTiles.Length)];
                }
                else {
                    tileArray[index] = index % 2 == 0 ? tile : tile;
                }
            }

            tileMap.SetTilesBlock(bounds, tileArray);

            if (debug && performanceDebug) {
                // Stop timer
                _sw.Stop();
                var m_testResult = string.Format("[PerformanceTester] Execution time for loop: {0}ms",
                    _sw.ElapsedMilliseconds);
                Debug.Log(m_testResult);
            }
        }

        /// <summary>
        /// Clear out a tilemap
        /// </summary>
        /// <param name="tileMap"></param>
        public static void ClearTileMap(Tilemap tileMap) {
            tileMap.ClearAllTiles();
        }

        /// <summary>
        /// Pass in a tilemap to activate or deactivate
        /// </summary>
        /// <param name="tileMap"></param>get
        /// <param name="active"></param>
        public static void SetLayerActive(Tilemap tileMap, bool active) {
            //Get the tilemaps GO
            tileMap.gameObject.SetActive(active);
        }


        public static void DeleteTileBlock() {
            Debug.LogError("DeleteTileBlock not yet implemented!");
        }

        public static void DeleteTile(Vector3Int pos, Tilemap tileMap) {
            tileMap.SetTile(pos, null);
        }

        public static Tile.ColliderType GetCollider(Vector3Int pos, Tilemap tileMap) {
            return tileMap.GetColliderType(pos);
        }

        public static void SetCollider(Vector3Int pos, Tilemap tileMap) {

        }

        public static Matrix4x4 GetRotation(Vector3Int pos, Tilemap tileMap) {
            // tileMap.SetTileFlags(pos, TileFlags.None );
            return tileMap.GetTransformMatrix(pos);
        }

        public static void SetRotation(Vector3Int pos, Tilemap tileMap, float rotation) {
            tileMap.SetTileFlags(pos, TileFlags.None );
            Matrix4x4 matrix  = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, rotation), Vector3.one);
            tileMap.SetTransformMatrix(pos, matrix);
        }

        public static void SetColor(Vector3Int pos, Tilemap tileMap, Color color) {
            tileMap.SetTileFlags(pos, TileFlags.None );
            tileMap.SetColor(pos, color);
        }

        /// <summary>
        /// Check to see if you can paint something at a position.
        /// Check to make sure there is nothing on the paintLayer Currently
        /// Check to see if there is anything on the terrainLayer that would obstruct painting
        /// </summary>
        /// <param name="pos">Position of the tile</param>
        /// <param name="paintLayer">The tilemap that you will be painting on.</param>
        /// <param name="terrainLayer">The tilemap that is the terrain you would be painting over</param>
        /// <returns></returns>
        public static bool CheckPaintability(Vector3Int pos, Tilemap paintLayer, Tilemap terrainLayer) {
            bool result = true;

            //is there anything at the paintlayer to obstruct us from painting?
            if (TileTools.GetCollider(new Vector3Int(pos.x, pos.y, pos.z), paintLayer) != Tile.ColliderType.None
                ||TileTools.GetCollider(new Vector3Int(pos.x, pos.y, pos.z), terrainLayer) != Tile.ColliderType.None ) {
                return false;
            }

            return result;
        }

        public static bool ScreenToMapPosition(Vector3 screenPos, out Vector2 mapPosition, Tilemap tileMap = null, bool debug = false) {
            //Found near clip plane in example code https://docs.unity3d.com/ScriptReference/Camera.ScreenToWorldPoint.html
            //TODO make this support X number of cameras in a scene.
            mapPosition = WorldToMapPosition(tileMap, screenPos, debug);
            if ( mapPosition.x < 0 || mapPosition.x > tileMap.cellBounds.xMax || mapPosition.y <0 || mapPosition.y >= tileMap.cellBounds.yMax ) {
                //&& Input.mousePosition.y >= _myBottomEdge && !EventSystem.current.IsPointerOverGameObject()
                return false;
            }
            return true;
        }



        /// <summary>
        /// Take a World Position and convert it into a Map position
        /// Do this by comparing the grid to your world position.
        /// </summary>
        /// <param name="tileGrid"></param>
        /// <param name="screenPos">Pass in the screen position for us to convert to world then map</param>
        /// <returns></returns>
        public static Vector2 WorldToMapPosition(Tilemap tileMap, Vector3 screenPos, bool debug = false) {
            var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane));

            var cell = tileMap.layoutGrid.WorldToCell(worldPos);
            if (debug) Debug.Log(string.Format("worldPos {0} ScreenPos {1} cellPos {2}", worldPos, screenPos, cell));
            return new Vector2(cell.x, cell.y);
        }

    }

}
