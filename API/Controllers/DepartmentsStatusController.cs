using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Net;
using System.Text.Json;
using System.Text;
using System;
using System.Collections.Generic;
using Domain.Models;
using Newtonsoft.Json;

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

            var bytesAsString = Encoding.UTF8.GetString(buffer);
            Console.WriteLine(bytesAsString);
            string[] departmentNames = JsonConvert.DeserializeObject<string[]>(bytesAsString);
            foreach (var departmentName in departmentNames)
            {
                Console.WriteLine(departmentName);
            }
            
            while (!result.CloseStatus.HasValue)
            {
                //await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count),
                //                          result.MessageType, result.EndOfMessage, CancellationToken.None);

                var status = new[] { "Active", "Blocked" };

                var randomStatus = Enumerable.Range(1, departmentNames.Length)
                    .Select(index => status[Random.Shared.Next(status.Length)]).ToArray();

                Dictionary<string, string> response = new();

                for (int i = 0; i < departmentNames.Length; i++)
                {
                    response.Add(departmentNames[i], randomStatus[i]);
                }

                var json = System.Text.Json.JsonSerializer.Serialize(response);
                var jsonResponse = Encoding.UTF8.GetBytes(json);

                await webSocket.SendAsync(new ArraySegment<byte>(jsonResponse, 0, jsonResponse.Length),
                    result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        public static void AcceptWebSocketAsyncBackgroundSocketProcessor(WebApplication app)
        {
            // <snippet_AcceptWebSocketAsyncBackgroundSocketProcessor>
            app.Run(async (context) =>
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var socketFinishedTcs = new TaskCompletionSource<object>();

                BackgroundSocketProcessor.AddSocket(webSocket, socketFinishedTcs);

                await socketFinishedTcs.Task;
            });
            // </snippet_AcceptWebSocketAsyncBackgroundSocketProcessor>
        }

        internal class BackgroundSocketProcessor
        {
            internal static void AddSocket(WebSocket webSocket, TaskCompletionSource<object> socketFinishedTcs) { }
        }
    }
}
