using Godot;
using System;

namespace Mars_Seal_Crimson
{
    public class SceneUtilities : Node
    {
        public PackedScene newScene;

        public void CleanPreviousScenes(Spatial referenceScene, String callSource = "")
        {
            int numChildren = referenceScene.GetTree().Root.GetChildCount();
            GD.Print($"CleanPreviousScenes() - scenes count = {numChildren}, called from {callSource} \n");
            Node previousScene = referenceScene.GetTree().Root.GetChild(0);
            /*
            var props = GetProperties(previousScene);
            if (props.Count > 0)
            {
                PrintObjectProperties("previousScene", introTimer);
            }
            */
            referenceScene.GetTree().Root.RemoveChild(previousScene);
        }

        public static void DebugPrintScenesList(Spatial referenceScene)
        {
            int numChildren = referenceScene.GetTree().Root.GetChildCount();
            if (numChildren > 0)
            {
                GD.Print($"DebugPrintScenesList - scenes count = {numChildren}\n");
                int idx = 0;
                foreach (object sceneX in referenceScene.GetTree().Root.GetChildren())
                {
                    Diagnostics.PrintObjectProperties($"scene [{idx}] = ", sceneX);
                    idx += 1;
                }
            }
        }

        public void ChangeScene(Spatial referenceScene, string scenePath)
        {
            newScene = (PackedScene)ResourceLoader.Load(scenePath);
            if (newScene != null)
                referenceScene.GetTree().Root.AddChild(newScene.Instance());
        }

        public void ChangeSceneFrom2D(Node2D referenceScene, string scenePath)
        {
            newScene = (PackedScene)ResourceLoader.Load(scenePath);
            if (newScene != null)
                referenceScene.GetTree().Root.AddChild(newScene.Instance());
        }

        public static void LinkSceneToViewport(string scenePath, Viewport parentViewport)
        {
            if (parentViewport != null)
            {
                PackedScene viewportScene = (PackedScene)ResourceLoader.Load(scenePath);
                if (viewportScene != null)
                {
                    //Link to the parent Viewport
                    //ViewportFrameInterface.HookupExistingNode(parentViewport, (Node2D)viewportScene.Instance());
                }
            }
        }

        public static void ExitApplication(Node referenceScene)
        {
            referenceScene.GetTree().Quit();
        }

        public static void SetCameraPosition(Camera aCamera, Vector3 newPosition)
        {
            if ((aCamera != null) && (newPosition != null))
            {
                aCamera.Translation = newPosition;
            }
        }

        public static void GameCameraTranslateAxis(Camera aCamera, float xlateValue, GeometricAxis axisChoice/* = GeometricAxis.GEOMETRIC_AXIS_Z*/)
        {
            if (aCamera != null)
            {
                var cameraPos = aCamera.Translation;
                switch (axisChoice)
                {
                    case GeometricAxis.GEOMETRIC_AXIS_X:
                        {
                            cameraPos.x += xlateValue;
                            break;
                        }
                    case GeometricAxis.GEOMETRIC_AXIS_Y:
                        {
                            cameraPos.y += xlateValue;
                            break;
                        }
                    case GeometricAxis.GEOMETRIC_AXIS_Z:
                        {
                            cameraPos.z += xlateValue;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                GD.Print($"GameCameraTranslateAxis , position = {cameraPos}");
                aCamera.Translation = cameraPos;
            }
        }

        public static Godot.Vector3 CalculateCirclePosition(Godot.Vector3 circlePosition, float radiusCircle, float angle)
        {
            Godot.Vector3 res = new Vector3(circlePosition.x, circlePosition.y, circlePosition.z);
            Godot.Vector3 rotation = new Vector3(angle, 0f, 0f);
            if ((circlePosition != null) && (radiusCircle >= 0.1f))
            {
                Godot.Vector3 setPos = new Vector3(0f, 0f, 0f);
                setPos.x += radiusCircle * Mathf.Cos(rotation.x) * Mathf.Cos(rotation.y);
                setPos.y += radiusCircle * Mathf.Sin(rotation.y);
                setPos.z += radiusCircle * Mathf.Sin(rotation.x) * Mathf.Cos(rotation.y);
                res += setPos;
                GD.Print($"CalculateCirclePosition(), angle = {angle}, calculated position= {res}\n");
            }
            return res;
        }

        public static void MoveCameraAroundPosition(Camera aCamera, Godot.Vector3 circlePosition, float radiusCircle, int step, float arcSweep = 180.0f)
        {
            float ratio = arcSweep / 180.0f;
            Godot.Vector3 circleCoord = CalculateCirclePosition(circlePosition, radiusCircle, (ratio * step / arcSweep) * Mathf.Pi /*+ 180.0f*/);
            aCamera.Translation = circleCoord;
        }

        public static Rect2 GetApplicationWindowExtent(Spatial referenceScene)
        {
            Rect2 res;
            res = referenceScene.GetTree().Root.GetVisibleRect();
            return res;
        }

        public static Vector2 GetExtentOffsetsForCenter(Spatial referenceScene, Control graphicsControl)
        {
            Vector2 res = new Vector2(100.0f, 20.0f);
            if (graphicsControl != null)
            {
                Rect2 appExtent = GetApplicationWindowExtent(referenceScene);
                GD.Print($"GetExtentOffsetsForCenter(), appExtent = {appExtent}\n");
                if ( (appExtent.Size.x >= 100.0f) && (graphicsControl.RectSize.x >= 100.0f) )
                {
                    GD.Print($"GetExtentOffsetsForCenter(), appExtent = {appExtent}, graphicsControl size={graphicsControl.RectSize}\n");
                    res = new Vector2( (appExtent.Size.x / 2.0f) - (graphicsControl.RectSize.x / 2.0f), (appExtent.Size.y / 2.0f) - (graphicsControl.RectSize.y / 2.0f) );
                }
            }
            return res;
        }

        public static void PlaceControlCentered(Spatial referenceScene, Control graphicsControl)
        {
            Vector2 placeCenterPos = GetExtentOffsetsForCenter(referenceScene, graphicsControl);
            PlaceControlTopLeft(referenceScene, graphicsControl, placeCenterPos);
        }

        public static void PlaceControlTopLeft(Spatial referenceScene, Control graphicsControl, Vector2 placePosition)
        {
            if ( (graphicsControl != null) && (placePosition != null) )
            {
                Rect2 appExtent = GetApplicationWindowExtent(referenceScene);
                if (appExtent.Size.x >= 100.0f)
                {
                    Vector2 controlExtent = graphicsControl.RectSize;
                    graphicsControl.RectPosition = placePosition;
                }
            }
        }
    }
}
