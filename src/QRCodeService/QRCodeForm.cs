using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QRCodeService
{
    public class QRCodeForm
    {
        /// <summary>
        /// Content to gen to qrcode
        /// </summary>
        [Required]
        public string? Text { get; set; }

        /// <summary>
        /// Length of the qrcode image range 1 to 40
        /// </summary>
        [Range(1, 40)]
        public int? Version { get; set; } = null;

        /// <summary>
        /// Error correction level, is one of L, M, Q and H, default H
        /// </summary>
        [DefaultValue("H")]
        [LevelValidation]
        public string Level { get; set; } = "H";

        /// <summary>
        /// Is qrcode image colorized
        /// </summary>
        public bool Colorized { get; set; } = false;

        /// <summary>
        /// The contrast of the qrcode image, defaule 1.0
        /// </summary>
        [DefaultValue(1.0)]
        public double Contrast { get; set; } = 1.0;

        /// <summary>
        /// The brightness of the qrcode image, defaule 1.0
        /// </summary>
        [DefaultValue(1.0)]
        public double Brightness { get; set; } = 1.0;
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