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
        /// �߳�����Χ��1��40������Խ��߳�Խ��
        /// </summary>
        [Range(1, 40)]
        [DefaultValue(null)]
        public int? Version { get; set; } = null;

        /// <summary>
        /// ����ˮƽ����Χ��L��M��Q��H���������������ߣ�Ĭ���� H
        /// </summary>
        [DefaultValue("H")]
        public string Level { get; set; } = "H";
    }
}