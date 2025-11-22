using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.Models;

namespace TMDT.Controllers
{
    public class WarehouseController : Controller
    {
        ChoDoCuEntities db = new ChoDoCuEntities();

        // =============================================
        // QUẢN LÝ KHO CHO SELLER
        // =============================================

        /// <summary>
        /// Dashboard quản lý kho - Hiển thị tổng quan
        /// </summary>
        [HttpGet]
        public ActionResult DashboardKho()
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 1)) return HttpNotFound();

            int idSeller = Int32.Parse(Session["idAccount"].ToString());

            // Lấy thống kê
            var dashboard = new WarehouseDashboardViewModel
            {
                TotalProducts = db.Products.Count(p => p.idAccount == idSeller && !p.hideProduct),
                TotalInStock = db.Products.Count(p => p.idAccount == idSeller && !p.hideProduct && p.amountProduct > 5),
                OutOfStockCount = db.Products.Count(p => p.idAccount == idSeller && !p.hideProduct && p.amountProduct == 0),
                LowStockCount = db.Products.Count(p => p.idAccount == idSeller && !p.hideProduct && p.amountProduct > 0 && p.amountProduct <= 5),
                TotalInventoryValue = db.Products.Where(p => p.idAccount == idSeller && !p.hideProduct).Sum(p => (decimal?)p.amountProduct * p.priceProduct) ?? 0,
                ReceiptsThisMonth = db.Database.SqlQuery<int>("SELECT COUNT(*) FROM WarehouseReceipt WHERE idSeller = @p0 AND MONTH(receiptDate) = MONTH(GETDATE()) AND YEAR(receiptDate) = YEAR(GETDATE())", idSeller).FirstOrDefault(),
                IssuesThisMonth = db.Database.SqlQuery<int>("SELECT COUNT(*) FROM WarehouseIssue WHERE idSeller = @p0 AND MONTH(issueDate) = MONTH(GETDATE()) AND YEAR(issueDate) = YEAR(GETDATE())", idSeller).FirstOrDefault()
            };

            // Sản phẩm cần chú ý (sắp hết + hết hàng)
            dashboard.AlertProducts = GetInventoryReport(idSeller)
                .Where(p => p.stockStatus == "Sắp hết" || p.stockStatus == "Hết hàng")
                .Take(10).ToList();
            
            // Phiếu nhập gần đây (5 phiếu mới nhất)
            dashboard.RecentReceipts = db.Database.SqlQuery<WarehouseReceiptListViewModel>(@"
                SELECT TOP 5 idReceipt, receiptCode, receiptDate, receiptType, totalQuantity, status
                FROM WarehouseReceipt
                WHERE idSeller = @p0
                ORDER BY receiptDate DESC
            ", idSeller).ToList();

            // Phiếu xuất gần đây (5 phiếu mới nhất)
            dashboard.RecentIssues = db.Database.SqlQuery<WarehouseIssueListViewModel>(@"
                SELECT TOP 5 idIssue, issueCode, issueDate, issueType, totalQuantity, status
                FROM WarehouseIssue
                WHERE idSeller = @p0
                ORDER BY issueDate DESC
            ", idSeller).ToList();

            return View(dashboard);
        }

        /// <summary>
        /// Báo cáo tồn kho - Hiển thị tất cả sản phẩm
        /// </summary>
        [HttpGet]
        public ActionResult BaoCaoTonKho(string filterStatus = "all", int? idCategory = null)
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 1)) return HttpNotFound();

            int idSeller = Int32.Parse(Session["idAccount"].ToString());
            
            var inventoryList = GetInventoryReport(idSeller);

            // Lọc theo trạng thái
            if (filterStatus != "all")
            {
                inventoryList = inventoryList.Where(p => p.stockStatus == filterStatus).ToList();
            }

            // Lọc theo danh mục
            if (idCategory.HasValue && idCategory.Value > 0)
            {
                var categoryName = db.Category_Product.Find(idCategory.Value)?.nameCategory;
                inventoryList = inventoryList.Where(p => p.categoryName == categoryName).ToList();
            }

            // Load danh mục cho dropdown
            ViewBag.Categories = db.Category_Product.ToList();
            ViewBag.FilterStatus = filterStatus;
            ViewBag.IdCategory = idCategory;

            return View(inventoryList);
        }

        /// <summary>
        /// Lịch sử nhập xuất của 1 sản phẩm
        /// </summary>
        [HttpGet]
        public ActionResult LichSuNhapXuat(int? idProduct)
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 1)) return HttpNotFound();

            int idSeller = Int32.Parse(Session["idAccount"].ToString());

            // Nếu không có idProduct, hiển thị danh sách sản phẩm để chọn
            if (!idProduct.HasValue)
            {
                var products = db.Database.SqlQuery<InventoryReportViewModel>(@"
                    EXEC SP_GetInventoryReportBySeller @idSeller = @p0
                ", idSeller).ToList();

                ViewBag.Message = "Chọn sản phẩm để xem lịch sử nhập xuất";
                return View("BaoCaoTonKho", products);
            }

            // Kiểm tra sản phẩm có thuộc seller không
            var product = db.Products.Find(idProduct.Value);
            if (product == null || product.idAccount != idSeller)
            {
                return HttpNotFound();
            }

            // Tạo ProductInventoryHistoryViewModel
            var productInfo = db.Database.SqlQuery<InventoryReportViewModel>(@"
                EXEC SP_GetInventoryReportBySeller @idSeller = @p0
            ", idSeller).FirstOrDefault(p => p.idProduct == idProduct.Value);

            var historyList = GetProductInventoryHistory(idProduct.Value, idSeller);

            var viewModel = new ProductInventoryHistoryViewModel
            {
                ProductInfo = productInfo,
                HistoryList = historyList
            };

            return View(viewModel);
        }

        // =============================================
        // PHIẾU NHẬP KHO
        // =============================================

        /// <summary>
        /// Danh sách phiếu nhập kho
        /// </summary>
        [HttpGet]
        public ActionResult DanhSachPhieuNhap(DateTime? startDate = null, DateTime? endDate = null)
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 1)) return HttpNotFound();

            int idSeller = Int32.Parse(Session["idAccount"].ToString());

            if (!startDate.HasValue) startDate = DateTime.Now.AddMonths(-1);
            if (!endDate.HasValue) endDate = DateTime.Now;

            var receipts = db.Database.SqlQuery<WarehouseReceiptListViewModel>(@"
                SELECT 
                    wr.idReceipt,
                    wr.receiptCode,
                    wr.receiptDate,
                    wr.receiptType,
                    wr.supplierInfo,
                    wr.totalQuantity,
                    wr.status
                FROM WarehouseReceipt wr
                WHERE wr.idSeller = @p0
                AND wr.receiptDate BETWEEN @p1 AND @p2
                ORDER BY wr.receiptDate DESC
            ", idSeller, startDate, endDate).ToList();

            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");

            return View(receipts);
        }

        /// <summary>
        /// Chi tiết phiếu nhập kho
        /// </summary>
        [HttpGet]
        public ActionResult ChiTietPhieuNhap(int id)
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 1)) return HttpNotFound();

            int idSeller = Int32.Parse(Session["idAccount"].ToString());

            var receipt = db.Database.SqlQuery<FullWarehouseReceipt>(@"
                SELECT 
                    wr.idReceipt,
                    wr.receiptCode,
                    wr.receiptDate,
                    wr.receiptType,
                    wr.supplierInfo,
                    wr.totalQuantity,
                    wr.status,
                    wr.notes,
                    i.Fullname as creatorName
                FROM WarehouseReceipt wr
                INNER JOIN Login l ON wr.createdBy = l.idAccount
                LEFT JOIN infoAccount i ON l.idAccount = i.idAccount
                WHERE wr.idReceipt = @p0 AND wr.idSeller = @p1
            ", id, idSeller).FirstOrDefault();

            if (receipt == null) return HttpNotFound();

            // Lấy chi tiết
            receipt.Details = db.Database.SqlQuery<WarehouseReceiptDetailViewModel>(@"
                SELECT 
                    wrd.idProduct,
                    p.nameProduct,
                    p.imageProduct_1,
                    wrd.quantity,
                    wrd.condition,
                    wrd.notes
                FROM WarehouseReceiptDetail wrd
                INNER JOIN Product p ON wrd.idProduct = p.idProduct
                WHERE wrd.idReceipt = @p0
            ", id).ToList();

            return View(receipt);
        }

        /// <summary>
        /// Form nhập hàng vào kho
        /// </summary>
        [HttpGet]
        public ActionResult NhapHang()
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 1)) return HttpNotFound();

            int idSeller = Int32.Parse(Session["idAccount"].ToString());

            // Load danh sách sản phẩm của seller
            var products = db.Products
                .Where(p => p.idAccount == idSeller && !p.hideProduct && p.confirmProduct == true)
                .Select(p => new { p.idProduct, p.nameProduct })
                .ToList();

            ViewBag.Products = new SelectList(products, "idProduct", "nameProduct");

            return View();
        }

        /// <summary>
        /// Xử lý nhập hàng
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NhapHang(WarehouseReceiptViewModel model)
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 1)) return HttpNotFound();

            int idSeller = Int32.Parse(Session["idAccount"].ToString());

            if (!ModelState.IsValid)
            {
                LoadProductsForSeller(idSeller);
                return View(model);
            }

            try
            {
                // Gọi stored procedure để nhập hàng
                var result = db.Database.SqlQuery<WarehouseReceiptResult>(@"
                    EXEC SP_AddWarehouseReceipt 
                        @idProduct = @p0, 
                        @idSeller = @p1, 
                        @quantity = @p2, 
                        @receiptType = @p3, 
                        @supplierInfo = @p4, 
                        @condition = @p5, 
                        @notes = @p6
                ", 
                    model.idProduct, 
                    idSeller, 
                    model.quantity, 
                    model.receiptType ?? "Nhập bổ sung", 
                    model.supplierInfo, 
                    model.condition, 
                    model.notes
                ).FirstOrDefault();

                if (result != null)
                {
                    TempData["Success"] = "Nhập hàng thành công! Mã phiếu: " + result.receiptCode;
                }
                else
                {
                    TempData["Success"] = "Nhập hàng thành công!";
                }
                
                return RedirectToAction("DanhSachPhieuNhap");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi nhập hàng: " + ex.Message;
                LoadProductsForSeller(idSeller);
                return View(model);
            }
        }

        // Helper class để nhận kết quả từ SP
        public class WarehouseReceiptResult
        {
            public int idReceipt { get; set; }
            public string receiptCode { get; set; }
        }

        // =============================================
        // PHIẾU XUẤT KHO
        // =============================================

        /// <summary>
        /// Danh sách phiếu xuất kho
        /// </summary>
        [HttpGet]
        public ActionResult DanhSachPhieuXuat(DateTime? startDate = null, DateTime? endDate = null, string filterType = "all")
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 1)) return HttpNotFound();

            int idSeller = Int32.Parse(Session["idAccount"].ToString());

            if (!startDate.HasValue) startDate = DateTime.Now.AddMonths(-1);
            if (!endDate.HasValue) endDate = DateTime.Now;

            var query = @"
                SELECT 
                    wi.idIssue,
                    wi.issueCode,
                    wi.issueDate,
                    wi.issueType,
                    wi.idOrder,
                    i.Fullname as buyerName,
                    wi.totalQuantity,
                    wi.status
                FROM WarehouseIssue wi
                LEFT JOIN Login l ON wi.idBuyer = l.idAccount
                LEFT JOIN infoAccount i ON l.idAccount = i.idAccount
                WHERE wi.idSeller = @p0
                AND wi.issueDate BETWEEN @p1 AND @p2
            ";

            if (filterType != "all")
            {
                query += " AND wi.issueType = @p3";
            }

            query += " ORDER BY wi.issueDate DESC";

            var issues = filterType != "all" 
                ? db.Database.SqlQuery<WarehouseIssueListViewModel>(query, idSeller, startDate, endDate, filterType).ToList()
                : db.Database.SqlQuery<WarehouseIssueListViewModel>(query, idSeller, startDate, endDate).ToList();

            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
            ViewBag.FilterType = filterType;

            return View(issues);
        }

        /// <summary>
        /// Chi tiết phiếu xuất kho
        /// </summary>
        [HttpGet]
        public ActionResult ChiTietPhieuXuat(int id)
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 1)) return HttpNotFound();

            int idSeller = Int32.Parse(Session["idAccount"].ToString());

            var issue = db.Database.SqlQuery<FullWarehouseIssue>(@"
                SELECT 
                    wi.idIssue,
                    wi.issueCode,
                    wi.issueDate,
                    wi.issueType,
                    wi.idOrder,
                    i.Fullname as buyerName,
                    wi.totalQuantity,
                    wi.status,
                    wi.reason
                FROM WarehouseIssue wi
                LEFT JOIN Login l ON wi.idBuyer = l.idAccount
                LEFT JOIN infoAccount i ON l.idAccount = i.idAccount
                WHERE wi.idIssue = @p0 AND wi.idSeller = @p1
            ", id, idSeller).FirstOrDefault();

            if (issue == null) return HttpNotFound();

            // Lấy chi tiết
            issue.Details = db.Database.SqlQuery<WarehouseIssueDetailViewModel>(@"
                SELECT 
                    wid.idProduct,
                    p.nameProduct,
                    p.imageProduct_1,
                    wid.quantity,
                    wid.notes
                FROM WarehouseIssueDetail wid
                INNER JOIN Product p ON wid.idProduct = p.idProduct
                WHERE wid.idIssue = @p0
            ", id).ToList();

            return View(issue);
        }

        // =============================================
        // KIỂM KÊ KHO
        // =============================================

        /// <summary>
        /// Danh sách phiếu kiểm kê
        /// </summary>
        [HttpGet]
        public ActionResult DanhSachKiemKe()
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 1)) return HttpNotFound();

            int idSeller = Int32.Parse(Session["idAccount"].ToString());

            var stocktakes = db.Database.SqlQuery<StocktakeListViewModel>(@"
                SELECT 
                    s.idStocktake,
                    s.stocktakeCode,
                    s.stocktakeDate,
                    s.status,
                    s.totalProductsChecked,
                    s.totalDiscrepancy,
                    s.completedDate
                FROM Stocktake s
                WHERE s.idSeller = @p0
                ORDER BY s.stocktakeDate DESC
            ", idSeller).ToList();

            return View(stocktakes);
        }

        /// <summary>
        /// Tạo phiếu kiểm kê mới
        /// </summary>
        [HttpGet]
        public ActionResult TaoPhieuKiemKe()
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 1)) return HttpNotFound();

            int idSeller = Int32.Parse(Session["idAccount"].ToString());

            try
            {
                // Tạo mã phiếu kiểm kê: KK-{idSeller}-{YYMMDD}
                string dateStr = DateTime.Now.ToString("yyMMdd");
                string stocktakeCode = $"KK-{idSeller}-{dateStr}";

                // Kiểm tra xem đã có phiếu kiểm kê hôm nay chưa
                var existingStocktake = db.Database.SqlQuery<int>(@"
                    SELECT COUNT(*) 
                    FROM Stocktake 
                    WHERE idSeller = @p0 
                    AND stocktakeCode LIKE @p1
                    AND status = 'InProgress'
                ", idSeller, stocktakeCode + "%").FirstOrDefault();

                if (existingStocktake > 0)
                {
                    TempData["Error"] = "Bạn đang có phiếu kiểm kê chưa hoàn thành!";
                    return RedirectToAction("DanhSachKiemKe");
                }

                // Thêm số thứ tự nếu cần
                int seq = db.Database.SqlQuery<int>(@"
                    SELECT ISNULL(MAX(CAST(RIGHT(stocktakeCode, 1) AS INT)), 0) + 1
                    FROM Stocktake
                    WHERE idSeller = @p0 
                    AND stocktakeCode LIKE @p1
                ", idSeller, stocktakeCode + "%").FirstOrDefault();

                if (seq > 1) stocktakeCode += "-" + seq;

                // Tạo phiếu kiểm kê
                db.Database.ExecuteSqlCommand(@"
                    INSERT INTO Stocktake (idSeller, stocktakeCode, stocktakeDate, status, totalProductsChecked, totalDiscrepancy, createdBy)
                    VALUES (@p0, @p1, GETDATE(), 'InProgress', 0, 0, @p2)
                ", idSeller, stocktakeCode, idSeller);

                var idStocktake = db.Database.SqlQuery<int>("SELECT SCOPE_IDENTITY()").FirstOrDefault();

                // Lấy tất cả sản phẩm của seller và tạo chi tiết kiểm kê
                var products = db.Products
                    .Where(p => p.idAccount == idSeller && !p.hideProduct && p.confirmProduct == true)
                    .ToList();

                foreach (var product in products)
                {
                    db.Database.ExecuteSqlCommand(@"
                        INSERT INTO StocktakeDetail (idStocktake, idProduct, systemQuantity, actualQuantity, isAdjusted)
                        VALUES (@p0, @p1, @p2, 0, 0)
                    ", idStocktake, product.idProduct, product.amountProduct);
                }

                TempData["Success"] = "Tạo phiếu kiểm kê thành công! Mã phiếu: " + stocktakeCode;
                return RedirectToAction("ChiTietKiemKe", new { id = idStocktake });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tạo phiếu kiểm kê: " + ex.Message;
                return RedirectToAction("DanhSachKiemKe");
            }
        }

        /// <summary>
        /// Chi tiết phiếu kiểm kê và nhập số lượng thực tế
        /// </summary>
        [HttpGet]
        public ActionResult ChiTietKiemKe(int id)
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 1)) return HttpNotFound();

            int idSeller = Int32.Parse(Session["idAccount"].ToString());

            var stocktake = db.Database.SqlQuery<StocktakeViewModel>(@"
                SELECT 
                    s.idStocktake,
                    s.stocktakeCode,
                    s.stocktakeDate,
                    s.status,
                    s.totalProductsChecked,
                    s.totalDiscrepancy,
                    s.notes,
                    s.completedDate
                FROM Stocktake s
                WHERE s.idStocktake = @p0 AND s.idSeller = @p1
            ", id, idSeller).FirstOrDefault();

            if (stocktake == null) return HttpNotFound();

            // Lấy chi tiết
            stocktake.Details = db.Database.SqlQuery<StocktakeDetailViewModel>(@"
                SELECT 
                    sd.idStocktakeDetail,
                    sd.idProduct,
                    p.nameProduct,
                    p.imageProduct_1,
                    sd.systemQuantity,
                    sd.actualQuantity,
                    sd.discrepancy,
                    sd.reason,
                    sd.isAdjusted,
                    sd.notes
                FROM StocktakeDetail sd
                INNER JOIN Product p ON sd.idProduct = p.idProduct
                WHERE sd.idStocktake = @p0
                ORDER BY p.nameProduct
            ", id).ToList();

            return View(stocktake);
        }

        /// <summary>
        /// Cập nhật số lượng thực tế cho 1 sản phẩm trong kiểm kê
        /// </summary>
        [HttpPost]
        public JsonResult UpdateActualQuantity(int idStocktakeDetail, int actualQuantity, string reason = null)
        {
            try
            {
                if (Session["idAccount"] == null) 
                    return Json(new { success = false, message = "Vui lòng đăng nhập" });

                db.Database.ExecuteSqlCommand(@"
                    UPDATE StocktakeDetail 
                    SET actualQuantity = @p0, reason = @p1
                    WHERE idStocktakeDetail = @p2
                ", actualQuantity, reason, idStocktakeDetail);

                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Hoàn thành kiểm kê
        /// </summary>
        [HttpPost]
        public ActionResult HoanThanhKiemKe(int idStocktake, string notes = null)
        {
            if (Session["idAccount"] == null) return RedirectToAction("DangNhap", "Login");
            if (!Equals(Session["idRole"], 1)) return HttpNotFound();

            try
            {
                // Cập nhật tổng số sản phẩm đã kiểm và chênh lệch
                db.Database.ExecuteSqlCommand(@"
                    UPDATE Stocktake
                    SET status = 'Completed',
                        completedDate = GETDATE(),
                        notes = @p0,
                        totalProductsChecked = (SELECT COUNT(*) FROM StocktakeDetail WHERE idStocktake = @p1),
                        totalDiscrepancy = (SELECT SUM(ABS(discrepancy)) FROM StocktakeDetail WHERE idStocktake = @p1)
                    WHERE idStocktake = @p1
                ", notes, idStocktake);

                TempData["Success"] = "Hoàn thành kiểm kê thành công!";
                return RedirectToAction("DanhSachKiemKe");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return RedirectToAction("ChiTietKiemKe", new { id = idStocktake });
            }
        }

        // =============================================
        // HELPER METHODS
        // =============================================

        private List<InventoryReportViewModel> GetInventoryReport(int idSeller)
        {
            return db.Database.SqlQuery<InventoryReportViewModel>(@"
                EXEC SP_GetInventoryReportBySeller @idSeller = @p0
            ", idSeller).ToList();
        }

        private List<InventoryHistoryViewModel> GetProductInventoryHistory(int idProduct, int idSeller)
        {
            return db.Database.SqlQuery<InventoryHistoryViewModel>(@"
                EXEC SP_GetProductInventoryHistory @idProduct = @p0, @idSeller = @p1
            ", idProduct, idSeller).ToList();
        }

        private void LoadProductsForSeller(int idSeller)
        {
            var products = db.Products
                .Where(p => p.idAccount == idSeller && !p.hideProduct && p.confirmProduct == true)
                .Select(p => new { p.idProduct, p.nameProduct })
                .ToList();

            ViewBag.Products = new SelectList(products, "idProduct", "nameProduct");
        }
    }
}
