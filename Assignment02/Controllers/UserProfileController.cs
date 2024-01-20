using Assignment02.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Assignment02.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        // GET: api/UserProfile
        [HttpGet]
        public IActionResult GetAllUserProfiles()
        {
            List<UserProfile> userProfiles = DBManager.GetAllUserProfiles();
            return Ok(userProfiles);
        }

        // GET: api/UserProfile/1
        [HttpGet("{id}")]
        public IActionResult GetUserProfile(int id)
        {
            var userProfile = DBManager.GetById(id);
            if (userProfile == null)
            {
                return NotFound("User profile not found");
            }
            return Ok(userProfile);
        }

        // POST: api/UserProfile
        [HttpPost]
        public IActionResult CreateUserProfile([FromBody] UserProfile userProfile)
        {
            if (DBManager.Insert(userProfile))
            {
                return Ok("User profile created successfully");
            }
            return BadRequest("Error in creating user profile");
        }

        // PUT: api/UserProfile/1
        [HttpPut("{id}")]
        public IActionResult UpdateUserProfile(int id, [FromBody] UserProfile updatedProfile)
        {
            updatedProfile.Id = id;
            if (DBManager.UpdateUserProfile(updatedProfile))
            {
                return Ok("User profile updated successfully");
            }
            return NotFound("User profile not found");
        }

        // DELETE: api/UserProfile/1
        [HttpDelete("{id}")]
        public IActionResult DeleteUserProfile(int id)
        {
            if (DBManager.DeleteUserProfile(id))
            {
                return Ok("User profile deleted successfully");
            }
            return NotFound("User profile not found");
        }
    }
}
