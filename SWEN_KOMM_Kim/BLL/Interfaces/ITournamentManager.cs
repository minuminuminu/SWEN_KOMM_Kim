﻿using SWEN_KOMM_Kim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.BLL.Interfaces
{
    internal interface ITournamentManager
    {
        List<TournamentEntry> GetUserHistory(string authToken);
    }
}
