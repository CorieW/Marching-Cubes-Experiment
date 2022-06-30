using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rubbish_Implementation
{
    public class Controller : MonoBehaviour
    {
        public const float SURFACE_LEVEL = 0;

        public static Controller Instance;

        [SerializeField] private Vector3Int _size;
        [SerializeField] private MeshGenerator _meshGenerator;

        private double[,,] _grid;

        private void Awake()
        {
            Instance = this;

            _grid = new double[_size.x, _size.y, _size.z];
            
            Noise noise = new Noise(3434);
            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.y; y++)
                {
                    for (int z = 0; z < _size.z; z++)
                    {
                        double noiseVal = noise.Evaluate((double)x / 5, (double)y / 5, (double)z / 5);
                        // Debug.Log(noiseVal);
                        _grid[x, y, z] = noiseVal;
                    }
                }
            }
        }

        private void Start()
        {
            _meshGenerator.GenerateSoft(new MarchGrid(_grid));
        }

        public double GetGridPointToggle(Vector3Int pos)
        {
            return _grid[pos.x, pos.y, pos.z];
        }
    }
    public class MarchGrid
    {
        private double[,,] _grid;

        public MarchGrid(double[,,] grid)
        {
            this._grid = grid;
        }

        public double GetValueAt(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0) return -1;
            if (x >= _grid.GetLength(0) || y >= _grid.GetLength(1) || z >= _grid.GetLength(2)) return -1;

            return _grid[x, y, z];
        }

        public int GetLength(int axis)
        {
            return _grid.GetLength(axis);
        }
    }
}