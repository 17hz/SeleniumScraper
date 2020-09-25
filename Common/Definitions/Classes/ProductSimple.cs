using System;
using Newtonsoft.Json;

namespace Common.Definitions.Classes
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ProductSimple
    {
        [JsonProperty]
        public string MenuTitle { get; set; }
        [JsonProperty]
        public string MenuDescription { get; set; }
        [JsonProperty]
        public string MenuSectionTitle { get; set; }
        [JsonProperty]
        public string DishName { get; set; }
        [JsonProperty]
        public string DishDescription { get; set; }
        [JsonIgnore]
        public string Url;
    }
}