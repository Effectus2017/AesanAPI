using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Api.Interfaces;
using Api.Models.Request;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HouseholdController : ControllerBase
    {
        private readonly IHouseholdRepository _repo;
        public HouseholdController(IHouseholdRepository repo)
        {
            _repo = repo;
        }

        // Household
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHouseholdById(int id)
        {
            var result = await _repo.GetHouseholdById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetHouseholds([FromQuery] int take = 50, [FromQuery] int skip = 0)
        {
            var result = await _repo.GetHouseholds(take, skip);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> InsertHousehold([FromBody] HouseholdRequest request)
        {
            var id = await _repo.InsertHousehold(request);
            return Ok(new { Id = id });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateHousehold([FromBody] HouseholdRequest request)
        {
            var ok = await _repo.UpdateHousehold(request);
            if (!ok) return NotFound();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHousehold(int id)
        {
            var ok = await _repo.DeleteHousehold(id);
            if (!ok) return NotFound();
            return Ok();
        }

        // HouseholdMember
        [HttpGet("member/{id}")]
        public async Task<IActionResult> GetHouseholdMemberById(int id)
        {
            var result = await _repo.GetHouseholdMemberById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("{householdId}/members")]
        public async Task<IActionResult> GetHouseholdMembers(int householdId, [FromQuery] int take = 50, [FromQuery] int skip = 0)
        {
            var result = await _repo.GetHouseholdMembers(householdId, take, skip);
            return Ok(result);
        }

        [HttpPost("member")]
        public async Task<IActionResult> InsertHouseholdMember([FromBody] HouseholdMemberRequest request)
        {
            var id = await _repo.InsertHouseholdMember(request);
            return Ok(new { Id = id });
        }

        [HttpPut("member")]
        public async Task<IActionResult> UpdateHouseholdMember([FromBody] HouseholdMemberRequest request)
        {
            var ok = await _repo.UpdateHouseholdMember(request);
            if (!ok) return NotFound();
            return Ok();
        }

        [HttpDelete("member/{id}")]
        public async Task<IActionResult> DeleteHouseholdMember(int id)
        {
            var ok = await _repo.DeleteHouseholdMember(id);
            if (!ok) return NotFound();
            return Ok();
        }

        // HouseholdMemberIncome
        [HttpGet("income/{id}")]
        public async Task<IActionResult> GetHouseholdMemberIncomeById(int id)
        {
            var result = await _repo.GetHouseholdMemberIncomeById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("member/{memberId}/incomes")]
        public async Task<IActionResult> GetHouseholdMemberIncomes(int memberId, [FromQuery] int take = 50, [FromQuery] int skip = 0)
        {
            var result = await _repo.GetHouseholdMemberIncomes(memberId, take, skip);
            return Ok(result);
        }

        [HttpPost("income")]
        public async Task<IActionResult> InsertHouseholdMemberIncome([FromBody] HouseholdMemberIncomeRequest request)
        {
            var id = await _repo.InsertHouseholdMemberIncome(request);
            return Ok(new { Id = id });
        }

        [HttpPut("income")]
        public async Task<IActionResult> UpdateHouseholdMemberIncome([FromBody] HouseholdMemberIncomeRequest request)
        {
            var ok = await _repo.UpdateHouseholdMemberIncome(request);
            if (!ok) return NotFound();
            return Ok();
        }

        [HttpDelete("income/{id}")]
        public async Task<IActionResult> DeleteHouseholdMemberIncome(int id)
        {
            var ok = await _repo.DeleteHouseholdMemberIncome(id);
            if (!ok) return NotFound();
            return Ok();
        }
    }
}