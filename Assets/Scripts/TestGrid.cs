using narkdagas.tbcs;
using UnityEngine;

public class TestGrid : MonoBehaviour {
    public Transform debugPrefab; 
    private GridSystem _gridSystem;

    // Start is called before the first frame update
    void Start() {
        _gridSystem = new GridSystem(10, 10, 2, true, debugPrefab);
    }

    // Update is called once per frame
    void Update() {
        Debug.Log(_gridSystem.GetGridPosition(MouseWorld.GetPosition()));
    }
}