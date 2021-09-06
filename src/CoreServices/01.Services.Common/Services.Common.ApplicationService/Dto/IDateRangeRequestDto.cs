using System;

namespace Services.Common.ApplicationService.Dto
{
    public interface IDateRangeRequestDto
    {
        DateTime? FromDateTime { get; set; }
        DateTime? ToDateTime { get; set; }
    }
}