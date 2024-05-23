using Microsoft.AspNetCore.Hosting;
using MyBiz_Business.Exceptions;
using MyBiz_Business.Services.Abstracts;
using MyBiz_Core.Models;
using MyBiz_Core.RepositoryAbstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBiz_Business.Services.Concretes
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TeamService(ITeamRepository teamRepository, IWebHostEnvironment webHostEnvironment)
        {
            _teamRepository = teamRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public void AddTeam(Team team)
        {
            if (team == null)
                throw new NullReferenceException("Team not found");

            if (team.ImgFile == null)
                throw new ImageRequiredException("ImgFile", "Image is required");

            if (!team.ImgFile.ContentType.Contains("image/"))
                throw new FileContentTypeException("ImgFile", "File content type error");
            if (team.ImgFile.Length > 2097152)
                throw new FileSizeException("ImgFile", "fiile size error");

            string fileName = team.ImgFile.FileName;
            string path = _webHostEnvironment.WebRootPath + @"\upload\team\" + fileName;
            using(FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                team.ImgFile.CopyTo(fileStream);
            }
            team.ImgUrl = fileName;

            _teamRepository.Add(team);
            _teamRepository.Commit();
        }

        public void DeleteTeam(int id)
        {
            var existTeam = _teamRepository.Get(x =>  x.Id == id);
            if (existTeam == null)
                throw new EntityNotFoundException("", "Entity not found");

            string path = _webHostEnvironment.WebRootPath + @"\upload\team\" + existTeam.ImgUrl;
            if (!File.Exists(path))
                throw new Exceptions.FileNotFoundException("ImgFile", "File not found");

            File.Delete(path);

            _teamRepository.Delete(existTeam);
            _teamRepository.Commit();
        }

        public List<Team> GetAllTeams(Func<Team, bool>? func = null)
        {
            return _teamRepository.GetAll(func);
        }

        public Team GetTeam(Func<Team, bool>? func = null)
        {
            return _teamRepository.Get(func);
        }

        public void UpdateTeam(int id, Team team)
        {
            var existTeam = _teamRepository.Get(x => x.Id == id);
            if (existTeam == null)
                throw new EntityNotFoundException("", "Entity not found");

            if(team.ImgFile != null)
            {

                if (!team.ImgFile.ContentType.Contains("image/"))
                    throw new FileContentTypeException("ImgFile", "File content type error");
                if (team.ImgFile.Length > 2097152)
                    throw new FileSizeException("ImgFile", "fiile size error");

                string fileName = team.ImgFile.FileName;
                string path = _webHostEnvironment.WebRootPath + @"\upload\team\" + fileName;
                using (FileStream fileStream = new FileStream(path, FileMode.Create))
                {
                    team.ImgFile.CopyTo(fileStream);
                }
                team.ImgUrl = fileName;

                existTeam.ImgUrl = team.ImgUrl;
            }

            existTeam.Name = team.Name;
            existTeam.Position = team.Position;
            existTeam.Description = team.Description;

            _teamRepository.Commit();
        }
    }
}
