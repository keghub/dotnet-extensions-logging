using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EMG.Extensions.Logging.Loggly
{
    public interface ILogglyClient
    {
        Task PublishAsync(LogglyMessage message);
    }

    public class LogglyHttpClient : ILogglyClient
    {
        private readonly HttpClient _http;
        private readonly LogglyOptions _options;

        public LogglyHttpClient(HttpClient http, LogglyOptions options)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task PublishAsync(LogglyMessage message)
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, string.Empty))
                {
                    var content = JsonConvert.SerializeObject(FixData(message), SerializerSettings);
                    request.Content = new StringContent(content, _options.ContentEncoding, "application/json");

                    using (var response = await _http.SendAsync(request).ConfigureAwait(false))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            var state = new
                            {
                                response.StatusCode,
                                response.ReasonPhrase,
                                EndpointUrl = response.RequestMessage.RequestUri,
                                Payload = message
                            };
                        }
                    }
                }
            }
            catch (Exception) when (_options.SuppressExceptions)
            {

            }
        }

        private static LogglyMessage FixData(LogglyMessage message)
        {
            if (message.Data is IEnumerable<KeyValuePair<string, object>> values)
            {
                var newData = values.ToDictionary(k => k.Key, v => v.Value);
                return CloneLogglyEvent(message, newData);
            }

            return message;
        }

        private static LogglyMessage CloneLogglyEvent(LogglyMessage logglyMessage, object newData)
        {
            return new LogglyMessage
            {
                ApplicationName = logglyMessage.ApplicationName,
                Category = logglyMessage.Category,
                Data = newData,
                Error = logglyMessage.Error,
                Event = logglyMessage.Event,
                Level = logglyMessage.Level,
                MachineName = logglyMessage.MachineName,
                Message = logglyMessage.Message
            };
        }

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
        };
    }
}