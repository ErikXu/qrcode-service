using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QRCodeService
{
    public class QRCodeForm
    {
        /// <summary>
        /// ��Ҫ���ɵĶ�ά������
        /// </summary>
        [Required]
        public string? Text { get; set; }

        /// <summary>
        /// �߳�����Χ�� 1 �� 40������Խ��߳�Խ��
        /// </summary>
        [Range(1, 40)]
        public int? Version { get; set; } = null;

        /// <summary>
        /// ����ˮƽ����Χ�� L, M, Q, H���������������ߣ�Ĭ���� H
        /// </summary>
        [DefaultValue("H")]
        [LevelValidation]
        public string Level { get; set; } = "H";

        /// <summary>
        /// �Ƿ����ɲ�ɫ��ά�룬���ϴ��ļ�
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