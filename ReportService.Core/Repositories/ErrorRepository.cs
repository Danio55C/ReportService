﻿using ReportService.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ReportService.Core.Repositories
{
    public class ErrorRepository
    {
        public List<Error> GetLastErrors(int intervalInMinutess)
        {
            //pobieranie z bazy danych

            return new List<Error>
            { 
                new Error {Message="Błąd Testowy 1", Date=DateTime.Now},
                new Error {Message="Błąd Testowy 2", Date=DateTime.Now},
            };
        }
    }
}


            
            
