using MyBiz_Core.Models;
using MyBiz_Core.RepositoryAbstracts;
using MyBiz_Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBiz_Data.RepositoryConcretes
{
    public class TeamRepository : GenericRepository<Team>, ITeamRepository
    {
        public TeamRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
