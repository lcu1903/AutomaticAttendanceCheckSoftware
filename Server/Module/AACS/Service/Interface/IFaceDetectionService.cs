using System.Models;
using Core.Interfaces;

namespace AACS.Service.Interface
{
    public interface IFaceDetectionService : IDomainService
    {
        UserRes? FaceRecognitionAsync(string base64Img, string subjectScheduleDetailId);
    }
}
