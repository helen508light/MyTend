﻿namespace MyTend.Models
{
    using MyTend.Entites;
    using MyTend.Services.Common;
    using System.Collections.Generic;

    public class SubRegionModel
    {
        public List<string> Citys { get; set; }

        public List<string> Regions { get; set; }

        public SubRegionModel()
        {
            this.Citys = new List<string>();
            this.Regions = new List<string>();
        }

        public void Save(UserSystem user)
        {
            var filter = new RegionFilterService(user);

            filter.Save(this.Citys, this.Regions);
        }
    }
}
