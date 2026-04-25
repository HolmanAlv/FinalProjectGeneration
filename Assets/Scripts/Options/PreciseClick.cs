using UnityEngine;
using UnityEngine.UI;

public class PreciseClick : MonoBehaviour {
    void Start() {
        // Solo detecta click donde el alpha sea mayor a 0.1
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
    }
}