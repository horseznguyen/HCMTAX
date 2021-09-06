namespace Services.Common.ApplicationService.Dto
{
    public class PagingInfoDto : ITotalCount, IPagedRequest
    {
        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}