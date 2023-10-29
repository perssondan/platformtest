using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uwpPlatformer.Models
{
    public class ObjectGroup
    {
        public string DrawOrder { get; set; }
        public string Name { get; set; }
        public int Opacity { get; set; }

        [JsonProperty("type")]
        public string ObjectGroupType { get; set; }
        public bool Visible { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        [JsonProperty("objects")]
        public CollisionObject[] CollisionObjects { get; set; } = Array.Empty<CollisionObject>();
    }
}
