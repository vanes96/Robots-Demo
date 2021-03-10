using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ССP.Editors
{
    [CustomEditor(typeof(Controllers.CameraController))]
    [CanEditMultipleObjects]
    public class CameraEditor : CommonEditor
    {
        #region Fields
        #region Collections
        private static readonly List<Tuple<string, string>> ToolbarData = new List<Tuple<string, string>>() {
            new Tuple<string, string> (Control, "Camera Control and Following Parameters: Speed and Smoothness"),
            new Tuple<string, string> (Position, "Parameters of Camera Position and Limits of them"),            
            new Tuple<string, string> (Target, "Transform, Position and View of Camera Target"), 
        };

        private static readonly Dictionary<string, Tuple<string, string>> LabelsData = new Dictionary<string, Tuple<string, string>>() {
            { Position, new Tuple<string, string> (Position, "Parameters of Camera Position") },
            { Limits_, new Tuple<string, string> (Limits_, "Limits of Camera Position parameters") },
            { Zooming, new Tuple<string, string> (Zooming, null) },
            { Following, new Tuple<string, string> (Following, "Smoothness of Camera Following Target") },
            { Rotating, new Tuple<string, string> (Rotating, "Horizontal Camera Rotation") },
            { Lifting, new Tuple<string, string> (Lifting, "Vertical Camera Rotation") },
        };

        private static readonly Dictionary<string, Tuple<string, string>> PropertiesData = new Dictionary<string, Tuple<string, string>>() {
            { Player, new Tuple<string, string>(Object, "Transform of Target object") },
            { Position + Offset, new Tuple<string, string>(null, "Actual center Position of Target") },
            { Distance, new Tuple<string, string>(null, "Current Distance between Camera and Target") },
            { Rotation, new Tuple<string, string>(null, "Current Camera Rotation around Target") },
            { Height, new Tuple<string, string>(null, "Current Camera Height above Target") },
            { Zooming + Speed, new Tuple<string, string>(Speed, null) },
            { Zooming + Smoothness, new Tuple<string, string>(Smoothness, null) },
            { Rotating + Speed, new Tuple<string, string>(Speed, null) },
            { Rotating + Smoothness, new Tuple<string, string>(Smoothness, null) },
            { Lifting + Speed, new Tuple<string, string>(Speed, null) },
            { Lifting + Smoothness, new Tuple<string, string>(Smoothness, null) },
            { Following + Smoothness, new Tuple<string, string>(Smoothness, null)},
            { Drag + Key, null },
            //{ Separate + Drag, null },
            { "LookAtCamera", new Tuple<string, string>(null, "Option of Target object Looking at Camera") },
            { Min + Distance, new Tuple<string, string>(string.Join(Space, Min, Distance), null) },
            { Max + Distance, new Tuple<string, string>(string.Join(Space, Max, Distance), null) },
            { Min + Height, new Tuple<string, string>(string.Join(Space, Min, Height), null) },
            { Max + Height, new Tuple<string, string>(string.Join(Space, Max, Height), null) } 
        };
        #endregion
        #region Constants
        private const int LogoPadding = 1, MaxLogoHeight = 50, GroupPadding = 10;
        private const float MinWidth = 250, MaxWidth = 450;
        private const string Rotation = "Rotation", Rotating = "Rotating", Distance = "Distance", Lifting = "Lifting",
                             Zooming = "Zooming", Following = "Following", Camera_ = "Camera", LogoImageName = "CCP Logo", 
                             CanvasName = "Camera Canvas", PixelPrefabName = "Pixel Prefab";
        #endregion
        #region Static and Readonly
        private static int _currentToolbarIndex, _previousToolbarIndex;
        private static Texture _logo;
        private static GUIStyle _logoStyle, _groupStyle;
        private static GUILayoutOption _maxWidthOption, _minWidthOption;
        //private static readonly GameObject[] _screenBorderLines = new GameObject[BorderLinesNumber];
        //private static readonly RectTransform[] _screenBorderLinesTransforms = new RectTransform[BorderLinesNumber];
        private readonly Vector2 _pixelSize = new Vector2(1, 1);
        //private static GameObject _canvasObject;
        //private static GameObject _pixelPrefab;
        //private static Canvas _canvas;
        #endregion
        #region Debug
#if DEBUG
        private SerializedProperty _currentControlState, _currentSmoothingState, _currentMouseState, 
                                   _previousSmoothingState, _previousDragPosition, _currentMouseDragX, _currentMouseDragY;
        private SerializedProperty _absoluteCurrentRotation, _absoluteTargetRotation, _targetRotation;
        private SerializedProperty _targetDeltaRotation;
        private SerializedProperty _currentHeight, _targetHeight, _targetDeltaHeight, _targetDistance, 
                                   _currentRotation, _currentDistance, _targetPlayerPoisition;
#endif
        #endregion
        #endregion

        #region Methods
        #region Main
        protected override void OnEnable()
        {
            base.OnEnable();

            if (PropertiesDictionary.Count == 0)
            {
                foreach (var propertyData in PropertiesData)
                {
                    PropertiesDictionary[propertyData.Key] = new Tuple<SerializedProperty, string, string>
                        (GetProperty(propertyData.Key), propertyData.Value?.Item1, propertyData.Value?.Item2);
                }
            }

            //if (_canvasObject == null && false)
            //{
            //    _canvasObject = GameObject.Find(CanvasName);
            //}
            //if (_canvasObject == null && false)
            //{
            //    _canvasObject = new GameObject(CanvasName, new Type[] { typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster) });
            //    _canvas = _canvasObject.GetComponent<Canvas>();
            //    _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            //}

            if (_logo == null)
            {
                _logo = (Texture)Resources.Load(LogoImageName);
            }

            if (_logoStyle == null)
            {
                _logoStyle = new GUIStyle { alignment = TextAnchor.MiddleCenter, padding = new RectOffset(0, 0, LogoPadding, 0) };
            }

            #region Debug
#if DEBUG
            SetProperty(ref _absoluteTargetRotation, "_absoluteTargetRotation");

            //SetProperty(ref _currentControlState, "_currentControlState");
            //SetProperty(ref _previousSmoothingState, "_previousSmoothingState");
            //SetProperty(ref _currentSmoothingState, "_currentSmoothingState");
            //SetProperty(ref _currentMouseState, "_currentMouseState");
            //SetProperty(ref _previousDragPosition, "_previousDragPosition"); 
            //SetProperty(ref _currentMouseDragX, "_currentMouseDragX");
            //SetProperty(ref _currentMouseDragY, "_currentMouseDragY");
            //SetProperty(ref _absoluteCurrentRotation, "_absoluteCurrentRotation");
            //SetProperty(ref _targetDeltaRotation, "_targetDeltaRotation"); 
            //SetProperty(ref _targetDeltaHeight, "_targetDeltaHeight");         
            //SetProperty(ref _targetPlayerPoisition, "_targetPlayerPosition"); 
            //SetProperty(ref _currentRotation, "_currentRotation");
            //SetProperty(ref _targetRotation, "_targetRotation");
            //SetProperty(ref _absoluteCurrentRotation, "_absoluteCurrentRotation");
            //SetProperty(ref _currentHeight, "_currentHeight");
            //SetProperty(ref _targetHeight, "_targetHeight");
            //SetProperty(ref _currentDistance, "_currentDistance");
            //SetProperty(ref _targetDistance, "_targetDistance");

#endif
            #endregion
        }

        private void  OnDisable()
        {

        }

        public override void OnInspectorGUI()
        {
            //if (_canvas == null && false)
            //{
            //    _canvas = _canvasObject.GetComponent<Canvas>();
            //}

            if (_groupStyle == null)
            {
                _groupStyle = UnityEngine.GUI.skin.box;
                _groupStyle.padding = new RectOffset(GroupPadding, GroupPadding, GroupPadding, GroupPadding);
            }

            if (_minWidthOption == null)
            {
                _minWidthOption = GUILayout.MinWidth(MinWidth);
            }
            if (_maxWidthOption == null)
            {
                _maxWidthOption = GUILayout.MaxWidth(MaxWidth);
            }

            ShowImage(_logo, _logoStyle, GUILayout.MaxHeight(MaxLogoHeight), _maxWidthOption, _minWidthOption);
            ShowToolbar(ref _currentToolbarIndex, ToolbarData.ToArray(), _maxWidthOption);

            BeginGroup(_groupStyle, _maxWidthOption, _minWidthOption);
            switch (_currentToolbarIndex)
            {
                case 0:
                    ShowLabel(LabelsData[Following], true);
                    ShowSlider(PropertiesDictionary[Following + Smoothness]);

                    foreach (string optionName in new List<string> { Zooming, Rotating, Lifting })
                    {
                        var speedProperty = PropertiesDictionary[optionName + Speed];

                        ShowLabel(LabelsData[optionName]);
                        ShowSlider(speedProperty);
                        if (IsPropertyValuePositive(speedProperty.Item1))
                        {
                            ShowSlider(PropertiesDictionary[optionName + Smoothness]);
                        }
                    }

                    bool isRotationEnabled = IsPropertyValuePositive(PropertiesDictionary[Rotating + Speed].Item1),
                         isLiftingEnabled = IsPropertyValuePositive(PropertiesDictionary[Lifting + Speed].Item1);

                    if (isRotationEnabled || isLiftingEnabled)
                    {
                        ShowPropertyField(PropertiesDictionary[Drag + Key], LargeSpace);
                    }
                    break;
                    #region Debug
#if DEBUG
                    //ShowPropertyField(_absoluteTargetRotation, Rotation);
                    //ShowPropertyField(PropertiesDictionary[Height]);
                    //ShowSlider(PropertiesDictionary[Rotation], Limits.MinSliderValue, Angles.Turnover);
                    //ShowPropertyField(PropertiesDictionary[Rotation]);
                    //ShowPropertyField(PropertiesDictionary[Distance]);
                    //ShowPropertyField(_currentRotation, "Current Rotation", LargeSpace);
                    //ShowPropertyField(_targetRotation, "Target Rotation");
                    //ShowPropertyField(_absoluteCurrentRotation, "Abs Current Rotation");
                    //ShowPropertyField(_absoluteTargetRotation, Rotation);
                    //ShowPropertyField(_targetPlayerPoisition, "Target Player Position");
                    //ShowPropertyField(_targetDeltaRotation, "Target Delta Rotation");
                    //ShowPropertyField(_currentHeight, "Current Height", LargeSpace);
                    //ShowSlider(_targetHeight, Height);
                    //ShowPropertyField(_targetHeight, "Target Height");
                    //ShowPropertyField(_currentDistance, "Current Distance", LargeSpace);
                    //ShowSlider(_targetDistance, Distance);
                    //ShowPropertyField(_targetDistance, "Target Distance");
                    //ShowPropertyField(_targetDeltaHeight, "Target Delta Height");
#endif
                    #endregion
                case 1:
                    ShowLabel(LabelsData[Position], true);
                    ShowSlider(PropertiesDictionary[Distance]);
                    ShowSlider(PropertiesDictionary[Height]);
                    ShowPropertyField(PropertiesDictionary[Rotation]);

                    ShowLabel(LabelsData[Limits_]);
                    ShowSlider(PropertiesDictionary[Min + Distance]);
                    ShowSlider(PropertiesDictionary[Max + Distance]);
                    ShowSlider(PropertiesDictionary[Min + Height], MediumSpace);
                    ShowSlider(PropertiesDictionary[Max + Height]);
                    break;
                case 2:
                    //ShowLabel(LabelsData[Target], true);
                    ShowPropertyField(PropertiesDictionary[Player], 0);
                    ShowPropertyField(PropertiesDictionary[Position + Offset], LargeSpace);
                    ShowPropertyField(PropertiesDictionary["LookAtCamera"], LargeSpace);
                    break;
            }

            EndGroup();
        }
        #endregion

        protected override void EndGroup()
        {
            base.EndGroup();

            ResetFocusControl();
            _previousToolbarIndex = _currentToolbarIndex;
        }

        private void ResetFocusControl()
        {
            if (_currentToolbarIndex != _previousToolbarIndex)
            {
                UnityEngine.GUI.FocusControl(null);
            }
        }   

        private GameObject CreatePixelPrefab()
        {
            var pixelPrefab = new GameObject(PixelPrefabName, new Type[] { typeof(CanvasRenderer), typeof(Image) });
            pixelPrefab.GetComponent<RectTransform>().sizeDelta = _pixelSize;
            return pixelPrefab;
        }

        #endregion
    }
}

//#regio Detail
//private bool _distancingDetails = false, _rotationDetails = false, _liftingDetails = false;
