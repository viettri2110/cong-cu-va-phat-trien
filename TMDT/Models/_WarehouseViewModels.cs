//------------------------------------------------------------------------------
// ViewModel cho Warehouse Management
// Created: 2025-11-15
//------------------------------------------------------------------------------

namespace TMDT.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    // ViewModel cho báo cáo tồn kho
    public class InventoryReportViewModel
    {
        public int idProduct { get; set; }
        public string nameProduct { get; set; }
        public string imageProduct_1 { get; set; }
        public string nameCategory { get; set; }
        public string categoryName => nameCategory; // Alias for view compatibility
        public decimal priceProduct { get; set; }
        public int currentStock { get; set; }
        public DateTime? lastUpdated { get; set; }
        public string stockStatus { get; set; }
    }

    // ViewModel cho lịch sử nhập xuất
    public class InventoryHistoryViewModel
    {
        public int idHistory { get; set; }
        public DateTime changeDate { get; set; }
        public string changeType { get; set; }
        public int quantityBefore { get; set; }
        public int quantityChange { get; set; }
        public int quantityAfter { get; set; }
        public string referenceType { get; set; }
        public int? referenceId { get; set; }
        public string referenceCode { get; set; }
        public string notes { get; set; }
    }

    // ViewModel cho phiếu nhập kho đầy đủ
    public class FullWarehouseReceipt
    {
        public int idReceipt { get; set; }
        public string receiptCode { get; set; }
        public DateTime receiptDate { get; set; }
        public string receiptType { get; set; }
        public string supplierInfo { get; set; }
        public int totalQuantity { get; set; }
        public string status { get; set; }
        public string notes { get; set; }
        public string creatorName { get; set; }
        public List<WarehouseReceiptDetailViewModel> Details { get; set; }
    }

    // ViewModel cho chi tiết phiếu nhập
    public class WarehouseReceiptDetailViewModel
    {
        public int idProduct { get; set; }
        public string nameProduct { get; set; }
        public string imageProduct_1 { get; set; }
        public int quantity { get; set; }
        public string condition { get; set; }
        public string notes { get; set; }
    }

    // ViewModel cho phiếu xuất kho đầy đủ
    public class FullWarehouseIssue
    {
        public int idIssue { get; set; }
        public string issueCode { get; set; }
        public DateTime issueDate { get; set; }
        public string issueType { get; set; }
        public int? idOrder { get; set; }
        public string buyerName { get; set; }
        public int totalQuantity { get; set; }
        public string status { get; set; }
        public string reason { get; set; }
        public List<WarehouseIssueDetailViewModel> Details { get; set; }
    }

    // ViewModel cho chi tiết phiếu xuất
    public class WarehouseIssueDetailViewModel
    {
        public int idProduct { get; set; }
        public string nameProduct { get; set; }
        public string imageProduct_1 { get; set; }
        public int quantity { get; set; }
        public string notes { get; set; }
    }

    // ViewModel cho form nhập hàng
    public class WarehouseReceiptViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn sản phẩm")]
        public int idProduct { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại nhập")]
        public string receiptType { get; set; }

        [StringLength(200, ErrorMessage = "Thông tin nguồn hàng tối đa 200 ký tự")]
        public string supplierInfo { get; set; }

        [StringLength(50, ErrorMessage = "Tình trạng tối đa 50 ký tự")]
        public string condition { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        public string notes { get; set; }
    }

    // ViewModel cho kiểm kê
    public class StocktakeViewModel
    {
        public int idStocktake { get; set; }
        public string stocktakeCode { get; set; }
        public DateTime stocktakeDate { get; set; }
        public string status { get; set; }
        public int totalProductsChecked { get; set; }
        public int totalDiscrepancy { get; set; }
        public string notes { get; set; }
        public DateTime? completedDate { get; set; }
        public List<StocktakeDetailViewModel> Details { get; set; }
    }

    // ViewModel cho chi tiết kiểm kê
    public class StocktakeDetailViewModel
    {
        public int idStocktakeDetail { get; set; }
        public int idProduct { get; set; }
        public string nameProduct { get; set; }
        public string imageProduct_1 { get; set; }
        public int systemQuantity { get; set; }
        public int actualQuantity { get; set; }
        public int discrepancy { get; set; }
        public string reason { get; set; }
        public bool isAdjusted { get; set; }
        public string notes { get; set; }
    }

    // ViewModel cho danh sách phiếu nhập
    public class WarehouseReceiptListViewModel
    {
        public int idReceipt { get; set; }
        public string receiptCode { get; set; }
        public DateTime receiptDate { get; set; }
        public string receiptType { get; set; }
        public string supplierInfo { get; set; }
        public int totalQuantity { get; set; }
        public string status { get; set; }
    }

    // ViewModel cho danh sách phiếu xuất
    public class WarehouseIssueListViewModel
    {
        public int idIssue { get; set; }
        public string issueCode { get; set; }
        public DateTime issueDate { get; set; }
        public string issueType { get; set; }
        public int? idOrder { get; set; }
        public string buyerName { get; set; }
        public int totalQuantity { get; set; }
        public string status { get; set; }
    }

    // ViewModel cho danh sách kiểm kê
    public class StocktakeListViewModel
    {
        public int idStocktake { get; set; }
        public string stocktakeCode { get; set; }
        public DateTime stocktakeDate { get; set; }
        public string status { get; set; }
        public int totalProductsChecked { get; set; }
        public int totalDiscrepancy { get; set; }
        public DateTime? completedDate { get; set; }
    }

    // ViewModel cho lịch sử sản phẩm
    public class ProductInventoryHistoryViewModel
    {
        public InventoryReportViewModel ProductInfo { get; set; }
        public List<InventoryHistoryViewModel> HistoryList { get; set; }
    }

    // ViewModel cho dashboard kho
    public class WarehouseDashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalInStock { get; set; }
        public int OutOfStockCount { get; set; }
        public int LowStockCount { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int ReceiptsThisMonth { get; set; }
        public int IssuesThisMonth { get; set; }
        public List<WarehouseReceiptListViewModel> RecentReceipts { get; set; }
        public List<WarehouseIssueListViewModel> RecentIssues { get; set; }
        public List<InventoryReportViewModel> AlertProducts { get; set; }
    }
}
