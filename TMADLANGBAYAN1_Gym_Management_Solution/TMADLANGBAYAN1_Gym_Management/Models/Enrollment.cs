﻿namespace TMADLANGBAYAN1_Gym_Management.Models
{
    public class Enrollment
    {
        public int ClientID { get; set; }
        public Client? Client { get; set; }

        public int GroupClassID { get; set; }
        public GroupClass? GroupClass { get; set; }
    }
}
