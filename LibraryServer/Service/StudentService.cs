using LibraryServer.DbContext;
using LibraryServer.DTO;
using LibraryServer.Model;
using Microsoft.EntityFrameworkCore;

namespace LibraryServer.Service
{
    public class StudentService
    {
        private readonly LibraryContext _context;

        public StudentService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<StudentDTO> GetById(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            var student = await _context.Students.
                FirstOrDefaultAsync(t => t.Id == id);

            if (student == null)
                throw new Exception("Student was not found!");

            var teacherDTO = new StudentDTO()
            {
                UserID = student.UserID,
                FirstName = student.FirstName,
                LastName = student.LastName,
                SecondName = student.SecondName,
                ClassNum = student.ClassNum,
                IsProfileComplete = student.IsProfileComplete,
            };

            return teacherDTO;
        }

        public async Task<int> AddStudent(StudentDTO studentDTO)
        {
            if (string.IsNullOrEmpty(studentDTO.FirstName))
                throw new Exception("First name is required!");

            if (string.IsNullOrEmpty(studentDTO.SecondName))
                throw new Exception("Second name is required!");

            if (string.IsNullOrEmpty(studentDTO.LastName))
                throw new Exception("Last name is required!");

            if (string.IsNullOrEmpty(studentDTO.ClassNum))
                throw new Exception("Contact is required!");

            if (studentDTO.IsProfileComplete)
                throw new Exception("Teacher has already been registered!");

            if (studentDTO.UserID == null)
                throw new Exception("User id is required!");

            var teacher = new Teacher()
            {
                UserID = studentDTO.UserID.Value,
                FirstName = studentDTO.FirstName,
                SecondName = studentDTO.SecondName,
                LastName = studentDTO.LastName,
                Contact = studentDTO.ClassNum,
                IsProfileComplete = true
            };

            await _context.Teachers.AddAsync(teacher);
            await _context.SaveChangesAsync();

            return teacher.Id;
        }

        public async Task<StudentDTO> UpdateStudent(StudentDTO studentDTO)
        {
            if (string.IsNullOrEmpty(studentDTO.FirstName))
                throw new Exception("First name is required!");

            if (string.IsNullOrEmpty(studentDTO.SecondName))
                throw new Exception("Second name is required!");

            if (string.IsNullOrEmpty(studentDTO.LastName))
                throw new Exception("Last name is required!");

            if (string.IsNullOrEmpty(studentDTO.ClassNum))
                throw new Exception("Contact is required!");

            if (!studentDTO.IsProfileComplete)
                throw new Exception("Teacher is not activated!");

            if (studentDTO.UserID == null)
                throw new Exception("User id is required!");

            var student = await _context.Students.
               FirstOrDefaultAsync(t => t.UserID == studentDTO.UserID);

            if (student == null)
                throw new Exception("Teacher was not registered!");

            student.FirstName = studentDTO.FirstName;
            student.SecondName = studentDTO.SecondName;
            student.LastName = studentDTO.LastName;

            await _context.SaveChangesAsync();
            return studentDTO;
        }

        public async Task<bool> RemoveStudent(int? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            var student = await _context.Students.
                FirstOrDefaultAsync(t => t.Id == id);

            if (student == null)
                throw new Exception("Teacher was not found!");

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
