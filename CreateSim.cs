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
using UnityEngine.U2D;


namespace AepsLabs.TileManager {

#if UNITY_EDITOR
    public class CreateSim : MonoBehaviour{

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


        public UnityEngine.Tilemaps.Tilemap topMap;
        public UnityEngine.Tilemaps.Tilemap botMap;
        private int[,] terrainMap;

        public UnityEngine.Tilemaps.Tile topTile;
        public UnityEngine.Tilemaps.Tile botTile;

        private int width;
        private int height;


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

        public void SaveAssetMap() {
            string saveName = "tmapXY_" + count;
            var    mf       = GameObject.Find("Grid");

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

    }
#endif

}
