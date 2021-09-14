using HCMTAX.API.Controllers.v1.BaseClasses;
using HCMTAX.API.Infrastructure;
using HCMTAX.API.ViewModels.PNNVDMMTHU;
using HCMTAX.API.ViewModels.PNNVDMMTHUS;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Services.Common.ApplicationService.Dto;
using Services.Common.MethodResultUtils;
using System.Net;
using System.Threading.Tasks;

namespace HCMTAX.API.Controllers.v1
{
    public class PNNVDMMTHUSController : APIControllerBase
    {
        private const string PNNVDMMTHUS = nameof(PNNVDMMTHUS);

        /// <summary>
        /// GetPNNVDMMTHUSByFilters.
        /// Author : Hung
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(PNNVDMMTHUS)]
        [ProducesResponseType(typeof(MethodResult<PagingItems<PNNVDMMTHUResponseDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetPNNVDMMTHUSByFiltersAsync(PNNVDMMTHURequestDto request)
        {
            var methodResult = new MethodResult<PagingItems<PNNVDMMTHUResponseDto>>
            {
                Result = new PagingItems<PNNVDMMTHUResponseDto>()
            };

            using (OracleConnection conn = DBUtils.GetDBConnection())
            {
                await conn.OpenAsync();

                OracleCommand cmd = new OracleCommand();

                cmd.Connection = conn;

                cmd.CommandText = "SELECT p.ma_muc, p.ten_muc, p.ma_tmuc, p.ten_tmuc FROM iqlbl_owner.PNN_V_DM_MTHU p";

                using OracleDataReader reader = cmd.ExecuteReader();

                methodResult.Result.Items = reader.QueryTo<PNNVDMMTHUResponseDto>();
            }

            return Ok(methodResult);
        }
    }
}