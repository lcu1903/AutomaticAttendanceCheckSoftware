using Core.Interfaces;
using DataAccess.Models;

namespace AACS.Repository.Interface;

public interface IAttendanceRepo : IRepository<Attendance>
{
    public bool AddAttendanceFromFaceRecognition(Attendance attendance, string? subjectScheduleDetailId);
}