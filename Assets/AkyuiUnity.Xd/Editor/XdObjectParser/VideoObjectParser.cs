using System.Collections.Generic;
using System.Linq;
using AkyuiUnity.Loader;
using Unity.VectorGraphics;
using UnityEngine;
using XdParser;
using XdParser.Internal;

namespace AkyuiUnity.Xd
{
    public class VideoObjectParser : IXdObjectParser
    {
        bool IXdObjectParser.Is(XdObjectJson xdObject)
        {
            return Is(xdObject);
        }

        public static bool Is(XdObjectJson xdObject)
        {
            return xdObject.Type == "shape" && xdObject.Shape?.Meta?.Ux?.SubType == "video";
        }

        Rect IXdObjectParser.CalcSize(XdObjectJson xdObject)
        {
            return CalcSize(xdObject);
        }

        public static Rect CalcSize(XdObjectJson xdObject)
        {
            var position = Vector2.zero;
            var size = Vector2.zero;

            var xdObjectShape = xdObject.Shape;
            var shapeType = xdObjectShape.Type;
            if (shapeType == "rect")
            {
                position.x = xdObjectShape.X;
                position.y = xdObjectShape.Y;
                size.x = xdObjectShape.Width;
                size.y = xdObjectShape.Height;
            }
            return new Rect(position, size);
        }

        public (IComponent[], IAsset[]) Render(XdObjectJson xdObject, Obb obb, XdAssetHolder assetHolder)
        {
            var (imageComponent, assets) = RenderVideo(xdObject, obb, assetHolder);
            return (new IComponent[] { imageComponent }, assets);
        }

        public static (VideoComponent, IAsset[]) RenderVideo(XdObjectJson xdObject, Obb obb, XdAssetHolder assetHolder)
        {
            VideoComponent videoComponent = null;
            var assets = new List<IAsset>();

            var ux = xdObject.Shape?.Meta?.Ux;
            var isPlaceholder = xdObject.HasParameter("placeholder");

            if (!string.IsNullOrWhiteSpace(ux?.MediaFileGuid))
            {
                string mediaUid = null;
                if (!isPlaceholder)
                {
                    mediaUid = $"{xdObject.GetSimpleName()}_{ux?.MediaFileGuid.Substring(0, 8)}";
                    var fileName = mediaUid + ".mp4";
                    var asset = new MediaAsset(fileName, FastHash.CalculateHash(fileName), obb.Size, null);
                    assetHolder.Save(fileName, xdObject.Shape.Meta.Ux);
                    assets.Add(asset);
                }
                videoComponent = new VideoComponent(mediaUid);
            }
            else
            {
                Debug.LogError($"Invalid MediaFileGuid in {xdObject.Name}({xdObject.Id}, {xdObject.Guid})");
            }

            return (videoComponent, assets.ToArray());
        }
    }
}
