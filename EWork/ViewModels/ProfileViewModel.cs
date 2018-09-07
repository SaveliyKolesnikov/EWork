using System;
using EWork.Models;

namespace EWork.ViewModels
{
    public class ProfileViewModel
    {
        public ProfileViewModel(User profileOwner, User currentUser, string pathToProfilePhotos, bool isCanAddReview) :
            this(profileOwner, currentUser, pathToProfilePhotos) =>
            IsCanAddReview = isCanAddReview;

        public ProfileViewModel(User profileOwner, User currentUser, string pathToProfilePhotos)
        {
            ProfileOwner = profileOwner ?? throw new ArgumentNullException(nameof(profileOwner));
            CurrentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            PathToProfilePhotos = pathToProfilePhotos;
        }

        public User ProfileOwner { get; }
        public User CurrentUser { get; }
        public string PathToProfilePhotos { get; }
        public bool IsCanAddReview { get; }
        public Review Review { get; set; }
    }
}

