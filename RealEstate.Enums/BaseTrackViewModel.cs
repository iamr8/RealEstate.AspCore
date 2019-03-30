using RealEstate.Base.Enums;
using System;
using System.Collections.Generic;

namespace RealEstate.Base
{
    public class BaseTrackViewModel : BaseViewModel
    {
        public List<TrackViewModel> Tracks { get; set; }
    }

    public class TrackViewModel
    {
        public TrackTypeEnum Type { get; set; }
        public DateTime DateTime { get; set; }
        public string Id { get; set; }
        public TrackUserViewModel User { get; set; }
    }

    public class TrackUserViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Mobile { get; set; }

        public Role Role { get; set; }
    }
}