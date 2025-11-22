using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TMDT.Models
{
    public class CODOrderInfo
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên người nhận")]
        [Display(Name = "Họ tên người nhận")]
        [StringLength(100, ErrorMessage = "Họ tên không quá 100 ký tự")]
        public string ReceiverName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        [RegularExpression(@"^0[0-9]{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string ReceiverPhone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng")]
        [Display(Name = "Địa chỉ nhận hàng")]
        [StringLength(500, ErrorMessage = "Địa chỉ không quá 500 ký tự")]
        public string DeliveryAddress { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(500, ErrorMessage = "Ghi chú không quá 500 ký tự")]
        public string Note { get; set; }

        // Danh sách sản phẩm trong giỏ hàng
        public List<_GioHang> CartItems { get; set; }

        // Tổng tiền
        public double TotalAmount { get; set; }
    }
}
