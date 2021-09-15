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

        ///// <summary>
        ///// GetPTNByFilters.
        ///// Author : Hung
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route(PTNByFilters)]
        //[ProducesResponseType(typeof(MethodResult<PagingItems<PTNByFiltersResponseDto>>), (int)HttpStatusCode.OK)]
        //[ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.BadRequest)]
        //public async Task<IActionResult> GetPTNByFiltersAsync(PTNByFiltersRequestDto request)
        //{
        //    var methodResult = new MethodResult<PagingItems<PTNByFiltersResponseDto>>
        //    {
        //        Result = new PagingItems<PTNByFiltersResponseDto>()
        //    };

        //    using (OracleConnection conn = DBUtils.GetDBConnection())
        //    {
        //        await conn.OpenAsync();

        //        OracleCommand cmd = new OracleCommand("pnn_pck_sono.prc_get_data_nnt", conn);

        //        cmd.CommandType = CommandType.StoredProcedure;

        //        #region Declare input

        //        cmd.Parameters.Add("@P_MST", OracleDbType.Varchar2, 30).Value = request.MaSoThue;

        //        #endregion Declare input

        //        #region Declare output

        //        cmd.Parameters.Add(new OracleParameter("@ZZMAPNN", OracleDbType.Varchar2, 200));
        //        cmd.Parameters.Add(new OracleParameter("@MST", OracleDbType.Varchar2, 30));
        //        cmd.Parameters.Add(new OracleParameter("@TEN_NNT", OracleDbType.Varchar2, 300));
        //        cmd.Parameters.Add(new OracleParameter("@DC_THUADAT", OracleDbType.Varchar2, 300));
        //        cmd.Parameters.Add(new OracleParameter("@CHUONG", OracleDbType.Varchar2, 20));
        //        cmd.Parameters.Add(new OracleParameter("@MA_TKHAI", OracleDbType.Varchar2, 20));
        //        cmd.Parameters.Add(new OracleParameter("@MA_LTHUE", OracleDbType.Varchar2, 20));
        //        cmd.Parameters.Add(new OracleParameter("@TEN_LTHUE", OracleDbType.Varchar2, 150));
        //        cmd.Parameters.Add(new OracleParameter("@SO_TDAT", OracleDbType.Varchar2, 50));
        //        cmd.Parameters.Add(new OracleParameter("@SO_BDO", OracleDbType.Varchar2, 50));
        //        cmd.Parameters.Add(new OracleParameter("@MA_CQT_QLT", OracleDbType.Varchar2, 20));
        //        cmd.Parameters.Add(new OracleParameter("@TEN_CQT", OracleDbType.Varchar2, 100));
        //        cmd.Parameters.Add(new OracleParameter("@MA_PHUONG", OracleDbType.Varchar2, 30));
        //        cmd.Parameters.Add(new OracleParameter("@TEN_PHUONG", OracleDbType.Varchar2, 150));
        //        cmd.Parameters.Add(new OracleParameter("@MA_TO", OracleDbType.Varchar2, 30));
        //        cmd.Parameters.Add(new OracleParameter("@TEN_TO", OracleDbType.Varchar2, 150));
        //        cmd.Parameters.Add(new OracleParameter("@KY_THUE", OracleDbType.Varchar2, 20));
        //        cmd.Parameters.Add(new OracleParameter("@NAM_HTOAN", OracleDbType.Varchar2, 20));
        //        cmd.Parameters.Add(new OracleParameter("@THANG_HTOAN", OracleDbType.Varchar2, 20));
        //        cmd.Parameters.Add(new OracleParameter("@MA_TMUC", OracleDbType.Varchar2, 20));
        //        cmd.Parameters.Add(new OracleParameter("@TEN_TMUC", OracleDbType.Varchar2, 200));
        //        cmd.Parameters.Add(new OracleParameter("@NO_CUOI_KY", OracleDbType.Decimal));
        //        cmd.Parameters.Add(new OracleParameter("@MA_CQT_THU", OracleDbType.Varchar2, 30));
        //        cmd.Parameters.Add(new OracleParameter("@MA_KBNN", OracleDbType.Varchar2, 20));
        //        cmd.Parameters.Add(new OracleParameter("@TEN_KBNN", OracleDbType.Varchar2, 150));
        //        cmd.Parameters.Add(new OracleParameter("@SO_TAI_KHOAN_CO", OracleDbType.Varchar2, 20));

        //        cmd.Parameters["@ZZMAPNN"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@MST"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@TEN_NNT"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@DC_THUADAT"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@CHUONG"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@MA_TKHAI"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@MA_LTHUE"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@TEN_LTHUE"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@SO_TDAT"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@SO_BDO"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@MA_CQT_QLT"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@TEN_CQT"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@MA_PHUONG"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@TEN_PHUONG"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@MA_TO"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@TEN_TO"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@KY_THUE"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@NAM_HTOAN"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@THANG_HTOAN"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@MA_TMUC"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@TEN_TMUC"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@NO_CUOI_KY"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@MA_CQT_THU"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@MA_KBNN"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@TEN_KBNN"].Direction = ParameterDirection.Output;
        //        cmd.Parameters["@SO_TAI_KHOAN_CO"].Direction = ParameterDirection.Output;

        //        #endregion Declare output

        //        #region Execute and Get Data Column

        //        cmd.ExecuteNonQuery();

        //        Console.WriteLine(cmd.Parameters["@ZZMAPNN"].Value.ToString());
        //        Console.WriteLine(cmd.Parameters["@MST"].Value.ToString());
        //        Console.WriteLine(cmd.Parameters["@TEN_NNT"].Value.ToString());

        //        //using OracleDataReader reader = cmd.ExecuteReader();

        //        //methodResult.Result.Items = reader.QueryTo<PTNByFiltersResponseDto>();

        //        #endregion Execute and Get Data Column
        //    }

        //    return Ok(methodResult);
        //}

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