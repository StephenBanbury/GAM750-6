using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Enums;
using Assets.Scripts.Models;
using Proyecto26;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class ApiService
    {
        public List<MediaDetail> VideosGet()
        {
            try
            {
                var uri = "https://not-near-enough.firebaseio.com/";
                var videoList = new List<MediaDetail>();

                RestClient.GetArray<MediaDetail>(uri + ".json").Then(response =>
                {
                    if (response != null)
                    {
                        Debug.Log($"VideosGet: {response}");
                        var currentVideos = MediaDisplayManager.instance.Videos;
                        var i = currentVideos.Count + 1;

                        string[] formats =
                        {
                            ".mp4", ".mov", ".3GP", ".asf", ".avi", ".dv", ".m4v", ".mpg", ".mpeg", ".ogv", ".vp8",
                            ".webm", ".wmv"
                        };

                        foreach (var video in response.Where(v => (v.Title != null) && formats.Any(v.Url.Contains)))
                        {
                            video.Url = video.Url.Replace("www.dropbox.com", "dl.dropbox.com").Replace("?dl=0", "");

                            var startPos = video.Url.LastIndexOf("/");
                            var fileName = video.Url.Substring(startPos + 1, video.Url.Length - startPos - 1);
                            video.Filename = fileName.Replace("%20", " ");

                            int spacePos = video.Title.IndexOf(" ", StringComparison.Ordinal);
                            string idText = video.Title.Substring(0, spacePos + 1).Trim();
                            
                            bool isInt = int.TryParse(idText, out var id);

                            if (isInt)
                            {
                                Debug.Log($"VideosGet: {video.Title} / {video.Filename} / {video.Url} / ID: {id}");

                                video.MediaType = MediaType.VideoClip;
                                video.Source = Source.Url;
                                video.Id = id;
                                videoList.Add(video);
                            }
                            else
                            {
                                Debug.Log($"Id for video clip {video.Title} is in an incorrect format");
                            }

                            i++;
                        }
                    }

                    return videoList;
                });

                return videoList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}