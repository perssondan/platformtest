using System;
using System.Linq;
using uwpPlatformer.Models;

namespace uwpPlatformer.Extensions
{
    public static class TileSetExtensions
    {
        public static bool IsHeroTileSet(this TileSet tileSet)
        {
            return tileSet.IsTileSetOf("hero");
        }

        public static bool IsCharacterSetTileSet(this TileSet tileSet)
        {
            return tileSet.IsTileSetOf("characterset_tiles");
        }

        public static bool IsWorldMapTileSet(this TileSet tileSet)
        {
            return tileSet.IsTileSetOf("world_tiles");
        }

        public static bool IsEnemyWaspTileSet(this TileSet tileSet)
        {
            return tileSet.IsTileSetOf("enemy_wasp");
        }

        public static bool IsTileSetOf(this TileSet tileSet, string tileSetType)
        {
            return tileSet.TileAtlas.CustomProperties
                .Any(x => string.Equals(x.Name, "tileSetType", StringComparison.InvariantCultureIgnoreCase) && string.Equals($"{x.Value}", tileSetType, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
