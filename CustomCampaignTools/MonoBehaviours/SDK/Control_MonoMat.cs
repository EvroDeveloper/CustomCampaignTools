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

		private int _multiplier;
		private int _itemPrice;

        private int _trueItemPrice => _itemPrice * _multiplier;

		private int _lightBullets;
		private int _mediumBullets;
		private int _heavyBullets;

        private int _totalBullets => _lightBullets + _mediumBullets + _heavyBullets;

		private bool _opened;
		private bool _unlocked;

		private TMP_Text _bulletBalanceTextmesh;
		private TMP_Text _refundTextmesh;

		private AmmoReciever reciever;

		private string lightRefundSpawnable;
		private string mediumRefundSpawnable;
		private string heavyRefundSpawnable;

		private AudioClip _openedClip;
		private AudioClip _unlockedClip;
		private AudioClip _lockedClip;

		private Transform giveChangeTransform;

		private bool _hasFields = false;

		public void StartFields(GameObject lootParent, int itemPrice, int multiplier, TMP_Text _bulletBalanceTextmesh, TMP_Text _refundTextmesh, AmmoReciever reciever, string lightRefundSpawnable, string mediumRefundSpawnable, string heavyRefundSpawnable, AudioClip openedClip, AudioClip unlockedClip, AudioClip lockedClip, Transform giveChangeTransform)
		{
			SetLootsFromParent(lootParent);
			this._itemPrice = itemPrice;
			this._multiplier = multiplier;
			this._bulletBalanceTextmesh = _bulletBalanceTextmesh;
			this._refundTextmesh = _refundTextmesh;
			this.reciever = reciever;
			this.lightRefundSpawnable = lightRefundSpawnable;
			this.mediumRefundSpawnable = mediumRefundSpawnable;
			this.heavyRefundSpawnable = heavyRefundSpawnable;
			this._openedClip = openedClip;
			this._unlockedClip = unlockedClip;
			this._lockedClip = lockedClip;
			this.giveChangeTransform = giveChangeTransform;

			_hasFields = true;

			SafeStart();
		}

		private void SafeStart()
		{
            reciever.OnInsert += InsertMagazine;
            _bulletBalanceTextmesh.text = _trueItemPrice.ToString();
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
                HelperMethods.SpawnCrate(lightRefundSpawnable, giveChangeTransform.position, giveChangeTransform.rotation, Vector3.one);
            }
            if(_mediumBullets > 0)
            {
                HelperMethods.SpawnCrate(mediumRefundSpawnable, giveChangeTransform.position, giveChangeTransform.rotation, Vector3.one);
            }
            if(_heavyBullets > 0)
            {
                HelperMethods.SpawnCrate(heavyRefundSpawnable, giveChangeTransform.position, giveChangeTransform.rotation, Vector3.one);
            }

            _lightBullets = 0;
            _mediumBullets = 0;
            _heavyBullets = 0;

			_refundTextmesh.text = "0";
		}

		public void PurchaseItem()
		{
			// If the inserted bullets is less than the item price
			// (Unable to buy)
			if(_totalBullets < _trueItemPrice)
			{
				_doorMotorCircuit.Value = 1;
				return;
			}

			int ammoToRemove = _trueItemPrice;
			
			ammoToRemove = CleanupLight(ammoToRemove);
			ammoToRemove = CleanupMedium(ammoToRemove);
			CleanupHeavy(ammoToRemove);

			foreach(Rigidbody loot in _lootBodies)
			{
				loot.isKinematic = false;
			}

			_refundTextmesh.text = _totalBullets.ToString();

		}

		private int CleanupLight(int ammoToRemove)
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

		private int CleanupMedium(int ammoToRemove)
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

		private int CleanupHeavy(int ammoToRemove)
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

			if(_totalBullets >= _trueItemPrice && !_unlocked && !_opened)
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

            _bulletBalanceTextmesh.text = (Mathf.Max(_trueItemPrice - _totalBullets, 0f)).ToString();
            _refundTextmesh.text = _totalBullets.ToString();

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