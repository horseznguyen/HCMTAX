using HCMTAX.API.Controllers.v1.BaseClasses;
using HCMTAX.API.Infrastructure;
using HCMTAX.API.ViewModels.PTNS;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Services.Common.ApplicationService.Dto;
using Services.Common.MethodResultUtils;
using System.Data;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HCMTAX.API.Controllers.v1
{
    public class PTNSController : APIControllerBase
    {
        private const string PTNByFilters = nameof(PTNByFilters);

        private const string MaSoThuesByFilters = nameof(MaSoThuesByFilters);

        private const string FullPTNByFilters = nameof(FullPTNByFilters);

        /// <summary>
        /// GetPTNByFilters.
        /// Author : Hung
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(PTNByFilters)]
        [ProducesResponseType(typeof(MethodResult<PagingItems<PTNByFiltersResponseDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetPTNByFiltersAsync(PTNByFiltersRequestDto request)
        {
            var methodResult = new MethodResult<PagingItems<PTNByFiltersResponseDto>>
            {
                Result = new PagingItems<PTNByFiltersResponseDto>()
            };

            using (OracleConnection conn = DBUtils.GetDBConnection())
            {
                await conn.OpenAsync();

                OracleCommand cmd = new OracleCommand();

                var sql = new StringBuilder("SELECT " +
                  "p.ma_cqt_tms, p.ma_cqt_thu, p.mst, " +
                  "p.ten_nnt, p.zzmapnn, p.dc_thuadat, p.ma_phuong, " +
                  "p.ma_to, p.so_tdat, p.so_bdo, p.chuong, p.ma_tmuc, p.no_cuoi_ky " +
                  "FROM iqlbl_owner.pnn_v_sono_hcmtax p");

                if (!string.IsNullOrWhiteSpace(request.MaSoThue))
                {
                    sql.Append($"WHERE p.mst = @mst");

                    cmd.Parameters.Add("@mst", SqlDbType.VarChar).Value = request.MaSoThue;
                }

                cmd.Connection = conn;

                cmd.CommandText = sql.ToString();

                using OracleDataReader reader = cmd.ExecuteReader();

                methodResult.Result.Items = reader.QueryTo<PTNByFiltersResponseDto>();
            }

            return Ok(methodResult);
        }

        /// <summary>
        /// GetMaSoThuesByFilters.
        /// Author : Hung
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(MaSoThuesByFilters)]
        [ProducesResponseType(typeof(MethodResult<PagingItems<MaSoThuesByFiltersResponseDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetMaSoThuesByFiltersAsync(MaSoThuesByFiltersRequestDto request)
        {
            return Ok();
        }

        /// <summary>
        /// GetFullPTNByFilters.
        /// Author : Hung
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(FullPTNByFilters)]
        [ProducesResponseType(typeof(MethodResult<PagingItems<FullPTNByFiltersResponseDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetFullPTNByFiltersAsync(FullPTNByFiltersRequestDto request)
        {
            var methodResult = new MethodResult<PagingItems<FullPTNByFiltersResponseDto>>
            {
                Result = new PagingItems<FullPTNByFiltersResponseDto>()
            };

            using (OracleConnection conn = DBUtils.GetDBConnection())
            {
                await conn.OpenAsync();

                OracleCommand cmd = new OracleCommand();

                var sql = new StringBuilder("SELECT " +
                  "p.zzmapnn, p.mst, p.chuong, p.ten_nnt, p.dc_thuadat, p.so_tdat, " +
                  "p.so_bdo, p.ma_cqt_tms, p.ma_cqt_qlt, p.ten_cqt, p.ma_phuong, " +
                  "p.ten_phuong, p.ma_to, p.ten_to, p.ky_thue, p.nam_htoan, p.thang_htoan, " +
                  "p.ma_tmuc, p.no_cuoi_ky, p.ma_cqt_thu, p.ma_kbnn, p.ten_kbnn, p.so_tai_khoan_co " +
                  "FROM iqlbl_owner.pnn_v_sono_hcmtax p");

                if (!string.IsNullOrWhiteSpace(request.MaSoThue))
                {
                    sql.Append($"WHERE p.mst = @mst");

                    cmd.Parameters.Add("@mst", SqlDbType.VarChar).Value = request.MaSoThue;
                }

                cmd.Connection = conn;

                cmd.CommandText = sql.ToString();

                using OracleDataReader reader = cmd.ExecuteReader();

                methodResult.Result.Items = reader.QueryTo<FullPTNByFiltersResponseDto>();
            }

            return Ok(methodResult);
        }
    }
}