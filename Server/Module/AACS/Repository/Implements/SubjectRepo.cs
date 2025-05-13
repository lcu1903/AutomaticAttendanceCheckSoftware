using AACS.Repository.Interface;
using Core.Bus;
using Core.Repository;
using DataAccess.Contexts;
using DataAccess.Models;

namespace AACS.Repository.Implements;
public class SubjectRepo : Repository<Subject>, ISubjectRepo
{

    private readonly ApplicationDbContext _context;
    private readonly IMediatorHandler _bus;

    public SubjectRepo(ApplicationDbContext context,
        IMediatorHandler bus)
        : base(context)
    {
        _context = context;
        _bus = bus;
    }

}