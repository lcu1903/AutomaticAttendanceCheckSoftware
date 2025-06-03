using Microsoft.AspNetCore.Mvc;
using AACS.Service.Interface;
using DataAccess.Models;
using Infrastructure.Models;
using System.Text.Json;
using Core.Controller;
using Microsoft.AspNetCore.Authorization;
using Core.Bus;
using Core.Notifications;
using MediatR;
using System.Models;

namespace AACS.Controller
{
    [Route("api/face-detection")]
    [Authorize]
    public class FaceDetectionController : ApiController
    {
        private readonly IFaceDetectionService _faceDetectionService;

        public FaceDetectionController(IFaceDetectionService faceDetectionService,
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler bus) : base(notifications, bus)
        {
            _faceDetectionService = faceDetectionService;
        }


        [HttpPost("check")]
        [ProducesResponseType(typeof(ResponseModel<UserRes?>), StatusCodes.Status200OK)]
        public IActionResult CheckFace([FromBody] FaceCheckReq request)
        {
            if (string.IsNullOrEmpty(request.ImageData))
                return Response();
            var res = _faceDetectionService.FaceRecognitionAsync(request.ImageData, request.SubjectScheduleDetailId);

            return Response(res);
        }
    }
}
