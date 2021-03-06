﻿using Bannerflow.Api.Models;
using System;
using System.Linq;

namespace Bannerflow.Api.Extensions
{
    public static class BannerExtensions
    {
        public static BannerDto KeepProperties(this Banner banner, string[] fields)
        {
            var bannerDto = new BannerDto();
            Guid? bannerId = banner.Id;
            bannerDto.Id = fields.Any(f => f.Equals("id", StringComparison.InvariantCultureIgnoreCase)) ? bannerId : null;

            DateTime? bannerCreated = banner.Created;
            bannerDto.Created = fields.Any(f => f.Equals("created", StringComparison.InvariantCultureIgnoreCase)) ? bannerCreated : null;

            string bannerHtml = banner.Html;
            bannerDto.Html = fields.Any(f => f.Equals("html", StringComparison.InvariantCultureIgnoreCase)) ? bannerHtml : null;

            DateTime? bannerModified = banner.Modified;
            bannerDto.Modified = fields.Any(f => f.Equals("modified", StringComparison.InvariantCultureIgnoreCase)) ? bannerModified : null;

            return bannerDto;
        }
    }
}
