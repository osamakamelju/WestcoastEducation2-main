using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WestcoastEducation.Web.Data;
using WestcoastEducation.Web.Models;

namespace WestcoastEducation.Web.Controllers;

[Route("useradmin")]
public class UserAdminController : Controller
{
    private readonly WestcoastEducationContext _context;
    public UserAdminController(WestcoastEducationContext context)
    {
        _context = context;
    }

    private async Task<List<User>> GetUsers()
    {
        var students = await _context.Students.ToListAsync();
        var teachers = await _context.Teachers.ToListAsync();

        List<User> users = new List<User>();
        users.AddRange(teachers);
        users.AddRange(students);
        return users;
    }

    public async Task<IActionResult> Index()
    {
        List<User> users = await GetUsers();
        return View("Index", users);
    }

    [Route("details/{userId}")]
    public async Task<IActionResult> Details(int userId)
    {
        List<User> users = await GetUsers();
        User? user = users.FirstOrDefault(u => u.UserId == userId);

        if (user is null) return View(nameof(Index));

        return View("Details", user);
    }

    [HttpGet("create-teacher")]
    public IActionResult CreateTeacher()
    {
        var teacher = new Teacher();
        return View("CreateTeacher", teacher);
    }

    [HttpPost("create-teacher")]
    public async Task<IActionResult> CreateTeacher(Teacher teacher)
    {
        try
        {
            var exists = await _context.Teachers.SingleOrDefaultAsync(
                t => t.SocialSecurityNumber.Trim().ToLower() == teacher.SocialSecurityNumber.Trim().ToLower());

            if (exists is not null)
            {
                var error = new ErrorModel
                {
                    ErrorTitle = "Error",
                    ErrorMessage = $"The teacher with social Security Number {teacher.SocialSecurityNumber} is already registered in the system"
                };

                return View("_Error", error);
            }
        }        
        catch (Exception ex)
        {
            var error = new ErrorModel
            {
                ErrorTitle = "Error",
                ErrorMessage = ex.Message
            };

            return View("_Error", error);
        }


        await _context.Teachers.AddAsync(teacher);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("create-student")]
    public IActionResult CreateStudent()
    {
        var student = new Student();
        return View("CreateStudent", student);
    }

    [HttpPost("create-student")]
    public async Task<IActionResult> CreateStudent(Student student)
    {
        try
        { 
            var exists = await _context.Students.SingleOrDefaultAsync(
                s => s.SocialSecurityNumber.Trim().ToLower() == student.SocialSecurityNumber.Trim().ToLower());

            if (exists is not null)
            {
                var error = new ErrorModel
                {
                    ErrorTitle = "Error",
                    ErrorMessage = $"The student with Social Security Number {student.SocialSecurityNumber} is already registered in the system"
                };

                
                return View("_Error", error);
            }
        }
        catch (Exception ex)
        {
            var error = new ErrorModel
            {
                ErrorTitle = "Error",
                ErrorMessage = ex.Message
            };

            return View("_Error", error);
        }


        await _context.Students.AddAsync(student);        
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("edit/{userId}")]
    public async Task<IActionResult> Edit(int userId)
    {
        try
        {
            List<User> users = await GetUsers();
            User? user = users.FirstOrDefault(u => u.UserId == userId);

            if (user is not null) return View("Edit", user);

            var error = new ErrorModel
            {
                ErrorTitle = "Error",
                ErrorMessage = $"Hittar ingen användare med id {userId}"
            };

            return View("_Error", error);
        }
        catch (Exception ex)
        {
            var error = new ErrorModel
            {
                ErrorTitle = "Error",
                ErrorMessage = ex.Message
            };

            return View("_Error", error);
        }
    }

    [HttpPost("edit/{userId}")]
    public async Task<IActionResult> Edit(int userId, User user)
    {
        try
        {
            List<User> users = await GetUsers();
            User? userToUpdate = users.FirstOrDefault(u => u.UserId == userId);

            if (userToUpdate is null) return RedirectToAction(nameof(Index));

            userToUpdate.Email = user.Email;
            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.SocialSecurityNumber = user.SocialSecurityNumber;
            userToUpdate.StreetAddress = user.StreetAddress;
            userToUpdate.PostalCode = user.PostalCode;
            userToUpdate.Phone = user.Phone;

            //uppdatera en kurs via ef 
            if (userToUpdate.GetType() == typeof(Teacher))
            {
                _context.Teachers.Update(userToUpdate as Teacher);
            }
            else
            {
                _context.Students.Update(userToUpdate as Student);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
        catch (Exception ex)
        {
            var error = new ErrorModel
            {
                ErrorTitle = "Error",
                ErrorMessage = ex.Message
            };

            return View("_Error", error);
        }
    }

    [Route("delete/{userId}")]
    public async Task<IActionResult> Delete(int userId)
    {
        try
        {
            List<User> users = await GetUsers();
            User? userToDelete = users.FirstOrDefault(c => c.UserId == userId);

            if (userToDelete is null) return RedirectToAction(nameof(Index));

            if (userToDelete.GetType() == typeof(Teacher))
            {
                _context.Teachers.Remove(userToDelete as Teacher);
            }
            else
            {
                _context.Students.Remove(userToDelete as Student);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            var error = new ErrorModel
            {
                ErrorTitle = "Error",
                ErrorMessage = ex.Message
            };

            return View("_Error", error);
        }
    }
}
