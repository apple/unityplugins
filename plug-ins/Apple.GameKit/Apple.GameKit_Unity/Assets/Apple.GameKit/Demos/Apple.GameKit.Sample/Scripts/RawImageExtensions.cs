using UnityEngine;
using UnityEngine.UI;

namespace Apple.GameKit.Sample
{
    static class RawImageExtensions
    {
        public static void DestroyTexture(this RawImage image)
        {
            if (image?.texture != null)
            {
                var texture = image.texture;
                image.texture = null;
                Object.Destroy(texture);
            }
        }

        public static void DestroyTextureAndAssign(this RawImage image, Texture2D newTexture)
        {
            image.DestroyTexture();
            image.texture = newTexture ?? Texture2D.whiteTexture;
        }
    }
}