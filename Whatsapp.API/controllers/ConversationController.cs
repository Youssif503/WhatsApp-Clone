using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whatsapp.API.Helpers;
using Whatsapp.BLL.DTOs;
using Whatsapp.BLL.Services;
using Whatsapp.DAL.Helpers;

namespace Whatsapp.API.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConversationController : ControllerBase
    {
        private readonly ConversationService _conversationService;
        public ConversationController(ConversationService conversationService)
        {
            _conversationService = conversationService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateConversation
            (CreateConverstaionDto Obj)
        {
            var UserId = User.FindFirst("Sub")?.Value;
            if (UserId == null)
                return Unauthorized(Response<string>.Fail("Unauthorized User"));
            
            Obj.UserMemberId = UserId;
            var result = 
                await _conversationService.CreateConversation(Obj);
            
            if(result == null)
                return BadRequest(Response<string>.Fail("Conversation creation failed"));
            
            return Ok(Response<string>.Success("Conversation created Successfully"));
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
