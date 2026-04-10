#if MELONLOADER
using BoneLib;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Audio;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppTMPro;
using Il2CppUltEvents;
using MelonLoader;
#else
using SLZ.Bonelab;
using SLZ.Marrow;
using SLZ.Marrow.Warehouse;
using TMPro;
using UltEvents;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomCampaignTools.LabWorks
{
#if MELONLOADER
	[RegisterTypeInIl2Cpp]
#endif
    public class Control_MonoMat : MonoBehaviour
    {
#if MELONLOADER
		public Control_MonoMat(IntPtr ptr) : base(ptr) {}
#endif

        private CrateSpawner[] _loots;
        public void SetLootsFromParent(GameObject parent) => _loots = parent.GetComponentsInChildren<CrateSpawner>();

#if MELONLOADER
		public List<Rigidbody> _lootBodies = [];

		public int _multiplier;
		public int _itemPrice;

		private int _lightBullets;
		private int _mediumBullets;
		private int _heavyBullets;
        private int _totalBullets => _lightBullets + _mediumBullets + _heavyBullets;

		private bool _opened;
		private bool _unlocked;

		public Il2CppReferenceField<TMP_Text> _bulletBalanceTextmesh;
		public Il2CppReferenceField<TMP_Text> _refundTextmesh;

		public Il2CppReferenceField<AmmoReciever> reciever;

		private string lightRefundSpawnable;
		private string mediumRefundSpawnable;
		private string heavyRefundSpawnable;

		private AudioClip _openedClip;
		private AudioClip _unlockedClip;
		private AudioClip _lockedClip;

		public Il2CppReferenceField<Transform> giveChangeTransform;

		public Il2CppReferenceField<UltEventHolder> OnAmountRose;
		public Il2CppReferenceField<UltEventHolder> OnAmountDropped;
		public Il2CppReferenceField<UltEventHolder> OnItemBought;
#else
		public TMP_Text _bulletBalanceTextmesh;
		public TMP_Text _refundTextmesh;
		public AmmoReciever reciever;

		public Transform giveChangeTransform;
		public UltEventHolder OnAmountRose;
		public UltEventHolder OnAmountDropped;
		public UltEventHolder OnItemBought;
#endif

        public void StartFields(
            GameObject lootParent,
            int itemPrice,
            int multiplier,
            string lightRefundSpawnable,
            string mediumRefundSpawnable,
            string heavyRefundSpawnable,
            AudioClip openedClip,
            AudioClip unlockedClip,
            AudioClip lockedClip)
        {
#if MELONLOADER
			SetLootsFromParent(lootParent);
			foreach(Rigidbody rb in lootParent.GetComponentsInChildren<Rigidbody>())
			{
				rb.isKinematic = true;
				_lootBodies.Add(rb);
			}
			this._itemPrice = itemPrice;
			this._multiplier = multiplier;
			this.lightRefundSpawnable = lightRefundSpawnable;
			this.mediumRefundSpawnable = mediumRefundSpawnable;
			this.heavyRefundSpawnable = heavyRefundSpawnable;
			this._openedClip = openedClip;
			this._unlockedClip = unlockedClip;
			this._lockedClip = lockedClip;

			SafeStart();
#endif
        }

#if MELONLOADER
		private void SafeStart()
		{
			reciever.Get().OnInserted += (Il2CppSystem.Action<Magazine>)((g) => InsertMagazine(g));
			foreach(CrateSpawner loot in _loots)
			{
				UltEvent<CrateSpawner, GameObject> lootOnSpawn = loot.onSpawnEvent.Cast<UltEvent<CrateSpawner, GameObject>>();
				Action<CrateSpawner, GameObject> dingusSpawny = new Action<CrateSpawner, GameObject>(OnLootSpawned);
				UltEvent<CrateSpawner, GameObject>.AddDynamicCall(ref lootOnSpawn, dingusSpawny);
			}
			UpdateTMP();
		}

		private void UpdateTMP()
		{
			_refundTextmesh.Get().text = $"{_totalBullets}";
			int max = Mathf.Max(0, (_itemPrice - _totalBullets));
			_bulletBalanceTextmesh.Get().text = _opened ? "0" : $"{max}";
		}
#endif

        public void OnLootSpawned(CrateSpawner spawner, GameObject entity)
        {
#if MELONLOADER
			foreach(Rigidbody rb in entity.GetComponentsInChildren<Rigidbody>())
			{
				rb.isKinematic = true;
				_lootBodies.Add(rb);
			}
#endif
        }

        public void GiveChange()
        {
#if MELONLOADER
			if(giveChangeTransform.Get() == null)
			{
				throw new NullReferenceException("Object reference \"giveChangeTransform\" not set to instance of an object");
			}
            if(_lightBullets > 0)
            {
                HelperMethods.SpawnCrate(lightRefundSpawnable, giveChangeTransform.Get().position, giveChangeTransform.Get().rotation, Vector3.one, spawnAction: (gobj) =>
				{
					gobj.GetComponent<AmmoPickupProxy>().ammoPickup.ammoCount = _lightBullets;
				});
            }
            if(_mediumBullets > 0)
            {
                HelperMethods.SpawnCrate(mediumRefundSpawnable, giveChangeTransform.Get().position, giveChangeTransform.Get().rotation, Vector3.one, spawnAction: (gobj) =>
                {
                    gobj.GetComponent<AmmoPickupProxy>().ammoPickup.ammoCount = _mediumBullets;
                });
            }
            if(_heavyBullets > 0)
            {
                HelperMethods.SpawnCrate(heavyRefundSpawnable, giveChangeTransform.Get().position, giveChangeTransform.Get().rotation, Vector3.one, spawnAction: (gobj) =>
                {
                    gobj.GetComponent<AmmoPickupProxy>().ammoPickup.ammoCount = _heavyBullets;
                });
            }

            _lightBullets = 0;
            _mediumBullets = 0;
            _heavyBullets = 0;

			if(!_opened && OnAmountDropped.Get() != null)
				OnAmountDropped.Get().Invoke();

			UpdateTMP();
#endif
        }

        public void PurchaseItem()
        {
#if MELONLOADER
			// If the inserted bullets is less than the item price
			// (Unable to buy)
			if(_totalBullets < _itemPrice)
			{
				return;
			}

			int ammoToRemove = _itemPrice;
			
			ammoToRemove = CleanupLight(ammoToRemove);
			ammoToRemove = CleanupMedium(ammoToRemove);
			CleanupHeavy(ammoToRemove);

			foreach(Rigidbody loot in _lootBodies)
				loot.isKinematic = false;

			_opened = true;

			if(OnItemBought.Get() != null)
				OnItemBought.Get().Invoke();

			if(_openedClip != null)
				Audio3dManager.PlayAtPoint(_openedClip, transform.position, Audio3dManager.ui);

			UpdateTMP();
#endif
        }

#if MELONLOADER
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
			int ammoType = GetAmmoTypeFromMagazine(magazine);

			string group = AmmoInventory.Instance.GetGroupByCartridge(magazine.magazineState.cartridgeData);
			int multAmmo = magazine.magazineState.AmmoCount * _multiplier;
			
			int bulletsToAdd = Mathf.Min(AmmoInventory.Instance.GetCartridgeCount(group), multAmmo); // Don't let ammo count go over the amount you have

            AddBullets(bulletsToAdd, ammoType);

			AmmoInventory.Instance.RemoveCartridge(magazine.magazineState.cartridgeData, bulletsToAdd);

			if(_totalBullets >= _itemPrice && !_unlocked && !_opened)
			{
				_unlocked = true;
			}
		}

		public int GetAmmoTypeFromMagazine(Magazine mag)
		{
			string group = AmmoInventory.Instance.GetGroupByCartridge(mag.magazineState.cartridgeData);

			if(group == "light")
				return 0;
			else if (group == "medium")
				return 1;
			else if (group == "heavy")
				return 2;
			else
				return -1;
		}

		public void AddBullets(int addedBullets, int type)
		{
			int prevTotal = _totalBullets;
            if(type == 0)
                _lightBullets += addedBullets;
            else if(type == 1)
                _mediumBullets += addedBullets;
            else if(type == 2)
                _heavyBullets += addedBullets;

			if(prevTotal < _itemPrice && _totalBullets >= _itemPrice && OnAmountRose.Get() != null)
			{
				OnAmountRose.Get().Invoke();
			}

            UpdateTMP();
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
#endif
    }
}