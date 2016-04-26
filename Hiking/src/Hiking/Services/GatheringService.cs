﻿using Hiking.Models;
using Hiking.Repositories;
using Hiking.ViewModels.Gatherings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hiking.Services
{
    public class GatheringService : IGatheringService
    {
        private IGenericRepository repo;

        public GatheringService(IGenericRepository _repo)
        {
            this.repo = _repo;
        }

        public List<Gathering> GetAllGatherings()
        {
            var list = repo.Query<Gathering>().Where(g => g.Time >= DateTime.Now).OrderBy(g => g.Time).ToList();
            //List<Gathering> newList = new List<Gathering>();
            //foreach (var item in list)
            //{
            //    if (item.Time >= DateTime.Now)
            //    {
            //        newList.Add(item);
            //    }
            //}
            return list;
        }

        public GatheringViewModel GetOneGathering(int id)
        {
            var gathering = repo.Query<Gathering>().Where(g => g.Id == id).Include(g => g.GatheringUsers).FirstOrDefault();
            var data = new GatheringViewModel {
                Id = gathering.Id,
                Description = gathering.Description,
                Name = gathering.Name,
                OwnerId = gathering.OwnerId,
                Time = gathering.Time,
                Users = repo.Query<GatheringUsers>().Where(gu => gu.GatheringID == id).Select(gu => new GatheringUserViewModel {
                    DisplayName = gu.ApplicationUser.DisplayName,
                    FirstName = gu.ApplicationUser.FirstName,
                    LastName = gu.ApplicationUser.LastName,
                    UserId = gu.ApplicationUser.Id
                }).ToList()
            };
            return data;
        }

        public void SaveGathering(Gathering data)
        {
            return;
        }

        public void DeleteGathering(int id)
        {
            return;
        }

        public void AddToGathering(AddUserToGatherViewModel data)
        {
            //var user = repo.Query<ApplicationUser>().Where(u => u.Id == id).FirstOrDefault();
            repo.Add<GatheringUsers>(new GatheringUsers
            {
                ApplicationUserId = data.UserID,
                GatheringID = data.GatherID
            });
            repo.SaveChanges();
        }

        public UserInGatheringViewModel IsUserInGathering(AddUserToGatherViewModel data)
        {
            //var check = repo.Query<GatheringUsers>().Where(gu => gu.ApplicationUserId == data.UserID).Any(gu => gu.GatheringID == data.GatherID);
            //var check = repo.Query<Gathering>().Any(g => g.Id == data.GatherID && g.GatheringUsers.Any(gu => gu.ApplicationUserId == data.UserID));

            var check = new UserInGatheringViewModel
            {
                Check = false
            };
            var users = repo.Query<GatheringUsers>().ToList();
            foreach (var user in users)
            {
                if (data.GatherID == user.GatheringID && data.UserID == user.ApplicationUserId)
                {
                    check.Check = true;
                }
            }

            return check;
        }

        public void RemoveFromGathering(AddUserToGatherViewModel data)
        {
            var model = new GatheringUsers
            {
                ApplicationUserId = data.UserID,
                GatheringID = data.GatherID
            };
            repo.Delete<GatheringUsers>(model);
            repo.SaveChanges();
        }
    }
}
