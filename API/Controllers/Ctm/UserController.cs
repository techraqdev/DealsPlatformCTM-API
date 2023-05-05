using API.AppAuthorization;
using API.Controllers.Ctm;
using Deals.Business.Interface.Ctm;
using DTO.Ctm;
using DTO.Response.Ctm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Deals.Domain.Constants.DomainConstants;

namespace PostGresBaseTemplate.Controllers.Ctm
{
    [ApiController]
    [Route("Ctm/[controller]")]
    public class UserController : BaseController
    {
        private readonly IUserBusiness _businessProvider;

        public UserController(
            ILogger<UserController> logger, IUserBusiness businessProvider) : base(logger)
        {
            _businessProvider = businessProvider;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost("list")]
        [Authorize(Permissions.User.List)]
        public async Task<ActionResult<List<UserDTO>>> Post()
        {
            ConstructUsersParams(out SearchUserDTO query);
            try
            {
                //var id = UserInfo.UserId;
                Logger.Log(LogLevel.Information, "Get method called!");
                var result = await _businessProvider.GetUsersListAsync(query).ConfigureAwait(false);
                if (result != null)
                {
                    Logger.Log(LogLevel.Information, "Get method results received with count!", result.RecordsTotal);
                    return Ok(result);
                }
                else
                {
                    Logger.Log(LogLevel.Information, "No records found!");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Exception in Get!{ex.Message}", ex);
                return Problem();
            }
        }

        private void ConstructUsersParams(out SearchUserDTO query)
        {
            int start = 0;
            int length = 10;
            int draw = 1;
            string name = string.Empty;
            string searchValue = string.Empty;
            string sortColumnName = string.Empty;
            string sortDirection = string.Empty;

            if (Request.HasFormContentType)
            {
                draw = Convert.ToInt32(Request.Form["draw"]);
                start = Convert.ToInt32(Request.Form["start"]);
                length = Convert.ToInt32(Request.Form["length"]);
                searchValue = Request.Form["search[value]"];
                sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"];
                sortDirection = Request.Form["order[0][dir]"];
                name = Request.Form["columns[0][search][value]"];
            }

            query = new SearchUserDTO()
            {
                Name = name,
                PageQueryModel = { Page = start, Limit = length, SortColName = sortColumnName, SortDirection = sortDirection, Draw = draw }
            };
        }

        [HttpPost]
        [Authorize(Permissions.User.Create)]
        public async Task<ActionResult<UserDTO?>> Create([FromBody] AddUserDTO user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    user.CreatedBy = UserInfo.UserId;
                    var result = await _businessProvider.AddUser(user).ConfigureAwait(false);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Exception in Create user!{ex.Message}", ex);
                return Problem();
            }

            return Problem();
        }

        [HttpGet("{id}")]
        [Authorize(Permissions.User.View)]
        public async Task<ActionResult<UserDTO?>> Details(Guid id)
        {
            var result = await _businessProvider.GetUser(id).ConfigureAwait(false);
            if (result != null)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpPut("{id}")]
        [Authorize(Permissions.User.Edit)]
        public async Task<ActionResult<UserDTO?>> Edit(Guid id, [FromBody] UpdateUserDTO user)
        {
            if (ModelState.IsValid)
            {
                user.ModifieddBy = UserInfo.UserId;
                var result = await _businessProvider.UpdateUser(id, user).ConfigureAwait(false);
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        [Authorize(Permissions.User.Delete)]
        public async Task<ActionResult<bool>> DeleteConfirmed(Guid id)
        {
            if (id != Guid.Empty)
            {
                var result = await _businessProvider.DeleteUser(id).ConfigureAwait(false);
                return Ok(result);
            }
            return BadRequest();
        }

        [Route("BulkUpload")]
        [HttpPost]
        public async Task<ActionResult<BulkUploadResponse>> UploadExcel([FromForm] IFormFile body)
        {
            try
            {
                Guid createdBy = new();
                if (ModelState.IsValid)
                {
                    createdBy = UserInfo.UserId;
                    var result = await _businessProvider.BulkUploadExcel(body, createdBy).ConfigureAwait(false);
                    return Ok(result);
                }
                return BadRequest();
            }

            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Exception in UploadExcel!{ex.Message}", ex);
                return Problem();
            }

        }
    }
}