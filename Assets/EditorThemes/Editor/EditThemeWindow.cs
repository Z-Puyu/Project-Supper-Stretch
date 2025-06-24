using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//to do TextColor
//EditorStyles.label.normal.textColor 

namespace EditorThemes.Editor 
{ 

public class EditThemeWindow : EditorWindow
{
        
        public static CustomTheme ct;

        string Name;

        Vector2 scrollPosition;


        List<Color> SimpleColors = new List<Color>();
        List<Color> LastSimpleColors = new List<Color>();

        enum CustomView { Simple, Advanced };
        CustomView customView;

        bool Rhold;
        bool STRGHold;


        
        
        private void OnDestroy()
        {
            EditThemeWindow.ct = null;

        }

        private void Awake()
        {
            //Debug.Log(ct.Items[0].Color);
            this.SimpleColors = this.CreateAverageCoolors();
            this.LastSimpleColors = this.CreateAverageCoolors();


            this.Name = EditThemeWindow.ct.Name;
        }
        private void OnGUI()
        {
            

            
            if (EditThemeWindow.ct == null)
            {
                this.Close();
                return;
            }
                
                
            bool Regenerate = false;

            Event e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.R)
                {
                    this.Rhold = true;
                }
            }

            if (e.type == EventType.KeyUp)
            {
                if (e.keyCode == KeyCode.R)
                {
                    this.Rhold = false;
                }
            }
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.LeftControl)
                {
                    this.STRGHold = true;
                }
            }

            if (e.type == EventType.KeyUp)
            {
                if (e.keyCode == KeyCode.LeftControl)
                {
                    this.STRGHold = false;
                }
            }
            if (this.Rhold && this.STRGHold)
            {
                Regenerate = true;
                this.Rhold = false;
                this.STRGHold = false;
            }

            if (Regenerate && EditorUtility.DisplayDialog("Do you want to regenarate this Theme? (Make a Clone first!)", "Regenarating is helpfull when the Theme was made with an older version of the Plugin (but you might loose small amounts of data)", "Continue", "Cancel") == true)
            {
                EditThemeWindow.ct.Items = new List<CustomTheme.UIItem>();
                //fetch all ColorObjects
                for (int i = 0; i < 6; i++)
                {

                    foreach (string s in ThemesUtility.GetColorListByInt(i))
                    {
                        CustomTheme.UIItem uiItem = new CustomTheme.UIItem();
                        uiItem.Name = s;
                        uiItem.Color = this.SimpleColors[i];

                        EditThemeWindow.ct.Items.Add(uiItem);
                    }
                }
            }


            EditorGUILayout.LabelField("\n");

            this.Name = EditorGUILayout.TextField(this.Name);
            EditorGUILayout.LabelField("\n");
            this.customView = (CustomView)EditorGUILayout.EnumPopup(this.customView, GUILayout.Width(100));






            if (this.customView == CustomView.Advanced)
            {
                EditorGUILayout.LabelField("");
                this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);

                List<CustomTheme.UIItem> CTItemsClone = new List<CustomTheme.UIItem>(EditThemeWindow.ct.Items);
                foreach (CustomTheme.UIItem I in CTItemsClone)
                {
                    EditorGUILayout.BeginHorizontal();
                    I.Name = EditorGUILayout.TextField(I.Name, GUILayout.Width(200));
                    if (GUILayout.Button("Del", GUILayout.Width(50)))
                    {
                        EditThemeWindow.ct.Items.Remove(I);
                    }
                    EditorGUILayout.EndHorizontal();
                    I.Color = EditorGUILayout.ColorField(I.Color, GUILayout.Width(200));


                }
                EditorGUILayout.EndScrollView();


                EditorGUILayout.LabelField("");

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add", GUILayout.Width(200)))
                {
                    CustomTheme.UIItem I = new CustomTheme.UIItem();
                    I.Name = "Enter Name";

                    EditThemeWindow.ct.Items.Add(I);
                }
                if (EditThemeWindow.ct.Items.Count > 0)
                {
                    if (GUILayout.Button("Remove", GUILayout.Width(200)))
                    {
                        EditThemeWindow.ct.Items.RemoveAt(EditThemeWindow.ct.Items.Count - 1);
                    }
                }

                EditorGUILayout.EndHorizontal();



            }
            else
            {


                if (this.SimpleColors[0] != null)
                {
                    GUILayout.Label("Base Color:", EditorStyles.boldLabel);
                    this.SimpleColors[0] = EditorGUILayout.ColorField(this.SimpleColors[0]);
                }
                if (this.SimpleColors[1] != null)
                {
                    GUILayout.Label("Accent Color:", EditorStyles.boldLabel);
                    this.SimpleColors[1] = EditorGUILayout.ColorField(this.SimpleColors[1]);
                }
                if (this.SimpleColors[2] != null)
                {
                    GUILayout.Label("Secondery Base Color:", EditorStyles.boldLabel);
                    this.SimpleColors[2] = EditorGUILayout.ColorField(this.SimpleColors[2]);
                }
                if (this.SimpleColors[3] != null)
                {
                    GUILayout.Label("Tab Color:", EditorStyles.boldLabel);
                    this.SimpleColors[3] = EditorGUILayout.ColorField(this.SimpleColors[3]);
                }
                if (this.SimpleColors[4] != null)
                {
                    GUILayout.Label("Command Bar Color:", EditorStyles.boldLabel);
                    this.SimpleColors[4] = EditorGUILayout.ColorField(this.SimpleColors[4]);
                }
                if (this.SimpleColors[5] != null)
                {
                    GUILayout.Label("Additional Color:", EditorStyles.boldLabel);
                    this.SimpleColors[5] = EditorGUILayout.ColorField(this.SimpleColors[5]);
                }






                for (int i = 0; i < this.SimpleColors.Count; i++)
                {
                    if (this.SimpleColors[i] != null && this.SimpleColors[i] != this.LastSimpleColors[i])
                    {
                        //Debug.Log("not same");
                        this.EditColor(i, this.SimpleColors[i]);
                    }
                }

            }
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Unity Theme:");
            EditThemeWindow.ct.unityTheme = (CustomTheme.UnityTheme)EditorGUILayout.EnumPopup(EditThemeWindow.ct.unityTheme, GUILayout.Width(100));
            EditorGUILayout.LabelField("");
            EditorGUILayout.BeginHorizontal();
            //Debug.Log(ct.Name);
            //Debug.Log(Name);
            if (GUILayout.Button("Save", GUILayout.Width(200)))
            {

                if (EditThemeWindow.ct.Name != this.Name)
                {
                    ThemesUtility.DeleteFileWithMeta(ThemesUtility.GetPathForTheme(EditThemeWindow.ct.Name));
                }

                EditThemeWindow.ct.Name = this.Name;

                ThemesUtility.SaveJsonFileForTheme(EditThemeWindow.ct);

            }
            if (GUILayout.Button("Clone", GUILayout.Width(200)))
            {

                EditThemeWindow.ct.Name = this.Name + " - c";

                ThemesUtility.SaveJsonFileForTheme(EditThemeWindow.ct);

            }


        }
        
        
        
        CustomTheme.UIItem GeItemByName(string s)
        {
            CustomTheme.UIItem item = null;

            foreach (CustomTheme.UIItem u in EditThemeWindow.ct.Items)
            {
                if (u.Name == s)
                {
                    item = u;
                }

            }
            return item;
        }
        
        List<Color> CreateAverageCoolors()
        {
            List<Color> colors = new List<Color>();


            for (int i = 0; i < 6; i++)
            {
                List<string> ColorObjects = ThemesUtility.GetColorListByInt(i);
                List<Color> AllColors = new List<Color>();

                foreach (string s in ColorObjects)
                {
                    if (this.GeItemByName(s) != null)
                    {
                        AllColors.Add(this.GeItemByName(s).Color);
                    }

                }

                if (AllColors.Count > 0)
                {
                    colors.Add(this.GetAverage(AllColors));
                }
                else
                {
                    colors.Add(ThemesUtility.HtmlToRgb("#9A7B6E"));
                }


            }


            return colors;
        }

        void EditColor(int i, Color nc)
        {


            //Color difrence = oc - nc;
            List<string> edit = ThemesUtility.GetColorListByInt(i);


            foreach (string s in edit)
            {

                CustomTheme.UIItem Item = this.GeItemByName(s);
                if (Item != null)
                {
                    Item.Color = nc;
                }
            }

            this.LastSimpleColors[i] = this.SimpleColors[i];
        }

        Color GetAverage(List<Color> cl)
        {

            float r = 0;
            float g = 0;
            float b = 0;

            int Count = cl.Count;
            foreach (Color c in cl)
            {
                
                r += c.r;
                g += c.g;
                b += c.b;
            }



            return new Color(r / Count, g / Count, b / Count);
        }

        
    }
}
