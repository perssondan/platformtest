using System;

namespace uwpPlatformer.Models
{
    public class Tile
    {
        public int Id { get; set; }

        public string Class { get; set; }

        public AnimationFrame[] Animation { get; set; } = Array.Empty<AnimationFrame>();

        public ObjectGroup ObjectGroup { get; set; }
    }
}