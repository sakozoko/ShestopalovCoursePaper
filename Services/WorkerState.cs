using CourseWork.Models;

namespace CourseWork.Services;

public class WorkerState
{
    public WorkerResult WorkerResult { get; init; }
    public User? User { get; init; }
}