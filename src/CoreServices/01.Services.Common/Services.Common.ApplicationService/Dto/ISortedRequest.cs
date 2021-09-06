namespace Services.Common.ApplicationService.Dto
{
    public interface ISortedRequest
    {
        string SortName { get; set; }

        bool SortASC { get; set; }
    }
}