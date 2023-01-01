using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Linq;
using System.Threading.Tasks;
using uwpPlatformer.Models;
using uwpPlatformer.Utilities;

namespace uwpPlatformer.Platform
{
    public class GameAssetsProvider : IGameAssetsProvider
    {
        private readonly ICanvasAnimatedControl _canvasControl;
        private readonly string _rootPath;
        private readonly string _tilmapFileName;

        public GameAssetsProvider(ICanvasAnimatedControl canvasControl, string rootPath, string tilmapFileName)
        {
            _canvasControl = canvasControl;
            _rootPath = rootPath;
            _tilmapFileName = tilmapFileName;
        }

        public async Task LoadAssetsAsync()
        {
            var tiledLoader = new TiledLoader();
            Map = await tiledLoader.LoadResourceAsync<Map>(new Uri($"{_rootPath}/{_tilmapFileName}"));

            TileAtlases = await Task.WhenAll(Map.TileSets
                .Select(tileSet => tiledLoader.LoadResourceAsync<TileAtlas>(new Uri($"{_rootPath}/{tileSet.Source}"))));

            Bitmaps = await Task.WhenAll(TileAtlases
                .Select(tileAtlas => CanvasBitmap.LoadAsync(_canvasControl, new Uri($"{_rootPath}/{tileAtlas.ImageSource}")).AsTask()));
        }

        public CanvasBitmap[] Bitmaps { get; private set; }
        public Map Map { get; private set; }
        public TileAtlas[] TileAtlases { get; private set; }
    }
}
