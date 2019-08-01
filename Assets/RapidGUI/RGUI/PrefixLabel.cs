﻿using System;
using UnityEngine;


namespace RapidGUI
{
    public static partial class RGUI
    {
        public static class PrefixLabelSetting
        {
            public static float width = 130f;
        }

        public static bool PrefixLabel(string label)
        {
            var isLong = false;

            if (!string.IsNullOrEmpty(label))
            {
                var style = GUI.skin.label;
                isLong = style.CalcHeight(RGUIUtility.TempContent(label), PrefixLabelSetting.width) > 21;


                if (isLong)
                {
                    GUILayout.Label(label);
                }
                else
                {
                    GUILayout.Label(label, GUILayout.Width(PrefixLabelSetting.width));                    
                }
            }

            return isLong;
        }


        public static object PrefixLabelDraggable(string label, object obj, Type type, params GUILayoutOption[] options)
        {
            var ret = PrefixLabelDraggable(label, obj, type, out var isLong);
            if (isLong)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(options);
                GUILayout.Space(PrefixLabelSetting.width + GUI.skin.label.margin.horizontal);
            }

            return ret;
        }

        public static object PrefixLabelDraggable(string label, object obj, Type type, out bool isLong)
        {
            isLong = false;
            if (!string.IsNullOrEmpty(label))
            {
                isLong = PrefixLabel(label);
                if (IsDraggable(type))
                {
                    obj = DoDrag(obj, type);
                }
            }

            return obj;
        }

#region implement drag

        static Vector2 lastMousePos;
        static readonly int doDragHash = "DoDrag".GetHashCode();

        static object DoDrag(object obj, Type type)
        {
            var controlID = GUIUtility.GetControlID(doDragHash, FocusType.Passive);

            var rect = GUILayoutUtility.GetLastRect();

            var ev = Event.current;
            var etype = ev.GetTypeForControl(controlID);

            switch (etype)
            {
                case EventType.MouseDown:
                    {
                        if ((ev.button == 0) && rect.Contains(ev.mousePosition))
                        {
                            GUIUtility.hotControl = controlID;
                            lastMousePos = ev.mousePosition;
                            ev.Use();
                        }
                    }
                    break;

                case EventType.MouseUp:
                    {
                        if (GUIUtility.hotControl == controlID)
                            GUIUtility.hotControl = 0;
                    }
                    break;

                case EventType.MouseDrag:
                    {
                        if ((ev.button == 0) && (GUIUtility.hotControl == controlID))
                        {
                            var diff = ev.mousePosition - lastMousePos;
                            var add = (Mathf.Abs(diff.x) > Mathf.Abs(diff.y)) ? diff.x : diff.y;
                            add = Math.Sign(add);

                            lastMousePos = ev.mousePosition;
                            if (typeof(int) == type)
                            {
                                var v = (int)obj;
                                v += (int)(add);
                                obj = v;
                            }
                            else if (typeof(float) == type)
                            {
                                var scale = 0.03f;
                                var v = (float)obj;
                                v += add * scale;
                                v = Mathf.Floor(v * 100f) * 0.01f; // chop
                                obj = v;
                            }


                            ev.Use();
                        }

                    }
                    break;
            }

            return obj;
        }




        public static bool IsDraggable(Type type)
        {
            return (
                (typeof(int) == type) ||
                (typeof(float)) == type
                );
        }
    }

#endregion
}