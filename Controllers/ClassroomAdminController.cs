using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WestcoastEducation.Web.Data;
using WestcoastEducation.Web.Models;

namespace WestcoastEducation.Web.Controllers;

[Route("classroomadmin")]
public class ClassroomAdminController : Controller
{
    private readonly WestcoastEducationContext _context;
    public ClassroomAdminController(WestcoastEducationContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            // Link view to the database
            var classrooms = await _context.Classrooms.ToListAsync();
            return View("Index", classrooms);
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

    [HttpGet("create")]
    public IActionResult Create()
    {
        var classroom = new Classroom();
        return View("Create", classroom);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(Classroom classroom)
    {
        try
        {
             
            var exists = await _context.Classrooms.SingleOrDefaultAsync(
                c => c.Title.Trim().ToUpper() == classroom.Title.Trim().ToUpper());

            
            if (exists is not null)
            {
                var error = new ErrorModel
                {
                    ErrorTitle = "Error",
                    ErrorMessage = $"This course {classroom.Title} is already regestered in the system."
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

        await _context.Classrooms.AddAsync(classroom);
       
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("edit/{classroomId}")]
    public async Task<IActionResult> Edit(int classroomId)
    {
        try
        {

            var classroom = await _context.Classrooms.SingleOrDefaultAsync(c => c.ClassroomId == classroomId);

            if (classroom is not null) return View("Edit", classroom);

            var error = new ErrorModel
            {
                ErrorTitle = "Error",
                ErrorMessage = $"there is no course with id {classroomId}"
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

    [HttpPost("edit/{classroomId}")]
    public async Task<IActionResult> Edit(int classroomId, Classroom classroom)
    {
        try
        {
            
            var classroomToUpdate = _context.Classrooms.SingleOrDefault(c => c.ClassroomId == classroomId);

            if (classroomToUpdate is null) return RedirectToAction(nameof(Index));

            classroomToUpdate.Name = classroom.Name;
            classroomToUpdate.Title = classroom.Title;
            classroomToUpdate.Start = classroom.Start;
            classroomToUpdate.End = classroom.End;

           
            _context.Classrooms.Update(classroomToUpdate);

            
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

    [Route("delete/{classroomId}")]
    public async Task<IActionResult> Delete(int classroomId)
    {
        try
        {
             
            var classroomToDelete = await _context.Classrooms.SingleOrDefaultAsync(c => c.ClassroomId == classroomId);

            if (classroomToDelete is null) return RedirectToAction(nameof(Index));
            
            _context.Classrooms.Remove(classroomToDelete);

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