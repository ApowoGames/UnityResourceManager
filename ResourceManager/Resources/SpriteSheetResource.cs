using System;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;

namespace ApowoGames.Resources
{
    public class SpriteSheetResource : Resource
    {
        public Dictionary<string, Sprite> Sprites;
        public Texture2D Texture2D;

        public static SpriteSheetRequest BuildRequest(string uri)
        {
            return new SpriteSheetRequest(uri);
        }
        
        public SpriteSheetResource(ResponseFile[] responses) : base(responses)
        {
            ImageResponseFile imageResponseFile = null;
            JsonResponseFile jsonResponseFile = null;
            foreach (var response in responses)
            {
                if (response.MimeType == MimeType.Image)
                {
                    imageResponseFile = response as ImageResponseFile;
                }

                if (response.MimeType == MimeType.Json)
                {
                    jsonResponseFile = response as JsonResponseFile;
                }
            }
            
            if (imageResponseFile?.Data == null || jsonResponseFile?.Data == null) return;

            Texture2D = imageResponseFile.Data;
            var jsonStr = jsonResponseFile.Data;
            var texWidth = Texture2D.width;
            var texHeight = Texture2D.height;
            var jsonResult = SpriteSheetJson.CreateFromJson(jsonStr);
            if (jsonResult?.frames == null)
            {
                Debug.LogError($"cannot parse response {jsonStr}");
                return;
            }

            Sprites = new Dictionary<string, Sprite>();
            foreach (var f in jsonResult.frames)
            {
                Sprite sprite = Sprite.Create(Texture2D, SpriteSheetRectToUnitySpriteEditorRect(f.frame, texWidth, texHeight), new Vector2(0f, 1f));
                sprite.name = f.filename;
                Sprites[sprite.name] = sprite;
            }
        }
        
        private static Rect SpriteSheetRectToUnitySpriteEditorRect(SpriteSheetFrameRect spriteSheetFrameRect, int texWidth, int texHeight)
        {
            return new Rect(
                spriteSheetFrameRect.x,
                texHeight - spriteSheetFrameRect.y - spriteSheetFrameRect.h,
                spriteSheetFrameRect.w,
                spriteSheetFrameRect.h);
        }
    }
    
    [Serializable]
    public class SpriteSheetJson
    {
        public SpriteSheetFrame[] frames;

        public static SpriteSheetJson CreateFromJson(string jsonString)
        {
            // var spriteSheetJson = new SpriteSheetJson();
            //
            // var jo = JsonSerializer.Deserialize(jsonString) as JsonObject;
            // var framesStr = jo["frames"].ToString();
            // var framesJo = JsonSerializer.Deserialize(framesStr) as JsonArray;
            //
            // spriteSheetJson.frames = new SpriteSheetFrame[framesJo.Count];
            // for (int i = 0; i < framesJo.Count; i++)
            // {
            //     var spriteSheetFrame = new SpriteSheetFrame();
            //     var spriteSheetFrameRect = new SpriteSheetFrameRect();
            //     spriteSheetFrame.frame = spriteSheetFrameRect;
            //     var frameJo = framesJo[i] as JsonObject;
            //     spriteSheetFrame.filename = frameJo["filename"].ToString();
            //     var frameRectStr = frameJo["frame"].ToString();
            //     var frameRectJo = JsonSerializer.Deserialize(frameRectStr) as JsonObject;
            //     spriteSheetFrameRect.x = (frameRectJo["x"] as JsonPrimitive).ToInt32(null);
            //     spriteSheetFrameRect.y = (frameRectJo["y"] as JsonPrimitive).ToInt32(null);
            //     spriteSheetFrameRect.w = (frameRectJo["w"] as JsonPrimitive).ToInt32(null);
            //     spriteSheetFrameRect.h = (frameRectJo["h"] as JsonPrimitive).ToInt32(null);
            //
            //     spriteSheetJson.frames[i] = spriteSheetFrame;
            // }
            //
            // return spriteSheetJson;
            return JsonUtility.FromJson<SpriteSheetJson>(jsonString);
        }
    }

    [Serializable]
    public class SpriteSheetFrame
    {
        public string filename;
        public SpriteSheetFrameRect frame;
    }
    [Serializable]
    public class SpriteSheetFrameRect
    {
        public int x;
        public int y;
        public int w;
        public int h;
    }
}