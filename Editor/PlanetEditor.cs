using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    Planet planet;
    Editor ShapeEditor;
    Editor ColorEditor;
    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                planet.GeneratePlanet();
            }
        }
        if(GUILayout.Button("Generate Planet"))
        {
            planet.GeneratePlanet();
        }
        DrawSettings(planet.shapeSettings,planet.OnShapeSettingsUpdated,ref planet.shapeFoldout,ref ShapeEditor);
        DrawSettings(planet.colorSettings,planet.OnColorSettingsUpdated,ref planet.colorFoldout,ref ColorEditor);

    }
    void DrawSettings(Object settings,System.Action OnSettingsUpdated,ref bool foldout,ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            using (var check = new EditorGUI.ChangeCheckScope())
            {


                if (foldout)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed)
                    {
                        if (OnSettingsUpdated != null)
                        {
                            OnSettingsUpdated();
                        }
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        planet = (Planet)target;
    }
}
