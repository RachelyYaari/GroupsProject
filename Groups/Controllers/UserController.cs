using DAL.Dtos;
using DAL.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;

namespace Groups.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUser _dbUser;
        public UserController(IUser User)
        {
            _dbUser = User;
        }

        [HttpGet("{groupId},{one}")]
        public async Task<List<UserDto>> Get(int groupId, int one)
        {
            var users = await _dbUser.getAllUsers(groupId);
            return users;
        }

        [HttpGet("{userId}")]
        public async Task<User> Get(int userId)
        {
            var @user = await _dbUser.getUserById(userId);
            return @user;
        }

        [HttpGet("getUserByEmail/{email}")]
        public IActionResult Get(string email)
        {
            var user = _dbUser.getUserByEmail(email);
            if(user.Result == null) 
                return NotFound();
            return Ok(user.Result);
        }



        //[HttpPost]
        //public async Task<IActionResult> Post([FromBody] UserDto _user)
        //{
        //    bool create = await _dbUser.createUser(_user);
        //    if (create)
        //        return Ok();
        //    return BadRequest();
        //}
        

        [HttpPut("{userId},{eventId}")]
        public async Task<IActionResult> Put(int userId, int eventId)
        {
            bool ans = await _dbUser.addEvent(userId, eventId);
            if (ans)
                return Ok();
            return BadRequest();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(int userId)
        {
            bool ans = await _dbUser.deleteUser(userId);
            if (ans)
                return Ok();
            return BadRequest();
        }

    }
}