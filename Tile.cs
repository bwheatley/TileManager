using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using Tile = UnityEngine.Tilemaps.Tile;
using Debug = UnityEngine.Debug;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Diagnostics;
using System.IO;
using OdinSerializer;

namespace AepsLabs.TileManager {

    public class TileTools : MonoBehaviour {

        private int[,] terrainMap;

        public UnityEngine.Tilemaps.Tilemap topMap;
        public UnityEngine.Tilemaps.Tilemap botMap;
        public UnityEngine.Tilemaps.Tile topTile;
        public UnityEngine.Tilemaps.Tile botTile;

        private int width;
        private int height;


        // Remove this potentially
        [Range(0,100)]
        public int iniChance;

        [Range(1, 8)]
        public int birthLimit;

        [Range(1, 8)]
        public int deathLimit;

        [Range(0, 10)]
        public int numR;
        private int count = 0;

        public Vector3Int tmapSize;
        // End Remove potentially




        public void doSim(int numR) {
            clearMap(false);

            width = tmapSize.x;
            height = tmapSize.y;

            if (terrainMap == null) {
                terrainMap = new int[width, height];
                initPos();
            }

            for (int i = 0; i < numR; i++) {
                terrainMap = genTilePos(terrainMap);
            }

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    if (terrainMap[x, y] == 1) {
                        topMap.SetTile(new Vector3Int(-x + width /2, -y + height /2, 0), topTile);
                        topMap.SetTile(new Vector3Int(-x + width /2, -y + height /2, 0), botTile);
                    }
                }
            }

        }

        public int[,] genTilePos(int[,] oldMap) {
            int[,] newMap = new int[width,height];
            int neighb;
            BoundsInt myB = new BoundsInt(-1,-1,0,3,3,1);

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    neighb = 0;

                    foreach (var b in myB.allPositionsWithin) {
                        if(b.x==0 && b.y == 0) continue; //Current Position
                        if (x + b.x >= 0 && x + b.x < width && y + b.y >= 0 && y + b.y < height) {
                            neighb += oldMap[x + b.x, y + b.y];
                        }
                        else {
                            neighb++;
                        }

                    }

                    if (oldMap[x, y] == 1) {
                        if (neighb < deathLimit) newMap[x, y] = 0;
                        else {
                            newMap[x, y] = 1;
                        }
                    }

                    if (oldMap[x, y] == 0) {
                        if (neighb > birthLimit) newMap[x, y] = 1;
                        else {
                            newMap[x, y] = 0;
                        }
                    }

                }
            }


            return newMap;
        }

        public void initPos() {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    terrainMap[x, y] = Random.Range(1, 101) < iniChance ? 1 : 0;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="theTile"></param>
        public static Tile GetTile(Vector3Int theTile, Tilemap theMap) {
            return (Tile)theMap.GetTile(theTile);
        }



        /// <summary>
        /// Clear the Map that is passed in
        /// </summary>
        /// <param name="complete"></param>
        /// <param name="theMap"></param>
        // public void clearMap(bool complete, Tilemap theMap) {
        public void clearMap(bool complete) {
            topMap.ClearAllTiles();
            botMap.ClearAllTiles();

            if (complete) {
                terrainMap = null;
            }
        }

        public void Update() {
            // // TODO convert this to the new input managers
            // // Generate the Map
            // if (Input.GetMouseButtonDown(0)) {
            //     Debug.Log(string.Format("Starting TileMap simulation {0} times", numR));
            //     doSim(numR);
            // }
            //
            // //Clear the Map
            // if (Input.GetMouseButtonDown(1)) {
            //     Debug.Log(string.Format("Stopping TileMap simulation {0} times", numR));
            //     clearMap(true);
            // }
            //
            // if (Input.GetMouseButtonDown(2)) {
            //     SaveAssetMap();
            // }



        }

        public void SaveAssetMap() {
            string saveName = "tmapXY_" + count;
            var mf = GameObject.Find("Grid");

            if (mf) {
                var savePath = string.Format("Assets/_prefabs/tilemaps/{0}.prefab", saveName);
                // TODO update this to use SaveAsPrefabAsset
                // if (PrefabUtility.CreatePrefab(savePath,mf)) {
                //     EditorUtility.DisplayDialog("Tilemap Saved",
                //         string.Format("Your tilemap was saved under {0}", savePath), "Continue");
                // }
                // else {
                //     EditorUtility.DisplayDialog("Tilemap NOT Saved",
                //         string.Format("An Error occured while trying to save Tilemap under {0}", savePath), "Continue");
                // }
            }

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

#if UNITY_EDITOR
        /// <summary>
        /// Take in a tilemap and save out to a binary format, that can be reloaded later. Currently only supports 2D
        /// TODO add support for z axis
        /// </summary>
        /// <param name="tileMap">Tilemap to Save out to binary</param>
        /// <param name="path">Path to save to</param>
        /// <param name="rows">Number of rows in the tilemap</param>
        /// <param name="columns">Number of columns in the tilemaps</param>
        public static void SaveTilemap(Tilemap tileMap, string path, int columns, int rows) {
            WorldTile[,] tiles = new WorldTile[columns,rows];

            for (int x = 0; x <= columns; x++) {
                for (int y = 0; y <= rows; y++) {
                    var localPlace = new Vector3Int(x, y, 0);

                    if (!tileMap.HasTile(localPlace)) continue;

                    var spriteLoc = tileMap.GetSprite(localPlace);
                    var spritePath = AssetDatabase.GetAssetPath(spriteLoc);
                    var resourcePath = spritePath.Split('.');

                    //TODO button up the sprite path since it could break when using
                    // a path w/o an extension
                    var tile = new WorldTile
                    {
                        LocalPlace    = localPlace,
                        WorldLocation = tileMap.CellToWorld(localPlace),
                        TileSprite = resourcePath[0],
                        // TileBase      = tileMap.GetTile(localPlace),
                        // TilemapMember = tileMap,
                        Color = tileMap.GetColor(localPlace),
                        Name          = localPlace.x + "," + localPlace.y,
                    };

                    tiles[x, y] = tile;
                }
            }
            // tiles = new Dictionary<Vector3, WorldTile>();
            // foreach (Vector3Int pos in Tilemap.cellBounds.allPositionsWithin)

            //TODO convert to binary once we get the deserializatio working
            DataFormat dataFormat = DataFormat.Binary;
            var        bytes      = SerializationUtility.SerializeValue(tiles, dataFormat);
            File.WriteAllBytes(path, bytes);
        }
#endif


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
