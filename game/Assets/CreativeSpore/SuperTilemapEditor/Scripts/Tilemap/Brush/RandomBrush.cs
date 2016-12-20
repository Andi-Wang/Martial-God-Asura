using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CreativeSpore.SuperTilemapEditor
{
    public class RandomBrush : TilesetBrush
    {
        [System.Obsolete("Use RandomTileList instead")]
        public List<uint> RandomTiles = new List<uint>();
        
        [System.Serializable]
        public class RandomTileData
        {
            public uint tileData;
            [Range(0f, 1f)]
            public float probabilityFactor;            
        }
        public List<RandomTileData> RandomTileList = new List<RandomTileData>();

        public uint RandomizeFlagMask = 0u;

        void OnEnable()
        {
            // fix compatibility with previous versions using RandomTiles
#pragma warning disable 618
            if ( (RandomTileList == null || RandomTileList.Count == 0) && RandomTiles != null && RandomTiles.Count > 0)
            {
                //Debug.Log(name + " Fix " + RandomTiles.Count);
                RandomTileList = new List<RandomTileData>(RandomTiles.Select(x => new RandomTileData() { tileData = x, probabilityFactor = 1f }));
                RandomTiles = null;
            }
#pragma warning restore 618

            InvalidateSortedList();
        }

        public void InvalidateSortedList()
        {
            m_sortedList = new List<RandomTileData>(RandomTileList.OrderBy(x => x.probabilityFactor));
            m_sumProbabilityFactor = Mathf.Max(GetSumProbabilityFactor(), float.Epsilon);
        }

        private List<RandomTileData> m_sortedList;
        private float m_sumProbabilityFactor;
        public uint GetRandomTile()
        {
            float randPercent = Random.value;
            //float sumProbabilityFactor = Mathf.Max(GetSumProbabilityFactor(), float.Epsilon); //commented to fix GC allocation
            if (m_sortedList == null || m_sortedList.Count == 0) InvalidateSortedList();
            for (int i = 0; i < m_sortedList.Count; ++i)
            {
                RandomTileData randomTileData = m_sortedList[i];
                float probability = randomTileData.probabilityFactor / m_sumProbabilityFactor;
                if (randPercent <= probability)
                {
                    return randomTileData.tileData;
                }
                randPercent -= probability;
            }
            return m_sortedList.Count > 0 ? m_sortedList[m_sortedList.Count - 1].tileData : Tileset.k_TileData_Empty;
        }

        public float GetSumProbabilityFactor()
        {
            return RandomTileList.Sum(x => x.probabilityFactor);
        }

        #region IBrush

        public override uint PreviewTileData()
        {
            return RandomTileList.Count > 0 ? RandomTileList[0].tileData : Tileset.k_TileData_Empty;
        }

        public override uint Refresh(Tilemap tilemap, int gridX, int gridY, uint tileData)
        {
            if (RandomTileList.Count > 0)
            {
                uint randomTileData = GetRandomTile();
                if (RandomizeFlagMask != 0)
                {
                    uint flags = ((uint)Random.Range(0, 8) << 29) & RandomizeFlagMask;
                    randomTileData &= ~RandomizeFlagMask;
                    randomTileData |= flags;
                }
                uint brushTileData = RefreshLinkedBrush(tilemap, gridX, gridY, randomTileData);
                // overwrite flags
                brushTileData &= ~Tileset.k_TileDataMask_Flags;
                brushTileData |= randomTileData & Tileset.k_TileDataMask_Flags;
                

                TilesetBrush brush = Tileset.FindBrush(Tileset.GetBrushIdFromTileData(brushTileData));
                if (brush && brush.IsAnimated())
                {
                    // Set the animated brush (overwriting the random brush for this tile)
                    brushTileData &= ~Tileset.k_TileDataMask_BrushId;
                    brushTileData |= randomTileData & Tileset.k_TileDataMask_BrushId;
                }

                // - Commented:
                // This code will make the Random Brush to work only once
                // Copy the tile will copy the tile placed the first time
                // - Uncommented:
                // This code will make Refresh Tilemap to update all random brushes
                // Copy the tile will copy a random brush tile
                /*
                {
                    // overwrite brush id
                    brushTileData &= ~Tileset.k_TileDataMask_BrushId;
                    brushTileData |= tileData & Tileset.k_TileDataMask_BrushId;
                }*/

                return brushTileData;
            }
            return tileData;
        }

        #endregion
    }
}