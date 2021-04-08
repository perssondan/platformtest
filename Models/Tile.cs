using System;

namespace uwpKarate.Models
{
    public class Tile
    {
        public int Id { get; set; }
        public AnimationFrame[] Animation { get; set; } = Array.Empty<AnimationFrame>();
    }
}