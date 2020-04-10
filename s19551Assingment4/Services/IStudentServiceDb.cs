using s19551Assingment4.DTOs;
using s19551Assingment4.DTOs.Responce;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace s19551Assingment4.Services
{
    public interface IStudentServiceDb
    {
        string EnrollStudent(EnrollStudentRequest request);
        EnrollStudentResponse PromoteStudent(string study, int semester);
    }
}
