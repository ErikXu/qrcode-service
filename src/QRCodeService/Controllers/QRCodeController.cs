using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace QRCodeService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QRCodeController : ControllerBase
    {
        /// <summary>
        /// Generate QRCode
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Generate([FromBody] QRCodeForm form)
        {
            var filename = Guid.NewGuid().ToString();
            var command = $"amzqr {form.Text} -d {filename}.txt";
            var (code, message) = ExecuteCommand(command);

            if (code != 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = message });
            }

            return Ok();
        }

        private (int, string) ExecuteCommand(string command)
        {
            var escapedArgs = command.Replace("\"", "\\\"");
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/sh",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();

            var message = process.StandardOutput.ReadToEnd();
            if (process.ExitCode != 0)
            {
                message = process.StandardError.ReadToEnd();
            }

            return (process.ExitCode, message);
        }
    }
}