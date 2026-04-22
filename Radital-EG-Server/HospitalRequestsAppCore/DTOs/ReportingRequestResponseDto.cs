using Domain;

namespace HospitalRequestsAppCore.DTOs
{
    /// <summary>
    /// US-01 / US-02: Returned to the Hospital Technician after creating or querying a request.
    /// Contains the request Id and current status so the technician can track progress.
    /// </summary>
    public class ReportingRequestResponseDto
    {
        public Guid Id { get; set; }

        // --- Patient summary ---
        public string PatientName { get; set; } = string.Empty;
        public DateTime PatientDateOfBirth { get; set; }

        // --- Request details ---
        public ImageModalitiesEnum ImageModality { get; set; }
        public string SuggestedDepartment { get; set; } = string.Empty;
        public PrioritiesEnum Priority { get; set; }
        public bool IsEmergency { get; set; }

        // --- US-02: Status tracking fields ---
        public ReportingRequestStatusEnum Status { get; set; }
        public DateTime SubmissionTime { get; set; }
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Human-readable label derived from Status enum for convenience.
        /// </summary>
        public string StatusLabel => Status.ToString();
    }
}
