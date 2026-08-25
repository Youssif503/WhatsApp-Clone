using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whatsapp.API.Helpers;
using Whatsapp.BLL.DTOs;
using Whatsapp.BLL.Services;
using Whatsapp.DAL.Helpers;
using Microsoft.AspNetCore.Identity;
using Whatsapp.DAL.models;

namespace Whatsapp.API.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConversationController : ControllerBase
    {
        private readonly ConversationService _conversationService;
        private readonly UserManager<User> _userManager;
        public ConversationController(
            ConversationService conversationService,
            UserManager<User> userManager)
        {
            _conversationService = conversationService;
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> CreateConversation
            (CreateConverstaionDto Obj)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId == null)
                return Unauthorized(Response<string>.Fail("Unauthorized User"));

            if (string.IsNullOrWhiteSpace(Obj.OtherUserId) || Obj.OtherUserId == UserId)
                return BadRequest(Response<string>.Fail("Choose another user to start a conversation"));

            if (await _userManager.FindByIdAsync(Obj.OtherUserId) is null)
                return NotFound(Response<string>.Fail("User not found"));
            
            Obj.UserMemberId = UserId;
            var result = 
                await _conversationService.CreateConversation(Obj);
            
            if(result == null)
                return BadRequest(Response<string>.Fail("Conversation creation failed"));
            
            return Ok(Response<CreatedConversationDto>.Success(new CreatedConversationDto
            {
                ConversationId = result.Id
            }, "Conversation created Successfully"));
        }
        
        [HttpPost("CreateGroup")]
        public async Task<IActionResult> CreateGroup
            (CreateGroupDto Group)
        {
            var UserMemberId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (UserMemberId == null)
                return Unauthorized(Response<string>.Fail("Unauthorized User"));
            
            Group.MemberId = UserMemberId;
            await _conversationService.CreateGroup(Group);
            return Ok(Response<string>.Success("Group created Successfully"));
        }

        [HttpGet]
        public async Task<IActionResult> GetUserConversation()
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId == null)
                return Unauthorized(Response<string>.Fail("Unauthorized User"));
            
            var conversations = 
                await _conversationService.GetUserConversationsAsync(UserId);
            
            return Ok(Response<List<ConversationResponse>>.Success(conversations,""));
        }
    }
}
