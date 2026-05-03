namespace MercuryOMS.Domain.Constants
{
    public static class CacheKeys
    {
        // Category
        public const string AllCategories = "all_categories";

        // Product - Pagination + Filter
        public static string ProductsPaged(
            int pageIndex,
            int pageSize,
            bool onlyActive,
            decimal? minPrice,
            decimal? maxPrice)
        {
            return $"products:" +
                   $"{pageIndex}:{pageSize}:" +
                   $"{onlyActive}:" +
                   $"{minPrice ?? 0}:{maxPrice ?? 0}";
        }

        // detail
        public static string ProductDetail(Guid productId)
        {
            return $"product:detail:{productId}";
        }

        // User / Auth
        public const string UserById = "user:{0}";
        public const string RevokedJwt = "revoked_jwt:{0}";
    }
}