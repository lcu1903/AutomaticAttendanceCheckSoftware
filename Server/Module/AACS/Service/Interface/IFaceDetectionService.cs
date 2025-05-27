using System.Models;
using Core.Interfaces;

namespace AACS.Service.Interface
{
    public interface IFaceDetectionService : IDomainService
    {
        string? RecognizeUser(byte[] imageBytes);
        UserRes? FaceRecognitionAsync(byte[] imageData, string subjectScheduleId);
    }
}
