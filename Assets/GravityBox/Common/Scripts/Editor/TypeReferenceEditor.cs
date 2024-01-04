using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;


namespace GravityBox
{
    [CustomPropertyDrawer(typeof(TypeReference))]
    public class TypeReferenceEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string fullname = property.FindPropertyRelative("Fullname").stringValue;
            string name = fullname.Substring(fullname.LastIndexOf(".") + 1).Replace("+", ".");


            Rect r1, r2 = position;

            if (!string.IsNullOrEmpty(label.text))
            {
                r1 = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
                r2 = new Rect(r1.x + r1.width, position.y, position.width - r1.width, position.height);
                GUI.Label(r1, property.displayName);
            }

            if (EditorGUI.DropdownButton(r2, new GUIContent(name, fullname), FocusType.Passive))
            {
                var dropdown = new TypesDropdown(new AdvancedDropdownState());
                dropdown.Show(position);
                dropdown.onTypeSelected += (x) => CatchMenu(property, x);

                Event.current.Use();
            }
        }

        private void CatchMenu(SerializedProperty property, System.Type type)
        {
            property.FindPropertyRelative("Assembly").stringValue = type.Assembly.GetName().Name;
            property.FindPropertyRelative("Fullname").stringValue = type.FullName;
            property.FindPropertyRelative("assemblyQualifiedName").stringValue = type.AssemblyQualifiedName;
            property.serializedObject.ApplyModifiedProperties();
        }
    }

    class TypesDropdown : AdvancedDropdown
    {
        public event System.Action<System.Type> onTypeSelected;

        private static List<System.Type> _types;

        private void CollectTypeNames()
        {
            if (_types != null) return;

            _types = new List<System.Type>();
            HashSet<System.Type> temp = new HashSet<System.Type>();

            ////very slow but loads all assemblies
            //Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            //foreach (var a in assemblies)
            //    temp.UnionWith(a.GetTypes());

            //Unity assemblies were split, that's why UnityEngine.Object and Collider are here
            //you'll need to add more classes in case they are not in dropdown
            temp.UnionWith(Assembly.GetAssembly(typeof(int)).GetTypes());
            temp.UnionWith(Assembly.GetAssembly(typeof(Object)).GetTypes());
            temp.UnionWith(Assembly.GetAssembly(typeof(Collider)).GetTypes());
            temp.UnionWith(Assembly.GetAssembly(typeof(Animation)).GetTypes());
            temp.UnionWith(Assembly.GetAssembly(typeof(TypeReference)).GetTypes());

            List<string> _names = new List<string>();
            foreach (var t in temp)
            {
                if (IsValidType(t))
                    _types.Add(t);
            }
        }

        private static bool IsValidType(System.Type type)
        {
            //type is not in currently executing assembly
            if (string.IsNullOrEmpty(type.FullName))
                return false;
            //auto generated type
            if (type.Name.StartsWith("<") || type.FullName.StartsWith("<"))
                return false;
            //generic class
            if (type.FullName.Contains("`"))
                return false;

            return true;
        }

        public TypesDropdown(AdvancedDropdownState state) : base(state)
        {
        }

        AdvancedDropdownItem FindChild(AdvancedDropdownItem parent, string name)
        {
            foreach (var c in parent.children)
            {
                if (c.name == name)
                    return c;
            }

            return null;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            CollectTypeNames();

            var root = new AdvancedDropdownItem("Types");
            for (int i = 0; i < _types.Count; i++)
            {
                AdvancedDropdownItem current = root;
                string[] temp = _types[i].FullName.Split('.');
                for (int j = 0; j < temp.Length; j++)
                {
                    string s = temp[j];
                    if (FindChild(current, s) == null)
                    {
                        var child = new AdvancedDropdownItem(s);
                        if (j == temp.Length - 1)
                            child.id = i;
                        current.AddChild(child);
                        current = child;
                    }
                    else
                    {
                        current = FindChild(current, s);
                    }
                }
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            onTypeSelected?.Invoke(_types[item.id]);
            base.ItemSelected(item);
        }
    }
}