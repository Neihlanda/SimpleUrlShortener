using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleUrlShortener.Tests
{
    internal static class ObjectExtensions
    {
        public static HttpContent ToJsonHttpContent(this object obj)
        {
            return new StringContent(obj.ToJsonString(),
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
        }

        public static string ToJsonString(this object? obj)
            => System.Text.Json.JsonSerializer.Serialize(obj);

        public static async Task<object?> Deserialize(this HttpResponseMessage httpResponse, Type typeofResult)
        {
            return System.Text.Json.JsonSerializer.Deserialize(
                await httpResponse.Content.ReadAsStringAsync(),
                typeofResult,
                new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web)
            );
        }

        public static async Task<TValue?> Deserialize<TValue>(this HttpResponseMessage httpResponse) where TValue : class
        {
            return System.Text.Json.JsonSerializer.Deserialize<TValue>(
                await httpResponse.Content.ReadAsStringAsync(),
                new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web)
            );
        }
    }
}
