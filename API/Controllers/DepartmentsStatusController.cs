using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Net;
using System.Text.Json;
using System.Text;
using System;
using System.Collections.Generic;
using Domain.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Buffers;

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
            var buffer = new byte[1024 * 4];

            WebSocketReceiveResult result =
                await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            string bytesAsString = Encoding.UTF8.GetString(buffer);

            Dictionary<string, int> departmentsResponse =
                JsonConvert.DeserializeObject<Dictionary<string, int>>(bytesAsString);

            int iterations = 1000;
            int counter = 0;           
            long[] resultsMillis = new long[iterations];
            var stopWatch = Stopwatch.StartNew();

            while (!result.CloseStatus.HasValue && counter != iterations)
            {
                InverseStatus(ref departmentsResponse);

                // var json = JsonConvert.SerializeObject(departmentsResponse);
                // var jsonResponse = Encoding.UTF8.GetBytes(json);

                var json = System.Text.Json.JsonSerializer.Serialize(departmentsResponse);
                var jsonResponse = Encoding.UTF8.GetBytes(json);

                await webSocket.SendAsync(new ArraySegment<byte>(jsonResponse, 0, jsonResponse.Length),
                    result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                resultsMillis[counter] = stopWatch.ElapsedMilliseconds;
                counter++;
                stopWatch.Restart();
            }
            // forced closure for benchmarking
            await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

            // benchmark
            string newtonsoft = "Newtonsoft.Json";
            string systemTextJson = "System.Text.Json";

            StringBuilder sb = new(8192);

            for (int i = 0; i < iterations; i++)
                // sb.AppendLine($"Elapsed milliseconds with {newtonsoft}: {resultsMillis[i]}");
                sb.AppendLine($"Elapsed milliseconds with {systemTextJson}: {resultsMillis[i]}");

            sb.AppendLine(Environment.NewLine + "******** STATS ********");
            sb.AppendLine($"AVG: {resultsMillis.Average()}");
            sb.AppendLine($"MIN: {resultsMillis.Min()}");
            sb.AppendLine($"MAX: {resultsMillis.Max()}");
            sb.AppendLine($"SUM (in seconds): {resultsMillis.Sum() / 1000L}");

            string folder = "BenchmarksResults";
            Directory.CreateDirectory(folder);

            // string filename = "Newtonsoft.Json_results.txt";
            string filename = "System.Text.Json_results.txt";
            string filepath = folder + Path.DirectorySeparatorChar + filename;

            using StreamWriter streamWriter = new(filepath);
            await streamWriter.WriteLineAsync(sb.ToString());

            // normal procedure code
            //await webSocket.CloseAsync(
            //    result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        private static void InverseStatus(ref Dictionary<string, int> departmentsResponse)
        {
            foreach (string key in departmentsResponse.Keys.ToArray())
            {
                // inverse status ~ XOR; check Departments.Status
                departmentsResponse[key] = 1 - departmentsResponse[key];
            }
        }

        #region new
        //private async Task Echo(WebSocket webSocket)
        //{
        //    //var buffer = new byte[1024 * 4];

        //    //WebSocketReceiveResult result =
        //    //    await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        //    //string bytesAsString = Encoding.UTF8.GetString(buffer);
        //    using IMemoryOwner<byte> memory = MemoryPool<byte>.Shared.Rent(1024 * 4);
        //    ValueWebSocketReceiveResult result = await webSocket.ReceiveAsync(memory.Memory, CancellationToken.None);
        //    string msg = Encoding.UTF8.GetString(memory.Memory.Span);
        //    //Dictionary<string, int> departmentsResponse =
        //    //    JsonConvert.DeserializeObject<Dictionary<string, int>>(bytesAsString);

        //    JsonDocument jsonDocument = JsonDocument.Parse(memory.Memory.Slice(0, result.Count));

        //    Dictionary<string, int> departmentsResponse =
        //        System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(jsonDocument);

        //    while (!webSocket.CloseStatus.HasValue)
        //    {
        //        foreach (string key in departmentsResponse.Keys.ToArray())
        //        {
        //            // inverse status ~ XOR; check Departments.Status
        //            departmentsResponse[key] = 1 - departmentsResponse[key];
        //        }

        //        //var json = System.Text.Json.JsonSerializer.Serialize(departmentsResponse);
        //        //var jsonResponseAsBytes = Encoding.UTF8.GetBytes(json);

        //        var jsonResponseAsBytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(departmentsResponse);

        //        await webSocket.SendAsync(new ArraySegment<byte>(jsonResponseAsBytes, 0, jsonResponseAsBytes.Length),
        //            result.MessageType, result.EndOfMessage, CancellationToken.None);

        //        result = await webSocket.ReceiveAsync(memory.Memory, CancellationToken.None);
        //    }

        //    await webSocket.CloseAsync(
        //        webSocket.CloseStatus.Value, webSocket.CloseStatusDescription, CancellationToken.None);
        //}
        #endregion

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
