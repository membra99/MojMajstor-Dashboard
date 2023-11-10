using Entities.Universal.MainData;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Authorization;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;
using static Universal.DTO.CommonModels.CommonModels;

namespace Universal.Admin_Controllers.AdminAPI
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersServices _userDataServices;

        public UsersController(UsersServices userServices)
        {
            _userDataServices = userServices;
        }

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult<List<UsersODTO>>> AllUsers()
		{
			var Users = await _userDataServices.GetAllUsers();
			if (Users == null)
			{
				return NotFound();
			}
			return Users;
		}

		[HttpGet("{id}")]
		[AllowAnonymous]
		public async Task<ActionResult<UsersODTO>> GetById(int id)
        {
            var User = await _userDataServices.GetUserById(id);
            if (User == null)
            {
                return NotFound();
            }
            return User;
        }

        [HttpPut]
        public async Task<ActionResult<UsersODTO>> PutUsers(UsersIDTO userIDTO)
        {
            try
            {
                return await _userDataServices.EditUser(userIDTO);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<IActionResult> LoginUser(AuthenticateRequest model)
        {
            try
            {
                var user = await _userDataServices.Authenticate(model);
                if (user == null)
                {
                    return Unauthorized(); // Return 401 Unauthorized status
                }
                return Ok(user);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

		[AllowAnonymous]
		[HttpPost("Register")]
		public async Task<ActionResult<string>> Register(UsersRegisterIDTO userModel)
		{
			if (string.IsNullOrEmpty(userModel.Email) && string.IsNullOrEmpty(userModel.Password))
				return BadRequest("User must have all data assigned");

			var user = await _userDataServices.RegisterUser(userModel);
            switch (user)
            {
                case "EmailExists":
                    return "User with this email already exists";

                case "RegexException":
                    return "Password doesn't match requirements";

                case "Done":
                    return Ok();
			}
            return null;
		}

		[AllowAnonymous]
		[HttpGet("Redirect")]
		public async Task<IActionResult> ActivateAccount(string key)
		{
			var Users = await _userDataServices.RedirectLink(key);
			if (Users == null)
			{
				return NotFound();
			}
			return Redirect(Users);
		}


		[HttpPost]
        public async Task<ActionResult<UsersODTO>> AddUsers(UsersIDTO userIDTO)
        {
            try
            {
                return await _userDataServices.AddUser(userIDTO);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [AllowAnonymous]
		[HttpPost("ChangePassword")]
		public async Task<ActionResult<UsersODTO>> ChangePassword(ChangePasswordIDTO passwordData)
		{
			try
			{
                    UsersIDTO userIDTO = await _userDataServices.GetUserByPassword(passwordData.Key);
                    if(userIDTO == null) return NotFound();
                    userIDTO.Password = BCrypt.Net.BCrypt.HashPassword(passwordData.Password);
                    return await _userDataServices.EditUser(userIDTO);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		[HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<UsersODTO>> DeleteUsers(int id)
        {
            try
            {
                var user = await _userDataServices.DeleteUser(id);
                if (user == null) return NotFound();
                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
