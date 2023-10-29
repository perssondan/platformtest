using Microsoft.Graphics.Canvas;
using Newtonsoft.Json;

namespace uwpPlatformer.Models
{
    public class TileSet
    {
        public int FirstGid { get; set; }

        public string Source { get; set; }

        [JsonIgnore]
        public TileAtlas TileAtlas { get; set; }
    }
}