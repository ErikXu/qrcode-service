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

            var isGif = false;
            var tmpPath = string.Empty;
            if (file != null)
            {
                var ext = Path.GetExtension(file.FileName).ToLower();
                if (ext == ".gif")
                {
                    isGif = true;
                }

                tmpPath = Path.Combine(outDir, isGif ? $"{Guid.NewGuid()}.gif" : $"{Guid.NewGuid()}.png");
                await using var stream = System.IO.File.Create(tmpPath);
                await file.CopyToAsync(stream);

                command = form.Colorized ? $"{command} -p {tmpPath} -c" : $"{command} -p {tmpPath}";

                if (form.Version == null)
                {
                    form.Version = 10;
                }
            }

            if (form.Version != null)
            {
                command = $"{command} -v {form.Version}";
            }

            var filename = isGif ? $"{Guid.NewGuid()}.gif" : $"{Guid.NewGuid()}.png";
            var filePath = Path.Combine(outDir, filename);

            command = $"{command} -n {filePath} -d {outDir} -con {form.Contrast} -bri {form.Brightness}";

            var (code, message) = ExecuteCommand(command);

            if (code != 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = message });
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            System.IO.File.Delete(filePath);

            if (string.IsNullOrWhiteSpace(tmpPath))
            {
                System.IO.File.Delete(tmpPath);
            }

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