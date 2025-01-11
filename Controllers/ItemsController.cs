using Microsoft.AspNetCore.Mvc;
using TodoAPI.Data;
using TodoAPI.Dtos;
using TodoAPI.Models;
using TodoAPI.Services;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private ItemService _itemService;

        public ItemsController(ItemService itemService)
        {
            _itemService = itemService;
        }

        // GET: api/<ItemsController>
        [HttpGet]
        public async Task<IActionResult> Get(int page = 1, int pageSize = 10)
        {
            
            var (items, pageInfo) = await _itemService.GetItems(page, pageSize);

            var response = new
            {
                items,
                pageInfo
            };
            return Ok(response);

        }


        // GET: api/<ItemsController>/completed
        [HttpGet("completed")]
        public async Task<IActionResult> GetCompleted(int page = 1, int pageSize = 10)
        {
            var (items, pageInfo) = await _itemService.GetCompletedItems(page, pageSize);

            var response = new
            {
                items,
                pageInfo
            };
            return Ok(response);

        }

        // GET: api/<ItemsController>/uncompleted
        [HttpGet("uncompleted")]
        public async Task<IActionResult> GetUncompleted(int page = 1, int pageSize = 10)
        {
            var (items, pageInfo) = await _itemService.GetUncompletedItems(page, pageSize);
            var response = new
            {
                items,
                pageInfo
            };
            return Ok(response);

        }


        // GET api/<ItemsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _itemService.GetItem(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        // POST api/<ItemsController>
        [HttpPost]
        public async Task<IActionResult> Post(AddItemDto itemDto)
        {
            if (ModelState.IsValid)
            {
                bool isSuccess = await _itemService.AddItem(itemDto);
                if (isSuccess)
                {
                    return Created("Get", itemDto);
                }
                else
                {
                    return BadRequest();
                }

            }
            return BadRequest();
        }

        // PUT api/<ItemsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UpdateItemDto itemDto)
        {
           if(ModelState.IsValid)
            {
                bool isSuccess = await _itemService.UpdateItem(id, itemDto);

                if (isSuccess)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        // DELETE api/<ItemsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool isSuccess = await _itemService.DeleteItem(id);
            if (isSuccess)
            {
                return NoContent();
            }
            else
            {
               return NotFound();
            }
        }
    }
}
