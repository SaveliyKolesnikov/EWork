using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EWork.Models;

namespace EWork.ViewModels
{
    public class ProfileViewModel
    {
        public ProfileViewModel(User currentUser, string pathToProfilePhotos)
        {
            CurrentUser = currentUser;
            PathToProfilePhotos = pathToProfilePhotos;
        }

        public User CurrentUser { get; }
        public string PathToProfilePhotos { get; }
    }
}
