using Kitchen;
using Kitchen.Components;
using KitchenMods;
using MessagePack;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace FunnyThings.GarageDoor
{
    public class GarageDoorView : UpdatableObjectView<GarageDoorView.ViewData>
    {
        public class UpdateView : IncrementalViewSystemBase<ViewData>, IModSystem
        {
            EntityQuery Views;

            protected override void Initialise()
            {
                base.Initialise();
                Views = GetEntityQuery(typeof(CGarageDecorations), typeof(CLinkedView));
            }

            protected override void OnUpdate()
            {
                using NativeArray<CLinkedView> views = Views.ToComponentDataArray<CLinkedView>(Allocator.Temp);
                using NativeArray<CGarageDecorations> garageDecorations = Views.ToComponentDataArray<CGarageDecorations>(Allocator.Temp);
                for (int i = 0; i < views.Length; i++)
                {
                    CLinkedView view = views[i];
                    CGarageDecorations garageDecoration = garageDecorations[i];
                    SendUpdate(view, new ViewData()
                    {
                        IsOpen = garageDecoration.IsOpen
                    });
                }
            }
        }

        [MessagePackObject(false)]
        public class ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)] public bool IsOpen;

            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<GarageDoorView>();

            public bool IsChangedFrom(ViewData check)
            {
                return IsOpen != check.IsOpen;
            }
        }

        public SoundSource SoundSource;
        public AudioClip AudioClip;

        private bool IsAnimating = false;
        private float AnimationProgress = 0f;
        private bool IsOpen = false;

        public Transform GarageShutterDoor;
        public GameObject ParcelPrefab;
        private Transform Container;
        private List<(Transform parcelTransform, float distance)> Parcels;

        private const int PARCEL_COUNT = 15;
        private const float PARCEL_START_TIME = 5f;
        private const float PARCEL_DURATION = 0.3f;

        private const float ANIMATION_TOTAL_DURATION = 12f;

        protected override void UpdateData(ViewData data)
        {
            IsOpen = data.IsOpen;
            if (IsOpen && !IsAnimating)
            {
                IsAnimating = true;
                if (SoundSource != null && AudioClip != null)
                {
                    SoundSource.Configure(SoundCategory.Effects, AudioClip);
                    SoundSource.Play();
                }
            }
        }

        void Update()
        {
            if (IsAnimating)
            {
                AnimateDoor();
                AnimateParcels();
                if (AnimationProgress > ANIMATION_TOTAL_DURATION)
                {
                    Reset();
                }
                AnimationProgress += Time.deltaTime;
            }
        }

        void AnimateDoor()
        {
            if (GarageShutterDoor == null)
                return;
            if (AnimationProgress < 7f)
            {
                GarageShutterDoor.localPosition = new Vector3(0f, Mathf.Clamp01(AnimationProgress / 3f) * 2f, 0f);
            }
            else
            {
                GarageShutterDoor.localPosition = new Vector3(0f, Mathf.Clamp01((10f - AnimationProgress) / 3f) * 2f, 0f);
            }
        }

        void AnimateParcels()
        {
            if (AnimationProgress <= PARCEL_START_TIME || ParcelPrefab == null)
                return;

            if (Container == null)
            {
                Container = new GameObject("Parcels").transform;
                Container.SetParent(transform);
                Container.localPosition = Vector3.zero;
            }
            if (Parcels == null)
            {
                Parcels = new List<(Transform, float)>();
                for (int i = 0; i < PARCEL_COUNT; i++)
                {
                    GameObject parcel = GameObject.Instantiate(ParcelPrefab);
                    LetterView letterView = parcel.GetComponent<LetterView>();
                    if (letterView != null)
                        Component.DestroyImmediate(letterView);
                    Animator animator = parcel.GetComponent<Animator>();
                    if (animator != null)
                        Component.DestroyImmediate(animator);
                    parcel.transform.SetParent(Container);
                    parcel.transform.localPosition = new Vector3(-1f, 0f, Random.Range(-1.5f, 1.5f));
                    parcel.transform.localRotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
                    Parcels.Add((parcel.transform, Random.Range(4f, 6f)));
                }
            }
            if (Parcels != null)
            {
                for (int i = 0; i < Parcels.Count; i++)
                {
                    Transform parcel = Parcels[i].parcelTransform;
                    if (parcel != null)
                        parcel.localPosition = new Vector3(
                            x: -1f + Mathf.Clamp01((AnimationProgress - PARCEL_START_TIME) / PARCEL_DURATION) * Parcels[i].distance,
                            y: Mathf.Clamp01((PARCEL_START_TIME + PARCEL_DURATION - AnimationProgress) / PARCEL_DURATION),
                            z: parcel.localPosition.z);
                }
            }
        }

        void Reset()
        {
            IsAnimating = false;
            AnimationProgress = 0f;
            if (GarageShutterDoor != null)
                GarageShutterDoor.localPosition = Vector3.zero;
            if (Container != null)
                GameObject.Destroy(Container.gameObject);
            Parcels.Clear();
            Parcels = null;
        }
    }
}
