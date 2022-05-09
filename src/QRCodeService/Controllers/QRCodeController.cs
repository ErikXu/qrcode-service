using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
        public async Task<IActionResult> Generate([FromBody] QRCodeForm form)
        {
            var outDir = Directory.GetCurrentDirectory();

            var isUnix = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            if (isUnix)
            {
                outDir = "/tmp";
            }

            var filename = $"{Guid.NewGuid()}.png";

            var command = $"amzqr {form.Text} -l {form.Level}";

            if (form.Version != null)
            {
                command = $"{command} -v {form.Version}";
            }

            command = $"{command} -n {filename} -d {outDir}";

            var (code, message) = ExecuteCommand(command);

            if (code != 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = message });
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(Path.Combine(outDir, filename));
            System.IO.File.Delete(Path.Combine(outDir, filename));

            return File(bytes, "image/png", filename);
        }

        private (int, string) ExecuteCommand(string command)
        {
            var isUnix = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            var escapedArgs = command.Replace("\"", "\\\"");
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = isUnix ? "/bin/sh" : "powershell",
                    Arguments = isUnix ? $"-c \"{escapedArgs}\"" : command,
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