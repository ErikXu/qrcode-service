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
        /// 边长，范围是 1 至 40，数字越大边长越大
        /// </summary>
        [Range(1, 40)]
        public int? Version { get; set; } = null;

        /// <summary>
        /// 纠错水平，范围是 L, M, Q, H，从左到右依次升高，默认是 H
        /// </summary>
        [DefaultValue("H")]
        [LevelValidation]
        public string Level { get; set; } = "H";

        /// <summary>
        /// 是否生成彩色二维码，需上传文件
        /// </summary>
        public bool Colorized { get; set; } = false;
    }

    public class LevelValidationAttribute : ValidationAttribute
    {
        public string GetErrorMessage() => "Level value should be one of the values: L, M, Q, H";

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string level)
            {
                return new ValidationResult(GetErrorMessage());
            }

            level = level.ToUpper();
            if (level != "L" && level != "M" && level != "Q" && level != "H")
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }
    }
}