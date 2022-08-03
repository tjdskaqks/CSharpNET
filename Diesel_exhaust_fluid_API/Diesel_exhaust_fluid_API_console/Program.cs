// See https://aka.ms/new-console-template for more information
using System.Net.Http.Headers;
using Diesel_exhaust_fluid_API_console;

const string GET_URL = "https://api.odcloud.kr/api/15094782/v1/uddi:6b2017af-659d-437e-a549-c59788817675?page={0}&perPage={1}&serviceKey={2}";
string privateApiKey = "개인키";

System.Net.Http.HttpClient httpClient = new();
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(privateApiKey);

string Get_Api_Url = string.Format(GET_URL, 1, 1000, privateApiKey);
string responseResult = await httpClient.GetStringAsync(Get_Api_Url);
if (!string.IsNullOrEmpty(responseResult))
{
    var deserializeAsyncResult = System.Text.Json.JsonSerializer.Deserialize<DieselExhaustFluid>(responseResult);

    Console.WriteLine(deserializeAsyncResult);
}
else
    Console.WriteLine("데이터 없음.");