using DohrniiBackoffice.Domain.Abstract;
using DohrniiBackoffice.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DohrniiBackoffice.Domain.Contract
{
    public class EFLessonRepository : GenericRepository<Lesson>, ILessonRepository
    {
        public EFLessonRepository(DohrniiBackOfficeContext context) : base(context)
        {

        }
    }
}
