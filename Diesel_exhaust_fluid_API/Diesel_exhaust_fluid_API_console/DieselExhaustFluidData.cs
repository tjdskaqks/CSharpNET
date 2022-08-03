using System.Text.Json.Serialization;

namespace Diesel_exhaust_fluid_API_console
{
    public class DieselExhaustFluidData
    {
        [JsonPropertyName("코드")]
        public string? 코드 { get; set; }
        [JsonPropertyName("명칭")]
        public string? 명칭 { get; set; }
        [JsonPropertyName("주소")]
        public string? 주소 { get; set; }
        [JsonPropertyName("전화번호")]
        public string? 전화번호 { get; set; }
        [JsonPropertyName("영업시간")]
        public string? 영업시간 { get; set; }
        [JsonPropertyName("재고량")]
        public int 재고량 { get; set; }
        [JsonPropertyName("가격")]
        public string? 가격 { get; set; }
        [JsonPropertyName("위도")]
        public string? 위도 { get; set; }
        [JsonPropertyName("경도")]
        public string? 경도 { get; set; }
        [JsonPropertyName("데이터기준일")]
        public string? 데이터기준일 { get; set; }

        public override string ToString() => $"데이터기준일 : {데이터기준일}, 코드 : {코드}, 명칭 : {명칭}, 주소 : {주소}, 재고량 : {재고량}, 가격 : {가격}, 전화번호 : {전화번호}, 영업시간 : {영업시간}{Environment.NewLine}";
    }
}