using System;
using SLZ.Marrow.Data;
using SLZ.Marrow;
using TMPro;
using UnityEngine;

namespace LabWorks.Bonelab
{
	// Todo: Testing, Audio Playing
	[RegisterTypeInIl2Cpp]
	public class Control_MonoMat : MonoBehaviour
	{
		public Control_MonoMat(IntPtr ptr) : base(ptr) {}

		[SerializeField]
		private CrateSpawner[] _loots;

		private List<Rigidbody> _lootBodies;

		[SerializeField]
		private HingeController _doorLeverCircuit;

		[SerializeField]
		private ValueCircuit _doorMotorCircuit;

		[SerializeField]
		private int _multiplier;

		[SerializeField]
		private int _itemPrice;

		[SerializeField]
		private int _lightBullets;

		[SerializeField]
		private int _mediumBullets;

		[SerializeField]
		private int _heavyBullets;

		[SerializeField]
		private bool _opened;

		[SerializeField]
		private bool _unlocked;

		[SerializeField]
		private TextMeshProUGUI _bulletBalanceTextmesh;

		[SerializeField]
		private TextMeshProUGUI _refundTextmesh;

		[SerializeField]
		private AmmoReciever reciever;

		[SerializeField]
		private Spawnable lightRefundSpawnable;

		[SerializeField]
		private Spawnable mediumRefundSpawnable;

		[SerializeField]
		private Spawnable heavyRefundSpawnable;

		[SerializeField]
		private AudioClip _openedClip;

		[SerializeField]
		private AudioClip _unlockedClip;

		[SerializeField]
		private AudioClip _lockedClip;

		public Transform giveChangeTransform;

		private void Start()
		{
            reciever.OnInsert += InsertMagazine;
            _bulletBalanceTextmesh = (_itemPrice * _multiplier);
			_doorMotorCircuit.Value = 1f;

			foreach(CrateSpawner loot in _loots)
			{
				var call = loot.onSpawnEvent.AddPersistentListener(OnLootSpawned);
			}
		}

		public void OnLootSpawned(CrateSpawner spawner, GameObject entity)
		{
			foreach(Rigidbody rb in entity.GetComponentsInChildren<Rigidbody>())
			{
				rb.isKinematic = true;
				_lootBodies.Add(rb);
			}
		}

		public void GiveChange()
		{
            if(_lightBullets > 0)
            {
                AssetSpawner.SpawnAsync(lightRefundSpawnable, giveChangeTransform.position, giveChangeTransform.rotation, Vector3.one);
            }
            if(_mediumBullets > 0)
            {
                AssetSpawner.SpawnAsync(mediumRefundSpawnable, giveChangeTransform.position, giveChangeTransform.rotation, Vector3.one);
            }
            if(_heavyBullets > 0)
            {
                AssetSpawner.SpawnAsync(heavyRefundSpawnable, giveChangeTransform.position, giveChangeTransform.rotation, Vector3.one);
            }

            _lightBullets = 0;
            _mediumBullets = 0;
            _heavyBullets = 0;

			_refundTextmesh.text = "0";
		}

		public void PurchaseItem()
		{
			float ammoToRemove = _itemPrice * _multiplier;
			
			if(_lightBullets + _mediumBullets + _heavyBullets < ammoToRemove)
			{
				_doorMotorCircuit.Value = 1;
				return;
			}

			ammoToRemove = CleanupLight(ammoToRemove);
			ammoToRemove = CleanupMedium(ammoToRemove);
			CleanupHeavy(ammoToRemove);

			foreach(Rigidbody loot in _lootBodies)
			{
				loot.isKinematic = false;
			}

			_refundTextmesh.text = (_lightBullets + _mediumBullets + _heavyBullets).ToString();

		}

		private float CleanupLight(float ammoToRemove)
		{
			if(ammoToRemove > _lightBullets)
			{
				ammoToRemove -= _lightBullets;
				_lightBullets = 0;
			}
			else
			{
				_lightBullets -= ammoToRemove;
				ammoToRemove = 0;
			}
			return ammoToRemove;
		}

		private float CleanupMedium(float ammoToRemove)
		{
			if(ammoToRemove > _lightBullets)
			{
				ammoToRemove -= _mediumBullets;
				_mediumBullets = 0;
			}
			else
			{
				_mediumBullets -= ammoToRemove;
				ammoToRemove = 0;
			}
			return ammoToRemove;
		}

		private float CleanupHeavy(float ammoToRemove)
		{
			if(ammoToRemove > _heavyBullets)
			{
				ammoToRemove -= _heavyBullets;
				_heavyBullets = 0;
			}
			else
			{
				_heavyBullets -= ammoToRemove;
				ammoToRemove = 0;
			}
			return ammoToRemove;
		}

		public void InsertMagazine(Magazine magazine)
		{
            AddBullets(magazine.magazineState.AmmoCount, 0);

			if(_lightBullets + _mediumBullets + _heavyBullets >= _itemPrice * _multiplier && !_unlocked && !_opened)
			{
				_unlocked = true;
				_doorMotorCircuit.Value = 0f;
			}
		}

		public void AddBullets(int addedBullets, int type)
		{
            if(type == 0)
            {
                _lightBullets += addedBullets;
            }
            else if(type == 1)
            {
                _mediumBullets += addedBullets;
            }
            else if(type == 1)
            {
                _heavyBullets += addedBullets;
            }

            _bulletBalanceTextmesh.text = (Mathf.Max(_itemPrice * _multiplier - (_lightBullets + _mediumBullets + _heavyBullets), 0f)).ToString();
            _refundTextmesh.text = (_lightBullets + _mediumBullets + _heavyBullets).ToString();

		}

		public Rigidbody[] GetLoots()
		{
			return _lootBodies.ToArray();
		}

		public void SetPrice(int price, int mult)
		{
            _itemPrice = price;
            _multiplier = mult;
		}

		public void SetLoots(Rigidbody[] loots)
		{
		}
	}
}