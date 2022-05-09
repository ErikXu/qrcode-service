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
        public async Task<IActionResult> Generate([FromForm] QRCodeForm form, IFormFile? file)
        {
            var outDir = Directory.GetCurrentDirectory();
            var isUnix = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            if (isUnix)
            {
                outDir = "/tmp";
            }

            var command = $"amzqr {form.Text} -l {form.Level.ToUpper()}";
            if (form.Version != null)
            {
                command = $"{command} -v {form.Version}";
            }

            var isGif = false;

            if (file != null)
            {
                var ext = Path.GetExtension(file.FileName).ToLower();
                if (ext == ".gif")
                {
                    isGif = true;
                }

                var tmpPath = Path.Combine(outDir, isGif ? $"{Guid.NewGuid()}.gif" : $"{Guid.NewGuid()}.png");
                await using var stream = System.IO.File.Create(tmpPath);
                await file.CopyToAsync(stream);
           
                command = form.Colorized ? $"{command} -p {tmpPath} -c" : $"{command} -p {tmpPath}";
            }

            var filename = Path.Combine(outDir, isGif ? $"{Guid.NewGuid()}.gif" : $"{Guid.NewGuid()}.png");

            command = $"{command} -n {filename} -d {outDir}";

            var (code, message) = ExecuteCommand(command);

            if (code != 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = message });
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(Path.Combine(outDir, filename));
            System.IO.File.Delete(Path.Combine(outDir, filename));

            var contentType = isGif ? "image/gif" : "image/png";
            return File(bytes, contentType, filename);
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