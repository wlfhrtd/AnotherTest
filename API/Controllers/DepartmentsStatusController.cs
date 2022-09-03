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

            string bytesAsString = Encoding.UTF8.GetString(buffer);

            Dictionary<string, int> departmentsResponse =
                JsonConvert.DeserializeObject<Dictionary<string, int>>(bytesAsString);

            while (!result.CloseStatus.HasValue)
            {
                foreach (string key in departmentsResponse.Keys.ToArray())
                {
                    // inverse status ~ XOR; check Departments.Status
                    departmentsResponse[key] = 1 - departmentsResponse[key];
                }

                var json = System.Text.Json.JsonSerializer.Serialize(departmentsResponse);
                var jsonResponse = Encoding.UTF8.GetBytes(json);

                await webSocket.SendAsync(new ArraySegment<byte>(jsonResponse, 0, jsonResponse.Length),
                    result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

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
    }
}
