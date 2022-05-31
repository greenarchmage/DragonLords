using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Serialization
{
    internal class MapLoader
    {

        private string[,] SiegeMathos = new string[,]
        {
            { "F","F","F","F","F","F","F","G","G","G","G","G","G","G","G","G","G","G","G","G","G","R","G","G","G","G","G","G","G","G","G","M","M","M","M","M","M","M","M","M","M", },
            { "F","F","F","F","F","F","F","G","G","G","G","G","G","G","G","W","W","W","W","W","W","R","W","W","W","W","W","W","W","W","G","M","M","M","M","M","M","M","M","M","M", },
            { "F","F","F","F","F","F","F","G","G","G","G","G","G","G","W","W","W","W","W","W","W","R","W","W","W","W","W","W","W","W","G","M","M","M","M","M","M","M","M","M","M", },
            { "F","F","F","F","F","F","F","G","G","G","G","G","G","G","W","W","G","C","C","G","G","R","W","W","W","W","W","W","W","W","G","M","M","M","M","M","M","M","M","M","M", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","G","G","G","G","M","M","M","M", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","G","M","M","M","M","M","M","M", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","C","C","G","G","M","M","M","M","M","M","M", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","S","S","S","G","C","C","G","G","M","M","M","M","M","M","M", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","S","S","M","M","M","M","M","M","M","M","M","M","M","M","M", },
            { "X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","X","W","W","M","M","M","M","M","M","M","M","M","M","M","M","M", },
            { "F","F","F","F","F","F","F","F","F","F","L","G","G","G","G","G","G","S","S","S","S","S","S","G","W","W","W","W","M","M","M","M","M","M","M","M","M","M","M","M","M", },
            { "F","F","F","F","F","F","F","F","F","F","G","G","G","G","G","G","G","S","S","S","S","M","M","M","M","M","M","M","M","M","M","M","M","M","M","M","M","M","M","M","M", },
        };

        public TerrainTile[,] GetTerrainTiles()
        {
            var tiles = TransformToGameData(SiegeMathos);
            return tiles;
        }




        private TerrainTile[,] TransformToGameData(string[,] stringRepresenation)
        {
            var tiles = new TerrainTile[stringRepresenation.Length, stringRepresenation.LongLength];
            for(int i = 0; i < stringRepresenation.Length; i++)
            {
                for(int j = 0; j <stringRepresenation.LongLength; j++)
                {
                    tiles[i,j] = GetTileFromString(stringRepresenation[i,j]);
                    
                }
            }
            return tiles;
        }

        private TerrainTile GetTileFromString (string strRep)
        {
            var tileType = TerrainTile.TerrainType.Grass;
            switch (strRep)
            {
                case "B":
                    tileType = TerrainTile.TerrainType.Bridge;
                    break;
                case "C":
                    tileType = TerrainTile.TerrainType.Castle;
                    break;
                case "F":
                    tileType = TerrainTile.TerrainType.Forest;
                    break;
                case "G":
                    tileType = TerrainTile.TerrainType.Grass;
                    break;
                case "M":
                    tileType = TerrainTile.TerrainType.Mountain;
                    break;
                case "R":
                    tileType = TerrainTile.TerrainType.Road;
                    break;
                case "W":
                    tileType = TerrainTile.TerrainType.Water;
                    break;
                case "S":
                    tileType = TerrainTile.TerrainType.Swamp;
                    break;
                case "L":
                    tileType = TerrainTile.TerrainType.ForestGrass;
                    break;
                default:
                    break;
            }
            return new TerrainTile(tileType);
        }
    }
}
