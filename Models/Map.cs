using Newtonsoft.Json;
using System;

namespace uwpPlatformer.Models
{
    public class Map
    {
        /// <summary>
        /// Height of map, number of tiles
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Width of map, number of tiles
        /// </summary>
        public int Width { get; set; }

        public bool Infinite { get; set; }

        public Layer[] Layers { get; set; } = Array.Empty<Layer>();

        public string Orientation { get; set; }

        public string RenderOrder { get; set; }

        public Version TiledVersion { get; set; }

        public int TileHeight { get; set; }

        public int TileWidth { get; set; }

        [JsonProperty("type")]
        public string MapType { get; set; }

        public float Version { get; set; }

        public TileSet[] TileSets { get; set; } = Array.Empty<TileSet>();

        [JsonIgnore]
        public int MapWidth => Width * TileWidth;

        [JsonIgnore]
        public int MapHeight => Height * TileHeight;
    }
}