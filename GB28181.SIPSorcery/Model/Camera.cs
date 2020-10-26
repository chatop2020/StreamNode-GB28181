using System;

namespace GB28181.Sys.Model
{
    [Serializable]
    public enum CameraType
    {
        /// <summary>
        /// GB28181设备
        /// </summary>
        GB28181,
        /// <summary>
        /// RTSP设备
        /// </summary>
        Rtsp,
        /// <summary>
        /// 直播设备
        /// </summary>
        LiveCast,
        /// <summary>
        /// 未知设备
        /// </summary>
        None,
    }

    [Serializable]
    public class Camera
    {
        public long ID { get; set; }

        public CameraType Ctype { get; set; }
        public string ChannelID { get; set; }
        public string Name { get; set; }
        public string DeviceID { get; set; }
        public long? NvrID { get; set; }
        public string Status { get; set; }
        public long RecordStatus { get; set; }
        public long? FrameRate { get; set; }
        public string AudioFomate { get; set; }
        public string VideoFomate { get; set; }
        public long? RealStreamType { get; set; }
        public long? Cache { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Adddress { get; set; }
        public long IsPTZ { get; set; }
        public DateTime EndTime { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Owner { get; set; }
        public string CivilCode { get; set; }
        public string Block { get; set; }
        public long Parental { get; set; }
        public long AccessType { get; set; }
        public string ParentID { get; set; }
        public long? SafetyWay { get; set; }
        public long RegisterWay { get; set; }
        public string CertNum { get; set; }
        public long Certifiable { get; set; }
        public long ErrCode { get; set; }
        public long Secrecy { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
        public long? PTZType { get; set; }
        public long? PositionType { get; set; }
        public long? RoomType { get; set; }
        public long? UserType { get; set; }
        public long? SupplyLightType { get; set; }
        public long? DirectionType { get; set; }
        public string Resolution { get; set; }
        public string BusinessGroupID { get; set; }
        public string DownloadSpeed { get; set; }
        public long? SVCSpaceSupportMode { get; set; }
        public long? SVCTimeSupportMode { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public string RtspMain { get; set; }
        public string RtspSub { get; set; }
        public string SubordinatePlatform { get; set; }
        public string RtmpStreamKey { get; set; }
        public string ExAttribute { get; set; }
        public int? VoiceTwoWay { get; set; }


        public override string ToString()
        {
            return
                $"{nameof(ID)}: {ID}, {nameof(ChannelID)}: {ChannelID}, {nameof(Name)}: {Name}, {nameof(DeviceID)}: {DeviceID}, {nameof(NvrID)}: {NvrID}, {nameof(Status)}: {Status}, {nameof(RecordStatus)}: {RecordStatus}, {nameof(FrameRate)}: {FrameRate}, {nameof(AudioFomate)}: {AudioFomate}, {nameof(VideoFomate)}: {VideoFomate}, {nameof(RealStreamType)}: {RealStreamType}, {nameof(Cache)}: {Cache}, {nameof(Longitude)}: {Longitude}, {nameof(Latitude)}: {Latitude}, {nameof(Adddress)}: {Adddress}, {nameof(IsPTZ)}: {IsPTZ}, {nameof(EndTime)}: {EndTime}, {nameof(Manufacturer)}: {Manufacturer}, {nameof(Model)}: {Model}, {nameof(Owner)}: {Owner}, {nameof(CivilCode)}: {CivilCode}, {nameof(Block)}: {Block}, {nameof(Parental)}: {Parental}, {nameof(AccessType)}: {AccessType}, {nameof(ParentID)}: {ParentID}, {nameof(SafetyWay)}: {SafetyWay}, {nameof(RegisterWay)}: {RegisterWay}, {nameof(CertNum)}: {CertNum}, {nameof(Certifiable)}: {Certifiable}, {nameof(ErrCode)}: {ErrCode}, {nameof(Secrecy)}: {Secrecy}, {nameof(IPAddress)}: {IPAddress}, {nameof(Port)}: {Port}, {nameof(Password)}: {Password}, {nameof(PTZType)}: {PTZType}, {nameof(PositionType)}: {PositionType}, {nameof(RoomType)}: {RoomType}, {nameof(UserType)}: {UserType}, {nameof(SupplyLightType)}: {SupplyLightType}, {nameof(DirectionType)}: {DirectionType}, {nameof(Resolution)}: {Resolution}, {nameof(BusinessGroupID)}: {BusinessGroupID}, {nameof(DownloadSpeed)}: {DownloadSpeed}, {nameof(SVCSpaceSupportMode)}: {SVCSpaceSupportMode}, {nameof(SVCTimeSupportMode)}: {SVCTimeSupportMode}, {nameof(CreateTime)}: {CreateTime}, {nameof(UpdateTime)}: {UpdateTime}, {nameof(RtspMain)}: {RtspMain}, {nameof(RtspSub)}: {RtspSub}, {nameof(SubordinatePlatform)}: {SubordinatePlatform}, {nameof(RtmpStreamKey)}: {RtmpStreamKey}, {nameof(ExAttribute)}: {ExAttribute}, {nameof(VoiceTwoWay)}: {VoiceTwoWay}";
        }
    }
}