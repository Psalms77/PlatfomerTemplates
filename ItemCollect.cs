using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollect : MonoBehaviour
{
    //public GameObject collectEffect;
    public GameObject itemCollectEffect;
    private SpriteRenderer _spriteRenderer;
    private CircleCollider2D _circleCollider2d;

    /// <summary>
    /// 给可拾取物的拾取代码，未加动画，注意可拾取物为trigger
    /// </summary>
    // Start is called before the first frame update
    void Start()
    {
        _circleCollider2d = GetComponent<CircleCollider2D>();   
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")  //注意给对应的gameobject上tag
        {
            _spriteRenderer.enabled = false;
            _circleCollider2d.enabled = false;

            itemCollectEffect.SetActive(true);
            Destroy(gameObject, 1f); //此处1s为动画播放时间
        }
    }
}
