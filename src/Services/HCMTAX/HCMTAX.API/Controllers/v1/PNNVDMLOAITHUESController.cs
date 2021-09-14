using HCMTAX.API.Controllers.v1.BaseClasses;
using HCMTAX.API.Infrastructure;
using HCMTAX.API.ViewModels.PNNVDMLOAITHUES;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Services.Common.ApplicationService.Dto;
using Services.Common.MethodResultUtils;
using System.Net;
using System.Threading.Tasks;

namespace HCMTAX.API.Controllers.v1
{
    public class PNNVDMLOAITHUESController : APIControllerBase
    {
        private const string PNNVDMLOAITHUES = nameof(PNNVDMLOAITHUES);

        /// <summary>
        /// GetPNNVDMLOAITHUESByFilters.
        /// Author : Hung
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(PNNVDMLOAITHUES)]
        [ProducesResponseType(typeof(MethodResult<PagingItems<PNNVDMLOAITHUEResponseDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetPNNVDMLOAITHUESByFiltersAsync(PNNVDMLOAITHUERequestDto request)
        {
            var methodResult = new MethodResult<PagingItems<PNNVDMLOAITHUEResponseDto>>
            {
                Result = new PagingItems<PNNVDMLOAITHUEResponseDto>()
            };

            using (OracleConnection conn = DBUtils.GetDBConnection())
            {
                await conn.OpenAsync();

                OracleCommand cmd = new OracleCommand();

                cmd.Connection = conn;

                cmd.CommandText = "SELECT p.ma_lthue, p.ten_lthue FROM iqlbl_owner.PNN_V_DM_LOAITHUE p";

                using OracleDataReader reader = cmd.ExecuteReader();

                methodResult.Result.Items = reader.QueryTo<PNNVDMLOAITHUEResponseDto>();
            }

            return Ok(methodResult);
        }
    }
}