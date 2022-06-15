using Assets.Scripts.Units;
using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameLogic
{
    public class CurrentGameData
    {
        public int MapSize { get; set; }
        public List<Player> Players { get; set; }
        public List<Stack> AllStacks = new List<Stack>();

        public TerrainTile[,] TerrainTiles { get; private set; }

        // Turn objects
        public Player CurrentPlayer { get { return Players[PlayerPointer]; } }

        private int PlayerPointer;

        public CurrentGameData(GameDataContainer gameDataContainer)
        {
            // TODO remove This is for testing only
            UnitType heavyInf = gameDataContainer.UnitTypes[0];
            UnitType cavalry = gameDataContainer.UnitTypes[1];
            UnitType dragon = gameDataContainer.UnitTypes[2];
            MapSize = 20;
            TerrainTiles = GetTestTerrain();
            Players = new List<Player>()
            {
                new Player
                {
                    Name = "Karath",
                    PlayerUnits = new PriorityQueueMin<UnitType>(new UnitType[] { heavyInf, cavalry, dragon }),
                    Gold = 1000,
                    BanneSpriteName = "DragonBanner",
                },
                new Player
                {
                    Name = "Algast",
                    PlayerUnits = new PriorityQueueMin<UnitType>(new UnitType[] { heavyInf, cavalry }),
                    Gold = 1000,
                    BanneSpriteName = "KnightBanner",
                }
            };
        }

        public void NextTurn()
        {
            // change current player
            PlayerPointer++;
            if (PlayerPointer >= Players.Count)
            {
                PlayerPointer = 0;
            }
        }

        public void RemoveStackFromAllStacks(Stack stack)
        {
            AllStacks.Remove(stack);
        }

        private TerrainTile[,] GetTestTerrain()
        {
            #region TempTerrain
            // temp manual terrain 
            var terrainTiles = new TerrainTile[MapSize, MapSize];
            // Fill with grass
            for (int i = 0; i < terrainTiles.GetLength(0); i++)
            {
                for (int j = 0; j < terrainTiles.GetLength(1); j++)
                {
                    terrainTiles[i, j] = new TerrainTile(TerrainTile.TerrainType.Grass);
                }
            }
            // top left castle
            tempBuildCastle(terrainTiles, 2, 2);
            // top right castle
            tempBuildCastle(terrainTiles, 16, 2);
            // center castle 
            tempBuildCastle(terrainTiles, 10, 7);
            // bottom center castle
            tempBuildCastle(terrainTiles, 9, 15);
            // bottom right castle
            tempBuildCastle(terrainTiles, 15, 18);

            // top right forest
            for (int i = 0; i < 4; i++)
            {
                terrainTiles[16 + i, 0] = new TerrainTile(TerrainTile.TerrainType.Forest);
            }
            terrainTiles[19, 1] = new TerrainTile(TerrainTile.TerrainType.Forest);
            terrainTiles[19, 2] = new TerrainTile(TerrainTile.TerrainType.Forest);

            // bottom left forest
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    terrainTiles[4 + i, 17 + j] = new TerrainTile(TerrainTile.TerrainType.Forest);
                }
            }
            terrainTiles[6, 18] = new TerrainTile(TerrainTile.TerrainType.Forest);

            // center mountain
            terrainTiles[9, 4] = new TerrainTile(TerrainTile.TerrainType.Mountain);
            terrainTiles[9, 5] = new TerrainTile(TerrainTile.TerrainType.Mountain);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    terrainTiles[7 + i, 5 + j] = new TerrainTile(TerrainTile.TerrainType.Mountain);
                }
            }
            terrainTiles[6, 7] = new TerrainTile(TerrainTile.TerrainType.Mountain);
            terrainTiles[7, 7] = new TerrainTile(TerrainTile.TerrainType.Mountain);

            // center road
            for (int i = 0; i < 7; i++)
            {
                terrainTiles[9, 8 + i] = new TerrainTile(TerrainTile.TerrainType.Road);
            }

            // Center bridge, overwrites road
            terrainTiles[9, 12] = new TerrainTile(TerrainTile.TerrainType.Bridge);

            // bottom road
            for (int i = 0; i < 2; i++)
            {
                terrainTiles[11 + i, 16] = new TerrainTile(TerrainTile.TerrainType.Road);
            }
            for (int i = 0; i < 4; i++)
            {
                terrainTiles[12 + i, 17] = new TerrainTile(TerrainTile.TerrainType.Road);
            }

            // water
            tempWaterVertLine(terrainTiles, 4, 15, 19);
            tempWaterVertLine(terrainTiles, 6, 14, 18);
            tempWaterVertLine(terrainTiles, 6, 13, 17);
            tempWaterVertLine(terrainTiles, 8, 12, 16);
            tempWaterVertLine(terrainTiles, 8, 12, 15);
            tempWaterVertLine(terrainTiles, 11, 12, 14);
            tempWaterHoriLine(terrainTiles, 10, 13, 12);

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    terrainTiles[8 + i, 0 + j] = new TerrainTile(TerrainTile.TerrainType.Water);
                }
            }

            tempWaterVertLine(terrainTiles, 0, 4, 7);
            tempWaterVertLine(terrainTiles, 0, 6, 6);
            tempWaterVertLine(terrainTiles, 0, 7, 5);
            tempWaterHoriLine(terrainTiles, 0, 5, 0);

            tempWaterVertLine(terrainTiles, 5, 13, 4);
            tempWaterVertLine(terrainTiles, 5, 14, 3);
            tempWaterVertLine(terrainTiles, 5, 19, 2);
            tempWaterVertLine(terrainTiles, 5, 19, 1);
            tempWaterVertLine(terrainTiles, 0, 19, 0);

            tempWaterVertLine(terrainTiles, 11, 12, 5);
            tempWaterHoriLine(terrainTiles, 6, 8, 12);
            terrainTiles[2, 5] = new TerrainTile(TerrainTile.TerrainType.Bridge);
            #endregion

            return terrainTiles;
        }

        private void tempBuildCastle(TerrainTile[,] terrainLayout, int xcoord, int ycoord)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    terrainLayout[xcoord + i, ycoord + j] = new TerrainTile(TerrainTile.TerrainType.Castle);
                }
            }
        }
        private void tempWaterVertLine(TerrainTile[,] terrainLayout, int starty, int endy, int x)
        {
            for (int i = 0; i < endy - starty + 1; i++)
            {
                terrainLayout[x, starty + i] = new TerrainTile(TerrainTile.TerrainType.Water);
            }
        }
        private void tempWaterHoriLine(TerrainTile[,] terrainLayout, int startx, int endx, int y)
        {
            for (int i = 0; i < endx - startx + 1; i++)
            {
                terrainLayout[startx + i, y] = new TerrainTile(TerrainTile.TerrainType.Water);
            }
        }
    }
}
