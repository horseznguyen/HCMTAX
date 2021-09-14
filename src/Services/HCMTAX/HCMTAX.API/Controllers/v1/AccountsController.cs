using HCMTAX.API.Controllers.v1.BaseClasses;
using HCMTAX.API.Infrastructure;
using HCMTAX.API.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Services.Common.MethodResultUtils;
using System;
using System.Data;
using System.Net;
using System.Threading.Tasks;

namespace HCMTAX.API.Controllers.v1
{
    public class AccountsController : APIControllerBase
    {
        private const string CheckAccount = nameof(CheckAccount);

        /// <summary>
        /// CheckAccount.
        /// Author : Hung
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(CheckAccount)]
        [ProducesResponseType(typeof(MethodResult<AccountCheckingResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(VoidMethodResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CheckAccountAsync(AccountCheckingRequestDto request)
        {
            var methodResult = new MethodResult<AccountCheckingResponseDto>();

            using (OracleConnection conn = DBUtils.GetDBConnection())
            {
                await conn.OpenAsync();

                OracleCommand cmd = new OracleCommand("pnn_pck_etax.prc_get_data_ntdt", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                #region Declare input

                cmd.Parameters.Add("@p_mst", OracleDbType.Varchar2, 30).Value = request.p_mst;

                cmd.Parameters.Add("@p_email", OracleDbType.Varchar2, 100).Value = request.p_email;

                #endregion Declare input

                #region Declare output

                cmd.Parameters.Add(new OracleParameter("@ma_cqt_qlt", OracleDbType.Varchar2, 30));

                cmd.Parameters.Add(new OracleParameter("@mst", OracleDbType.Varchar2, 30));

                cmd.Parameters.Add(new OracleParameter("@email", OracleDbType.Varchar2, 100));

                cmd.Parameters["@ma_cqt_qlt"].Direction = ParameterDirection.Output;

                cmd.Parameters["@mst"].Direction = ParameterDirection.Output;

                cmd.Parameters["@email"].Direction = ParameterDirection.Output;

                #endregion Declare output

                #region Execute and Get Data Column

                var number = cmd.ExecuteNonQuery();

                Console.WriteLine(number);

                methodResult.Result ??= new AccountCheckingResponseDto();

                if (!string.IsNullOrWhiteSpace(cmd.Parameters["@mst"].Value.ToString()))
                {
                    methodResult.Result.mst = cmd.Parameters["@mst"].Value.ToString();

                    methodResult.Result.ma_cqt_qlt = cmd.Parameters["@ma_cqt_qlt"].Value.ToString();

                    methodResult.Result.email = cmd.Parameters["@email"].Value.ToString();
                }

                #endregion Execute and Get Data Column
            }

            return Ok(methodResult);
        }
    }
}