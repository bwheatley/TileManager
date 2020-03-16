using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using Tile = UnityEngine.Tilemaps.Tile;
using UnityEditor;

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
        public void GetTile(Vector2 theTile, Tilemap theMap) {
            Debug.Log(string.Format("Tile.GetTile Not Implemented Yet"));
            // theMap.GetTile()
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
            // TODO convert this to the new input managers
            // Generate the Map
            if (Input.GetMouseButtonDown(0)) {
                Debug.Log(string.Format("Starting TileMap simulation {0} times", numR));
                doSim(numR);
            }

            //Clear the Map
            if (Input.GetMouseButtonDown(1)) {
                Debug.Log(string.Format("Stopping TileMap simulation {0} times", numR));
                clearMap(true);
            }

            if (Input.GetMouseButtonDown(2)) {
                SaveAssetMap();
            }



        }

        public void SaveAssetMap() {
            string saveName = "tmapXY_" + count;
            var mf = GameObject.Find("Grid");

            if (mf) {
                var savePath = string.Format("Assets/_prefabs/tilemaps/{0}.prefab", saveName);
                if (PrefabUtility.CreatePrefab(savePath,mf)) {
                    EditorUtility.DisplayDialog("Tilemap Saved",
                        string.Format("Your tilemap was saved under {0}", savePath), "Continue");
                }
                else {
                    EditorUtility.DisplayDialog("Tilemap NOT Saved",
                        string.Format("An Error occured while trying to save Tilemap under {0}", savePath), "Continue");
                }
            }

        }

        /// <summary>
        /// Set a tile on a tilemap
        /// </summary>
        /// <param name="tilePos">Location to paint a tile</param>
        /// <param name="tile">What is the tile to be painted</param>
        /// <param name="tileMap">Tilemap to paint on</param>
        public static void SetTile(Vector3Int tilePos, Tile tile, Tilemap tileMap) {
            tileMap.SetTile(tilePos, tile);
        }

        /// <summary>
        /// Loop through a bunch of tile positions and set all of the tiles to thd defined tile
        /// </summary>
        /// <param name="tilePosStart"></param>
        /// <param name="tilePosStop"></param>
        /// <param name="tile"></param>
        /// <param name="tileMap"></param>
        public static void SetTileBlock(Vector3Int tilePosStart, Vector3Int tilePosStop, Tile tile, Tilemap tileMap) {
            var startX = tilePosStart.x;
            var startY= tilePosStart.y;
            var endX = tilePosStop.x;
            var endY = tilePosStop.y;

            Debug.Log(string.Format("TileTools:SetTileBlock Start X{0},Y{1} - End X{2},Y{3}",startX,startY,endX,endY));

            for (int x = startX; x <= endX; x++) {
                for (int y = startY; y <= endY; y++) {
                    tileMap.SetTile(new Vector3Int(x,y,0), tile );
                    Debug.Log(string.Format("Looping through tile block X{0},Y{1} tile set to {2}",x,y, tile.name));
                }
            }

        }


    }

}
