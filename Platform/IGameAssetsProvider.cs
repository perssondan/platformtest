using Microsoft.Graphics.Canvas;
using System.Threading.Tasks;
using uwpPlatformer.Models;

namespace uwpPlatformer.Platform
{
    public interface IGameAssetsProvider
    {
        Task LoadAssetsAsync();
        CanvasBitmap[] Bitmaps { get; }
        Map Map { get; }
        TileAtlas[] TileAtlases { get; }
    }
}
