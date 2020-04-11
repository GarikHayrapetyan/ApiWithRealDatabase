using Microsoft.AspNetCore.Mvc;
using s19551Assingment4.DTOs;
using s19551Assingment4.Services;

namespace s19551Assingment4.Controllers
{
    [ApiController]
    [Route("api/enrollment")]
    public class EnrollmentsController:ControllerBase
    {
        private readonly IStudentServiceDb _db;

        public EnrollmentsController(IStudentServiceDb db)
        {
            _db = db;
        }

       [HttpPost]
       public IActionResult EnrollStudents(EnrollStudentRequest request)
        {
            var response = _db.EnrollStudent(request);
            if (response == null) return NotFound();
            return Ok(response);
        }



        [HttpPost("promotion")]
        public IActionResult PromoteStudents(string study, int semester)
        {

            var r = _db.PromoteStudent(study, semester);
            if (r != null)
                return Ok(r);
            else return BadRequest("Semester or study does not exists!");
        }
    }
}
