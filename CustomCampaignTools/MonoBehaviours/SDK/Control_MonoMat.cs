using System;
using SLZ.Marrow.Data;
using SLZ.Marrow;
using TMPro;
using UnityEngine;

namespace CustomCampaignTools.LabWorks
{
	// Todo: Testing, Audio Playing
	[RegisterTypeInIl2Cpp]
	public class Control_MonoMat : MonoBehaviour
	{
		public Control_MonoMat(IntPtr ptr) : base(ptr) {}

		private CrateSpawner[] _loots;
        public void SetLootsFromParent(GameObject parent) => _loots = parent.GetComponentsInChildren<CrateSpawner>();

		private List<Rigidbody> _lootBodies;

		private HingeController _doorLeverCircuit;
		private ValueCircuit _doorMotorCircuit;

		private int _multiplier { get; set; }
		private int _itemPrice { get; set; }

        private int _trueItemPrice => _itemPrice * _multiplier;

		private int _lightBullets;
		private int _mediumBullets;
		private int _heavyBullets;

        private int _totalBullets => _lightBullets + _mediumBullets + _heavyBullets;

		[SerializeField]
		private bool _opened;

		[SerializeField]
		private bool _unlocked;

		private TextMeshProUGUI _bulletBalanceTextmesh { get; set; }
		private TextMeshProUGUI _refundTextmesh { get; set; }

		private AmmoReciever reciever { get; set; }

		private Spawnable lightRefundSpawnable { get; set; }
		private Spawnable mediumRefundSpawnable { get; set; }
		private Spawnable heavyRefundSpawnable { get; set; }

		private AudioClip _openedClip { get; set; }
		private AudioClip _unlockedClip { get; set; }
		private AudioClip _lockedClip { get; set; }

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