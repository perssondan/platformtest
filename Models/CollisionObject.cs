using Newtonsoft.Json;

namespace uwpPlatformer.Models
{
    public class CollisionObject
    {
        public int Id { get; set; }

        public float Height { get; set; }

        public float Width { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public bool Visible { get; set; }

        [JsonProperty("type")]
        public string CollisionType { get; set; }

        public float Rotation { get; set; }
    }
}
