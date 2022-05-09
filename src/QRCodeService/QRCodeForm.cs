using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QRCodeService
{
    public class QRCodeForm
    {
        /// <summary>
        /// 需要生成的二维码内容
        /// </summary>
        [Required]
        public string? Text { get; set; }

        /// <summary>
        /// 边长，范围是1至40，数字越大边长越大
        /// </summary>
        [Range(1, 40)]
        [DefaultValue(null)]
        public int? Version { get; set; } = null;

        /// <summary>
        /// 纠错水平，范围是L、M、Q、H，从左到右依次升高，默认是 H
        /// </summary>
        [DefaultValue("H")]
        public string Level { get; set; } = "H";
    }
}