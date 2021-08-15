using Assets.Scripts;
using Normal.Realtime.Serialization;
using Normal.Realtime;

[RealtimeModel]
public partial class MediaScreenDisplayModel
{
    //[RealtimeProperty(1, true)] private RealtimeArray<MediaScreenDisplayStateModel> _mediaScreenDisplayStates;
    //[RealtimeProperty(2, true, true)] private RealtimeArray<ScreenPortalStateModel> _screenPortalStates;
    [RealtimeProperty(3, true)] private RealtimeSet<MediaScreenDisplayStateModel> _mediaScreenDisplayStates;
    [RealtimeProperty(4, true, true)] private RealtimeSet<ScreenPortalStateModel> _screenPortalStates;

}

/* ----- Begin Normal Autogenerated Code ----- */
public partial class MediaScreenDisplayModel : RealtimeModel {
    public Normal.Realtime.Serialization.RealtimeSet<MediaScreenDisplayStateModel> mediaScreenDisplayStates {
        get => _mediaScreenDisplayStates;
    }
    
    public Normal.Realtime.Serialization.RealtimeSet<Assets.Scripts.ScreenPortalStateModel> screenPortalStates {
        get => _screenPortalStates;
    }
    
    public enum PropertyID : uint {
        MediaScreenDisplayStates = 1,
        ScreenPortalStates = 2,
    }
    
    #region Properties
    
    private ModelProperty<RealtimeSet<MediaScreenDisplayStateModel>> _mediaScreenDisplayStatesProperty;
    
    private ModelProperty<RealtimeSet<Assets.Scripts.ScreenPortalStateModel>> _screenPortalStatesProperty;
    
    #endregion
    
    public MediaScreenDisplayModel() : base(null) {
        RealtimeModel[] childModels = new RealtimeModel[2];
        
        _mediaScreenDisplayStates = new Normal.Realtime.Serialization.RealtimeSet<MediaScreenDisplayStateModel>();
        childModels[0] = _mediaScreenDisplayStates;
        
        _screenPortalStates = new Normal.Realtime.Serialization.RealtimeSet<Assets.Scripts.ScreenPortalStateModel>();
        childModels[1] = _screenPortalStates;
        
        SetChildren(childModels);
        
        _mediaScreenDisplayStatesProperty = new ModelProperty<RealtimeSet<MediaScreenDisplayStateModel>>(3, _mediaScreenDisplayStates);
        _screenPortalStatesProperty = new ModelProperty<RealtimeSet<Assets.Scripts.ScreenPortalStateModel>>(4, _screenPortalStates);
    }
    
    protected override int WriteLength(StreamContext context) {
        var length = 0;
        length += _mediaScreenDisplayStatesProperty.WriteLength(context);
        length += _screenPortalStatesProperty.WriteLength(context);
        return length;
    }
    
    protected override void Write(WriteStream stream, StreamContext context) {
        var writes = false;
        writes |= _mediaScreenDisplayStatesProperty.Write(stream, context);
        writes |= _screenPortalStatesProperty.Write(stream, context);
        if (writes) InvalidateContextLength(context);
    }
    
    protected override void Read(ReadStream stream, StreamContext context) {
        var anyPropertiesChanged = false;
        while (stream.ReadNextPropertyID(out uint propertyID)) {
            var changed = false;
            switch (propertyID) {
                case (uint) PropertyID.MediaScreenDisplayStates: {
                    changed = _mediaScreenDisplayStatesProperty.Read(stream, context);
                    break;
                }
                case (uint) PropertyID.ScreenPortalStates: {
                    changed = _screenPortalStatesProperty.Read(stream, context);
                    break;
                }
                default: {
                    stream.SkipProperty();
                    break;
                }
            }
            anyPropertiesChanged |= changed;
        }
        if (anyPropertiesChanged) {
            UpdateBackingFields();
        }
    }
    
    private void UpdateBackingFields() {
        _mediaScreenDisplayStates = mediaScreenDisplayStates;
        _screenPortalStates = screenPortalStates;
    }
    
}
/* ----- End Normal Autogenerated Code ----- */