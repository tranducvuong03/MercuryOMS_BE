using System.ComponentModel;

namespace MercuryOMS.Application.Models.Requests
{
    public class ProductFilterRequest
    {
        [DefaultValue(1)]
        public int PageIndex { get; set; } = 1;

        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;

        public bool IsActive { get; set; } = true;

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }
    }
}