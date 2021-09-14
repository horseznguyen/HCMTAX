using HCMTAX.API.Controllers.v1.BaseClasses;
using HCMTAX.API.Infrastructure;
using HCMTAX.API.ViewModels.PNNVDMKBNN;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Services.Common.ApplicationService.Dto;
using Services.Common.MethodResultUtils;
using System.Net;
using System.Threading.Tasks;

namespace HCMTAX.API.Controllers.v1
{
    public class PNNVDMKBNNSController : APIControllerBase
    {
        private const string PNNVDMKBNNS = nameof(PNNVDMKBNNS);

        /// <summary>
        /// GetPNNVDMKBNNSByFilters.
        /// Author : Hung
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(PNNVDMKBNNS)]
        [ProducesResponseType(typeof(MethodResult<PagingItems<PNNVDMKBNNResponseDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetPNNVDMKBNNSByFiltersAsync(PNNVDMKBNNRequestDto request)
        {
            var methodResult = new MethodResult<PagingItems<PNNVDMKBNNResponseDto>>
            {
                Result = new PagingItems<PNNVDMKBNNResponseDto>()
            };

            using (OracleConnection conn = DBUtils.GetDBConnection())
            {
                await conn.OpenAsync();

                OracleCommand cmd = new OracleCommand();

                cmd.Connection = conn;

                cmd.CommandText = "SELECT p.ma_cha_cqt, p.ma_cha_qlt, p.ma_cqt_qlt, " +
                   "p.ma_cqt_thu, p.ma_cqt_tms, p.ma_kbnn, p.ten_cqt, " +
                   "p.ten_cqt_dai, p.ten_kbnn " +
                   "FROM iqlbl_owner.PNN_V_DM_KBNN p";

                using OracleDataReader reader = cmd.ExecuteReader();

                methodResult.Result.Items = reader.QueryTo<PNNVDMKBNNResponseDto>();
            }

            return Ok(methodResult);
        }
    }
}