using System;

namespace CommonFunctions.ManageStructs
{
    [Serializable]
    public class ReqGetDvrPlan
    {
        private string? _mediaServerId;
        private string? _cameraId;
       
        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string? CameraId
        {
            get => _cameraId;
            set => _cameraId = value;
        }

       
    }
}