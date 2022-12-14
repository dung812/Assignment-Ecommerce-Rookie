using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ShoesShop.Domain.Enum;

namespace ShoesShop.DTO
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        public string ProductName { get; set; } = null!;
        public string ImageName { get; set; } = null!;
        public string ImageFileName { get; set; } = null!;
        public int OriginalPrice { get; set; }
        [Range(1, 100)]
        public int PromotionPercent { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public int Quantity { get; set; }
        public Gender ProductGenderCategory { get; set; }
        public string? ManufactureName { get; set; }
        public string? CatalogName { get; set; }
        public string? AdminCreate { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int Income { get; set; } //  Tổng doanh thu sản phẩm
        public int AvgStar { get; set; } // Trung bình số sao đánh giá
        public int TotalComment { get; set; } // Tổng số lượt đánh giá
        public string ImageNameGallery1 { get; set; }
        public string ImageNameGallery2 { get; set; } 
        public string ImageNameGallery3 { get; set; } 

        public int AdminId { get; set; }
        public int CatalogId { get; set; }
        public int ManufactureId { get; set; }
        public string? Gender { get; set; }
    }
}
