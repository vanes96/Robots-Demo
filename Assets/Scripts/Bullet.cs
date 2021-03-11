using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    [SerializeField]
    private float _damage;

    public float Damage => _damage;

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(LayerMask.NameToLayer(Constanter.FloorLayerName));
        if (collision.gameObject.layer == LayerMask.NameToLayer(Constanter.EnemyLayerName) || 
            collision.gameObject.layer == LayerMask.NameToLayer(Constanter.PlayerLayerName))
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer(Constanter.FloorLayerName))
        {
            Destroy(gameObject, 0.5f);
        }
    }
}
