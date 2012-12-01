using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System.Collections.Generic;
using Random = System.Random;

public class BulletInfo
{
	public BulletInfo(GameObject bullet, float angle)
	{
		Bullet = bullet;
		Angle = angle;
	}
	
	public GameObject Bullet { get; set; }
	public float Angle { get; set; }
	public Vector3 Position
	{
		set
		{
			Bullet.transform.position = value;
		}
		
		get
		{
			return Bullet.transform.position;
		}
	}
}

public class MainScript : MonoBehaviour {
	public AudioClip shot;
	public AudioClip boom;
	
	private int _counter = 0;
	private TextMesh _score;
	
	private List<BulletInfo> _bullets = new List<BulletInfo>();
	
	private GameObject _control, _tank, _bullet, _tower;
	private GameObject _arrowUp, _arrowDown, _arrowRight, _arrowLeft, _shot;
	
	private GameObject _player;
	
	private float _speed = 0.05f;
	private float _bulletSpeed = 0.1f;
	private float _rotSpeed = 2f;
	private float _playerAngle = 90;
	private float _towerAngle = 90;
	
	private int myTimer = 0;
	private int myTimer2 = 0;
	private int startDelay = 300;
	
	void Awake() {
		_control = GameObject.Find("ControlPrefab_Control");
		_tank = GameObject.Find("ControlPrefab_Tank");
		_bullet = GameObject.Find("Bullet");
		_tower = GameObject.Find("Tower");
		
		var a = GameObject.Find("Score");
		_score = (TextMesh)a.GetComponent("TextMesh");
	}
	
	void Start () {
		_arrowDown = (GameObject)Instantiate(_control, Get2DVector(0, 0), Get2DAngle(0));		
		_arrowUp = (GameObject)Instantiate(_control, Get2DVector(0, 2.1f), Get2DAngle(180));		
		_arrowLeft = (GameObject)Instantiate(_control, Get2DVector(15.5f, 0), Get2DAngle(270));
		_arrowRight = (GameObject)Instantiate(_control, Get2DVector(19.7f, 0), Get2DAngle(90));
		
		
		_shot = (GameObject)Instantiate(_control, Get2DVector(17.6f, 0), Get2DAngle(270));
		_shot.renderer.material = GameObject.Find("Shot").renderer.material;
		
		_arrowDown.IncreaseSize(1, 1);
		_arrowUp.IncreaseSize(1, 1);
		_arrowLeft.IncreaseSize(1, 1);
		_arrowRight.IncreaseSize(1, 1);
		_shot.IncreaseSize(1, 1);
		
		_player = (GameObject)Instantiate(_tank, Get2DVector(10, 4), Get2DAngle(0));
		
		_tower.transform.position = Get2DVector(15, 4);
		_tower.transform.rotation = Get2DAngle(0);
		
	}
	
	private Quaternion Get2DAngle(float angle)
	{
		return Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
	}
	
	private Vector3 Get2DVector(float x, float y)
	{
		return new Vector3(x, y, 0);
	}
	
	private void GetClickedGameObject()
	{
		RaycastHit hit;
		
		if(Input.GetMouseButton(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, 10f))
			{
				MovePlayer(hit.transform.gameObject);
			}
		}
		
		foreach (var item in Input.touches)
		{
			Ray ray = Camera.main.ScreenPointToRay(item.position);
			if (Physics.Raycast(ray, out hit, 10f))
			{
				MovePlayer(hit.transform.gameObject);
			}
		}
	}
	
	void OnCollisionEnter(Collision collision)
	{
		Debug.Log(collision.gameObject.name);
	}
	
	private void MovePlayer(GameObject o)
	{
		var pos = _player.transform.position;
		
		if(o == _arrowDown) {			
			pos.x += _speed * Mathf.Cos(_playerAngle * Mathf.PI / 180);
			pos.y += _speed * Mathf.Sin(_playerAngle * Mathf.PI / 180);
		} else if(o == _arrowUp) {			
			pos.x -= _speed * Mathf.Cos(_playerAngle * Mathf.PI / 180);
			pos.y -= _speed * Mathf.Sin(_playerAngle * Mathf.PI / 180);
		} else if(o == _arrowLeft) {
			_playerAngle += _rotSpeed;
		} else if(o == _arrowRight) {
			_playerAngle -= _rotSpeed;
		} else if(o == _shot) {
			Shot();
		}
		
		if(pos.x <= 3.1f)
		{
			pos.x = 3.1f;
		}
		
		if(pos.x >= 20)
		{
			pos.x = 20;
		}
		
		if(pos.y >= 9)
		{
			pos.y = 9;
		}
		
		if(pos.y <= 3.1f)
		{
			pos.y = 3.1f;
		}
		
		_player.transform.position = pos;
		_player.transform.rotation = Get2DAngle(_playerAngle - 90);
	}
	
	private void Shot()
	{
		if(myTimer > 0)
			return;
		myTimer = 20;
		
		var obj = (GameObject)Instantiate(_bullet, _player.transform.position, Get2DAngle(0));
		var bullet = new BulletInfo(obj, _playerAngle);
		
		var pos = bullet.Position;
		pos.x -= 0.7f * Mathf.Cos(_playerAngle * Mathf.PI / 180);
		pos.y -= 0.7f * Mathf.Sin(_playerAngle * Mathf.PI / 180);
		bullet.Position = pos;
		
		_bullets.Add(bullet);
		
		AudioSource.PlayClipAtPoint(shot, _player.transform.position);
	}
	
	void Update () {
		if(startDelay > 0)
		{
			startDelay--;
				return;
		}
		if(myTimer > 0)
			myTimer--;
		if(myTimer2 > 0)
			myTimer2--;
		
		GetClickedGameObject();
		MoveBullets();
		AimTower();
	}
	
	private void AimTower()
	{	
		
		var v1 = _player.transform.position;
		var v2 = _tower.transform.position;
		if(v2.z < 0)
		{
			Random rnd = new System.Random();
			float x = -1;
			float y = -1;
			while(x <= 3.1f || x >= 20 || y >= 9 || y <= 3.1f)
			{
				x = 3.1f + (float)(rnd.NextDouble() * (20 - 3.1f + 1));
				y = 3.1f + (float)(rnd.NextDouble() * (9 - 3.1f + 1));
				if(x > _player.transform.position.x - 0.5f && x < _player.transform.position.x + 0.5f)
				{
					if(y > _player.transform.position.y - 0.5f && y < _player.transform.position.y + 0.5f)
					{
						x = -1;
						y = -1;
					}
				}
			}
			_tower.transform.position = new Vector3(x, y, 0);
			return;
		}
		
		var angleBetween = Mathf.Atan2(v2.y - v1.y, v2.x - v1.x) * 180 / Mathf.PI - 90;
		
		if(_towerAngle < angleBetween)
			_towerAngle += _rotSpeed / 3;
		else
			_towerAngle -= _rotSpeed / 3;
		_tower.transform.rotation = Get2DAngle(_towerAngle);
		
		if(myTimer2 == 0)
		{
			myTimer2 = 50;
			
			var obj = (GameObject)Instantiate(_bullet, v2, Get2DAngle(0));
			var bullet = new BulletInfo(obj, _towerAngle + 90);
			
			var pos = bullet.Position;
			pos.x -= 0.7f * Mathf.Cos((_towerAngle + 90) * Mathf.PI / 180);
			pos.y -= 0.7f * Mathf.Sin((_towerAngle + 90) * Mathf.PI / 180);
			bullet.Position = pos;
			
			_bullets.Add(bullet);
			
			AudioSource.PlayClipAtPoint(shot, v2);
			
		}
	}
	
	private void MoveBullets()
	{
		var removedBullets = new List<BulletInfo>();
		
		foreach (BulletInfo item in _bullets) {
			var pos = item.Position;
			pos.x -= _bulletSpeed * Mathf.Cos(item.Angle * Mathf.PI / 180);
			pos.y -= _bulletSpeed * Mathf.Sin(item.Angle * Mathf.PI / 180);
			item.Position = pos;
			
			if(pos.x <= 3.1f)
			{
				removedBullets.Add(item);
			}
			
			if(pos.x >= 20)
			{
				removedBullets.Add(item);
			}
			
			if(pos.y >= 9)
			{
				removedBullets.Add(item);
			}
			
			if(pos.y <= 3.1f)
			{
				removedBullets.Add(item);
			}
			
			if(pos.x > _tower.transform.position.x - 0.5f && pos.x < _tower.transform.position.x + 0.5f)
			{
				if(pos.y > _tower.transform.position.y - 0.5f && pos.y < _tower.transform.position.y + 0.5f)
				{
					var tpos = _tower.transform.position;
					tpos.z = -1;
					_tower.transform.position = tpos;
					removedBullets.Add(item);
					_counter++;
					_score.text = _counter.ToString();
				}
			}
			
			if(pos.x > _player.transform.position.x - 0.5f && pos.x < _player.transform.position.x + 0.5f)
			{
				if(pos.y > _player.transform.position.y - 0.5f && pos.y < _player.transform.position.y + 0.5f)
				{
					//Destroy(_player);
					_counter = 0;
					AudioSource.PlayClipAtPoint(boom, _player.transform.position);
					_score.text = _counter.ToString();
					removedBullets.Add(item);
				}
			}
			
		}
		
		foreach (var item in removedBullets) {
			_bullets.Remove(item);
			Destroy(item.Bullet);
		}
	}
}
