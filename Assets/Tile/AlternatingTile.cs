//For chercker tiles, got it from here : https://stackoverflow.com/questions/76378182/how-to-create-a-checkerboard-tile-pattern-in-unity

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class AlternatingTile : Tile
{
    [SerializeField] private Sprite[] tiles;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        uint mask = (uint)(position.x + position.y);
        tileData.sprite = tiles[mask % 2];
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/2D/Tiles/Alternating Tile")]
    public static void CreateAlternatingTile()
    {

        string path = EditorUtility.SaveFilePanelInProject(
            "Alternating Tile",
            "New Alternating Tile",
            "Asset",
            "Please enter a name for the new alternating tile",
            "Assets");

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<AlternatingTile>(), path);
    }
#endif
}