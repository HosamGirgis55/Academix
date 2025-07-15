using Academix.Application.Common.Interfaces;
using Academix.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Academix.Infrastructure.Services
{
    public class PointsService : IPointsService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ILogger<PointsService> _logger;

        public PointsService(
            IStudentRepository studentRepository,
            ITeacherRepository teacherRepository,
            ILogger<PointsService> logger)
        {
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _logger = logger;
        }

        public async Task<bool> AddPointsToUserAsync(string userId, int points)
        {
            try
            {
                // First check if user is a student
                var student = await _studentRepository.GetStudentByUserIdAsync(userId);
                if (student != null)
                {
                    student.Points += points;
                    await _studentRepository.UpdateAsync(student);
                    _logger.LogInformation($"Added {points} points to student {userId}. New balance: {student.Points}");
                    return true;
                }

                // If not a student, check if user is a teacher
                var teacher = await _teacherRepository.GetTeacherByUserIdAsync(userId);
                if (teacher != null)
                {
                    teacher.Points += points;
                    await _teacherRepository.UpdateAsync(teacher);
                    _logger.LogInformation($"Added {points} points to teacher {userId}. New balance: {teacher.Points}");
                    return true;
                }

                _logger.LogWarning($"User {userId} not found as student or teacher");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding points to user {userId}");
                return false;
            }
        }

        public async Task<int> GetUserPointsAsync(string userId)
        {
            try
            {
                // First check if user is a student
                var student = await _studentRepository.GetStudentByUserIdAsync(userId);
                if (student != null)
                {
                    return student.Points;
                }

                // If not a student, check if user is a teacher
                var teacher = await _teacherRepository.GetTeacherByUserIdAsync(userId);
                if (teacher != null)
                {
                    return teacher.Points;
                }

                _logger.LogWarning($"User {userId} not found as student or teacher");
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting points for user {userId}");
                return 0;
            }
        }

        public async Task<bool> DeductPointsFromUserAsync(string userId, int points)
        {
            try
            {
                // First check if user is a student
                var student = await _studentRepository.GetStudentByUserIdAsync(userId);
                if (student != null)
                {
                    if (student.Points >= points)
                    {
                        student.Points -= points;
                        await _studentRepository.UpdateAsync(student);
                        _logger.LogInformation($"Deducted {points} points from student {userId}. New balance: {student.Points}");
                        return true;
                    }
                    _logger.LogWarning($"Student {userId} has insufficient points. Required: {points}, Available: {student.Points}");
                    return false;
                }

                // If not a student, check if user is a teacher
                var teacher = await _teacherRepository.GetTeacherByUserIdAsync(userId);
                if (teacher != null)
                {
                    if (teacher.Points >= points)
                    {
                        teacher.Points -= points;
                        await _teacherRepository.UpdateAsync(teacher);
                        _logger.LogInformation($"Deducted {points} points from teacher {userId}. New balance: {teacher.Points}");
                        return true;
                    }
                    _logger.LogWarning($"Teacher {userId} has insufficient points. Required: {points}, Available: {teacher.Points}");
                    return false;
                }

                _logger.LogWarning($"User {userId} not found as student or teacher");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deducting points from user {userId}");
                return false;
            }
        }

        public async Task<bool> HasSufficientPointsAsync(string userId, int requiredPoints)
        {
            var currentPoints = await GetUserPointsAsync(userId);
            return currentPoints >= requiredPoints;
        }
    }
} 