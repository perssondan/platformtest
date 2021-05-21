using Newtonsoft.Json;
using System;

namespace uwpPlatformer.Models
{
    public class TileAtlas
    {
        public int Columns { get; set; }

        [JsonProperty("image")]
        public string ImageSource { get; set; }

        public int ImageHeight { get; set; }
        public int ImageWidth { get; set; }
        public int Margin { get; set; }
        public string Name { get; set; }
        public int Spacing { get; set; }
        public int TileCount { get; set; }
        public Version TiledVersion { get; set; }
        public int TileHeight { get; set; }
        public int TileWidth { get; set; }

        [JsonProperty("type")]
        public string TileType { get; set; }
        public float Version { get; set; }
        public Tile[] Tiles { get; set; } = Array.Empty<Tile>();
    }
}