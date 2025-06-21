using GraduateProject_Core.DTO_s;
using GraduateProject_Core.Interfaces;
using GraduateProject_Core.Models;
using GraduateProject_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GraduateProject_Infrastructure.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly AppDbContext _context;

        public DoctorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<LeaveRequestDTO>> GetLeaveRequestsAsync(int doctorId)
        {
            return await _context.LeaveRequests
                .Where(lr => lr.DoctorID == doctorId)
                .OrderByDescending(lr => lr.SubmittedAt)
                .Select(lr => new LeaveRequestDTO
                {
                    LeaveRequestId = lr.RequestID,
                    DoctorId = lr.DoctorID,
                    Reason = lr.Reason,
                    StartDate = lr.StartDate,
                    EndDate = lr.EndDate,
                    SubmittedAt = (DateTime)lr.SubmittedAt,
                    Status = lr.StatusID == 1 ? "Approved"
                            : lr.StatusID == 2 ? "Rejected"
                            : "Pending"
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByDoctorAsync(int doctorId)
        {
            return await _context.LeaveRequests
                                 .Where(lr => lr.DoctorID == doctorId)
                                 .ToListAsync();
        }

        public async Task<bool> RequestLeaveAsync(LeaveRequestDTO leaveRequestDto)
        {
            try
            {
                var leaveRequest = new LeaveRequest

                {

                    DoctorID = leaveRequestDto.DoctorId,  
                    StartDate = leaveRequestDto.StartDate,
                    EndDate = leaveRequestDto.EndDate,
                    Reason = leaveRequestDto.Reason,
                    StatusID = 3,
                    SubmittedAt = DateTime.UtcNow
                };
                Console.WriteLine($"🧪 DoctorID: {leaveRequest.DoctorID}");

                await _context.LeaveRequests.AddAsync(leaveRequest);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in RequestLeaveAsync: {ex.Message}");
                return false;
            }
        }


        public async Task<PatientFullHistoryDTO> GetPatientFullHistoryAsync(int patientId)
        {
            var patient = await _context.Patients

                .Include(p => p.MedicalHistories)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Doctor)
                .FirstOrDefaultAsync(p => p.PatientID == patientId);

            if (patient == null)
                return null;

            return new PatientFullHistoryDTO
            {
                PatientId = patient.PatientID,
                FullName = patient.User.FullName,
                Age = (int)patient.User.Age,
                Gender = patient.User.Gender,
                Visits = patient.Appointments?.Select(a => new VisitDTO
                {
                    AppointmentId = a.AppointmentID,
                    DoctorName = a.Doctor?.User?.FullName ?? "Unknown",
                    AppointmentDate = a.DateTime,
                    Status = a.Status?? "Unknown"
                }).ToList(),
                MedicalHistories = patient.MedicalHistories?.Select(m => new MedicalHistoryItemDTO
                {
                    Disease = m.Diagnosis,
                    Treatment = m.Treatment,
                    Notes = m.Note,
                    RecordedAt = m.RecordedAt
                }).ToList()
            };
        }


        public async Task<bool> RequestAppointmentRescheduleAsync(RescheduleRequestDTO dto)
        {
            var appointment = await _context.Appointments.FindAsync(dto.AppointmentId);
            if (appointment == null)
                return false;

            appointment.DateTime = dto.NewDate;
            appointment.RescheduleReason = dto.Reason;
            appointment.Status = "Pending";
            await _context.SaveChangesAsync();
            return true;
        }

        //public async Task<bool> AddPatientNoteAsync(PatientNoteDTO dto)
        //{
        //    var note = new MedicalHistory
        //    {
        //        PatientID = dto.PatientId,
        //        Diagnosis = dto.Note,
        //        VisitDate = DateTime.UtcNow
        //    };

        //    await _context.MedicalHistories.AddAsync(note);
        //    await _context.SaveChangesAsync();
        //    return true;
        //}


        public async Task<bool> AddPatientNoteAsync(PatientNoteDTO dto, int doctorId)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientID == dto.PatientId);

            if (patient == null)
                throw new Exception($"❌ Patient with ID {dto.PatientId} not found in Patients table.");

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.DoctorID == doctorId);
            if (doctor == null)
                throw new Exception($"Doctor with ID {doctorId} not found.");

            var note = new MedicalHistory
            {
                PatientID = dto.PatientId,
                DoctorID = doctorId,
                Diagnosis = dto.Diagnosis,
                Treatment = dto.Treatment,
                Note = dto.Note,
                VisitDate = DateTime.UtcNow,
                RecordedAt = DateTime.UtcNow,
                FollowUpNeeded = false
            };

            await _context.MedicalHistories.AddAsync(note);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EvaluatePatientComplianceAsync(PatientComplianceDTO dto)
        {
            var patient = await _context.Patients.FindAsync(dto.PatientId);
            if (patient == null)
                return false;

            patient.ComplianceLevel = dto.ComplianceLevel;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AppointmentDTO>> GetWorkHistoryAsync(int doctorId)
        {
            return await _context.Appointments
                .Where(a => a.DoctorID == doctorId)
                .Select(a => new AppointmentDTO
                {
                    AppointmentId = a.AppointmentID,
                    PatientName = a.Patient.User.FullName,
                    AppointmentDate = a.DateTime,
                    StatusName = a.Status
                }).ToListAsync();
        }

        public async Task<DailyTaskDetailsDTO> GetDailyTasksAsync(int doctorId)
        {
            var today = DateTime.Today;

            // كل مواعيد اليوم
            var todayAppointments = await _context.Appointments
                .Where(a => a.DoctorID == doctorId && a.DateTime.Date == today)
                .Include(a => a.Patient)
                .ThenInclude(p => p.User)
                .ToListAsync();

            // طلبات إعادة الجدولة المعلقة
            var pendingReschedules = await _context.Appointments
                .Where(a => a.DoctorID == doctorId && a.RescheduleStatus == "Pending")
                .Include(a => a.Patient)
                .ThenInclude(p => p.User)
                .ToListAsync();

            // مرضى بحاجة للمتابعة
            var patientsNeedingFollowUp = await _context.MedicalHistories
                .Where(m => m.DoctorID == doctorId && m.FollowUpNeeded == true)
                .Include(m => m.Patient)
                .ThenInclude(p => p.User)
                .ToListAsync();

            return new DailyTaskDetailsDTO
            {
                AppointmentsToday = todayAppointments.Count,
                PendingReschedules = pendingReschedules.Count,
                PatientsNeedingFollowUp = patientsNeedingFollowUp.Count,

                TodayAppointmentsDetails = todayAppointments.Select(a => new AppointmentInfoDTO
                {
                    AppointmentId = a.AppointmentID,
                    PatientName = a.Patient.User.FullName,
                    AppointmentDate = a.DateTime
                }).ToList(),

                PendingReschedulesDetails = pendingReschedules.Select(a => new RescheduleRequestInfoDTO
                {
                    AppointmentId = a.AppointmentID,
                    PatientName = a.Patient.User.FullName,
                    RequestedNewDate = a.RequestedRescheduleDate ?? a.DateTime,
                    Reason = a.RescheduleReason
                }).ToList(),

                PatientsNeedingFollowUpDetails = patientsNeedingFollowUp.Select(m => new FollowUpPatientInfoDTO
                {
                    PatientId = m.PatientID,
                    PatientName = m.Patient.User.FullName,
                    DiagnosisSummary = m.Diagnosis ?? "No Diagnosis"
                }).ToList()
            };
        }


        public async Task<DoctorPerformanceDetailsDTO> GetEnhancedPerformanceReportAsync(int doctorId)
        {
            var totalAppointments = await _context.Appointments.CountAsync(a => a.DoctorID == doctorId);
            var newPatientsThisMonth = await _context.Appointments
                .CountAsync(a => a.DoctorID == doctorId && a.DateTime.Month == DateTime.Now.Month);

            var avgWaitingTime = await _context.Appointments
                .Where(a => a.DoctorID == doctorId)
                .AverageAsync(a => (double?)a.WaitingMinutes) ?? 0;

            var approvedLeaves = await _context.LeaveRequests
                .CountAsync(lr => lr.DoctorID == doctorId && lr.StatusID == 1);

            var successfulProcedures = await _context.Procedures
                .CountAsync(p => p.DoctorID == doctorId && p.Success == true);

            return new DoctorPerformanceDetailsDTO
            {
                TotalPatients = totalAppointments,
                NewPatientsThisMonth = newPatientsThisMonth,
                AverageWaitingTimeMinutes = avgWaitingTime,
                ApprovedLeavesCount = approvedLeaves,
                SuccessfulProcedures = successfulProcedures
            };
        }

        public byte[] GeneratePatientHistoryPdf(PatientFullHistoryDTO history)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.Header()
                        .Text($"Patient Report: {history.FullName}")
                        .FontSize(20)
                        .SemiBold().AlignCenter();
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Age: {history.Age}");
                        col.Item().Text($"Gender: {history.Gender}");
                        col.Item().Text($"Phone: {history.Phone}");

                        col.Item().PaddingVertical(10).LineHorizontal(1);

                        col.Item().Text("Appointments:");
                        foreach (var appointment in history.Visits)
                        {
                            col.Item().Text(
                                $"- {appointment.AppointmentDate.ToShortDateString()} with Dr. {appointment.DoctorName}, Status: {appointment.Status}");
                        }

                        col.Item().PaddingVertical(10).LineHorizontal(1);

                        col.Item().Text("Medical History:");
                        foreach (var record in history.MedicalHistories)
                        {
                            col.Item().Text(
                                $"- {record.Disease} ({record.Treatment}) at {record.RecordedAt.ToShortDateString()}");
                        }
                    });
                });
            });

            return document.GeneratePdf();
        }
        public async Task<bool> UploadReportFileAsync(UploadReportFileDTO dto)
        {
            if (dto.ReportFile != null && dto.ReportFile.Length > 0)
            {
                var uploadsPath = Path.Combine("wwwroot", "uploads");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}_{dto.ReportFile.FileName}";
                var fullPath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.ReportFile.CopyToAsync(stream);
                }

                var fileRecord = new ReportFile
                {
                    PatientID = dto.PatientId,
                    FileName = dto.ReportFile.FileName,
                    FileUrl = $"/uploads/{fileName}",
                    UploadedAt = DateTime.UtcNow,
                    Description = dto.Description,
                    Specialization = dto.Specialization  
                };

                _context.ReportFiles.Add(fileRecord);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public async Task<List<ReportFileDTO>> GetPatientReportsAsync(int patientId)
        {
            return await _context.ReportFiles
                .Where(r => r.PatientID == patientId)
                .Select(r => new ReportFileDTO
                {
                    FileUrl = r.FileUrl,
                    FileName = r.FileName,
                    Description = r.Description,
                    Specialization = r.Specialization,
                    UploadedAt = r.UploadedAt
                }).ToListAsync();
        }

        public async Task<int> GetUserIdByDoctorIdAsync(int doctorId)
        {
            var user = await _context.Doctors
                .Where(d => d.DoctorID == doctorId)
                .Select(d => d.UserId)
                .FirstOrDefaultAsync();
            return user;
        }

        public async Task<IEnumerable<UserSimpleDTO>> GetMyChatPatientsAsync(int doctorId, int doctorUserId)
        {
            var fromAppointments = await _context.Appointments
                .Where(a => a.DoctorID == doctorId)
                .Select(a => a.Patient.User)
                .ToListAsync();

            var fromMessages = await _context.Messages
                .Where(m => m.ReceiverUserID == doctorUserId)
                .Select(m => m.Sender)
                .ToListAsync();

            var combined = fromAppointments
                .Concat(fromMessages)
                .GroupBy(p => p.Id)
                .Select(g => g.First())
                .Select(p => new UserSimpleDTO
                {
                    UserID = p.Id,
                    FullName = p.FullName
                });

            return combined;
        }

        public async Task<bool> UploadPatientReportAsync(UploadReportDTO dto)
        {
            if (dto.ReportFile != null && dto.ReportFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await dto.ReportFile.CopyToAsync(memoryStream);
                    var report = new PatientReport
                    {
                        PatientID = dto.PatientId,
                        FileName = dto.ReportFile.FileName,
                        ContentType = dto.ReportFile.ContentType,
                        Data = memoryStream.ToArray(),
                        UploadedAt = DateTime.UtcNow,
                        Description = dto.Description
                    };

                    await _context.PatientReports.AddAsync(report);
                    await _context.SaveChangesAsync();
                }
            }

            return true;
        }
        public async Task<IEnumerable<AppointmentBasicDTO>> GetAppointmentsForDoctorAsync(int doctorId)
        {
            return await _context.Appointments
                .Where(a => a.DoctorID == doctorId)
                .Include(a => a.Patient)
                .ThenInclude(p => p.User)
                .Select(a => new AppointmentBasicDTO
                {
                    AppointmentId = a.AppointmentID,
                    AppointmentDate = a.DateTime,
                    StatusID = a.StatusID,  
                    PatientName = a.Patient.User.FullName
                })
                .ToListAsync();
        }


        public async Task<List<PatientBasicInfoDTO>> GetMyPatientsAsync(int doctorId)
        {
            var patients = await _context.Appointments
                .Where(a => a.DoctorID == doctorId)
                .Select(a => a.Patient)
                .Distinct()
                .Include(p => p.User)
                .ToListAsync();

            return patients.Select(p => new PatientBasicInfoDTO
            {
                PatientId = p.PatientID,
                FullName = p.User.FullName,
                Gender = p.User.Gender
            }).ToList();
        }

        public async Task<PatientFullHistoryDTO> GetFullPatientHistoryAsync(int patientId)
        {
            var patient = await _context.Patients
                .Include(p => p.User)  
                .Include(p => p.MedicalHistories)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Doctor)
                        .ThenInclude(d => d.User)  
                .FirstOrDefaultAsync(p => p.PatientID == patientId);

            if (patient == null)
                return null;

            return new PatientFullHistoryDTO
            {
                PatientId = patient.PatientID,
                FullName = patient.User.FullName,
                Age = (int)(patient.User.Age ?? 0),
                Gender = patient.User.Gender,
                Visits = patient.Appointments?.Select(a => new VisitDTO
                {
                    AppointmentId = a.AppointmentID,
                    DoctorName = a.Doctor?.User?.FullName ?? "Unknown",
                    AppointmentDate = a.DateTime,
                    Status = a.Status ?? "Unknown"
                }).ToList(),
                MedicalHistories = patient.MedicalHistories?.Select(h => new MedicalHistoryItemDTO
                {
                    Disease = h.Diagnosis,
                    Treatment = h.Treatment,
                    Notes = h.Note,
                    RecordedAt = h.RecordedAt
                }).ToList()
            };
        }

    }
}
