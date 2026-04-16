namespace LibraryServer.Enums
{
    public enum Role
    {
        Librarian = 0,
        Teacher = 1,
        Student = 2,
    }

    public enum RentStatus
    {
        Active = 0, //Активна
        Pass = 1, //Сдана
        Expired = 2 //Просрочена
    }

    public enum BookingStatus
    {
        Pending = 0,   // Забронирована, ждет выдачи
        Issued = 1,    // Выдана
        Cancelled = 2,  // Отменена
        Returned = 3,  // Возвращена
    }

    public enum Subject
    {
        RussianLand = 0, // Русский язык
        Literature = 1, //Литература
    }
}
