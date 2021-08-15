using System.Collections.Generic;
using Assets.Scripts.Models;
using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class VideoService
    {
        public List<MediaDetail> GetLocalVideos()
        {
            var returnVideos = new List<MediaDetail>();
            returnVideos.AddRange(videoDetails());

            return returnVideos;
        }

        private List<MediaDetail> videoDetails()
        {
            var returnVideos = new List<MediaDetail>
            {
                //new MediaDetail
                //{
                //    Id = 1,
                //    Title = "Video1",
                //    MediaType = MediaType.VideoClip,
                //    Source = Source.LocalFile
                //},
                //new MediaDetail
                //{
                //    Id = 2,
                //    Title = "Video2",
                //    MediaType = MediaType.VideoClip,
                //    Source = Source.Url,
                //    Url = "https://dl.dropbox.com/s/ce99hmnjw7vndw5/Video2.mov"
                //},
                //new MediaDetail
                //{
                //    Id = 3,
                //    Title = "Video3",
                //    MediaType = MediaType.VideoClip,
                //    Source = Source.Url,
                //    Url = "https://dl.dropbox.com/s/cj111w6yhe38229/Video4.mov"
                //},
                //new MediaDetail
                //{
                //    Id = 4,
                //    Title = "Video4",
                //    MediaType = MediaType.VideoClip,
                //    Source = Source.Url,
                //    Url = "https://dl.dropbox.com/s/r53i3vmgrt55t2o/streaming-video-to-different-screens-2.mp4"
                //},
                //new MediaDetail
                //{
                //    Id = 5,
                //    Title = "Video5",
                //    MediaType = MediaType.VideoClip,
                //    Source = Source.Url,
                //    Url = "https://dl.dropbox.com/s/agc0jgl9gexwr17/agora-test.mp4"
                //},
                //new MediaDetail
                //{
                //    Id = 6,
                //    Title = "Video6",
                //    MediaType = MediaType.VideoClip,
                //    Source = Source.Url,
                //    Url = "https://dl.dropbox.com/s/6d4t651ikn62zqz/screenanimations-2.mp4"
                //},
                //new MediaDetail
                //{
                //    Id = 7,
                //    Title = "Video7",
                //    MediaType = MediaType.VideoClip,
                //    Source = Source.Url,
                //    Url = "https://dl.dropbox.com/s/4ez7hvv0ywttoil/20191023_145723.mp4"
                //},
                //new MediaDetail
                //{
                //    Id = 8,
                //    Title = "Video8",
                //    MediaType = MediaType.VideoClip,
                //    Source = Source.Url,
                //    Url = "https://dl.dropbox.com/s/yd2bcf8ugwfsnp4/multiple-screens-1.mp4"
                //}
            };

            return returnVideos;
        }

    }
}