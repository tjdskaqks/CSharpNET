using System.Text;
using System.Text.Json.Serialization;

namespace Diesel_exhaust_fluid_API_console
{
    public class DieselExhaustFluid
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }
        [JsonPropertyName("perPage")]
        public int PerPage { get; set; }
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
        [JsonPropertyName("currentCount")]
        public int CurrentCount { get; set; }
        [JsonPropertyName("data")]
        public List<DieselExhaustFluidData> DieselExhaustFluidDataList { get; set; } = new();

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            foreach (var dieselExhaustFluidData in DieselExhaustFluidDataList)
                stringBuilder.Append(dieselExhaustFluidData);
            return $"{stringBuilder}{Environment.NewLine}";
        }
    }
}
