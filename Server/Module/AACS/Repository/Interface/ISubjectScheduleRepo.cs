using Core.Interfaces;
using DataAccess.Models;

namespace AACS.Repository.Interface;

public interface ISubjectScheduleRepo : IRepository<SubjectSchedule>
{
    IQueryable<SubjectScheduleDetail?> GetByDetailIdAsync(string id);
    Task<bool> DeleteDetailAsync(string detailId);
}