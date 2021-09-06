namespace Services.Common.ApplicationService.Dto
{
    public interface IPagedRequest
    {
        int PageSize { get; set; }
        int PageNumber { get; set; }
    }
}