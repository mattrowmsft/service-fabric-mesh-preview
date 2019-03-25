// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.ServiceFabricMesh.Fireworks.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    // send ping to counter, until cancellaed
    public partial class PingClient
    {
        private const string OBJECTCOUNTER_ADDRESS = "OBJECTCOUNTER_ADDRESS";
        private const string OBJECTCOUNTER_COUNT = "OBJECTCOUNTER_COUNT";

        private static readonly HttpClient Client;
        private static Random Rand;

        private readonly IEnumerable<string> objectCounterAddress;
        private readonly ObjectInfo objInfo;
        private readonly PingSettings pingSettings;
        private bool reportError;

        public PingClient()
            : this(
                  GetObjectCounterAddressesFromEnvironment(),
                  ObjectInfo.FromEnvironment(),
                  PingSettings.FromEnvironment())
        {
        }

        public PingClient(
            IEnumerable<string> objectCounterAddress,
            ObjectInfo objInfo,
            PingSettings pingSettings)
        {
            this.objectCounterAddress = objectCounterAddress;
            this.objInfo = objInfo;
            this.pingSettings = pingSettings;
            this.reportError = true;
        }

        static PingClient()
        {
            Rand = new Random();
            Client = new HttpClient();
        }

        private static IEnumerable<string> GetObjectCounterAddressesFromEnvironment()
        {
            string baseAddress = "web:8080";
            if (Environment.GetEnvironmentVariable(OBJECTCOUNTER_ADDRESS) != null)
            {
                baseAddress = Environment.GetEnvironmentVariable(OBJECTCOUNTER_ADDRESS);
            }

            int endpointCount = 1;
            if (Environment.GetEnvironmentVariable(OBJECTCOUNTER_COUNT) != null)
            {
                int.TryParse(Environment.GetEnvironmentVariable(OBJECTCOUNTER_COUNT), out endpointCount);
            }

            var host = baseAddress.Split(':')[0];
            var port = baseAddress.Split(':')[1];
            return Enumerable.Range(0, endpointCount).Select(idx => $"{host}--{idx}:{port}");
        }

        public async Task SendPingAsync(CancellationToken cancellationToken)
        {
            var pingTasks = new List<Task>();
            foreach (var address in this.objectCounterAddress)
            {
                pingTasks.Add(Task.Run(async () =>
                    {
                        var requestUri = new Uri($"http://{address}/api/values?id={this.objInfo.Id}&type={this.objInfo.Type}&version={this.objInfo.Version}");
                        Console.WriteLine($"{DateTime.UtcNow}: Sending ping to {requestUri}");
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            var success = await SendData(requestUri, cancellationToken);
                            await Task.Delay(GetDueTime(success), cancellationToken);
                        }
                    }));
            }

            await Task.WhenAll(pingTasks);
        }

        private async Task<bool> SendData(Uri requestUri, CancellationToken cancellationToken)
        {
            try
            {
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(string.Empty, Encoding.UTF8),
                    RequestUri = requestUri
                };

                request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("text/plain; charset=utf-8");

                var response = await Client.SendAsync(request, cancellationToken);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (this.reportError)
                    {
                        Console.WriteLine($"{DateTime.UtcNow}: Error in sending the data {response}");
                        this.reportError = false;
                    }
                }
                else
                {
                    this.reportError = true;
                    return true;
                }
            }
            catch (Exception e)
            {
                if (this.reportError)
                {
                    Console.WriteLine($"{DateTime.UtcNow}: Error in sending the data {e.ToString()}");
                    this.reportError = false;
                }
            }

            return false;
        }

        private TimeSpan GetDueTime(bool success)
        {
            if (success)
            {
                var dueTimeMillis =
                    Rand.Next(this.pingSettings.PingIntervalMillis) +
                    Rand.Next(this.pingSettings.PingFuzzIntervalMillis);
                return TimeSpan.FromMilliseconds(dueTimeMillis);
            }
            else
            {
                var dueTimeMillis =
                    Rand.Next(this.pingSettings.PingFailureRetryIntervalMillis) +
                    Rand.Next(this.pingSettings.PingFuzzIntervalMillis);
                return TimeSpan.FromMilliseconds(dueTimeMillis);
            }
        }
    }
}