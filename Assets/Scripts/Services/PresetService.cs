using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Enums;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class PresetService
    {
        private List<ScreenDisplayPreset> _screenDisplayPresets;

        public PresetService()
        {
            _screenDisplayPresets = new List<ScreenDisplayPreset>();
        }

        public ScreenDisplayPreset GetPreset(int id)
        {
            return _screenDisplayPresets.FirstOrDefault(p => p.Id == id);
        }

        public int SetPresets(List<MediaScreenAssignState> mediaScreenAssignStates)
        {
            var presetId = _screenDisplayPresets.Count + 1;

            var preset = new ScreenDisplayPreset
            {
                Id = presetId,
                MediaScreenAssignStates = mediaScreenAssignStates
            };

            _screenDisplayPresets.Add(preset);

            Debug.Log($"preset set: {_screenDisplayPresets[0].MediaScreenAssignStates.Count}");

            return presetId;
        }

        public List<ScreenDisplayPreset> Test()
        {
            var mediaScreenAssignStates = new List<MediaScreenAssignState>();

            // test preset
            for (int i = 1; i <= 16; i++)
            {
                mediaScreenAssignStates.Add(
                    new MediaScreenAssignState
                    {
                        MediaId = i,
                        MediaTypeId = (int)MediaType.VideoClip,
                        ScreenDisplayId = i
                    }
                );
            }

            var preset = new ScreenDisplayPreset
            {
                Id = 1,
                MediaScreenAssignStates = mediaScreenAssignStates
            };

            _screenDisplayPresets.Add(preset);

            Debug.Log($"preset test: {_screenDisplayPresets[0].MediaScreenAssignStates.Count}");

            return _screenDisplayPresets;
        }

    }
}
