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
        private readonly string _tileMapFileName;

        public GameAssetsProvider(ICanvasAnimatedControl canvasControl, string rootPath, string tileMapFileName)
        {
            _canvasControl = canvasControl;
            _rootPath = rootPath;
            _tileMapFileName = tileMapFileName;
        }

        public Map Map { get; private set; }

        public async Task LoadAssetsAsync()
        {
            var tiledLoader = new TiledLoader();
            Map = await tiledLoader.LoadResourceAsync<Map>(new Uri($"{_rootPath}/{_tileMapFileName}"));

            foreach (var tileSet in Map.TileSets)
            {
                tileSet.TileAtlas = await tiledLoader.LoadResourceAsync<TileAtlas>(new Uri($"{_rootPath}/{tileSet.Source}"));
                tileSet.TileAtlas.Bitmap = await CanvasBitmap.LoadAsync(_canvasControl, new Uri($"{_rootPath}/{tileSet.TileAtlas.ImageSource}")).AsTask();
            }
        }

        public bool TryGetTileSet(int id, out TileSet tileSet)
        {
            tileSet = default;

            var matchingTileSet = Map.TileSets
                .FirstOrDefault(x => id >= x.FirstGid && id < (x.FirstGid + x.TileAtlas.TileCount));

            if (matchingTileSet is null)
            {
                return false;
            }

            tileSet = matchingTileSet;

            return true;
        }

        public bool TryGetTileSet(string name, out TileSet tileSet)
        {
            tileSet = default;

            tileSet = Map.TileSets
                .Where(x => x.TileAtlas.CustomProperties.Any(p => string.Equals(p.Name, name, StringComparison.InvariantCultureIgnoreCase)))
                .FirstOrDefault();

            return tileSet != null;
        }
    }
}
