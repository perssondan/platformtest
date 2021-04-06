using Newtonsoft.Json;
using System;

namespace uwpKarate.Models
{
    public class Layer
    {
        public int[] Data { get; set; } = Array.Empty<int>();
        public int Height { get; set; }
        public int Width { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonProperty("type")]
        public string LayerType { get; set; }

        public int Opacity { get; set; }
        public bool Visibile { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}