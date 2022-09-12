using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using System;
using System.Collections.Generic;
using Domain.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Buffers;
using Newtonsoft.Json.Serialization;

namespace API.Controllers
{
    public class DepartmentsStatusController : ControllerBase
    {
        [HttpGet("/ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                await Echo(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private async Task Echo(WebSocket webSocket)
        {
            // OnOpen(); current receive/transmit json bytes array length is < 512
            using IMemoryOwner<byte> receiveBuffer = MemoryPool<byte>.Shared.Rent(1024); // stack allocation

            ValueWebSocketReceiveResult request =
                await webSocket.ReceiveAsync(receiveBuffer.Memory, CancellationToken.None);

            var departmentsResponse = Deserialize<Dictionary<string, int>>(
                receiveBuffer.Memory.Slice(0, request.Count).ToArray());

            int iterations = 1000;
            int counter = 0;
            long[] resultsMillis = new long[iterations];
            var stopWatch = Stopwatch.StartNew();

            while (webSocket.State == WebSocketState.Open && counter != iterations)
            {
                InverseStatus(ref departmentsResponse);

                await OnMessage(webSocket, receiveBuffer, request, departmentsResponse);

                resultsMillis[counter] = stopWatch.ElapsedMilliseconds;
                counter++;
                stopWatch.Restart();
            }

            // forced closure for benchmarking
            await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

            // benchmark
            string newtonsoft = "Newtonsoft.Json";
            string systemTextJson = "System.Text.Json";
            string memoryOptimizations = "Memory Optimizations";

            StringBuilder sb = new(8192);

            for (int i = 0; i < iterations; i++)
                sb.AppendLine($"Elapsed milliseconds with {memoryOptimizations}: {resultsMillis[i]}");

            sb.AppendLine(Environment.NewLine + "******** STATS ********");
            sb.AppendLine($"AVG: {resultsMillis.Average()}");
            sb.AppendLine($"MIN: {resultsMillis.Min()}");
            sb.AppendLine($"MAX: {resultsMillis.Max()}");
            sb.AppendLine($"SUM (in seconds): {resultsMillis.Sum() / 1000L}");

            string folder = "BenchmarksResults";
            Directory.CreateDirectory(folder);

            // string filename = "Newtonsoft.Json_results.txt";
            string filename = "AfterMemoryOptimizations_results_2.txt";
            string filepath = folder + Path.DirectorySeparatorChar + filename;

            using StreamWriter streamWriter = new(filepath);
            await streamWriter.WriteLineAsync(sb.ToString());

            // regular
            // await OnClose(webSocket);
        }

        private static async Task OnMessage(WebSocket webSocket, IMemoryOwner<byte> receiveBuffer,
                                            ValueWebSocketReceiveResult request,
                                            Dictionary<string, int> departmentsResponse)
        {
            using MemoryStream memoryStream = new();
            Serialize(departmentsResponse, memoryStream);

            await webSocket.SendAsync(memoryStream.ToArray(), request.MessageType,
                                      request.EndOfMessage, CancellationToken.None);

            await webSocket.ReceiveAsync(receiveBuffer.Memory, CancellationToken.None);
        }

        private static async Task OnClose(WebSocket webSocket)
        {
            await webSocket.CloseAsync(
                            webSocket.CloseStatus.Value, webSocket.CloseStatusDescription, CancellationToken.None);
        }

        private static void InverseStatus(ref Dictionary<string, int> departmentsResponse)
        {
            foreach (string key in departmentsResponse.Keys.ToArray())
            {
                // inverse status ~ XOR; check Departments.Status
                departmentsResponse[key] = 1 - departmentsResponse[key];
            }
        }

        private static void Serialize<T>(T obj, Stream stream)
        {
            // true => leave open for underlying stream (MemoryStream in this case) when disposing StreamWriter
            using (StreamWriter streamWriter = new(stream, Encoding.UTF8, 1024, true)) 
            using (JsonTextWriter jsonWriter = new(streamWriter))
            {
                var serializer = new JsonSerializer();
                // serializer.Formatting = Formatting.Indented;
                serializer.Serialize(jsonWriter, obj);
            }
        }

        private static T Deserialize<T>(byte[] data) where T : class
        {
            using (MemoryStream memoryStream = new(data))
            using (StreamReader streamReader = new(memoryStream, Encoding.UTF8))

                return JsonSerializer.Create().Deserialize(streamReader, typeof(T)) as T;
        }

        #region BackgroundSocketProcessor from Microsoft docs
        //public static void AcceptWebSocketAsyncBackgroundSocketProcessor(WebApplication app)
        //{
        //    app.Run(async (context) =>
        //    {
        //        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        //        var socketFinishedTcs = new TaskCompletionSource<object>();

        //        BackgroundSocketProcessor.AddSocket(webSocket, socketFinishedTcs);

        //        await socketFinishedTcs.Task;
        //    });
        //}

        //internal class BackgroundSocketProcessor
        //{
        //    internal static void AddSocket(WebSocket webSocket, TaskCompletionSource<object> socketFinishedTcs)
        //    {

        //    }
        //}
        #endregion
    }
}
