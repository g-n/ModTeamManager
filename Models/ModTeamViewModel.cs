﻿namespace ModTeamManager.Models
{
    public class ModTeamViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
