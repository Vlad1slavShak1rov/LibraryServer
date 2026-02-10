using LibraryServer.DbContext;
using LibraryServer.DTO;
using LibraryServer.Model;
using Microsoft.EntityFrameworkCore;

namespace LibraryServer.Service
{
    public class TeacherService
    {
        private readonly LibraryContext _context;

        public TeacherService(LibraryContext context)
        {
            _context = context;   
        }

        public async Task<TeacherDTO> GetById(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            var teacher = await _context.Teachers.
                FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null)
                throw new Exception("Teacher was not found!");

            var teacherDTO = new TeacherDTO()
            {
                UserId = teacher.UserID,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                SecondName = teacher.SecondName,
                Contact = teacher.Contact,
                IsProfileComplete = teacher.IsProfileComplete,
            };

            return teacherDTO;
        }

        public async Task<int> AddTeacher(TeacherDTO teacherDTO)
        {
            if (string.IsNullOrEmpty(teacherDTO.FirstName))
                throw new Exception("First name is required!");

            if(string.IsNullOrEmpty(teacherDTO.SecondName))
                throw new Exception("Second name is required!");

            if (string.IsNullOrEmpty(teacherDTO.LastName))
                throw new Exception("Last name is required!");

            if (string.IsNullOrEmpty(teacherDTO.Contact))
                throw new Exception("Contact is required!");

            if (teacherDTO.IsProfileComplete)
                throw new Exception("Teacher has already been registered!");
            
            if(teacherDTO.UserId == null)
                throw new Exception("User id is required!");

            var teacher = new Teacher()
            {
                UserID = teacherDTO.UserId.Value,
                FirstName = teacherDTO.FirstName,
                SecondName = teacherDTO.SecondName,
                LastName = teacherDTO.LastName,
                Contact = teacherDTO.Contact,
                IsProfileComplete = true
            };

            await _context.Teachers.AddAsync(teacher);
            await _context.SaveChangesAsync();

            return teacher.Id;
        }

        public async Task<TeacherDTO> UpdateTeacher(TeacherDTO teacherDTO)
        {
            if (string.IsNullOrEmpty(teacherDTO.FirstName))
                throw new Exception("First name is required!");

            if (string.IsNullOrEmpty(teacherDTO.SecondName))
                throw new Exception("Second name is required!");

            if (string.IsNullOrEmpty(teacherDTO.LastName))
                throw new Exception("Last name is required!");

            if (string.IsNullOrEmpty(teacherDTO.Contact))
                throw new Exception("Contact is required!");

            if (!teacherDTO.IsProfileComplete)
                throw new Exception("Teacher is not activated!");

            if (teacherDTO.UserId == null)
                throw new Exception("User id is required!");

            var teacher = await _context.Teachers.
               FirstOrDefaultAsync(t => t.UserID == teacherDTO.UserId);

            if (teacher == null)
                throw new Exception("Teacher was not registered!");

            teacher.FirstName = teacherDTO.FirstName;
            teacher.SecondName = teacherDTO.SecondName;
            teacher.LastName = teacherDTO.LastName;
            teacher.Contact = teacherDTO.Contact;

            await _context.SaveChangesAsync();
            return teacherDTO;
        }

        public async Task<bool> RemoveTeacher(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            var teacher = await _context.Teachers.
                FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null)
                throw new Exception("Teacher was not found!");

            _context.Teachers.Remove(teacher); 
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
