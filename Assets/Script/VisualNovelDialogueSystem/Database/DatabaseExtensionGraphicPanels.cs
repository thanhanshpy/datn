using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace Commands
{
    public class DatabaseExtensionGraphicPanels : DatabaseExtention
    {
        private static string[] paramPanel = new string[] { "-p", "-panel"};
        private static string[] paramLayer = new string[] { "-l", "-layer" };
        private static string[] paramMedia = new string[] { "-m", "-media" };
        private static string[] paramSpeed = new string[] { "-spd", "-speed" };
        private static string[] paramImmediate = new string[] { "-i", "-immediate" };
        private static string[] paramBlendTex = new string[] { "-b", "-blend" };
        private static string[] paramUseVideoAudio = new string[] { "-aud", "-audio" };

        private const string homeDirectorySymbol = "~/";
        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("setlayermedia", new Func<string[], IEnumerator>(SetLayerMedia));
            database.AddCommand("clearlayermedia", new Func<string[], IEnumerator>(ClearLayerMedia));
        }

        private static IEnumerator SetLayerMedia(string[] data)
        {
            //parameters avaiable to funtion
            string panelName = "";
            int layer = 0;
            string mediaName = "";
            float transitionSpeed = 0f;
            bool immediate = false;
            string blendTexName = "";
            bool useAudio = false;

            string pathToGraphic = "";
            UnityEngine.Object graphic = null;
            Texture blendTex = null;

            //get parameters
            var parameters = ConvertDataToParameters(data);

            //get the panel that this media is applied to
            parameters.TryGetValue(paramPanel, out panelName);
            GraphicPanel panel = GraphicPanelManager.instance.GetPanel(panelName);
            if(panel == null)
            {
                Debug.LogError($"unable to grab panel '{panelName}' because it is not a valid panel");
                yield break;
            }

            parameters.TryGetValue(paramLayer, out layer, defaultValue: 0);

            parameters.TryGetValue(paramMedia, out mediaName);

            parameters.TryGetValue(paramImmediate, out immediate, defaultValue: false);

            if (!immediate)
            {
                parameters.TryGetValue(paramSpeed, out transitionSpeed, defaultValue: 1);
            }

            parameters.TryGetValue(paramBlendTex, out blendTexName);

            parameters.TryGetValue(paramUseVideoAudio, out useAudio, defaultValue: false);

            pathToGraphic = FilePath.GetPathToResource(FilePath.resources_backgroundImages, mediaName);
            graphic = Resources.Load<Texture>(pathToGraphic);

            if (graphic == null)
            {
                pathToGraphic = FilePath.GetPathToResource(FilePath.resources_backgroundVideos, mediaName);
                graphic = Resources.Load<VideoClip>(pathToGraphic);
            }

            if (graphic == null)
            {
                Debug.LogError($"could not find media file called '{mediaName}'");
                yield break;
            }

            if(!immediate && blendTexName != string.Empty)
            {
                blendTex = Resources.Load<Texture>(FilePath.resources_blendTextures + blendTexName);
            }

            //get layers
            GraphicLayer graphicLayer = panel.GetLayer(layer, createIfDoesNotExist: true);

            if(graphic is Texture)
            {
                yield return graphicLayer.SetTexture(graphic as Texture, transitionSpeed, blendTex, pathToGraphic, immediate);
            }
            else
            {
                yield return graphicLayer.SetVideo(graphic as VideoClip, transitionSpeed, useAudio, blendTex, pathToGraphic, immediate);
            }
        }
        private static IEnumerator ClearLayerMedia(string[] data)
        {
            //parameters avaiable to funtion
            string panelName = "";
            int layer = 0;
            string mediaName = "";
            float transitionSpeed = 0f;
            bool immediate = false;
            string blendTexName = "";
            
            Texture blendTex = null;

            //get parameters
            var parameters = ConvertDataToParameters(data);

            //get the panel that this media is applied to
            parameters.TryGetValue(paramPanel, out panelName);
            GraphicPanel panel = GraphicPanelManager.instance.GetPanel(panelName);
            if (panel == null)
            {
                Debug.LogError($"unable to grab panel '{panelName}' because it is not a valid panel");
                yield break;
            }

            parameters.TryGetValue(paramLayer, out layer, defaultValue: -1);

            parameters.TryGetValue(paramImmediate, out immediate, defaultValue: false);

            if (!immediate)
            {
                parameters.TryGetValue(paramSpeed, out transitionSpeed, defaultValue: 1);
            }

            parameters.TryGetValue(paramBlendTex, out blendTexName);

            if(!immediate && blendTexName != string.Empty)
            {
                blendTex = Resources.Load<Texture>(FilePath.resources_blendTextures + blendTexName);
            }

            if(layer == -1)
            {
                panel.Clear(transitionSpeed, blendTex, immediate);
            }
            else
            {
                GraphicLayer graphicLayer = panel.GetLayer(layer);
                if(graphicLayer == null)
                {
                    Debug.LogError($"could not clear layer '{layer}' on panel '{panel.panelName}'");
                    yield break;
                }

                graphicLayer.Clear(transitionSpeed, blendTex, immediate);
            }
        }

        private static string GetPathToGraphic(string defaultPath, string graphicName)
        {
            if (graphicName.StartsWith(homeDirectorySymbol))
            {
                return graphicName.Substring(homeDirectorySymbol.Length);
            }
            return defaultPath + graphicName;
        }
    }
}