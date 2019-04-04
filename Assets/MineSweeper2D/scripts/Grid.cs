using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minesweeper
{
    public class Grid : MonoBehaviour
    {
        public GameObject tilePrefab;
        public int width = 10, height = 10;
        public float spacing = .155f;

        private Tile[,] tiles;
        // Use this for initialization
        void Start()
        {
            GenerateTiles();
        }


        Tile SpawnTile(Vector3 pos)
        {
            GameObject clone = Instantiate(tilePrefab);
            clone.transform.position = pos;
            Tile currentTile = clone.GetComponent<Tile>();
            //Return it
            return currentTile;

        }
        void GenerateTiles()
        {
            tiles = new Tile[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2 halfSize = new Vector2(width * 0.5f, height * 0.5f);
                    // Pivot tiles around grid
                    Vector2 pos = new Vector2(x - halfSize.x, y - halfSize.y);
                    Vector2 offset = new Vector2(.5f, .5f);
                    pos += offset;
                    pos *= spacing;
                    Tile tile = SpawnTile(pos);
                    tile.transform.SetParent(transform);
                    tile.x = x;
                    tile.y = y;
                    tiles[x, y] = tile;

                }
            }
        }
        public int GetAdjacentMineCount(Tile tile)
        {
            // set count to 0
            int count = 0;
            // Loop through all the adjacent tiles on the x
            for (int x = -1; x <= 1; x++)
            {
                // Loop through all the adjacent tiles on the y
                for (int y = -1; y <= 1; y++)
                {
                    // Calculate which adjacent tile to look at
                    int desiredX = tile.x + x;
                    int desiredY = tile.y + y;
                    // check if the desired x& y is outside bounds
                    if (desiredX < 0 || desiredX >= width || 
                        desiredY < 0 || desiredY >= height)
                    {
                        // continue to next element in loop
                        continue;
                    }
                    Tile currentTile = tiles[desiredX, desiredY];
                    if (currentTile.isMine)
                    {
                        count++;
                    }
                }
                
            }
            return count;
        }
        void SelectATile()
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
            if (hit.collider != null)
            {
                Tile hitTile = hit.collider.GetComponent<Tile>();


                if (hitTile != null)
                {
                    int adjacentMines = GetAdjacentMineCount(hitTile);
                    hitTile.Reveal(adjacentMines);
                }
            }
        }
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
                if (hit.collider != null)
                {
                    Tile hitTile = hit.collider.GetComponent<Tile>();


                    if (hitTile != null)
                    {
                        SelectTile(hitTile);
                    }
                }
            }
        }

        void FFuncover(int x, int y, bool[,] visited)
        {
            if(x >=0 && y>=0 && x < width && y < height)
            {
                if (visited[x, y])
                    return;
                Tile tile = tiles[x, y];
                int adjacentMines = GetAdjacentMineCount(tile);
                tile.Reveal(adjacentMines);

                if(adjacentMines == 0)
                {
                    visited[x, y] = true;
                    FFuncover(x - 1, y, visited);
                    FFuncover(x + 1, y, visited);
                    FFuncover(x, y - 1, visited);
                    FFuncover(x, y + 1, visited);

                }
            }
        }
        void UncoverMines(int mineState = 0)
        {
            for (int x=0; x< width; x++)
            {
                for(int y =0; y < height; y++)
                {
                    Tile tile = tiles[x, y];
                    if (tile.isMine)
                    {
                        int adjacentMines = GetAdjacentMineCount(tile);
                        tile.Reveal(adjacentMines, mineState);
                    }
                }
            }
        }
        bool NoMoreEmptyTile()
        {
            int emptyTileCount = 0;
            for (int x=0;x<width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Tile tile = tiles[x, y];
                    if(!tile.isRevealed && !tile.isMine)
                    {
                        emptyTileCount += 1;
                    }
                }
            }
            return emptyTileCount == 0;
        }
        void SelectTile(Tile selected)
        {
            int adjacentMines = GetAdjacentMineCount(selected);
            selected.Reveal(adjacentMines);
            if (selected.isMine)
            {
                UncoverMines();

            }
            else if (adjacentMines == 0)
            {
                int x = selected.x;
                int y = selected.y;
                FFuncover(x, y, new bool[width, height]);
            }
            if (NoMoreEmptyTile())
            {
                UncoverMines(1);
            }
        }
    }
}
