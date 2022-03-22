using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 플레이어 기체
    public float _moveSpeed;

    // 경계선
    public bool _isTouchTop;
    public bool _isTouchBottom;
    public bool _isTouchLeft;
    public bool _isTouchRight;

    // 발사체
    public float _bulletSpeed;
    public float _bulletPower;
    public float _curShotDelay;         // 한발쏜 다음 충전되기 위한 딜레이
    public float _maxShotDelay;         // 실제 딜레이

    // 발사체 오브젝트
    public GameObject _bulletObjA;
    public GameObject _bulletObjB;

    public GameManager _manager;

    Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Fire();
        Reload();
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");   // 수평
        if ((_isTouchRight && h == 1) || (_isTouchLeft && h == -1))
        {
            h = 0;
        }
        float v = Input.GetAxisRaw("Vertical");     // 수직
        if ((_isTouchTop && v == 1) || (_isTouchBottom && v == -1))
        {
            v = 0;
        }
        Vector3 curPos = transform.position;        // 현재위치
        Vector3 nextPos = new Vector3(h, v, 0) * _moveSpeed * Time.deltaTime;

        transform.position = curPos + nextPos;

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        {
            _animator.SetInteger("Input", (int)h);
        }
    }

    void Fire()
    {

        if (!Input.GetButton("Fire1"))
        {
            return;
        }

        if (_curShotDelay < _maxShotDelay)
        {
            return;
        }

        switch (_bulletPower)
        {
            case 1:
                // Power 1
                GameObject bullet = Instantiate(_bulletObjA, transform.position, transform.rotation);
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * _bulletSpeed, ForceMode2D.Impulse);
                break;
            case 2:
                // Power 2 : Double Shot
                GameObject doubleBulletL = Instantiate(_bulletObjA, transform.position + Vector3.left * 0.1f, transform.rotation);
                GameObject doubleBulletR = Instantiate(_bulletObjA, transform.position + Vector3.right * 0.1f, transform.rotation);
                Rigidbody2D doubleBulletRigidL = doubleBulletL.GetComponent<Rigidbody2D>();
                Rigidbody2D doubleBulletRigidR = doubleBulletR.GetComponent<Rigidbody2D>();
                doubleBulletRigidL.AddForce(Vector2.up * _bulletSpeed, ForceMode2D.Impulse);
                doubleBulletRigidR.AddForce(Vector2.up * _bulletSpeed, ForceMode2D.Impulse);
                break;
            case 3:
                // Power 3 : Triple Shot
                GameObject TriplebulletL = Instantiate(_bulletObjA, transform.position + Vector3.left * 0.4f, transform.rotation);
                GameObject TripleBulletC = Instantiate(_bulletObjB, transform.position, transform.rotation);
                GameObject TripleBulletR = Instantiate(_bulletObjA, transform.position + Vector3.right * 0.4f, transform.rotation);
                Rigidbody2D TripleBulletRigidL = TriplebulletL.GetComponent<Rigidbody2D>();
                Rigidbody2D TripleBulletRigidC = TripleBulletC.GetComponent<Rigidbody2D>();
                Rigidbody2D TripleBulletRigidR = TripleBulletR.GetComponent<Rigidbody2D>();
                TripleBulletRigidL.AddForce(Vector2.up * _bulletSpeed, ForceMode2D.Impulse);
                TripleBulletRigidC.AddForce(Vector2.up * _bulletSpeed, ForceMode2D.Impulse);
                TripleBulletRigidR.AddForce(Vector2.up * _bulletSpeed, ForceMode2D.Impulse);
                break;
        }
        _curShotDelay = 0;
    }

    void Reload()
    {
        _curShotDelay += Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    _isTouchTop = true;
                    break;
                case "Bottom":
                    _isTouchBottom = true;
                    break;
                case "Left":
                    _isTouchLeft = true;
                    break;
                case "Right":
                    _isTouchRight = true;
                    break;
            }
        }
        else if (collision.gameObject.tag == "Enemy" || 
                collision.gameObject.tag == "EnemyBullet")
        {
            _manager.RespawnPlayer();
            gameObject.SetActive(false);
            // Invoke는 SetActive가 활성화되어야 가능하므로 GameManager로 넘겨준다.

        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    _isTouchTop = false;
                    break;
                case "Bottom":
                    _isTouchBottom = false;
                    break;
                case "Left":
                    _isTouchLeft = false;
                    break;
                case "Right":
                    _isTouchRight = false;
                    break;
            }
        }
    }

}
