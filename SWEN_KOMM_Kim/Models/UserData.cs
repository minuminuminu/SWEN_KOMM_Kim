﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.Models
{
    internal class UserData
    {
        public string Name { get; set; }
        public string Bio {  get; set; }
        public string Image { get; set; }

        public UserData(string name, string bio, string image)
        {
            Name = name;
            Bio = bio;
            Image = image;
        }
    }
}
