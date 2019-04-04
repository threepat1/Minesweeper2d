using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minesweeper3D
{
    public class Grid : MonoBehaviour
    {
        public GameObject tilePrefab;
        public int width = 10, height = 10, depth = 10;
        public float spacing = 1.0f;

        // 3D Array to store all the tiles
        public Tile[,,] tiles;

        // Use this for initialization
        void Start()
        {

            GenerateTiles();
        }

        Tile SpawnTile(Vector3 position)
        {
            GameObject clone = Instantiate(tilePrefab, transform);
            clone.transform.position = position;
            Tile currentTile = clone.GetComponent<Tile>();
            return currentTile;
        }
        void Update()
        {
            MouseOver();
        }
        void GenerateTiles()
        {
            tiles = new Tile[width, height, depth];
            //store half size for later use
            Vector3 halfSize = new Vector3(width * 0.5f, height * 0.5f, depth * 0.5f);

            //offset
            Vector3 offset = new Vector3(.5f, .5f, .5f);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        //pivot tiles around grid
                        Vector3 pos = new Vector3(x - halfSize.x, y - halfSize.y, z - halfSize.z);
                        //apply offset
                        pos += offset;
                        //apply spacing
                        pos *= spacing;
                        //spawn the tile
                        Tile newTile = SpawnTile(pos);
                        //set transform of the tile to the grid
                        newTile.transform.SetParent(transform);
                        //store its array cords
                        newTile.x = x;
                        newTile.y = y;
                        newTile.z = z;
                        //store tile in array at those cords
                        tiles[x, y, z] = newTile;
                    }
                }
            }
        }
        bool IsOutOfBounds(int x, int y, int z)
        {
            return x < 0 || x >= width ||
            y < 0 || y >= height ||
            z < 0 || z >= depth;
        }
        int GetAdjacentMineCount(Tile tile)
        {
            //set count to 0
            int count = 0;
            //loop through all the adjacent tiles on the x
            for (int x = -1; x <= 1; x++)
            {
                //loop through all the adjacent tiles on the y
                for (int y = -1; y <= 1; y++)
                {
                    //loop through all the adjacent tiles on the z
                    for (int z = -1; z <= 1; z++)
                    {
                        int desiredX = tile.x + x;
                        int desiredY = tile.y + y;
                        int desiredZ = tile.z + z;

                        if (IsOutOfBounds(desiredX, desiredY, desiredZ))
                        {
                            continue;
                        }
                        //select current tile
                        Tile currentTile = tiles[desiredX, desiredY, desiredZ];
                        if (currentTile.isMine)
                        {
                            //increase increment by 1
                            count++;
                        }

                    }


                }

            }
            return count;
        }
        void MouseOver()
        {

            if (Input.GetMouseButtonDown(0))
            {
                Tile hitTile = GetHitTile(Input.mousePosition);
                if (hitTile)
                {
                    SelectTile(hitTile);
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                Tile hitTile = GetHitTile(Input.mousePosition);
                if (hitTile)
                {
                    hitTile.Flag();

                }
            }
        }
        //FF = Flood fill Algorithm
        void FFuncover(int x, int y, int z, bool[,,] visited)
        {
            //is x and y out of bounds of the grid?
            if (IsOutOfBounds(x, y, z))
            {
                //exit
                return;
            }
            //Have the coordinates already been visited?
            if (visited[x, y, z])
            {
                return;
            }
            // Reveal that tile in that x and y coordinate
            Tile tile = tiles[x, y, z];
            // Get number of mines around that tile
            int adjacentMines = GetAdjacentMineCount(tile);
            //Reveal the tile
            tile.Reveal(adjacentMines);

            //If there are no adjacent mines around that tile
            if (adjacentMines == 0)
            {
                // This tile has been visited
                visited[x, y, z] = true;
                // Visit all other tiles around this tile
                FFuncover(x - 1, y, z, visited);
                FFuncover(x + 1, y, z, visited);

                FFuncover(x, y - 1, z, visited);
                FFuncover(x, y + 1, z, visited);

                FFuncover(x, y, z - 1, visited);
                FFuncover(x, y, z + 1, visited);
            }
        }
        void UncoverAllMines()
        {
            // Loop through 2d array
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        Tile tile = tiles[x, y, z];
                        if (tile.isMine)
                        {
                            tile.Reveal();
                            Lose();
                        }
                    }

                }

            }
        }
        bool NoMoreEmptyTiles()
        {
            int emptyTileCount = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        Tile tile = tiles[x, y, z];
                        // if tile is revealed or is a mine
                        if (tile.isRevealed || tile.isMine)
                        {
                            //Skip to next loop iteration
                            continue;
                        }
                        //An empty tile has not been revealed
                        emptyTileCount++;
                    }
                }
            }
            // Return true if all empty tiles have been revealed
            return emptyTileCount == 0;
        }
        // Performs set of actoins on selected Tile
        void SelectTile(Tile selected)
        {
            int adjacentMines = GetAdjacentMineCount(selected);
            selected.Reveal(adjacentMines);

            if (selected.isMine)
            {
                // Uncover all mines
                UncoverAllMines();
                //Game over 
                print("Game over - You lose");
            }
            // Else, are there no more mines aorund this tile?
            else if (adjacentMines == 0)
            {
                int x = selected.x;
                int y = selected.y;
                int z = selected.z;
                // Use Flood Fill to uncover all adjacent mines
                FFuncover(x, y, z, new bool[width, height, depth]);
            }
            // Are there no more empty tiles in the game at this point?
            if (NoMoreEmptyTiles())
            {
                UncoverAllMines();
                print("Game Over - You win");
            }

        }
        Tile GetHitTile(Vector2 mousePosition)
        {
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(camRay, out hit))
            {
                return hit.collider.GetComponent<Tile>();
            }
            return null;
        }




        void Lose()
        {

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        //Scan the grid to check if there are no more empty tiles


    }
}
