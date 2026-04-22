using HospitalRequestsAppCore.DTOs;
using HospitalRequestsAppCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HospitalRequestsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportingRequestsController : ControllerBase
    {
        private readonly IReportingRequestsManagementService _service;
        private readonly ILogger<ReportingRequestsController> _logger;

        public ReportingRequestsController(
            IReportingRequestsManagementService service,
            ILogger<ReportingRequestsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// US-01: Create a new imaging / reporting request.
        /// The technician supplies patient demographics and scan type.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ReportingRequestResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateRequest([FromBody] CreateReportingRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _service.CreateRequestAsync(dto);
                return CreatedAtAction(nameof(GetRequestById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reporting request");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while creating the reporting request.");
            }
        }

        /// <summary>
        /// US-02: Get all reporting requests with their current statuses.
        /// Allows the technician to track the progress of all submitted requests.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReportingRequestResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllRequests()
        {
            try
            {
                var results = await _service.GetAllRequestsAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reporting requests");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while fetching reporting requests.");
            }
        }

        /// <summary>
        /// US-02: Get a single reporting request by Id.
        /// Allows the technician to check the real-time status of a specific request.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ReportingRequestResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRequestById(Guid id)
        {
            try
            {
                var result = await _service.GetRequestByIdAsync(id);

                if (result == null)
                    return NotFound($"Reporting request with Id '{id}' was not found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reporting request {RequestId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while fetching the reporting request.");
            }
        }
    }
}
