using Assets.Scripts.Enums;
using Normal.Realtime;
using UnityEngine;

namespace Assets.Scripts
{
    public class FormationSelectSync : RealtimeComponent<FormationSelectSyncModel>
    {
        private int _formationId;

        private FormationSelect _formationSelectDisplay;

        private void Awake()
        {
            _formationId = 0;
            _formationSelectDisplay = GetComponent<FormationSelect>();
        }

        protected override void OnRealtimeModelReplaced(FormationSelectSyncModel previousModel,
            FormationSelectSyncModel currentModel)
        {
            //Debug.Log($"PreviousModel: {previousModel != null}");
            //Debug.Log($"CurrentModel: {currentModel != null}");

            if (previousModel != null)
            {
                // Unregister from events
                previousModel.formationIdDidChange -= FormationIdDidChange;
            }

            if (currentModel != null)
            {
                // If this is a model that has no data set on it, populate it with the current value.
                if (currentModel.isFreshModel)
                    currentModel.formationId = _formationId;

                // Update the formationId to match the new model
                UpdateFormationId();

                // Register for events so we'll know if the value changes later
                currentModel.formationIdDidChange += FormationIdDidChange;
            }
        }

        private void FormationIdDidChange(FormationSelectSyncModel model, int value)
        {
            Debug.Log($"FormationIdDidChange: {value}");
            UpdateFormationId();
        }

        private void UpdateFormationId()
        {
            //Debug.Log($"UpdateFormationId {model.formationId}");

            if (model != null && model.formationId > 0)
            {
                // Get the value from the model, display it 
                // _model.formationId comes in 'composite' form, e.g. 12 = scene 1, formation 2.

                string scenePlusFormation = model.formationId.ToString();
                int scene = int.Parse(scenePlusFormation.Substring(0, 1));
                int formationId = int.Parse(scenePlusFormation.Substring(1, 1));

                Debug.Log($"UpdateFormationId - scenePlusFormation: {scenePlusFormation}");
                Debug.Log($"UpdateFormationId - scene: {(Scene)scene}");
                Debug.Log($"UpdateFormationId - formationId: {formationId}");

                _formationSelectDisplay.SetFormationId((Scene) scene, formationId, 10);
            }
        }

        public void SetId(int id)
        {
            Debug.Log($"Model is null: {model == null}");

            // Set the value on the model
            // This will fire the valueChanged event on the model, which will update the value for both the local player and all remote players
            model.formationId = id;
        }
    }
}