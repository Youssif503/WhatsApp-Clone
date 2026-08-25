using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Whatsapp.API.Helpers;
using Whatsapp.BLL.Services;
using Whatsapp.DAL.Helpers;


namespace Whatsapp.API.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly MessageService _MessageService;
        public MessageController(MessageService messageService)
        {
            _MessageService = messageService;
        }

        [HttpGet("{ConversationId}/messages")]
        public async Task<IActionResult> GetMessage(string ConversationId,
           [FromQuery] CursorPaginationRequest PaginationRequest )
        {
            var userId = User.FindFirst("Sub")?.Value;
            if(userId == null)
                return Unauthorized("Pleaze Login....");
            
            var result = 
                await _MessageService.GetMessagesAsync(userId, ConversationId,PaginationRequest);
            
            return Ok(result);
        }
    }
}
