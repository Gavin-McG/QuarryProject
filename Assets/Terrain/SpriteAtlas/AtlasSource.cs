using System;
using UnityEngine;

namespace Terrain.SpriteAtlas
{
    [Serializable]
    public struct AtlasSource
    {
        public enum SourceType
        {
            Sprite,
            Texture
        }

        [SerializeField] public SourceType sourceType;
        [SerializeField] public Sprite sprite;
        [SerializeField] public Texture2D texture;

        public AtlasSource(Sprite sprite)
        {
            sourceType = SourceType.Sprite;
            this.sprite = sprite;
            texture = null;
        }

        public AtlasSource(Texture2D texture)
        {
            sourceType = SourceType.Texture;
            sprite = null;
            this.texture = texture;
        }

        public Vector2Int GetResolution() => sourceType switch
        {
            SourceType.Sprite => new Vector2Int((int)sprite.rect.width, (int)sprite.rect.height),
            SourceType.Texture => new Vector2Int(texture.width, texture.height),
            _ => Vector2Int.zero
        };

        public void WriteToTexture(Texture2D writeTexture, RectInt area)
        {
            switch (sourceType)
            {
                case SourceType.Sprite:
                {
                    Texture2D spriteTexture = sprite.texture;
                    for (int x = 0; x < area.width; x++)
                    {
                        for (int y = 0; y < area.height; y++)
                        {
                            Color pixel = spriteTexture.GetPixel((int)sprite.rect.xMin + x, (int)sprite.rect.yMin + y);
                            writeTexture.SetPixel(area.x + x, area.y + y, pixel);
                        }
                    }
                    break;
                }
                case SourceType.Texture:
                {
                    for (int x = 0; x < area.width; x++)
                    {
                        for (int y = 0; y < area.height; y++)
                        {
                            Color pixel = texture.GetPixel(x, y);
                            writeTexture.SetPixel(area.x + x, area.y + y, pixel);
                        }
                    }
                    break;
                }
            }
            
            Rect uv = new Rect(
                area.x / (float)writeTexture.width, 
                area.y / (float)writeTexture.height, 
                area.width / (float)writeTexture.width, 
                area.height / (float)writeTexture.height
            );
            BlockData.SetSourceUV(this, uv);
        }

        public override int GetHashCode() => sourceType switch
        {
            SourceType.Sprite => sprite.GetHashCode(),
            SourceType.Texture => texture.GetHashCode(),
            _ => 0
        };
    }
}