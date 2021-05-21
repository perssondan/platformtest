using Newtonsoft.Json;
using System;

namespace uwpPlatformer.Models
{
    public class Map
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public bool Infinite { get; set; }
        public Layer[] Layers { get; set; } = Array.Empty<Layer>();
        public int NextLayerId { get; set; }
        public int NextObjectId { get; set; }
        public string Orientation { get; set; }
        public string RenderOrder { get; set; }
        public Version TiledVersion { get; set; }
        public int TileHeight { get; set; }
        public int TileWidth { get; set; }

        [JsonProperty("type")]
        public string MapType { get; set; }

        public float Version { get; set; }
        public TileSet[] TileSets { get; set; } = Array.Empty<TileSet>();
    }
}