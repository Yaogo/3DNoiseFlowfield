using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFlowField : MonoBehaviour
{
    FastNoise _fastNoise;
    public Vector3Int _gridSize;
    public float _cellSize;
    public Vector3[,,] _flowfieldDirection;
    public float _increment;
    public Vector3 _offset, offsetSpeed;

    public GameObject _particlePrefab;
    public int _amountOfParticles;
    [HideInInspector]
    public List<FlowFieldParticle> _particles;
    public float _particleScale;
    public float _spawnRadius;
    bool _particleSpawnValidation(Vector3 position)
    {
        bool valid = true;
        foreach (FlowFieldParticle particle in _particles)
        {
            if (Vector3.Distance(position, particle.transform.position) < _spawnRadius)
            {
                valid = false;
                break;
            }
        }
        if (valid)
            return true;
        else
            return false;
    }
    void Start()
    {
        _flowfieldDirection = new Vector3[_gridSize.x, _gridSize.y, _gridSize.z];
        _fastNoise = new FastNoise();
        _particles = new List<FlowFieldParticle>();

        for (int i = 0; i < _amountOfParticles; i++)
        {
            int attempt = 0;
            while (attempt < 100)
            {
                Vector3 randomPos = new Vector3(Random.Range(transform.position.x, transform.position.x + _gridSize.x * _cellSize),
                                Random.Range(transform.position.y, transform.position.y + _gridSize.y * _cellSize),
                                Random.Range(transform.position.z, transform.position.z + _gridSize.z * _cellSize));
                bool isValid = _particleSpawnValidation(randomPos);
                if (isValid)
                {
                    GameObject particleInstance = Instantiate(_particlePrefab, transform);
                    particleInstance.transform.position = randomPos;
                    particleInstance.transform.localScale = new Vector3(_particleScale, _particleScale, _particleScale);
                    _particles.Add(particleInstance.GetComponent<FlowFieldParticle>());
                    break;
                }
                if (!isValid)
                {
                    attempt++;
                }
            }
            Debug.Log(_particles.Count);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateFlowfieldDirections();
    }
    void CalculateFlowfieldDirections()
    {
        _fastNoise = new FastNoise();
        float xOff = 0f;
        for (int x = 0; x < _gridSize.x; x++)
        {
            float yOff = 0.0f;
            for (int y = 0; y < _gridSize.y; y++)
            {
                float zOff = 0.0f;
                for (int z = 0; z < _gridSize.z; z++)
                {
                    float noise = _fastNoise.GetSimplex(xOff + _offset.x, yOff + _offset.y + zOff + _offset.z) + 1;
                    Vector3 noiseDirection = new Vector3(Mathf.Cos(noise * Mathf.PI), Mathf.Sin(noise * Mathf.PI), Mathf.Cos(noise * Mathf.PI));
                    _flowfieldDirection[x, y, z] = Vector3.Normalize(noiseDirection);
                    //Gizmos.color = new Color(noiseDirection.normalized.x, noiseDirection.normalized.y, noiseDirection.normalized.z);
                    //Vector3 pos = new Vector3(x, y, z) + transform.position;
                    //Vector3 endPos = pos + Vector3.Normalize(noiseDirection);
                    //Gizmos.DrawLine(pos, endPos);
                    //Gizmos.DrawSphere(endPos, 0.1f);
                    zOff += _increment;

                }
                yOff += _increment;
            }
            xOff += _increment;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + new Vector3((_gridSize.x * _cellSize) * 0.5f, (_gridSize.y * _cellSize) * 0.5f, (_gridSize.z * _cellSize) * 0.5f), new Vector3(_gridSize.x * _cellSize, _gridSize.y * _cellSize, _gridSize.z * _cellSize));

        //_fastNoise = new FastNoise();
        //float xOff = 0f;
        //for (int x = 0; x < _gridSize.x; x++)
        //{
        //    float yOff = 0.0f;
        //    for (int y = 0; y < _gridSize.y; y++)
        //    {
        //        float zOff = 0.0f;
        //        for (int z = 0; z < _gridSize.z; z++)
        //        {
        //            float noise = _fastNoise.GetSimplex(xOff + _offset.x, yOff + _offset.y + zOff + _offset.z) + 1;
        //            Vector3 noiseDirection = new Vector3(Mathf.Cos(noise * Mathf.PI), Mathf.Sin(noise * Mathf.PI), Mathf.Cos(noise * Mathf.PI));
        //            Gizmos.color = new Color(noiseDirection.normalized.x, noiseDirection.normalized.y, noiseDirection.normalized.z);
        //            Vector3 pos = new Vector3(x, y, z) + transform.position;
        //            Vector3 endPos = pos + Vector3.Normalize(noiseDirection);
        //            Gizmos.DrawLine(pos, endPos);
             
        //            zOff += _increment;

        //        }
        //        yOff += _increment;
        //    }
        //    xOff += _increment;
        //}
    }
}
