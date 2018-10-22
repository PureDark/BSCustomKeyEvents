using IllusionPlugin;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomKeyEvents
{
    public class Plugin : IPlugin
    {

        public string Name => "CustomKeyEvents";
        public string Version => "0.0.1";

        
        public void OnApplicationStart()
        {
        }

        public void OnApplicationQuit()
        {
        }


        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
        }

        public static bool SaveRenderTextureToPNG(Texture inputTex, Shader outputShader, string contents, string pngName)
        {
            RenderTexture temp = RenderTexture.GetTemporary(inputTex.width, inputTex.height, 0, RenderTextureFormat.ARGB32);
            Material mat = new Material(outputShader);
            Graphics.Blit(inputTex, temp, mat);
            bool ret = SaveRenderTextureToPNG(temp, contents, pngName);
            RenderTexture.ReleaseTemporary(temp);
            return ret;
        }
        public static bool SaveRenderTextureToPNG(RenderTexture rt, string contents, string pngName)
        {
            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;
            Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
            png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            byte[] bytes = png.EncodeToPNG();
            if (!Directory.Exists(contents))
                Directory.CreateDirectory(contents);
            FileStream file = File.Open(contents + "/" + pngName + ".png", FileMode.Create);
            BinaryWriter writer = new BinaryWriter(file);
            writer.Write(bytes);
            file.Close();
            Texture2D.DestroyImmediate(png);
            png = null;
            RenderTexture.active = prev;
            return true;
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
        }

        public void OnLevelWasLoaded(int level)
        {
        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }
    }
}
