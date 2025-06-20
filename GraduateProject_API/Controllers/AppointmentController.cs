using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using GraduateProject_Core.Models;
using GraduateProject_Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GraduateProject_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Patient,Doctor,Admin")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly AppDbContext _context;

 
        public AppointmentController(IAppointmentRepository appointmentRepository, INotificationRepository notificationRepository, AppDbContext context)
        {
            _appointmentRepository = appointmentRepository;
            _notificationRepository = notificationRepository;
            _context = context;
        }


        [HttpPost("Book")]
        public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

        
            bool isAvailable = await _appointmentRepository.CheckIfTimeSlotIsAvailable(dto.DoctorId, dto.DateTime);

            if (!isAvailable)
                return Conflict(new { Message = "This time slot is already booked." });

            // حاول الحجز
            var result = await _appointmentRepository.BookAppointmentAsync(dto);

            if (result)
            {
                
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.DoctorID == dto.DoctorId);
                if (doctor == null)
                    return BadRequest("Doctor not found");

                var notification = new Notification
                {
                    UserId = doctor.UserId,
                    Title = "New Appointment 📅",
                    Message = "A new appointment has been booked.",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };

                await _notificationRepository.CreateNotificationAsync(notification);

                return Ok(new { Message = "Appointment booked successfully." });
            }

            return BadRequest(new { Message = "Booking failed." });
        }

        //[HttpPost("Book")]

        //public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentDTO dto)
        //{
        //    var result = await _appointmentRepository.BookAppointmentAsync(dto);

        //    if (result)
        //    {
        //        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.DoctorID == dto.DoctorId);
        //        if (doctor == null) return BadRequest("Doctor not found");

        //        var notification = new Notification
        //        {
        //            UserId = doctor.UserId,   
        //            Title = "New Appointment 📅",
        //            Message = "A new appointment has been booked.",
        //            CreatedAt = DateTime.UtcNow,
        //            IsRead = false
        //        };

        //        await _notificationRepository.CreateNotificationAsync(notification);

        //        return Ok(new { Message = "Appointment booked successfully." });
        //    }

        //    return BadRequest("Booking failed.");
        //}


        [HttpGet("Patient/{patientId}")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetAppointmentsByPatient(int patientId)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByPatientAsync(patientId);
            return Ok(appointments);
        }

        [HttpGet("Doctor/{doctorId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetAppointmentsByDoctor(int doctorId)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByDoctorAsync(doctorId);
            return Ok(appointments);
        }

        [HttpGet("GetDoctorAppointments/{doctorId}")]
        public async Task<IActionResult> GetDoctorAppointments(int doctorId, [FromQuery] DateTime date)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByDoctorAndDateAsync(doctorId, date);
            return Ok(appointments);
        }

        [HttpGet("Appointments")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Select(a => new
                {
                    AppointmentId = a.AppointmentID,
                    PatientName = a.Patient.User.FullName,
                    DoctorName = a.Doctor.User.FullName,
                    AppointmentDate = a.DateTime,
                    Status = a.Status,
                    Notes = a.Notes
                })
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            return Ok(appointments);
        }
        [HttpDelete("Delete/{appointmentId}")]
        [Authorize(Roles = "Patient,Doctor,Admin")]
        public async Task<IActionResult> DeleteAppointment(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                return NotFound(new { Message = "Appointment not found." });

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Appointment deleted successfully." });
        }



        [HttpDelete("Cancel/{appointmentId}")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            var result = await _appointmentRepository.CancelAppointmentAsync(appointmentId);
            return result ? Ok(new { Message = "Appointment canceled." }) : NotFound("Appointment not found.");
        }

    }
}
