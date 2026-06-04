using System.Net;
using System.Text.Json;

namespace ApiMiniPrj.Mvc.Common
{
    public static class ApiErrorReader
    {
        public static async Task<string> ReadAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                return $"Request failed ({(int)response.StatusCode} {response.ReasonPhrase}).";
            }

            if (response.StatusCode == HttpStatusCode.UnsupportedMediaType)
            {
                return "Request format is not supported by the API.";
            }

            try
            {
                using var document = JsonDocument.Parse(content);
                var root = document.RootElement;

                if (root.ValueKind == JsonValueKind.String)
                {
                    return root.GetString() ?? content;
                }

                if (root.ValueKind != JsonValueKind.Object)
                {
                    return content;
                }

                var message = TryGetString(root, "message");
                var errors = ReadErrors(root);

                if (errors.Count > 0)
                {
                    return string.Join(Environment.NewLine, errors);
                }

                return string.IsNullOrWhiteSpace(message) ? content : message;
            }
            catch (JsonException)
            {
                return content;
            }
        }

        private static List<string> ReadErrors(JsonElement root)
        {
            var errors = new List<string>();
            if (!root.TryGetProperty("errors", out var errorsElement) || errorsElement.ValueKind == JsonValueKind.Null)
            {
                return errors;
            }

            if (errorsElement.ValueKind == JsonValueKind.String)
            {
                AddIfNotEmpty(errors, errorsElement.GetString());
                return errors;
            }

            if (errorsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in errorsElement.EnumerateArray())
                {
                    AddJsonValue(errors, item);
                }

                return errors;
            }

            if (errorsElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in errorsElement.EnumerateObject())
                {
                    AddJsonValue(errors, property.Value);
                }
            }

            return errors;
        }

        private static void AddJsonValue(List<string> errors, JsonElement value)
        {
            if (value.ValueKind == JsonValueKind.String)
            {
                AddIfNotEmpty(errors, value.GetString());
                return;
            }

            if (value.ValueKind != JsonValueKind.Array)
            {
                return;
            }

            foreach (var item in value.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String)
                {
                    AddIfNotEmpty(errors, item.GetString());
                }
            }
        }

        private static string? TryGetString(JsonElement root, string propertyName)
        {
            return root.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String
                ? value.GetString()
                : null;
        }

        private static void AddIfNotEmpty(List<string> errors, string? error)
        {
            if (!string.IsNullOrWhiteSpace(error))
            {
                errors.Add(error);
            }
        }
    }
}
